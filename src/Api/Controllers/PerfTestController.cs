using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PerfTestController : ControllerBase
    {
        private readonly IClusterClient _client;

        public PerfTestController(IClusterClient client)
        {
            _client = client;
        }

        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            // test sequential proxy generation speed
            var proxyCount = 10000;
            var grains = new IUser[proxyCount];
            var proxyWatch = Stopwatch.StartNew();
            for (var i = 0; i < proxyCount; ++i)
            {
                grains[i] = _client.GetGrain<IUser>(Guid.NewGuid());
            }
            proxyWatch.Stop();

            // test sequential activation speed
            var activationWatch = Stopwatch.StartNew();
            for (var i = 0; i < proxyCount; ++i)
            {
                await grains[i].Ping();
            }
            activationWatch.Stop();

            // test sequential second call speed
            var secondCallWatch = Stopwatch.StartNew();
            for (var i = 0; i < proxyCount; ++i)
            {
                await grains[i].Ping();
            }
            secondCallWatch.Stop();

            // test parallel activation speed
            var parallels = Enumerable.Range(0, proxyCount).Select(x => _client.GetGrain<IUser>(Guid.NewGuid())).ToArray();
            var parallelActivationWatch = Stopwatch.StartNew();
            Parallel.For(0, proxyCount, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
            {
                parallels[i].Ping().Wait();
            });
            parallelActivationWatch.Stop();

            // test parallel second call speed
            var parallelSecondCallWatch = Stopwatch.StartNew();
            Parallel.For(0, proxyCount, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
            {
                parallels[i].Ping().Wait();
            });
            parallelSecondCallWatch.Stop();

            return
                $"Sequential Proxy Generations /s: { proxyCount / (double)proxyWatch.ElapsedMilliseconds * 1000.0}\n" +
                $"Sequential Activations /s: { proxyCount / (double)activationWatch.ElapsedMilliseconds * 1000.0 }\n" +
                $"Sequential Second Calls /s: { proxyCount / (double)secondCallWatch.ElapsedMilliseconds * 1000.0}\n" +
                $"Parallel Activations /s: { proxyCount / (double)parallelActivationWatch.ElapsedMilliseconds * 1000.0}\n" +
                $"Parallel Second Calls /s: { proxyCount / (double)parallelSecondCallWatch.ElapsedMilliseconds * 1000.0}\n";
        }
    }
}