using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Analyzer
{
    public class AnalyzeTxt
    {
        public struct AnalyzePack
        {
            public List<Description> Descriptions;
            public Dictionary<char, List<int>>[] Map;
        }
        public static AnalyzePack AnalyzeText(string str, Language language)
        {
            int Index = 0;
            List<Description> temp = new List<Description>();
            Dictionary<char, List<int>>[] Map = new Dictionary<char, List<int>>[20];
            var lines = str.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            Description text = null;
            string lang = "English";
            Dictionary<string, Language> Dic_Language = new Dictionary<string, Language>();
            Regex regex_description = new Regex(@"^\s*description");
            Regex regex_lang = new Regex(@"lang\s+""(.*?)""");
            Regex regex_ID = new Regex(@"\d\s+(.*)");
            Regex regex_Text = new Regex("(.*?)\\s+\"(.*?)\"\\s*(.*)");
            for (int i = 0; i < lines.Count; i++)
            {
                if (regex_description.IsMatch(lines[i]))
                {
                    if (text != null)
                    {
                        temp.Add(text);
                        Map = MakeMap(text.Text, Map,Index++);
                    }
                    text = new Description();
                    lang = "English";
                }
                else
                {
                    if (text == null)
                        continue;
                    var m = regex_lang.Match(lines[i]);
                    if (m.Success)
                        lang = m.Groups[1].Value.ToString();
                    else
                    {
                        if (string.IsNullOrEmpty(text.ID))
                        {
                            var m2 = regex_ID.Match(lines[i]);
                            text.ID = m2.Groups[1].ToString();
                        }
                        else
                        {
                            lines[i] = lines[i].Trim();
                            if (!int.TryParse(lines[i], out int ttt))
                            {
                                var m3 = regex_Text.Match(lines[i]);
                                if (m3.Success)
                                {
                                    if (Dic_Language.ContainsKey(lang))
                                        text.Text.Add(new Text() { Language = Dic_Language[lang]});
                                    else
                                    {
                                        Text _text = new Text();
                                        try
                                        {
                                            if (lang.Contains(" "))
                                                _text.Language = (Language)Enum.Parse(typeof(Language), lang.Replace(" ", "_"));
                                            else
                                                _text.Language = (Language)Enum.Parse(typeof(Language), lang);
                                            text.Text.Add(_text);
                                        }
                                        catch { }
                                        Dic_Language.Add(lang, _text.Language);
                                    }
                                    string _temp = "";
                                    m3.Groups[2].ToString().Split(' ').ToList().ForEach(x =>
                                    {
                                        if (x.Contains("%"))
                                        {
                                            if(x.Contains("，%"))
                                            {
                                                var split = x.Split('，');
                                                if(split.Length==1)
                                                _temp += " ([\\d.+\\-%]+)";
                                                else
                                                    for(int j=0;j<split.Length;j++)
                                                    {
                                                        if (j == 0)
                                                            _temp += split[j].Replace("(", "\\(").Replace(")", "\\)").Replace(".", "\\.").Replace("*", "\\*");
                                                        else
                                                            _temp += "，" + "([\\d.+\\-%]+)";
                                                    }
                                            }
                                                else
                                            _temp += " ([\\d.+\\-%]+)";                                            
                                        }
                                        else
                                            _temp += " " + x.Replace("(","\\(").Replace(")", "\\)").Replace(".", "\\.").Replace("*", "\\*");
                                    });
                                    text.Text.Last().Line.Add(new Line() { Head = m3.Groups[1].ToString(), Content = _temp.Trim(), Tail = m3.Groups[3].ToString() });
                                }
                            }
                        }
                    }
                }
            }
            temp.Add(text);
            Map = MakeMap(text.Text, Map, Index++);
            return new AnalyzePack() { Descriptions = temp, Map = Map };
        }
        private static Dictionary<char, List<int>>[] MakeMap(List<Text> texts, Dictionary<char, List<int>>[] Map,int Index)
        {
            for (int i = 0; i < Map.Length; i++)
                if (Map[i] == null)
                    Map[i] = new Dictionary<char, List<int>>();
            texts.ForEach(x => x.Line.ForEach(y =>
            {
                string temp=Regex.Replace(y.Content.Replace("([\\d.+\\-%]+)",""), "([\\d\\s.+\\-%]+)", "");                
                for (int k = 0; k < 20; k++)
                    if (temp.Length > k)
                        if (Map[k].ContainsKey(temp[k]))
                            Map[k][temp[k]].Add(Index);
                        else
                            Map[k].Add(temp[k], new List<int>() { Index });
            }));
            return Map;
        }
        public static AnalyzePack AnalyzeTextFile(string path)
        {
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (var sr = new StreamReader(fs))
            {
                return AnalyzeText(sr.ReadToEnd(),0);
            }
        }
        public class Description
        {
            public Description()
            {
                Text = new List<Text>();
            }
            public int Index { get; set; }
            public string ID { get; set; }
            public List<Text> Text { get; set; }
        }
        public class Text
        {
            public Text()
            {
                Language = Language.Unknown;
                Line = new List<Line>();
            }
            public Language Language { get; set; }
            public List<Line> Line { get; set; }
        }
        public class Line
        {
            public string Head;
            public string Content;
            public string Tail;
        }
        public enum Language
        {
            English,
            Portuguese,
            Traditional_Chinese,
            Simplified_Chinese,
            Thai,
            Russian,
            French,
            German,
            Spanish,
            Unknown
        }
    }

}
