using System;
using System.Collections.Generic;

namespace Grains.Models
{
    public static class UserInfoComparer
    {
        public class UserInfoKeyComparer : IEqualityComparer<UserInfo>, IComparer<UserInfo>
        {
            public UserInfoKeyComparer() { }

            public int Compare(UserInfo x, UserInfo y) => x.Key.CompareTo(y.Key);

            public bool Equals(UserInfo x, UserInfo y) => x.Key == y.Key;

            public int GetHashCode(UserInfo obj) => obj.Key.GetHashCode();
        }

        public class UserInfoUserNameComparer : IEqualityComparer<UserInfo>, IComparer<UserInfo>
        {
            public UserInfoUserNameComparer() { }

            public int Compare(UserInfo x, UserInfo y) => string.Compare(x.UserName, y.UserName, StringComparison.OrdinalIgnoreCase);

            public bool Equals(UserInfo x, UserInfo y) => string.Equals(x.UserName, y.UserName, StringComparison.OrdinalIgnoreCase);

            public int GetHashCode(UserInfo obj) => obj.UserName.GetHashCode();
        }

        public static UserInfoKeyComparer ByKey { get; } = new UserInfoKeyComparer();

        public static UserInfoUserNameComparer ByUserName { get; } = new UserInfoUserNameComparer();
    }
}
