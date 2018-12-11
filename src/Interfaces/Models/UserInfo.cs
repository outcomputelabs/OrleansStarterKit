using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class UserInfo : IComparable<UserInfo>, IEquatable<UserInfo>
    {
        /// <summary>
        /// Creates a new instance of <see cref="UserInfo"/>.
        /// </summary>
        /// <param name="handle">The handle (user name) of the user.</param>
        /// <param name="displayName">The display name of the user.</param>
        public UserInfo(string handle, string displayName)
        {
            Handle = handle ?? throw new ArgumentNullException(nameof(handle));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        }

        /// <summary>
        /// The user name (handle) of the user.
        /// </summary>
        public string Handle { get; }

        /// <summary>
        /// The display name of the user.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Compares two <see cref="UserInfo"/> objects.
        /// </summary>
        public int CompareTo(UserInfo other)
        {
            // two user infos are the same if their handle is the same
            // the comparison is case-insensitive
            return string.Compare(Handle, other.Handle, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Compares two <see cref="UserInfo"/> for equality.
        /// </summary>
        public bool Equals(UserInfo other)
        {
            // two user infos are the same if their handle is the same
            // the comparison is case-insensitive
            return string.Equals(Handle, other.Handle, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
