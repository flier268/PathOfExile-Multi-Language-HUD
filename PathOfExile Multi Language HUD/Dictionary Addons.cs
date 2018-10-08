using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PathOfExile_Multi_Language_HUD
{
    public class Dictionary_Addons
    {
        public static List<string> ReloadList()
        {
            return Directory.GetFiles(App.Directory_Addon, "Addon.*.txt").ToList();
        }
        public static void Reload(string filename)
        {
            try
            {
                if (!File.Exists(Path.Combine(App.Directory_Addon, filename)))
                    return;
                var fs = new FileStream(Path.Combine(App.Directory_Addon, filename), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (var sr = new StreamReader(fs))
                {
                    var lines = sr.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    A_To_B.Clear();
                    B_To_A.Clear();
                    for (int i = 0; i < lines.Length; i++)
                    {
                        var line = lines[i].Split(new string[] { "<<---->>" }, StringSplitOptions.None);
                        if (line.Length != 2)
                            continue;
                        if (A_To_B.ContainsKey(line[0]))
                            A_To_B[line[0]] = line[1];
                        else
                            A_To_B.Add(line[0], line[1]);
                        if (B_To_A.ContainsKey(line[1]))
                            B_To_A[line[1]] = line[0];
                        else
                            B_To_A.Add(line[1], line[0]);
                    }
                }
            }
            catch { }
        }
        public List<Dictionary<string, string>> Dictionary_AddonsList = new List<Dictionary<string, string>>();
        public static Dictionary<string, string> A_To_B { get; set; } = new Dictionary<string, string>()
        {
            { "稀有度: 通貨不足" , "Rarity: Currency" },
            { "稀有度: 通貨" , "Rarity: Currency" },
            { "稀有度: 寶石" , "Rarity: Gem" },
            { "稀有度:" , "Rarity:" },
            {" 傳奇" , " Unique" },
            {" 稀有" , " Rare" },
            {" 魔法" , " Magic" },
            {" 普通" , " Normal" },
            { "需求:" , "Requirements:" },
            { "物品等級:" , "Item Level:" },
            { "等級:" , "Level:" },
            { "力量:" , "Str:" },
            { "敏捷:" , "Dex:" },
            { "智慧:" , "Int:" },
            { "插槽:" , "Sockets:" },
            { "護甲:" , "Armour:" },
            { "閃避值:" , "Evasion Rating:" },
            { "能量護盾:" , "Energy Shield:" },
            { "元素傷害:" , "Elemental Damage:" },
            { "物理傷害:" , "Physical Damage:" },
            { "混沌傷害:" , "Chaos Damage:" },
            { "暴擊率:" , "Critical Strike Chance:" },
            { "每秒攻擊次數:" , "Attacks per Second:" },
            { "武器範圍:" , "Weapon Range:" },
            { "格擋率:" , "Chance to Block:" },
            { "塑者之物" , "Shaper Item" },
            { "尊師之物" , "Elder Item" },
            { "品質:" , "Quality:" },
            { "未鑑定" , "Unidentified" },
            { "堆疊數量:" , "Stack Size:" },
            { "地圖階級:" , "Map Tier:" },            
            { "已汙染的" , "Corrupted" },
            { "魔力消耗:" , "Mana Reserved:" },
            { "附加傷害效用:" , "Effectiveness of Added Damage:" },
            { "經驗值:" , "Experience:" },
            { "施放時間: ([\\d.+\\-%]+) 秒" , "Cast Time: ([\\d.+\\-%]+) sec" },
            { "持續 ([\\d.+\\-%]+) \\(augmented\\) 秒" , "Lasts ([\\d.+\\-%]+) \\(augmented\\) Seconds" },
            { "持續 ([\\d.+\\-%]+) 秒" , "Lasts ([\\d.+\\-%]+) Seconds" },
            { "每次使用會從 ([\\d.+\\-%]+) 充能次數中消耗 ([\\d.+\\-%]+) 次" , "Consumes ([\\d.+\\-%]+) of ([\\d.+\\-%]+) Charges on use" },
            { "目前有 ([\\d.+\\-%]+) 充能次數" , "Currently has ([\\d.+\\-%]+) Charges" },
        };
        public static Dictionary<string, string> B_To_A { get; set; } = new Dictionary<string, string>()
        {
            { "Rarity: Currency" , "稀有度: 通貨"},
            { "Rarity: Gem" , "稀有度: 寶石"},
            { "Rarity:" , "稀有度:"},
            { " Unique" , " 傳奇"},
            { " Rare" , " 稀有"},
            { " Magic" , " 魔法" },
            { " Normal" , " 普通"},
            { "Requirements:" , "需求:"},
            { "Item Level:" , "物品等級:"},
            { "Level:" , "等級:"},
            { "Str:" , "力量:"},
            { "Dex:" , "敏捷:"},
            { "Int:" , "智慧:"},
            { "Sockets:" , "插槽:"},
            { "Armour:" , "護甲:"},
            { "Evasion Rating:" , "閃避值:"},
            { "Energy Shield:" , "能量護盾:"},
            { "Elemental Damage:" , "元素傷害:"},
            { "Physical Damage:" , "物理傷害:"},
            { "Chaos Damage:" , "混沌傷害:"},
            { "Critical Strike Chance:" , "暴擊率:"},
            { "Attacks per Second:" , "每秒攻擊次數:"},
            { "Weapon Range:" , "武器範圍:"},
            { "Chance to Block:" , "格擋率:"},
            { "Shaper Item" , "塑者之物"},
            { "Elder Item" , "尊師之物"},
            { "Quality:" , "品質:"},
            { "Unidentified" , "未鑑定"},
            { "Stack Size:" , "堆疊數量:"},
            { "Map Tier:" , "地圖階級:"},            
            { "Corrupted" , "已汙染的" },           
            { "Mana Reserved:" , "魔力消耗:"},
            { "Effectiveness of Added Damage:" , "附加傷害效用:"},
            { "Experience:" , "經驗值:"},
            { "Cast Time: ([\\d.+\\-%]+) sec" , "施放時間: ([\\d.+\\-%]+) 秒"},
            { "Lasts ([\\d.+\\-%]+) \\(augmented\\) Seconds" , "持續 ([\\d.+\\-%]+) \\(augmented\\) 秒"},
            { "Lasts ([\\d.+\\-%]+) Seconds" , "持續 ([\\d.+\\-%]+) 秒"},
            { "Consumes ([\\d.+\\-%]+) of ([\\d.+\\-%]+) Charges on use" , "每次使用會從 ([\\d.+\\-%]+) 充能次數中消耗 ([\\d.+\\-%]+) 次"},
            { "Currently has ([\\d.+\\-%]+) Charges" , "目前有 ([\\d.+\\-%]+) 充能次數"}
        };
    }
}
