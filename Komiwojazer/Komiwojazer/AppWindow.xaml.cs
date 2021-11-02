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
    /// Interaction logic for AppWindow.xaml
    /// </summary>
    public partial class AppWindow : Window
    {
        private int flag { get; set; } = -1;
        public AppWindow()
        {
            InitializeComponent();
        }

        private void endButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CanvasImage_MouseDown(object sender, MouseButtonEventArgs e)
        {

            var coord = e.GetPosition(this.CanvasImage);
            //MessageBox.Show($"X:{coord.X}, Y:{coord.Y}");
            var punkt = new Ellipse();
            if (coord.X < 764 && coord.Y < 577
                && coord.X>2 && coord.Y>3)
            {
                punkt.Width = 10;
                punkt.Height = 10;
                if (flag == 1) punkt.Fill = Brushes.Red;
                else if (flag == 2) punkt.Fill = Brushes.Blue;
                Canvas.SetLeft(punkt, coord.X - 5);
                Canvas.SetTop(punkt, coord.Y - 5);
                CanvasImage.Children.Add(punkt);
            }
        }

        private void CanvasImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void CanvasImage_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void CanvasImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void startPointButton_Click(object sender, RoutedEventArgs e)
        {
            flag = 1;
        }

        private void endPointsButton_Click(object sender, RoutedEventArgs e)
        {
            flag = 2;
        }
    }
}
