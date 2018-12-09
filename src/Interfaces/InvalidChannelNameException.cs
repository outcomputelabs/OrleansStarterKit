using System;

namespace Grains
{
    /// <summary>
    /// Thrown when a given string key is not a valid name for a channel.
    /// </summary>
    public class InvalidChannelNameException : ApplicationException
    {
        /// <summary>
        /// Creates a new instance of <see cref="InvalidChannelNameException"/>.
        /// </summary>
        /// <param name="name">The invalid channel name.</param>
        public InvalidChannelNameException(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The invalid channel name.
        /// </summary>
        public string Name { get; }
    }
}