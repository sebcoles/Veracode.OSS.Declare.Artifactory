using System;
using System.Collections.Generic;
using System.Text;

namespace Veracode.OSS.Declare.Artifactory
{
    public class ArtifactoryCommand
    {
        public string ArtifactoryUrl { get; set; }
        public string ArtifactorySourcePath { get; set; }
        public string ArtifactoryApiKey { get; set; }
        public bool LatestOnly { get; set; }

        public string ReturnCommand() {
            var commandText = $"rt dl --url {ArtifactoryUrl} --apikey {ArtifactoryApiKey} {ArtifactorySourcePath} /Artifactory";
            if (LatestOnly)
                commandText += " --sort-by=created --sort-order=desc --limit=1";

            return commandText;
        }
    }
}
