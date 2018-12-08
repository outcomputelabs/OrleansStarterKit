using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class UserInfo
    {
        /// <summary>
        /// Creates a new instance of <see cref="UserInfo"/>.
        /// </summary>
        /// <param name="userName">The user name (handle) of the user.</param>
        /// <param name="displayName">The display name of the user.</param>
        public UserInfo(string userName, string displayName)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        }

        /// <summary>
        /// The user name (handle) of the user.
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// The display name of the user.
        /// </summary>
        public string DisplayName { get; }
    }
}
