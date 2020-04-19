﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace JudgedProcessRun
{
    public static class JudgedProcess
    {
        public enum Result
        {
            Accept,
            WrongAnswer,
            TLE,
            RE,
            JudgeTLE
        }
        public static Result Run(string programPath, string judgePath, int time, string inputPath, out double executeTime, out string cout, out string cerr)
        {
            cerr = null;
            using (var process = new Process())
            {
                using (var judge = new Process())
                {
                    var builder = new StringBuilder();
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.FileName = programPath;
                    process.OutputDataReceived += (s, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            builder.AppendLine(e.Data);
                            if (!judge.HasExited)
                            {
                                judge.StandardInput.WriteLine(e.Data);
                            }
                        }
                    };
                    judge.StartInfo.UseShellExecute = false;
                    judge.StartInfo.CreateNoWindow = true;
                    judge.StartInfo.RedirectStandardOutput = true;
                    judge.StartInfo.RedirectStandardInput = true;
                    judge.StartInfo.RedirectStandardError = true;
                    judge.StartInfo.FileName = judgePath;
                    judge.StartInfo.Arguments = Path.GetFullPath(inputPath);
                    judge.OutputDataReceived += (s, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            if (!process.HasExited)
                            {
                                process.StandardInput.WriteLine(e.Data);
                            }
                        }
                    };
                    process.Start();
                    process.BeginOutputReadLine();
                    judge.Start();
                    judge.BeginOutputReadLine();
                    var taskA = Task.Run(() =>
                    {
                        process.WaitForExit(time);
                    });
                    var taskB = Task.Run(() =>
                    {
                        judge.WaitForExit(time);
                    });
                    Task.WaitAll(taskA, taskB);
                    var judgeTLE = false;
                    if (!judge.HasExited)
                    {
                        judge.Kill();
                        judgeTLE = true;
                    }
                    if (!process.HasExited)
                    {
                        process.Kill();
                        cout = builder.ToString();
                        executeTime = default;
                        return Result.TLE;
                    }
                    cout = builder.ToString();
                    executeTime = process.UserProcessorTime.TotalMilliseconds;
                    if (process.ExitCode != 0)
                    {
                        cerr = process.StandardError.ReadToEnd();
                        return Result.RE;
                    }
                    if (judgeTLE)
                    {
                        return Result.JudgeTLE;
                    }
                    if (judge.ExitCode != 0)
                    {
                        return Result.WrongAnswer;
                    }
                    return Result.Accept;
                }
            }
        }
    }
}
