using System;
using System.Collections.Generic;
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
            startInfo.FileName = "jfrog.exe";
            startInfo.Arguments = arguements;
            process.StartInfo = startInfo;
            process.Start();
            Console.WriteLine(process.StandardOutput.ReadToEnd());
            process.WaitForExit();
            return true;
        }
    }
}
