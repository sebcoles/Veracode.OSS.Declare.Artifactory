using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Veracode.OSS.Declare.Configuration.Models;

namespace Veracode.OSS.Declare.Artifactory
{
    public interface IDownloaderLogic
    {
        void PrepareDownloadFolder(string folderName);
        bool AnyArtifactoryProvidersConfigured(List<ApplicationProfile> apps);
        string[] GetArtifactoryPaths(List<ApplicationProfile> list);
    }
    public class DownloaderLogic : IDownloaderLogic
    {
        private ILogger _logger;

        public DownloaderLogic(ILogger<DownloaderLogic> logger)
        {
            _logger = logger;
        }

        public bool AnyArtifactoryProvidersConfigured(List<ApplicationProfile> apps)
        {
            if (!apps.Any())
            {
                _logger.LogWarning("No Applicaiton Profiles configured in the configuration");
                return false;
            }

            if (!apps.Any(x => x.download.Any(x => x.name.ToLower().Equals("artifactory"))))
            {
                _logger.LogWarning("There is no artifactory providers configured in the configuration");
                return false;
            }

            _logger.LogInformation("Artifactory provider configuration found");
            return true;
        }

        public string[] GetArtifactoryPaths(List<ApplicationProfile> list) => list
                .SelectMany(y => y.download.Where(x => x.name.ToLower().Equals("artifactory"))
                .SelectMany(x => x.files.Select(x => x.location))).ToArray();        
        public void PrepareDownloadFolder(string folderName)
        {
            if (Directory.Exists(folderName))
                Directory.Delete(folderName, true);

            Directory.CreateDirectory(folderName);
        }
    }
}
