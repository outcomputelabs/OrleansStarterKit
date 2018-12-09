using System;

namespace Grains
{
    /// <summary>
    /// Thrown when an invalid key is used to activate an <see cref="IUser"/> grain.
    /// </summary>
    public class InvalidUserGrainKeyException : ApplicationException
    {
        /// <summary>
        /// Creates a new instance of <see cref="InvalidUserGrainKeyException"/>
        /// </summary>
        /// <param name="key">The invalid key.</param>
        public InvalidUserGrainKeyException(string key)
        {
            Key = key;
        }

        /// <summary>
        /// The invalid key.
        /// </summary>
        public string Key { get; }
    }
}
