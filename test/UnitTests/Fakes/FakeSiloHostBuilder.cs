﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
using System;
using System.Collections.Generic;

namespace UnitTests.Fakes
{
    public class FakeSiloHostBuilder : ISiloHostBuilder
    {
        public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();

        public ISiloHost Build()
        {
            throw new NotImplementedException();
        }

        public ISiloHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            throw new NotImplementedException();
        }

        public ISiloHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
        {
            throw new NotImplementedException();
        }

        public ISiloHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
        {
            throw new NotImplementedException();
        }

        public ISiloHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
        {
            configureDelegate(new HostBuilderContext(Properties), ServiceCollection);
            return this;
        }

        public ISiloHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
        {
            throw new NotImplementedException();
        }

        public FakeServiceCollection ServiceCollection { get; } = new FakeServiceCollection();
    }
}
