using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    /// <inheritdoc />
    [Immutable]
    public class AccountInfo : IComparable<AccountInfo>, IEquatable<AccountInfo>
    {
        /// <summary>
        /// Creates a new instance of <see cref="AccountInfo"/>.
        /// </summary>
        /// <param name="handle">The handle (user name) of the account.</param>
        /// <param name="displayName">The display name of the account.</param>
        public AccountInfo(string handle, string displayName)
        {
            Handle = handle ?? throw new ArgumentNullException(nameof(handle));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        }

        /// <summary>
        /// The user name (handle) of the account.
        /// </summary>
        public string Handle { get; }

        /// <summary>
        /// The display name of the account.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Compares two <see cref="AccountInfo"/> objects.
        /// </summary>
        public int CompareTo(AccountInfo other)
        {
            // two accounts are the same if their handle is the same
            // the comparison is case-insensitive
            // otherwise they sort by handle
            return string.Compare(Handle, other.Handle, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Compares two <see cref="AccountInfo"/> instances for equality.
        /// </summary>
        public bool Equals(AccountInfo other)
        {
            // two accounts are the same if their handle is the same
            // the comparison is case-insensitive
            return string.Equals(Handle, other.Handle, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
