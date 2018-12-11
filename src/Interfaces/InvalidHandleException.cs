using System;

namespace Grains
{
    /// <summary>
    /// Thrown when an account handle does not match an account grain key.
    /// </summary>
    public class InvalidHandleException : ApplicationException
    {
        /// <summary>
        /// Creates a new instance of <see cref="InvalidHandleException"/>.
        /// </summary>
        /// <param name="handle">The inconsistent handle.</param>
        public InvalidHandleException(string handle)
        {
            Handle = handle;
        }

        /// <summary>
        /// The inconsistent handle.
        /// </summary>
        public string Handle { get; }
    }
}
