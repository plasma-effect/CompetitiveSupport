using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static System.Console;

namespace AttemptHack
{
    class AttemptHack
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
            if (!File.Exists(config.AttemptHack.TargetExecutableFile))
            {
                Error.WriteLine(@"""AttemptHack.TargetExecutableFile""に存在しないファイルが設定されています");
                return;
            }
            if(!File.Exists(config.AttemptHack.TargetInputFile))
            {
                Error.WriteLine(@"""AttemptHack.TargetInputFile""に存在しないファイルが設定されています");
                return;
            }
            string input;
            using(var stream = new StreamReader(config.AttemptHack.TargetInputFile))
            {
                input = stream.ReadToEnd();
            }
            if(!ProcessRun.Process.Run(config.ExecutableFile,config.TimeLimit,input,out var mExitCode,out _,out var mout,out var merr))
            {
                ForegroundColor = ConsoleColor.Red;
                Error.WriteLine("あなたの実行ファイルでTime Limit Exceedしました");
                ForegroundColor = ConsoleColor.White;
                return;
            }
            else if (mExitCode != 0)
            {
                ForegroundColor = ConsoleColor.Red;
                Error.WriteLine("あなたの実行ファイルで異常終了しました");
                Error.WriteLine("----------------");
                Error.Write(merr);
                Error.WriteLine("----------------");
                ForegroundColor = ConsoleColor.White;
                return;
            }
            if(!ProcessRun.Process.Run(config.AttemptHack.TargetExecutableFile,config.TimeLimit,input,out var tExitCode,out _,out var tout,out var terr))
            {
                ForegroundColor = ConsoleColor.Green;
                Error.WriteLine("ターゲットの実行ファイルでTime Limit Exceedしました");
                ForegroundColor = ConsoleColor.White;
                return;
            }
            else if (tExitCode != 0)
            {
                ForegroundColor = ConsoleColor.Green;
                Error.WriteLine("ターゲットの実行ファイルで異常終了しました");
                Error.WriteLine("----------------");
                Error.Write(terr);
                Error.WriteLine("----------------");
                ForegroundColor = ConsoleColor.White;
                return;
            }
            using(var stream = new StreamWriter(config.SingleRun.OutputFile))
            {
                stream.Write(mout);
            }
            using(var stream=new StreamWriter(config.AttemptHack.TargetOutputFile))
            {
                stream.Write(tout);
            }
            if (mout != tout)
            {
                ForegroundColor = ConsoleColor.Green;
                Error.WriteLine("あなたの実行ファイルとターゲットの実行ファイルで結果が異なります");
                Error.WriteLine($@"詳しくは""{config.SingleRun.OutputFile}""と""{config.AttemptHack.TargetOutputFile}""を見比べてください");
                ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Error.WriteLine("実行結果が一致しました");
            }
        }
    }
}
