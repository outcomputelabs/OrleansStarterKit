using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Interfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;

namespace Grains
{
    [Reentrant]
    public class FeedGrain : Grain<FeedGrainState>, IFeed
    {
        public FeedGrain(ILogger<FeedGrain> logger)
        {
            _logger = logger;
        }

        private readonly ILogger<FeedGrain> _logger;

        #region Settings

        private const int MaxReceivedMessagesCached = 100;
        private const int MaxPublishedMessagesCached = 100;

        #endregion

        #region Recovery

        private string _handle;

        public override Task OnActivateAsync()
        {
            // initialize the persisted state if necessary
            if (State.Followers == null) State.Followers = new Dictionary<string, IFeedSubscriber>();
            if (State.Following == null) State.Following = new Dictionary<string, IFeedPublisher>();
            if (State.PublishedMessages == null) State.PublishedMessages = new Queue<Message>();
            if (State.ReceivedMessages == null) State.ReceivedMessages = new Queue<Message>();

            // initialize volatile state
            _handle = this.GetPrimaryKeyString();

            return base.OnActivateAsync();
        }

        #endregion

        #region Public Interface

        public Task PublishMessageAsync(string content)
        {
            // create the new message
            var message = new Message(Guid.NewGuid(), content, DateTime.Now, _handle);

            // publish the message to this feed
            State.PublishedMessages.Enqueue(message);

            // notify all subscribers while saving state
            var tasks = new List<Task>(State.Followers.Count + 1);
            foreach (var sub in State.Followers.Values)
            {
                tasks.Add(sub.OnMessageAsync(message));
            }
            tasks.Add(WriteStateAsync());
            return Task.WhenAll(tasks);
        }

        public Task SubscribeAsync(string handle, IFeedSubscriber follower)
        {
            State.Followers[handle] = follower;
            return WriteStateAsync();
        }

        public Task UnsubscribeAsync(string handle)
        {
            State.Followers.Remove(handle);
            return WriteStateAsync();
        }

        public Task<ImmutableList<Message>> GetLatestMessagesAsync()
        {
            // return a mix of messages from this feed and followed feeds
            return Task.FromResult(State.PublishedMessages.Concat(State.ReceivedMessages).ToImmutableList());
        }

        public Task OnMessageAsync(Message message)
        {
            // add the message to state cache
            State.ReceivedMessages.Enqueue(message);
            while (State.ReceivedMessages.Count > MaxReceivedMessagesCached)
            {
                State.ReceivedMessages.Dequeue();
            }
            return WriteStateAsync();
        }

        #endregion

        #region Batch Persistence

        private Task outstandingWriteTask = null;

        /// <summary>
        /// Optimized batch write logic that handles back-pressure from the storage provider at the risk of possible data loss in case of a silo crash.
        /// A more reliable way of persisting state would be through formal event-sourcing.
        /// </summary>
        protected override async Task WriteStateAsync()
        {
            // wait for any current write operation if necessary
            var currentWriteTask = outstandingWriteTask;
            if (currentWriteTask != null)
            {
                try
                {
                    // await the outstanding write task
                    // multiple turns will await here and one will win the reentrant race
                    // the one that wins will get to initiate the next write request
                    await currentWriteTask;
                }
                catch
                {
                    // ignore any current errors and let the original caller handle them
                }
                finally
                {
                    // check whether this turn won the reentrant race
                    if (outstandingWriteTask == currentWriteTask)
                    {
                        // if so then clear the outstanding task
                        // this signals the block below that it is safe to initiate the next write task now
                        // other turns will see a new pending task and skip the next block
                        outstandingWriteTask = null;
                    }
                }
            }

            // the outstanding task will be null if either there is no oustanding write at all
            // or if this turn has won the await race above
            if (outstandingWriteTask == null)
            {
                // this turn has won the reentrant race above
                // so it gets to initiate the new write request
                outstandingWriteTask = base.WriteStateAsync();
            }

            // both winner and losing turns can now await on the same operation
            currentWriteTask = outstandingWriteTask;
            try
            {
                // this await will cause another reentrant race
                await currentWriteTask;
            }
            finally
            {
                // if this turn won the reentrant race then both current and outstanding tasks will be the same
                // we can therefore clear the outstanding operation
                // otherwise another turn won and has already batched another write operation
                if (currentWriteTask == outstandingWriteTask)
                {
                    outstandingWriteTask = null;
                }
            }
        }

        #endregion
    }

    public class FeedGrainState
    {
        /// <summary>
        /// Keeps the collection of followers to this feed.
        /// </summary>
        public Dictionary<string, IFeedSubscriber> Followers { get; set; }

        /// <summary>
        /// Keeps the collection of feeds this feed follows.
        /// </summary>
        public Dictionary<string, IFeedPublisher> Following { get; set; }

        /// <summary>
        /// Keeps the latest messages published by this feed.
        /// </summary>
        public Queue<Message> PublishedMessages { get; set; }

        /// <summary>
        /// Keeps the latest messages published by feeds this one follows.
        /// </summary>
        public Queue<Message> ReceivedMessages { get; set; }
    }
}
