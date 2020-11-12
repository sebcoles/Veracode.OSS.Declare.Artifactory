using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Linq;
using Veracode.OSS.Declare.Configuration;

namespace Veracode.OSS.Declare.Artifactory.Tests
{
    [TestFixture]
    public class DownloadLogicTests
    {
        private DownloaderLogic _downloaderLogic;
        public string complete_configuration_file = "complete.json";
        public string incomplete_configuration_file = "incomplete.json";

        [SetUp]
        public void Setup()
        {
            _downloaderLogic = new DownloaderLogic(new Mock<ILogger<DownloaderLogic>>().Object);
        }

        [Test]
        public void AnyArtifactoryProvidersConfigured_ShouldReturnTrueForCompleteConfig()
        {
            var repo = new DeclareConfigurationRepository(complete_configuration_file);
            Assert.IsTrue(_downloaderLogic.AnyArtifactoryProvidersConfigured(repo.Apps()));
        }

        [Test]
        public void AnyArtifactoryProvidersConfigured_ShouldReturnFalseForIncompleteConfig()
        {
            var repo = new DeclareConfigurationRepository(incomplete_configuration_file);
            Assert.IsFalse(_downloaderLogic.AnyArtifactoryProvidersConfigured(repo.Apps()));
        }

        [Test]
        public void GetArtifactoryPaths()
        {
            var repo = new DeclareConfigurationRepository(complete_configuration_file);
            var artifactoryPaths = _downloaderLogic.GetArtifactoryPaths(repo.Apps());
            Assert.AreEqual(5, artifactoryPaths.Length);
            Assert.IsTrue(artifactoryPaths.Any(x => x.Equals("mvn-public-local/org/owasp/encoder/encoder/1.1/encoder-1.1.jar")));
            Assert.IsTrue(artifactoryPaths.Any(x => x.Equals("mvn-public-local/org/owasp/encoder/encoder/1.1.1/encoder-1.1.1.jar")));
            Assert.IsTrue(artifactoryPaths.Any(x => x.Equals("mvn-public-local/org/owasp/encoder/encoder/1.2/encoder-1.2.jar")));
            Assert.IsTrue(artifactoryPaths.Any(x => x.Equals("mvn-public-local/org/owasp/encoder/encoder/1.2.2/encoder-1.2.2.jar")));
            Assert.IsTrue(artifactoryPaths.Any(x => x.Equals("mvn-public-local/org/owasp/encoder/encoder/1.2.3/encoder-1.2.3.jar")));
        }
    }
}
