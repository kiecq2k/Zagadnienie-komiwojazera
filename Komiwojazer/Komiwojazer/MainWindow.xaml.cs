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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Komiwojazer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public enum Version { Demo, Full };
    public partial class MainWindow : Window
    {
        Welcome w = new Welcome();
        public MainWindow()
        {
            w.ShowDialog();
            InitializeComponent();
            
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            var appWindow = new AppWindow();
            appWindow.Show();
            Close();
        }

        private void demoButton_Click(object sender, RoutedEventArgs e)
        {
            var demoWindow = new DemoWindow();
            demoWindow.Show();
            Close();
        }
    }
}
