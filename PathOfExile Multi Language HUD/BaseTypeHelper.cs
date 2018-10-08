using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PathOfExile_Multi_Language_HUD
{
    public class BaseTypeHelper
    {
        public static async Task DownloadAsync(string url=null,string filename=null)
        {
            WebClient webClient = new WebClient();
            if (!string.IsNullOrWhiteSpace(url) && !string.IsNullOrWhiteSpace(filename))
            {
                await webClient.DownloadFileTaskAsync(url, filename);
                return;
            }
            URL_List.ForEach(x =>
                 new WebClient().DownloadFile(x, Regex.Replace(x, "(.*?)cn=(.*)", "BaseType/$2.json"))
            );
        }
        private static List<string> URL_List { get; set; } = new List<string>(new string[] {
            //寶石
            "http://poedb.tw/json.php/item_class?cn=Active+Skill+Gem",
            "http://poedb.tw/json.php/item_class?cn=Support+Skill+Gem",
            //單手
            "http://poedb.tw/json.php/item_class?cn=Claw",
            "http://poedb.tw/json.php/item_class?cn=Dagger",
            "http://poedb.tw/json.php/item_class?cn=Wand",
            "http://poedb.tw/json.php/item_class?cn=One+Hand+Sword",
            "http://poedb.tw/json.php/item_class?cn=Thrusting+One+Hand+Sword",
            "http://poedb.tw/json.php/item_class?cn=One+Hand+Axe",
            "http://poedb.tw/json.php/item_class?cn=One+Hand+Mace",
            "http://poedb.tw/json.php/item_class?cn=Sceptre",
            //雙手
            "http://poedb.tw/json.php/item_class?cn=Bow",
            "http://poedb.tw/json.php/item_class?cn=Staff",
            "http://poedb.tw/json.php/item_class?cn=Two+Hand+Sword",
            "http://poedb.tw/json.php/item_class?cn=Two+Hand+Axe",
            "http://poedb.tw/json.php/item_class?cn=Two+Hand+Mace",
            "http://poedb.tw/json.php/item_class?cn=FishingRod",
            //副手
            "http://poedb.tw/json.php/item_class?cn=Shield",
            "http://poedb.tw/json.php/item_class?cn=Quiver",
            //護甲
            "http://poedb.tw/json.php/item_class?cn=Gloves",
            "http://poedb.tw/json.php/item_class?cn=Boots",
            "http://poedb.tw/json.php/item_class?cn=Body+Armour",
            "http://poedb.tw/json.php/item_class?cn=Helmet",
            //飾品
            "http://poedb.tw/json.php/item_class?cn=Amulet",
            "http://poedb.tw/json.php/item_class?cn=Ring",
            "http://poedb.tw/json.php/item_class?cn=Belt",
            //藥劑
            "http://poedb.tw/json.php/item_class?cn=LifeFlask",
            "http://poedb.tw/json.php/item_class?cn=ManaFlask",
            "http://poedb.tw/json.php/item_class?cn=HybridFlask",
            "http://poedb.tw/json.php/item_class?cn=UtilityFlask",
            "http://poedb.tw/json.php/item_class?cn=UtilityFlaskCritical",
            //其他
            "http://poedb.tw/json.php/item_class?cn=StackableCurrency",
            "http://poedb.tw/json.php/item_class?cn=MapFragment",
            "http://poedb.tw/json.php/item_class?cn=HideoutDoodad",
            "http://poedb.tw/json.php/item_class?cn=Microtransaction",
            "http://poedb.tw/json.php/item_class?cn=DivinationCard",
            "http://poedb.tw/json.php/item_class?cn=MiscMapItem",
            "http://poedb.tw/json.php/item_class?cn=PantheonSoul",
            "http://poedb.tw/json.php/item_class?cn=UniqueFragment",
            "http://poedb.tw/json.php/item_class?cn=Jewel",
            "http://poedb.tw/json.php/item_class?cn=AbyssJewel",
            //帝王迷宮
            "http://poedb.tw/json.php/item_class?cn=LabyrinthItem",
            "http://poedb.tw/json.php/item_class?cn=LabyrinthTrinket",
            "http://poedb.tw/json.php/item_class?cn=LabyrinthMapItem"
        });
    }
}
