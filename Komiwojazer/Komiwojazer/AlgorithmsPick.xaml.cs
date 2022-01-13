using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Komiwojazer
{
    /// <summary>
    /// Interaction logic for AlgorithmsPick.xaml
    /// </summary>
    public partial class AlgorithmsPick : Window
    {
        public AlgorithmsPick()
        {
            InitializeComponent();
            this.ShowDialog();
            this.Topmost = true;
        }

        public void buttonAP_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
