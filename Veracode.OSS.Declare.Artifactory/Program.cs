using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Veracode.OSS.Declare.Artifactory.Options;
using Veracode.OSS.Declare.Configuration;

namespace Veracode.OSS.Declare.Artifactory
{
    class Program
    {
        private static IServiceProvider _serviceProvider;
        private static ILogger _logger;
        private static ICommandRunner _commandRunner;
        private static string _artifactoryUrl;
        private static string _artifactoryApiKey;

        static void Main(string[] args)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
#if DEBUG
                .AddJsonFile($"appsettings.Development.json", false)
#else
                .AddJsonFile("appsettings.json", false)
#endif
                .Build();
            var serviceCollection = new ServiceCollection();
            _artifactoryUrl = Configuration.GetValue<string>("ArtifactoryUrl");
            _artifactoryApiKey = Configuration.GetValue<string>("ArtifactoryApiKey");
            serviceCollection.AddLogging(loggingBuilder => {
                loggingBuilder.AddNLog("nlog.config");
            });
            serviceCollection.AddScoped<ICommandRunner, CommandRunner>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
            _logger = _serviceProvider.GetService<ILogger<Program>>();
            _commandRunner = _serviceProvider.GetService<ICommandRunner>();

            Parser.Default.ParseArguments<
                DownloadOptions>(args)
                .MapResult(
                    (DownloadOptions options) => Download(options),
                    errs => HandleParseError(errs));
        }

        static int Download(DownloadOptions options)
        {
            _logger.LogDebug($"Entering {LoggingHelper.GetMyMethodName()} with scan options {options}");

            if (Directory.Exists("Artifactory"))
                Directory.Delete("Artifactory");

            Directory.CreateDirectory("Artifactory");

            var declareConfigRepository = new DeclareConfigurationRepository(options.JsonFileLocation);
            
            if(!declareConfigRepository.Apps().Any(x => !x.download.Any(x => x.Name.ToLower().Equals("artifactory"))))
            {
                _logger.LogWarning("There is no artifactory providers configured. Exiting");
                return 1;
            }

            var taskList = new List<Task>();
            foreach (var app in declareConfigRepository.Apps())
            {
                if (!app.download.Any(x => x.Name.ToLower().Equals("artifactory")))
                    continue;

                foreach(var file in app.download.Single(x => x.Name.Equals("artifactory")).Files)
                {
                    var command = new ArtifactoryCommand
                    {
                        ArtifactoryApiKey = _artifactoryApiKey,
                        ArtifactorySourcePath = CleanseHelper.Cleanse(file.location),
                        ArtifactoryUrl = _artifactoryUrl
                    };
                    taskList.Add(_commandRunner.RunJFrogTask(command.ReturnCommand())); ;
                }
            }
            Task.WaitAll(taskList.ToArray());

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
