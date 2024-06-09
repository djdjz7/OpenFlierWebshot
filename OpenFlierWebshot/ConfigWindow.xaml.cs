using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OpenFlierWebshot
{
    /// <summary>
    /// ConfigWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();
            ScriptDataGrid.ItemsSource = LocalStorage.Config?.JavaScriptMatchEntries;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var config = new Config()
            {
                BrowserExecutablePath = PathTextBox.Text,
                JavaScriptMatchEntries = LocalStorage.Config?.JavaScriptMatchEntries
            };
            LocalStorage.Config = config;
            var content = JsonSerializer.Serialize(config, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText("Plugins\\openflier.djdjz7.webshot\\config.json", content);
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
