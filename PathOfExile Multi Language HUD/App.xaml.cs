using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PathOfExile_Multi_Language_HUD
{
    /// <summary>
    /// App.xaml 的互動邏輯
    /// </summary>
    public partial class App : Application
    {
        public static string BasePath { get; set; } = "";
        public static string Directory_Addon { get => "Addon"; }
        public static string Filename_Setting { get => "Settings.json"; }

        public static string Filename_Origin { get => "origin/stat_descriptions.txt"; }
        public static string Filename_Translate { get => "Translate/stat_descriptions.txt"; }
        public static string Filename_Words { get => "Words.dat"; }
        public static string Directory_BaseType { get => "BaseType"; }
    }
}
