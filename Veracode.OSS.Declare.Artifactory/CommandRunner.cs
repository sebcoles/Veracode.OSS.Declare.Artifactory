using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Veracode.OSS.Declare.Artifactory
{
    public interface ICommandRunner
    {
        bool RunJFrogTask(string arguements);
    }

    public class CommandRunner : ICommandRunner
    {
        public bool RunJFrogTask(string arguements)
        {
            Environment.SetEnvironmentVariable("JFROG_CLI_OFFER_CONFIG", "false");
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
            {
                RedirectStandardOutput = true
            };
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                startInfo.FileName = "jfrog.exe";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                startInfo.FileName = "jfrog-linux";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                startInfo.FileName = "jfrog-mac.exe";
            startInfo.Arguments = arguements;
            process.StartInfo = startInfo;
            process.Start();
            Console.WriteLine(process.StandardOutput.ReadToEnd());
            process.WaitForExit();
            return true;
        }
    }
}
