using System;

namespace Grains
{
    /// <summary>
    /// Indicates that a channel already exists when attempting to create a new one.
    /// </summary>
    public class ChannelAlreadyCreatedException : ApplicationException
    {
        /// <summary>
        /// Creates a new instance of <see cref="ChannelAlreadyCreatedException"/>.
        /// </summary>
        /// <param name="name">The name of the existing channel.</param>
        public ChannelAlreadyCreatedException(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the existing channel.
        /// </summary>
        public string Name { get; }
    }
}
