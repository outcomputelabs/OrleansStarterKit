using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains
{
    public class Party : Grain, IParty
    {
        private readonly ILogger<Party> _logger;

        private IPlayer _leader;

        private Guid GrainKey => this.GetPrimaryKey();

        public Party(ILogger<Party> logger)
        {
            _logger = logger;
        }

        public Task CreateAsync(IPlayer leader)
        {
            if (_leader == null)
            {
                _leader = leader;

                _logger.LogInformation("Party with key {@PartyKey} has been created.", GrainKey);
                return Task.CompletedTask;
            }
            else
            {
                _logger.LogError("Party with id {@PartyKey} has already been created.", GrainKey);
                throw new InvalidOperationException();
            }
        }
    }
}