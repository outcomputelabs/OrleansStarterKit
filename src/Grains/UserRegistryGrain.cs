using Grains.Models;
using Microsoft.Extensions.Logging;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grains
{
    public class UserRegistryGrain : Grain, IUserRegistryGrain
    {
        private readonly ILogger<UserRegistryGrain> _logger;

        public UserRegistryGrain(ILogger<UserRegistryGrain> logger)
        {
            _logger = logger;
        }

        private readonly HashSet<UserInfo> _byUserName = new HashSet<UserInfo>(UserInfoComparer.ByUserName);
        private readonly HashSet<UserInfo> _byKey = new HashSet<UserInfo>(UserInfoComparer.ByKey);

        public Task<bool> TryCreateAsync(UserInfo info)
        {
            // check the key
            if (_byKey.Contains(info)) return Task.FromResult(false);

            // check the username
            if (_byUserName.Contains(info)) return Task.FromResult(false);

            // add the new user
            _byKey.Add(info);
            _byUserName.Add(info);
            return Task.FromResult(true);
        }
    }
}
