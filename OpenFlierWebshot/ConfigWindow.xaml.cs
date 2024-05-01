using System;
using System.Collections.Generic;
using System.IO;
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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText("Plugins\\openflier.djdjz7.webshot\\browserExecutablePath", PathTextBox.Text);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists("Plugins\\openflier.djdjz7.webshot\\browserExecutablePath"))
                PathTextBox.Text = File.ReadAllText("Plugins\\openflier.djdjz7.webshot\\browserExecutablePath");
        }
    }
}
