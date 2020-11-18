using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Veracode.OSS.Declare.Artifactory.Options;
using Veracode.OSS.Declare.Configuration;

namespace Veracode.OSS.Declare.Artifactory
{
    class Program
    {
        private static IServiceProvider _serviceProvider;
        private static ILogger _logger;
        private static ICommandRunner _commandRunner;
        private static IDownloaderLogic _downloaderLogic;
        private static string _artifactoryUrl;
        private static string _artifactoryApiKey;

        static void Main(string[] args)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
#if DEBUG
                .AddJsonFile($"appsettings.Development.json", false)
#else
                .AddJsonFile("appsettings.json", false)
#endif
                .Build();
            var serviceCollection = new ServiceCollection();
            var _useEnvironmentVariables = Configuration.GetValue<bool>("UseEnvironmentVariables");
            _artifactoryUrl = Configuration.GetValue<string>("ArtifactoryUrl");
            if (_useEnvironmentVariables || String.IsNullOrEmpty(_artifactoryUrl))
                _artifactoryUrl = Environment.GetEnvironmentVariable("ARTIFACTORY_URL");

            _artifactoryApiKey = Configuration.GetValue<string>("ArtifactoryAccessToken");
            if (_useEnvironmentVariables || String.IsNullOrEmpty(_artifactoryApiKey))
                _artifactoryUrl = Environment.GetEnvironmentVariable("ARTIFACTORY_ACCESS_TOKEN");

            serviceCollection.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddNLog("nlog.config");
            });
            serviceCollection.AddScoped<ICommandRunner, CommandRunner>();
            serviceCollection.AddScoped<IDownloaderLogic, DownloaderLogic>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
            _logger = _serviceProvider.GetService<ILogger<Program>>();
            _commandRunner = _serviceProvider.GetService<ICommandRunner>();
            _downloaderLogic = _serviceProvider.GetService<IDownloaderLogic>();

            Parser.Default.ParseArguments<
                DownloadOptions>(args)
                .MapResult(
                    (DownloadOptions options) => Download(options),
                    errs => HandleParseError(errs));
        }

        static int Download(DownloadOptions options)
        {
            _logger.LogDebug($"Entering {LoggingHelper.GetMyMethodName()} with scan options {options}");
            var declareConfigRepository = new DeclareConfigurationRepository(options.JsonFileLocation);

            _downloaderLogic.PrepareDownloadFolder(options.Target);

            if (!_downloaderLogic.AnyArtifactoryProvidersConfigured(declareConfigRepository.Apps()))
                return 1;

            var artifactoryPaths = _downloaderLogic.GetArtifactoryPaths(declareConfigRepository.Apps());

            foreach (var artifactoryPath in artifactoryPaths)
            {
                _logger.LogInformation($"Starting download for {artifactoryPath}");
                _commandRunner.RunJFrogTask(new ArtifactoryCommand
                {
                    ArtifactoryApiKey = _artifactoryApiKey,
                    ArtifactorySourcePath = CleanseHelper.Cleanse(artifactoryPath),
                    ArtifactoryUrl = _artifactoryUrl,
                    DownloadFolder = CleanseHelper.Cleanse(options.Target)
                }.ReturnCommand());
                _logger.LogInformation($"Download complete for {artifactoryPath}");
            }

            _logger.LogDebug($"Exiting {LoggingHelper.GetMyMethodName()} with value {1}");
            return 1;
        }

        static int HandleParseError(IEnumerable<Error> errs)
        {
            _logger.LogDebug($"Entering {LoggingHelper.GetMyMethodName()}");

            foreach (var error in errs)
                _logger.LogError($"{error}");

            _logger.LogDebug($"Exiting {LoggingHelper.GetMyMethodName()} with value {1}");
            return 1;
        }
    }
}
