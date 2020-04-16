using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static System.Console;

namespace SingleRun
{
    class SingleRun
    {
        static void Main(string[] args)
        {
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
            if (!File.Exists(config.SingleRun.InputFile))
            {
                Error.WriteLine(@"""SingleRun.InputFile""に存在しないファイルが設定されています");
                return;
            }
            string input;
            using (var stream = new StreamReader(config.SingleRun.InputFile))
            {
                input = stream.ReadToEnd();
            }
            if (!ProcessRun.Process.Run(config.ExecutableFile, config.TimeLimit, input, out var exitCode, out var executeTime, out var cout, out var cerr))
            {
                ForegroundColor = ConsoleColor.Red;
                Error.WriteLine("Time Limit Exceed");
                ForegroundColor = ConsoleColor.White;
                return;
            }
            WriteLine($"実行時間: {executeTime}ms");
            if (exitCode != 0)
            {
                ForegroundColor = ConsoleColor.Red;
                Error.WriteLine("異常終了しました");
                Error.WriteLine("----------------");
                Error.Write(cerr);
                Error.WriteLine("----------------");
                ForegroundColor = ConsoleColor.White;
            }
            else
            {
                ForegroundColor = ConsoleColor.Green;
                WriteLine("正常に終了しました");
                ForegroundColor = ConsoleColor.White;
            }
            using (var stream = new StreamWriter(config.SingleRun.OutputFile))
            {
                stream.Write(cout);
            }
        }
    }
}
