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
    /// Interaction logic for AlhorithmsPickFull.xaml
    /// </summary>
    public partial class AlgorithmsPickFull : Window
    {
        public AlgorithmsPickFull()
        {
            InitializeComponent();
            this.ShowDialog();
            this.Topmost = true;
        }

        private void buttonAPF_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
