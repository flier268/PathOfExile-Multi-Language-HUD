using Analyzer;
using Poe整理倉庫v2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace PathOfExile_Multi_Language_HUD
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        AnalyzeTxt.AnalyzePack Descriptions_Origin { get; set; }
        public static AnalyzeTxt.AnalyzePack Descriptions_Chinese { get; set; }
        Dictionary<string, string>[] Dictionary_ItemName { get; set; } = new Dictionary<string, string>[2];
        Dictionary<string, string>[] Dictionary_BaseType { get; set; } = new Dictionary<string, string>[2];
        Settings Settings = new Settings();
        IntPtr Handle;
        RECT POE_Rect = new RECT();
        bool POE_Foreground = false;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Settings.Reload();
            TextSize = Settings.FontSize;
            TextColor = Settings.FontColor;

            Descriptions_Origin = AnalyzeTxt.AnalyzeTextFile(App.Filename_Origin);
            Descriptions_Chinese = AnalyzeTxt.AnalyzeTextFile(App.Filename_Translate);
            Dictionary_ItemName = AnalyzeDatcs.GetDictionary(AnalyzeDatcs.ConvertDatToCSV(App.Filename_Words));
            
            if (!Directory.Exists(App.Directory_BaseType) || !File.Exists(Path.Combine(App.Directory_BaseType, "Active+Skill+Gem.json")))
                BaseTypeHelper.DownloadAsync().Wait();
            var jsonList = Directory.GetFiles(App.Directory_BaseType, "*.json").ToList();

            foreach(var x in jsonList)            
                Dictionary_BaseType = BaseType.AnalyzeJsonFile(x, Dictionary_BaseType);

            if (!string.IsNullOrWhiteSpace(Settings.AddonFile))
                Dictionary_Addons.Reload(Settings.AddonFile);
            ApplicationHelper applicationHelper = new ApplicationHelper();
            applicationHelper.ForegroundWindowChanged += () =>
            {
                POE_Foreground = ApplicationHelper.IsPathOfExileTop(Handle);
                Visibility = POE_Foreground ? Visibility.Visible : Visibility.Hidden;
                if (POE_Foreground)
                {
                    POE_Rect = ApplicationHelper.PathOfExileDimentions;
                    Left = POE_Rect.Left;
                    Top = POE_Rect.Top;
                    Width = POE_Rect.Right - POE_Rect.Left;
                    Height = POE_Rect.Bottom - POE_Rect.Top;

                }
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region 註冊Hook並監聽剪貼簿        
            WindowInteropHelper wih = new WindowInteropHelper(this);
            hWndSource = HwndSource.FromHwnd(wih.Handle);
            hWndSource.AddHook(this.WinProc);
            mNextClipBoardViewerHWnd = SetClipboardViewer(hWndSource.Handle);
            #endregion
            Handle = wih.Handle;
        }
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            ChangeClipboardChain(hWndSource.Handle, mNextClipBoardViewerHWnd);
        }

        private string reg = @":\s(.*?)\r\n(.*?)\r\n(.*?)\r\n--------\r\n(.*)";

        public string ConvertToChinese(string clip)
        {
            Regex r_WithBrackets = new Regex("\\((.*?)\\)");
            Regex r = new Regex(reg, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = r.Match(clip);
            StringBuilder sb = new StringBuilder();
            var lines = clip.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();

            //辨別用A到B還是B到A

            string A_To_B = lines[0];
            string B_To_A = lines[0];
            Dictionary_Addons.A_To_B.ToList().ForEach(x => A_To_B = A_To_B.Replace(x.Key, x.Value));
            Dictionary_Addons.B_To_A.ToList().ForEach(x => B_To_A = B_To_A.Replace(x.Key, x.Value));
            var diffRate1 = PrivateFunction.CalcDifference_Rate(lines[0], A_To_B);
            var diffRate2 = PrivateFunction.CalcDifference_Rate(lines[0], B_To_A);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (diffRate1 > diffRate2)
            {
                dic = Dictionary_Addons.A_To_B;
                sb.AppendLine(A_To_B);
            }
            else
            {
                dic = Dictionary_Addons.B_To_A;
                sb.AppendLine(B_To_A);
            }
            
            

            //物品名稱
            var m_WithBrackets = r_WithBrackets.Match(m.Groups[2].ToString());
            //如果物品名稱有括號
            if (m_WithBrackets.Success)
            {
                sb.AppendLine(m_WithBrackets.Groups[1].ToString());
            }
            else
            {
                if (Dictionary_ItemName[0].ContainsKey(m.Groups[2].ToString()))
                    sb.AppendLine(Dictionary_ItemName[0][m.Groups[2].ToString()]);
                else if (Dictionary_ItemName[1].ContainsKey(m.Groups[2].ToString()))
                    sb.AppendLine(Dictionary_ItemName[1][m.Groups[2].ToString()]);
                else
                    sb.AppendLine(m.Groups[2].ToString());
            }

            //基底
            m_WithBrackets = r_WithBrackets.Match(m.Groups[3].ToString());
            //如果物品名稱有括號
            if (m_WithBrackets.Success)
            {
                sb.AppendLine(m_WithBrackets.Groups[1].ToString());
            }
            else
            {
                if (Dictionary_BaseType[0].ContainsKey(m.Groups[3].ToString()))
                    sb.AppendLine(Dictionary_BaseType[0][m.Groups[3].ToString()]);
                else if (Dictionary_BaseType[1].ContainsKey(m.Groups[3].ToString()))
                    sb.AppendLine(Dictionary_BaseType[1][m.Groups[3].ToString()]);
                else
                    sb.AppendLine(m.Groups[3].ToString());
            }
            sb.AppendLine("--------");

            lines = m.Groups[4].ToString().Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
            lines.ForEach(x =>
            {
                if(x== "--------")
                {
                    sb.AppendLine(x);
                }
                else
                {
                    
                    var g = dic.Aggregate(x, (resoult, s) => Regex.Replace(resoult, s.Key, ConvertToReplacement(s.Value)));
                    if (g != x)
                    {
                        sb.AppendLine(g);
                    }
                    else
                    {
                        bool Converted = false;
                        /*%寫成%%
                        %1%代表第一個參數
                        %1$+d
                        %1$d
                        %2$d  = %1$d * 2
                        %3$d  = %1$d * 3
                        */

                        List<int> MatchedIndex = new List<int>();
                        {
                            string temp = Regex.Replace(x, "([\\d\\s.+\\-%]+)", "");
                            for (int k = 0; k < Descriptions_Chinese.Map.Length; k++)
                                if (temp.Length > k)
                                    if (Descriptions_Chinese.Map[k].ContainsKey(temp[k]))
                                        if (MatchedIndex.Count == 0)
                                            MatchedIndex = Descriptions_Chinese.Map[k][temp[k]];
                                        else
                                            MatchedIndex = MatchedIndex.Intersect(Descriptions_Chinese.Map[k][temp[k]]).ToList();
                        }
                        if (MatchedIndex.Count > 0)
                            for (int i = 0; i < MatchedIndex.Count; i++)
                            {
                                var temp = Descriptions_Chinese.Descriptions[MatchedIndex[i]].Text.Where(y => y.Line.Where(z =>
                                {
                                    try
                                    {
                                        return Regex.IsMatch(x, String.Format("^{0}$", z.Content));
                                    }
                                    catch
                                    {
                                        return false;
                                    }
                                }).Count() > 0).ToList();
                                if (temp.Count() > 0)
                                {
                                    var temp_origin = Descriptions_Origin.Descriptions.Where(y => y.ID == Descriptions_Chinese.Descriptions[MatchedIndex[i]].ID).ToList();
                                    if (temp_origin.Count > 0)
                                    {
                                        var temp2 = temp_origin[0].Text.Where(y => y.Language == Settings.Language).Where(y => y.Line.Where(z => z.Head == temp[0].Line[0].Head).Count() > 0).ToList();
                                        if (temp2.Count > 0)
                                        {
                                            sb.AppendLine(Regex.Replace(x, temp[0].Line[0].Content, ConvertToReplacement(temp2[0].Line[0].Content)).Trim());
                                            Converted = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        if (!Converted)
                            sb.AppendLine(x);
                    }
                    
                }
            });
            return sb.ToString();
        }
        string ConvertToReplacement(string str, string replace = "([\\d.+\\-%]+)")
        {
            int index = 1;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                int t = str.IndexOf(replace, i);
                if (t >= 0)
                {
                    sb.AppendFormat("{0}${1}", str.Substring(i, t - i), index++);
                    i = t + replace.Length;
                }
                else
                {
                    if (i == 0)
                        sb.AppendFormat("{0}", str.Substring(0));
                    else
                        sb.AppendFormat("{0}", str.Substring(i - 1));
                    break;
                }
            }
            return sb.ToString();
        }

        private IntPtr WinProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_CHANGECBCHAIN:
                    if (wParam == mNextClipBoardViewerHWnd)
                    {
                        // clipboard viewer chain changed, need to fix it. 
                        mNextClipBoardViewerHWnd = lParam;
                    }
                    else if (mNextClipBoardViewerHWnd != IntPtr.Zero)
                    {
                        // pass the message to the next viewer. 
                        SendMessage(mNextClipBoardViewerHWnd, msg, wParam, lParam);
                    }
                    break;

                case WM_DRAWCLIPBOARD:
                    if (!POE_Foreground)
                        break;
                    // clipboard content changed 
                    if (Clipboard.ContainsText())
                    {
                        string clip = Clipboard.GetText(TextDataFormat.UnicodeText);
                        if (clip.Contains("--------"))
                        {
                            if (!clip.Contains("Convert by PathOfExile Multi Language HUD."))
                            {
                                clip = ConvertToChinese(clip);
                                clip += "\r\n--------\r\nNote: Convert by PathOfExile Multi Language HUD.";
                                if (Settings.AutoCopyToClipboard)
                                    Clipboard.SetDataObject(clip);


                                tooltip.Text = clip;
                                var mousePos = Win32.GetMousePosition();
                                var p = ApplicationHelper.PathOfExilePosition(new POINT((int)mousePos.X, (int)mousePos.Y));
                                if (p.Left > p.Right)
                                    tooltip.Margin = new Thickness(10, 20, 0, 0);
                                else
                                    tooltip.Margin = new Thickness((p.Left + p.Right) / 2, 20, 0, 0);
                                tooltip_Visibility = Visibility.Visible;
                                Task.Run(() =>
                                {
                                    while (true)
                                    {
                                        if (!Win32.GetMousePosition().Equals(mousePos))
                                        {
                                            tooltip_Visibility = Visibility.Collapsed;
                                            break;
                                        }
                                        System.Threading.SpinWait.SpinUntil(() => false, 100);
                                    }
                                });
                            }
                        }
                    }
                    // pass the message to the next viewer. 
                    SendMessage(mNextClipBoardViewerHWnd, msg, wParam, lParam);
                    break;
            }

            return IntPtr.Zero;
        }
        HwndSource hWndSource;
        #region Definitions  
        //Constants for API Calls...  
        private const int WM_DRAWCLIPBOARD = 0x308;
        private const int WM_CHANGECBCHAIN = 0x30D;

        //Handle for next clipboard viewer...  
        private IntPtr mNextClipBoardViewerHWnd;

        //API declarations...  
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool ChangeClipboardChain(IntPtr HWnd, IntPtr HWndNext);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        #endregion

        private Visibility _tooltip_Visibility;
        public Visibility tooltip_Visibility { get => _tooltip_Visibility; set { _tooltip_Visibility = value; OnPropertyChanged(); } }
        private Brush _TextColor;
        public Brush TextColor { get => _TextColor; set { _TextColor = value;OnPropertyChanged(); } }
        private double _TextSize;
        public double TextSize { get => _TextSize; set { _TextSize = value; OnPropertyChanged(); } }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void MenuItem_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItem_Setting_Click(object sender, RoutedEventArgs e)
        {
            new Window_Setting().ShowDialog();
            Settings=Settings.Reload();
            TextColor = Settings.FontColor;
            TextSize = Settings.FontSize;
            Dictionary_Addons.Reload(Settings.AddonFile);
        }
    }
}
