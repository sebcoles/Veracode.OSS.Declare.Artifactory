using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Veracode.OSS.Declare.Artifactory
{
    public interface ICommandRunner {
        Task<bool> RunJFrogTask(string arguements);    
    }

    public class CommandRunner : ICommandRunner
    {
        public async Task<bool> RunJFrogTask(string arguements)
        {
            return await Task.Factory.StartNew(() =>
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "jfrog.exe";
                startInfo.Arguments = arguements;
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
                return true;
            });
        }
    }
}
