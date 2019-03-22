using Orleans.Concurrency;
using System;

namespace Grains.Models
{
    [Immutable]
    public class UserInfo
    {
        public UserInfo(Guid key, string username)
        {
            Key = key;
            UserName = username;
        }

        public Guid Key { get; }
        public string UserName { get; }
    }
}
