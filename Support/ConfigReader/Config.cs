using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace ConfigReader
{
    public class ConfigData
    {
        public class Single
        {
            public string InputFile { set; get; }
            public string OutputFile { set; get; }
        }
        public class Example
        {
            public string ExampleInputFileDirectory { set; get; }
            public string ExampleInputFilePrefix { set; get; }
            public string ExampleOutputFileDirectory { set; get; }
            public string ExampleOutputFilePrefix { set; get; }
        }
        public class Hack
        {
            public string TargetExecutableFile { set; get; }
            public string TargetInputFile { set; get; }
            public string TargetOutputFile { set; get; }
        }

        public string ExecutableFile { set; get; }
        public string JudgeExecutableFile { set; get; }
        public int TimeLimit { set; get; }
        public Single SingleRun { set; get; }
        public Example ExampleRun { set; get; }
        public Hack AttemptHack { set; get; }
    }

    public static class Config
    {
        public static void Exclude(string path, ConfigData data)
        {
            var option = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            using (var stream = new StreamWriter(path))
            {
                stream.Write(JsonSerializer.Serialize(data, option));
            }
        }
        public static ConfigData Include(string path)
        {
            using (var stream = new StreamReader(path))
            {
                return JsonSerializer.Deserialize<ConfigData>(stream.ReadToEnd());
            }
        }
    }
}
