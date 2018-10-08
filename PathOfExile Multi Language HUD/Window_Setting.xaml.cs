using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static Analyzer.AnalyzeTxt;

namespace PathOfExile_Multi_Language_HUD
{
    /// <summary>
    /// Window_Setting.xaml 的互動邏輯
    /// </summary>
    public partial class Window_Setting : Window
    {
        Settings settings=new Settings();
        public Window_Setting()
        {
            InitializeComponent();
            settings = Settings.Reload();
            for (double i = 7.5; i < 28; i = i + 0.5)
                Combobox_FontSize.Items.Add(i);
            Combobox_FontSize.SelectedItem = settings.FontSize;
            Combobox_AddonFile.ItemsSource = Dictionary_Addons.ReloadList();
            Combobox_AddonFile.SelectedItem = settings.AddonFile;
            Combobox_Language.ItemsSource= MainWindow.Descriptions_Chinese.LanguageList;
            Combobox_Language.SelectedIndex = Combobox_Language.Items.Count > 0 ? 0 : -1;
            TextBox_FontColor.Text= settings.FontColor.ToString();
            Checkbox_AutoCopyToClipboard.IsChecked = settings.AutoCopyToClipboard;
        }

        private void Button_ReloadList_Click(object sender, RoutedEventArgs e)
        {
            Combobox_AddonFile.ItemsSource= Dictionary_Addons.ReloadList();
        }
        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var converter = new System.Windows.Media.BrushConverter();
                try
                {
                    settings.FontColor = (Brush)converter.ConvertFromString(TextBox_FontColor.Text);
                }
                catch
                {
                    MessageBox.Show("文字顏色的色碼應為十六進制數值(Example: #FF000000)");
                    return;
                }
                settings.FontSize = (double)Combobox_FontSize.SelectedItem;
                settings.AddonFile = (string)Combobox_AddonFile.SelectedItem;
                settings.Language = (Language)Enum.Parse(typeof(Language), Combobox_Language.SelectedItem.ToString());
                settings.AutoCopyToClipboard = (bool)Checkbox_AutoCopyToClipboard.IsChecked; 
                Settings.Save(settings);
            }
            catch { MessageBox.Show("Something error!"); return; }
            MessageBox.Show("已儲存");
        }
    }
}
