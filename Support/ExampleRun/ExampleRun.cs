using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static System.Console;
using static System.Linq.Enumerable;

namespace ExampleRun
{
    class ExampleRun
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Error.WriteLine("実行するファイル数を設定してください");
                return;
            }
            var N = int.Parse(args[0]);
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
            if (config.ExampleRun.ExampleInputFileDirectory == null ||
                config.ExampleRun.ExampleInputFilePrefix == null ||
                config.ExampleRun.ExampleOutputFileDirectory == null ||
                config.ExampleRun.ExampleOutputFilePrefix == null)
            {
                Error.WriteLine(@"""ExampleRun""の中身が設定されていません");
                return;
            }
            var inputPrefix = config.ExampleRun.ExampleInputFileDirectory + config.ExampleRun.ExampleInputFilePrefix;
            var outputPrefix = config.ExampleRun.ExampleOutputFileDirectory + config.ExampleRun.ExampleOutputFilePrefix;
            var wa = new SortedSet<int>();
            var re = new SortedSet<int>();
            var tle = new SortedSet<int>();
            foreach (var i in Range(1, N))
            {
                var exampleFile = inputPrefix + $"{i}.txt";
                var answerFile = outputPrefix + $"{i}.txt";
                if (!File.Exists(exampleFile) || !File.Exists(answerFile))
                {
                    continue;
                }
                string input, answer;
                using (var stream = new StreamReader(exampleFile))
                {
                    input = stream.ReadToEnd();
                }
                using (var stream = new StreamReader(answerFile))
                {
                    answer = stream.ReadToEnd();
                }
                WriteLine($" input: {exampleFile}");
                WriteLine($"output: {answerFile}");
                if (!ProcessRun.Process.Run(config.ExecutableFile, config.TimeLimit, input, out var exitCode, out _, out var cout, out var cerr))
                {
                    tle.Add(i);
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine("Time Limit Exceed");
                    ForegroundColor = ConsoleColor.White;
                }
                else if (exitCode != 0)
                {
                    re.Add(i);
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine("Runtime Error");
                    Write(cerr);
                    ForegroundColor = ConsoleColor.White;
                }
                else if (cout != answer)
                {
                    wa.Add(i);
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine("Wrong Answer");
                    WriteLine();
                    ForegroundColor = ConsoleColor.White;
                    WriteLine("expected answer:");
                    Write(answer);
                    WriteLine();
                    WriteLine("your answer:");
                    Write(cout);
                }
                else
                {
                    ForegroundColor = ConsoleColor.Green;
                    WriteLine("Accept");
                    ForegroundColor = ConsoleColor.White;
                }
                WriteLine("--------------");
            }
            if (re.Count == 0 && wa.Count == 0 && tle.Count == 0)
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
                ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
