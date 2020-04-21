using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static System.Console;
using static System.Linq.Enumerable;

namespace JudgedExampleRun
{
    class JudgedExampleRun
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Error.WriteLine("実行するファイル数を設定してください");
                return;
            }
            var N = int.Parse(args[0]);
            var rewrite = args.Length >= 2 && args[1] == "--rewrite";
            if (!File.Exists("config.json"))
            {
                Error.WriteLine(@"""config.json""が存在しません");
                return;
            }
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
            string inputPrefix = null;
            if (config.ExampleRun.ExampleInputFileDirectory != null &&
                config.ExampleRun.ExampleInputFilePrefix != null)
            {
                inputPrefix = config.ExampleRun.ExampleInputFileDirectory + config.ExampleRun.ExampleInputFilePrefix;
            }
            var wa = new SortedSet<int>();
            var re = new SortedSet<int>();
            var tle = new SortedSet<int>();
            var judgeTLE = new SortedSet<int>();
            foreach (var i in Range(1, N))
            {
                string input = null;
                if (inputPrefix != null)
                {
                    input = inputPrefix + $"{i}.txt";
                }
                if (!string.IsNullOrEmpty(input))
                {
                    WriteLine($"input: {input}");
                }
                else
                {
                    WriteLine($@"input: ""null""");
                }
                var result = JudgedProcessRun.JudgedProcess.Run(config.ExecutableFile, config.JudgeExecutableFile, config.TimeLimit, input, out _, out _, out var cerr, rewrite);
                if (result == JudgedProcessRun.JudgedProcess.Result.TLE)
                {
                    tle.Add(i);
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine("Time Limit Exceed");
                    ForegroundColor = ConsoleColor.White;
                }
                else if(result==JudgedProcessRun.JudgedProcess.Result.RE)
                {
                    re.Add(i);
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine("Runtime Error");
                    Write(cerr);
                    ForegroundColor = ConsoleColor.White;
                }
                else if (result == JudgedProcessRun.JudgedProcess.Result.WrongAnswer)
                {
                    wa.Add(i);
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine("Wrong Answer");
                    ForegroundColor = ConsoleColor.White;
                }
                else if(result == JudgedProcessRun.JudgedProcess.Result.JudgeTLE)
                {
                    judgeTLE.Add(i);
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine("ジャッジがTLEしました");
                    ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    ForegroundColor = ConsoleColor.Green;
                    WriteLine("Accept");
                    ForegroundColor = ConsoleColor.White;
                }
                WriteLine("--------------");
            }
            if(re.Count==0&&wa.Count==0&&tle.Count==0&&judgeTLE.Count==0)
            {
                ForegroundColor = ConsoleColor.Green;
                WriteLine($"All tests passed!!");
                ForegroundColor = ConsoleColor.White;
            }
            else
            {
                ForegroundColor = ConsoleColor.Red;
                if (re.Count != 0)
                {
                    WriteLine("Exist Runtime Error Result");
                    foreach (var i in re)
                    {
                        Write($"{i} ");
                    }
                    WriteLine();
                    WriteLine();
                }
                if (wa.Count != 0)
                {
                    WriteLine("Exist Wrong Answer Result");
                    foreach (var i in wa)
                    {
                        Write($"{i} ");
                    }
                    WriteLine();
                    WriteLine();
                }
                if (tle.Count != 0)
                {
                    WriteLine("Exist Time Limit Exceed Result");
                    foreach (var i in tle)
                    {
                        Write($"{i} ");
                    }
                    WriteLine();
                    WriteLine();
                }
                if(judgeTLE.Count!=0)
                {
                    WriteLine("Exist Judge TLE Result");
                    foreach (var i in tle)
                    {
                        Write($"{i} ");
                    }
                    WriteLine();
                    WriteLine();
                }
                ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
