using System;

namespace Grains
{
    /// <summary>
    /// Thrown when an invalid key is used to activate a grain.
    /// </summary>
    public class InvalidGrainKeyException : ApplicationException
    {
        /// <summary>
        /// Creates a new instance of <see cref="InvalidGrainKeyException"/>
        /// </summary>
        /// <param name="key">The invalid key.</param>
        public InvalidGrainKeyException(string key)
        {
            Key = key;
        }

        /// <summary>
        /// The invalid key.
        /// </summary>
        public string Key { get; }
    }
}
