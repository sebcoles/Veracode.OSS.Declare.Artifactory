using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Veracode.OSS.Declare.Configuration;

namespace Veracode.OSS.Declare.Artifactory.Tests
{
    [TestFixture]
    public class CommandRunnerTests
    {
        private DownloaderLogic _downloaderLogic;
        private CommandRunner _commandRunner;
        public string complete_configuration_file = "complete.json";
        public string incomplete_configuration_file = "incomplete.json";
        public string _artifactoryUrl;
        public string _artifactoryApiKey;
        public string _downloadFolder;

        [SetUp]
        public void Setup()
        {
            _downloaderLogic = new DownloaderLogic(new Mock<ILogger<DownloaderLogic>>().Object);
            _commandRunner = new CommandRunner();

            IConfiguration Configuration = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
#if DEBUG
                .AddJsonFile($"appsettings.Development.json", false)
#else
                .AddJsonFile("appsettings.json", false)
#endif
                .Build();

            _artifactoryUrl = Configuration.GetValue<string>("ArtifactoryUrl");
            _artifactoryApiKey = Configuration.GetValue<string>("ArtifactoryApiKey");
            _downloadFolder = Configuration.GetValue<string>("DownloadFolder");

            if (Directory.Exists(_downloadFolder))
                Directory.Delete(_downloadFolder, true);

            Directory.CreateDirectory(_downloadFolder);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_downloadFolder))
                Directory.Delete(_downloadFolder, true);
        }

        [Test]
        public void RunJFrogTask_ShouldDownloadAllFilesInConfig()
        {
            var repo = new DeclareConfigurationRepository(complete_configuration_file);
            var artifactoryPaths = _downloaderLogic.GetArtifactoryPaths(repo.Apps());
            foreach (var artifactoryPath in artifactoryPaths)
                _commandRunner.RunJFrogTask(new ArtifactoryCommand
                {
                    ArtifactoryApiKey = _artifactoryApiKey,
                    ArtifactorySourcePath = CleanseHelper.Cleanse(artifactoryPath),
                    ArtifactoryUrl = _artifactoryUrl,
                    DownloadFolder = CleanseHelper.Cleanse(_downloadFolder)
                }.ReturnCommand());

            Assert.IsTrue(File.Exists($"{_downloadFolder}\\org\\owasp\\encoder\\encoder\\1.1\\encoder-1.1.jar"));
            Assert.IsTrue(File.Exists($"{_downloadFolder}\\org\\owasp\\encoder\\encoder\\1.1.1\\encoder-1.1.1.jar"));
            Assert.IsTrue(File.Exists($"{_downloadFolder}\\org\\owasp\\encoder\\encoder\\1.2\\encoder-1.2.jar"));
            Assert.IsTrue(File.Exists($"{_downloadFolder}\\org\\owasp\\encoder\\encoder\\1.2.2\\encoder-1.2.2.jar"));
            Assert.IsTrue(File.Exists($"{_downloadFolder}\\org\\owasp\\encoder\\encoder\\1.2.3\\encoder-1.2.3.jar"));
        }
    }
}
