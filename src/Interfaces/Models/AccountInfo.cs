using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    /// <inheritdoc />
    [Immutable]
    public class AccountInfo
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
        /// Case mixing is valid for display purposes but leading or trailing spaces are not.
        /// </summary>
        public string Handle { get; }

        /// <summary>
        /// The display name of the account.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// The uniform handle of the account.
        /// This is the regular handle in lower case invariant form.
        /// Use this to prevent multiple users having the same handle with different casing.
        /// </summary>
        public string UniformHandle => Handle.ToLowerInvariant();
    }
}
