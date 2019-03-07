﻿using Microsoft.Extensions.Hosting;
using Orleans;
using System;
using System.Threading;
using System.Threading.Tasks;
using X = System.Console;

namespace Client.Console
{
    public class ConsoleClientHostedService : IHostedService
    {
        public ConsoleClientHostedService(IClusterClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        private readonly IClusterClient _client;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            X.WriteLine("Welcome to the Console Client!");
            X.WriteLine("You can use the following commands. Type /help to see them again.");
            X.WriteLine("/help: shows this list.");

            while (true)
            {
                X.Write("> ");
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
