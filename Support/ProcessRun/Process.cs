using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessRun
{
    public static class Process
    {
        public static bool Run(string path, int time, string input, out int exitCode, out double executeTime, out string cout, out string cerr)
        {
            using (var process = new System.Diagnostics.Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = path;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();
                process.StandardInput.Write(input);
                process.WaitForExit(time);
                if (!process.HasExited)
                {
                    process.Kill();
                    exitCode = 0;
                    executeTime = time;
                    cout = null;
                    cerr = null;
                    return false;
                }
                exitCode = process.ExitCode;
                executeTime = process.UserProcessorTime.TotalMilliseconds;
                cout = process.StandardOutput.ReadToEnd();
                cerr = process.StandardError.ReadToEnd();
                return true;
            }
        }
    }
}
