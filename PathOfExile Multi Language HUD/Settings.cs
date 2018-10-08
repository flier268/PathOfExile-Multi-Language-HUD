using Analyzer;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;
using System.IO;

namespace PathOfExile_Multi_Language_HUD
{
    public partial class Settings
    {
        public Settings()
        {
            UiLanguage = "zh_TW";
            Language = AnalyzeTxt.Language.English;
            AddonFile = "Default";
            FontSize = 12;
            FontColor = "#000000";
            AutoCopyToClipboard = false;
        }
        public static Settings Reload()
        {
            if (!File.Exists(App.Filename_Setting))
                return new Settings();
            var fs = new FileStream(App.Filename_Setting, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (var sr = new StreamReader(fs))
            {
                return Settings.FromJson(sr.ReadToEnd());
            }
        }
        public static void Save(Settings settings)
        {
            using (var sr = new StreamWriter(App.Filename_Setting, false,System.Text.Encoding.UTF8))
            {
                sr.Write(settings.ToJson());
            }
        }

        [JsonProperty("UILanguage")]
        public string UiLanguage { get; set; }

        [JsonProperty("Language")]
        private string _Language { get; set; }

        [JsonProperty("AddonFile")]
        public string AddonFile { get; set; }

        public AnalyzeTxt.Language Language { get => (AnalyzeTxt.Language)Enum.Parse(typeof(AnalyzeTxt.Language), _Language); set { _Language = value.ToString(); } }

        [JsonProperty("FontSize")]
        public long FontSize { get; set; }

        [JsonProperty("FontColor")]
        public string FontColor { get; set; }

        [JsonProperty("AutoCopyToClipboard")]
        public bool AutoCopyToClipboard { get; set; }
    }

    public partial class Settings
    {
        public static Settings FromJson(string json) => string.IsNullOrWhiteSpace(json) ? new Settings() : JsonConvert.DeserializeObject<Settings>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Settings self) => JsonConvert.SerializeObject(self, Formatting.Indented, Converter.Settings);
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
