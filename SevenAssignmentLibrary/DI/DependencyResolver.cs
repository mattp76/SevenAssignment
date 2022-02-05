using System;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using SevenAssignmentLibrary.Clients;
using SevenAssignmentLibrary.Helpers;
using SevenAssignmentLibrary.Services;
using SevenAssignmentLibrary.Settings;

namespace SevenAssignmentLibrary.DI
{
    public class DependencyResolver
    {
        private const string DefaultEnv = "Development";

        private static IServiceProvider _serviceProvider;
        private static IServiceCollection _serviceCollection;

        public static T GetService<T>() where T : class
        {
            return _serviceProvider.GetService<T>();
        }

        public static IServiceProvider ConfigureDependency()
        {

            _serviceCollection = new ServiceCollection();

            var configuration = GetConfiguration(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

            ConfigureTransformation(_serviceCollection, configuration);
            ConfigureLogging();
            ConfigureHttpClients();

            //services
            _serviceCollection.AddScoped<ISevenService, SevenService>();
            _serviceCollection.AddScoped<ICacheService, CacheService>();

            //helpers
            _serviceCollection.AddScoped<ISevenAssignmentHelper, SevenAssignmentHelper>();

            //cache
            _serviceCollection.AddMemoryCache();


            _serviceProvider = _serviceCollection.BuildServiceProvider();

            return _serviceProvider;
        }

        private static void ConfigureLogging()
        {
            //create logger
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("seven-logfile.log")
                .CreateLogger();

            //logging
            _serviceCollection.AddLogging(configure => configure.AddSerilog());
        }

        private static void ConfigureHttpClients()
        {
            var sevenAssignmentClientSettings = GetService<SevenAssignmentClientSettings>();
            var pollySettings = GetService<PollySettings>();

            var basicCircuitBreakerPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .CircuitBreakerAsync(pollySettings.NumOfExceptionsAllowed,
                    TimeSpan.FromMinutes(pollySettings.DurationOfBreakInMins));

            var retryPolicy = HttpPolicyExtensions
                // HttpRequestException, 5XX and 408  
                .HandleTransientHttpError()
                // 404  
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                // Retry two times after delay  
                .WaitAndRetryAsync(pollySettings.RetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(pollySettings.RetrySeconds, retryAttempt)));

            _serviceCollection
                .AddHttpClient<ISevenClient, SevenClient>()
                .ConfigureHttpClient(conf =>
                {
                    conf.BaseAddress = new Uri(sevenAssignmentClientSettings.ServiceUrl);
                    conf.Timeout = TimeSpan.FromSeconds(sevenAssignmentClientSettings.TimeOutInSeconds);
                })
                .AddPolicyHandler(basicCircuitBreakerPolicy)
                .AddPolicyHandler(retryPolicy);


            _serviceCollection.AddHttpClient();
        }

        private static void ConfigureTransformation(IServiceCollection services, IConfiguration configuration)
        {
            var sevenAssignmentSettings = new SevenAssignmentSettings();
            var sevenAssignmentClientSettings = new SevenAssignmentClientSettings();
            var pollySettings = new PollySettings();
            configuration.Bind("SevenAssignmentClientSettings", sevenAssignmentClientSettings);
            configuration.Bind("SevenAssignmentSettings", sevenAssignmentSettings);
            configuration.Bind("PollySettings", pollySettings);
            services.AddScoped(i => sevenAssignmentClientSettings);
            services.AddScoped(i => sevenAssignmentSettings);
            services.AddScoped(i => pollySettings);

            _serviceProvider = services.BuildServiceProvider();
        }

        private static IConfiguration GetConfiguration(string env)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{(string.IsNullOrEmpty(env) ? DefaultEnv : env)}.json", true, false)
                .AddEnvironmentVariables()
                .Build();

            return configuration;
        }
    }
}
