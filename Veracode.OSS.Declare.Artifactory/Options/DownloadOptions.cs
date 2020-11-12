using CommandLine;
using Newtonsoft.Json;

namespace Veracode.OSS.Declare.Artifactory.Options
{
    [Verb("download", HelpText = "This will download the files provides in the artifactory provider section from the declare configuration")]
    public class DownloadOptions
    {
        [Option('f', "jsonfile", Default = "veracode.json", Required = true, HelpText = "Location of JSON configuration file")]
        public string JsonFileLocation { get; set; }

        [Option('t', "target", Default = "Artifactory", Required = true, HelpText = "Language for tool output")]
        public string Target { get; set; }

        [Option('l', "language", Default = "en-GB", Required = false, HelpText = "Language for tool output")]
        public string Language { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
