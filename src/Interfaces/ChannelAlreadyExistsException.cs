using System;

namespace Grains
{
    /// <summary>
    /// Indicates that a channel already exists when attempting to create a new one.
    /// </summary>
    public class ChannelAlreadyExistsException : ApplicationException
    {
        /// <summary>
        /// Creates a new instance of <see cref="ChannelAlreadyExistsException"/>.
        /// </summary>
        /// <param name="name">The name of the existing channel.</param>
        public ChannelAlreadyExistsException(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the existing channel.
        /// </summary>
        public string Name { get; }
    }
}
