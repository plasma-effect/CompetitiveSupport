using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static System.Console;

namespace JudgedAttemptHack
{
    class JudgedAttemptHack
    {
        static void Main(string[] args)
        {
            if (!File.Exists("config.json"))
            {
                Error.WriteLine(@"""config.json""が存在しません");
                return;
            }
            var config = ConfigReader.Config.Include("config.json");
            var rewrite = args.Length >= 1 && args[0] == "--rewrite";
            if (!File.Exists(config.AttemptHack.TargetExecutableFile))
            {
                Error.WriteLine(@"""AttemptHack.TargetExecutableFile""に存在しないファイルが設定されています");
                return;
            }
            if (!File.Exists(config.JudgeExecutableFile))
            {
                Error.WriteLine(@"""JudgeExecutableFile""に存在しないファイルが設定されています");
                return;
            }
            var result = JudgedProcessRun.JudgedProcess.Run(config.AttemptHack.TargetExecutableFile, config.JudgeExecutableFile, config.TimeLimit, config.AttemptHack.TargetInputFile, out _, out _, out _, rewrite);
            if (result == JudgedProcessRun.JudgedProcess.Result.JudgeTLE)
            {
                ForegroundColor = ConsoleColor.Red;
                Error.WriteLine("ジャッジがTLEしました");
                ForegroundColor = ConsoleColor.White;
            }
            else if (result == JudgedProcessRun.JudgedProcess.Result.Accept)
            {
                Error.WriteLine("Acceptしました");
            }
            else
            {
                ForegroundColor = ConsoleColor.Green;
                Error.WriteLine("Hackに成功しました");
                ForegroundColor = ConsoleColor.White;
                if (!string.IsNullOrEmpty(config.AttemptHack.TargetInputFile))
                {
                    Error.WriteLine($@"詳しくは""{config.AttemptHack.TargetInputFile}""を御覧ください");
                }
            }
        }
    }
}
