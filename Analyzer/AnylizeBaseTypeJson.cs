using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Analyzer
{
    public class AnylizeBaseTypeJson
    {
        static Regex r1 = new Regex(@">(.*?)<\/a>.*>(.*?)<\/span>");
        static Regex r2 = new Regex(@">(.*)");
        public static Dictionary<string, string> GetDictionaryFromJson(BaseType json)
        {
            var textlist = json.Data.Select(x => x[1]).ToList();
            Dictionary<string, string> temp = new Dictionary<string, string>(textlist.Count);
            Match m1, m2;
            textlist.ForEach(x =>
            {
                m1 = r1.Match(x);
                if (m1.Success)
                {
                    string chinese = m1.Groups[1].ToString();
                    if (m1.Groups[1].ToString().TrimStart().StartsWith("<img"))
                    {
                        chinese = r2.Match(chinese).Groups[1].ToString();
                    }
                    if (!temp.ContainsKey(chinese))
                        temp.Add(chinese, m1.Groups[2].ToString());
                }
            });
            return temp;
        }
    }
}
