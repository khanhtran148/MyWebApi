using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyWebApi.Application;
using MyWebApi.Infrastructure;

namespace Application.IT.Tests.Fixtures
{
    public sealed class ServiceProviderFixture
        : IDisposable
    {
        private ServiceProvider _serviceProvider;
        private readonly ConfigurationRoot _configurationRoot;

        public IConfigurationRoot ConfigurationRoot => _configurationRoot;

        public IServiceCollection ServiceCollection { get; }
        public IServiceProvider ServiceProvider(ServiceProviderOptions options = default) => _serviceProvider ??= ServiceCollection.BuildServiceProvider(options);

        public ServiceProviderFixture()
        {
            _configurationRoot = (ConfigurationRoot)new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: false)
                .Build();

            ServiceCollection = new ServiceCollection();
            ServiceCollection
                .AddLogging()
                .AddOptions()
                ;

            ServiceCollection.AddSingleton<IConfiguration>(ConfigurationRoot);

            ServiceCollection.RegisterDatabase(ConfigurationRoot);
            ServiceCollection.RegisterActiveMqPublisher(ConfigurationRoot);
            ServiceCollection.RegisterConsumer(ConfigurationRoot);
            ServiceCollection.RegisterApplicationDependencies(ConfigurationRoot);
            ServiceCollection.RegisterInfrastructureDependencies(ConfigurationRoot);
        }

        public void Dispose()
        {
            _serviceProvider.Dispose();
        }
    }
}
