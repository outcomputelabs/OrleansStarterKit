using System;

namespace Grains
{
    /// <summary>
    /// Thrown when a user name does not match a user grain key.
    /// </summary>
    public class InconsistentUserNameException : ApplicationException
    {
        /// <summary>
        /// Creates a new instance of <see cref="InconsistentUserNameException"/>.
        /// </summary>
        /// <param name="userName">The inconsistent user name.</param>
        public InconsistentUserNameException(string userName)
        {
            UserName = userName;
        }

        /// <summary>
        /// The inconsistent user name.
        /// </summary>
        public string UserName { get; }
    }
}
