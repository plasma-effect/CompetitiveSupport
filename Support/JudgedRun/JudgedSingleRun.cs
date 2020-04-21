using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using static System.Console;

namespace JudgedSingleRun
{
    class JudgedSingleRun
    {
        static void Main(string[] args)
        {
            if (!File.Exists("config.json"))
            {
                Error.WriteLine(@"""config.json""が存在しません");
                return;
            }
            var rewrite = args.Length >= 1 && args[0] == "--rewrite";
            var config = ConfigReader.Config.Include("config.json");
            if (!File.Exists(config.ExecutableFile))
            {
                Error.WriteLine(@"""ExecutableFile""に存在しないファイルが設定されています");
                return;
            }
            if (!File.Exists(config.JudgeExecutableFile))
            {
                Error.WriteLine(@"""JudgeExecutableFile""に存在しないファイルが設定されています");
                return;
            }
            var result = JudgedProcessRun.JudgedProcess.Run(config.ExecutableFile, config.JudgeExecutableFile, config.TimeLimit, config.SingleRun.InputFile, out var time, out var cout, out var cerr, rewrite);
            using (var stream = new StreamWriter(config.SingleRun.OutputFile))
            {
                stream.Write(cout);
            }
            if (result == JudgedProcessRun.JudgedProcess.Result.TLE)
            {
                ForegroundColor = ConsoleColor.Red;
                Error.WriteLine("Time Limit Exceed");
                ForegroundColor = ConsoleColor.White;
                return;
            }
            WriteLine($"実行時間: {time}ms");
            if (result == JudgedProcessRun.JudgedProcess.Result.RE)
            {
                ForegroundColor = ConsoleColor.Red;
                Error.WriteLine("異常終了しました");
                Error.WriteLine("----------------");
                Error.Write(cerr);
                Error.WriteLine("----------------");
                ForegroundColor = ConsoleColor.White;
                return;
            }
            if (result == JudgedProcessRun.JudgedProcess.Result.JudgeTLE)
            {
                ForegroundColor = ConsoleColor.Red;
                Error.WriteLine("ジャッジがTLEしました");
                ForegroundColor = ConsoleColor.White;
                return;
            }
            if (result == JudgedProcessRun.JudgedProcess.Result.WrongAnswer)
            {
                ForegroundColor = ConsoleColor.Red;
                Error.WriteLine("Wrong Answer、もしくはジャッジが異常終了しました");
                ForegroundColor = ConsoleColor.White;
            }
            else
            {
                ForegroundColor = ConsoleColor.Green;
                Error.WriteLine("Acceptしました");
                ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
