using Grains.Models;
using Orleans;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Grains
{
    /// <summary>
    /// The persisted state of a lobby.
    /// </summary>
    public class LobbyState
    {
        /// <summary>
        /// The group of channels that this lobby indexes.
        /// </summary>
        public ImmutableSortedSet<ChannelInfo> Channels { get; set; } = ImmutableSortedSet<ChannelInfo>.Empty;

        /// <summary>
        /// The list of users that this channel indexes.
        /// </summary>
        public ImmutableSortedSet<UserInfo> Users { get; set; } = ImmutableSortedSet<UserInfo>.Empty;
    }

    /// <summary>
    /// Indexes a group a channels for quick listing.
    /// </summary>
    public class Lobby : Grain<LobbyState>, ILobby
    {
        public async Task CreateChannelAsync(ChannelInfo info)
        {
            // validate new channel properties
            if (info == null) throw new ArgumentNullException(nameof(info));
            if (string.IsNullOrWhiteSpace(info.Name)) throw new ArgumentNullException(nameof(info.Name));
            if (info.Name != info.Name.Trim().ToLowerInvariant()) throw new InvalidChannelNameException(info.Name);

            // check if the channel already exists
            if (State.Channels.Contains(info)) throw new ChannelAlreadyCreatedException(info.Name);

            // all good so lets get to work
            try
            {
                // attempt to initialize the new channel
                await GrainFactory.GetGrain<IChannel>(info.Name).SetInfoAsync(info);

                // that worked so add it the index
                State.Channels = State.Channels.Add(info);

                // done
                await WriteStateAsync();
            }
            catch
            {
                // something went wrong so reset the state before faulting
                await ReadStateAsync();

                // fault the request regardless
                throw;
            }
        }

        public Task<IEnumerable<ChannelInfo>> GetChannelsAsync()
        {
            return Task.FromResult<IEnumerable<ChannelInfo>>(State.Channels);
        }

        public Task SetUserInfoAsync(UserInfo info)
        {
            // add or update any existing listing information
            // if there are casing differences in the handle
            // then the new handle wins
            State.Users = State.Users.Remove(info).Add(info);
            return WriteStateAsync();
        }
    }
}
