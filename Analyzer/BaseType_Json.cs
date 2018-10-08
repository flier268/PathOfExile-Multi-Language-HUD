using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Analyzer
{
    public partial class BaseType
    {
        public static Dictionary<string, string>[] AnalyzeJsonFile(string path, Dictionary<string, string>[] Dictionary_BaseType)
        {
            Dictionary_BaseType[0]=Dictionary_BaseType[0] ?? new Dictionary<string, string>();
            Dictionary_BaseType[1] = Dictionary_BaseType[1] ?? new Dictionary<string, string>();
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (var sr = new StreamReader(fs))
            {
                Analyzer.AnylizeBaseTypeJson.GetDictionaryFromJson(BaseType.FromJson(sr.ReadToEnd())).ToList().ForEach(y =>
                {
                    if (!Dictionary_BaseType[1].ContainsKey(y.Key))
                        Dictionary_BaseType[1].Add(y.Key, y.Value);
                    if (!Dictionary_BaseType[0].ContainsKey(y.Value))
                        Dictionary_BaseType[0].Add(y.Value, y.Key);
                });
            }
            return Dictionary_BaseType;
        }

        [JsonProperty("caption")]
        public string Caption { get; set; }

        [JsonProperty("data")]
        public List<List<string>> Data { get; set; }

        [JsonProperty("columns")]
        public List<Column> Columns { get; set; }
    }

    public partial class Column
    {
        [JsonProperty("title")]
        public string Title { get; set; }
    }

    public partial class BaseType
    {
        public static BaseType FromJson(string json) => JsonConvert.DeserializeObject<BaseType>(json, Analyzer.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this BaseType self) => JsonConvert.SerializeObject(self, Analyzer.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
