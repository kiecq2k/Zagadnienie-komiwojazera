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

        private List<Ellipse> points { get; set; } = new List<Ellipse>();
        private Ellipse startingPoint { get; set; } = null;
        private int licznik { get; set; } = 0;

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
            if (coord.X < 764 && coord.Y < 577 && coord.X > 2 && coord.Y > 3)
            {
                if (flag == 1)
                {
                    if (startingPoint == null)
                    {
                        startingPoint = new Ellipse();
                        startingPoint.Width = 10;
                        startingPoint.Height = 10;
                        startingPoint.Fill = Brushes.Red;
                        CanvasImage.Children.Add(startingPoint);
                    }
                    Canvas.SetLeft(startingPoint, coord.X - 5);
                    Canvas.SetTop(startingPoint, coord.Y - 5);
                }
                else if (flag == 2)
                {
                    points.Add(new Ellipse());
                    points[licznik].Width = 10;
                    points[licznik].Height = 10;
                    points[licznik].Fill = Brushes.Blue;
                    Canvas.SetLeft(points[licznik], coord.X - 5);
                    Canvas.SetTop(points[licznik], coord.Y - 5);
                    CanvasImage.Children.Add(points[licznik++]);
                }
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

        private void removePointsButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var point in points)
            {
                CanvasImage.Children.Remove(point);
            }
            licznik = 0;
        }

        private void startAlgorithmButton_Click(object sender, RoutedEventArgs e)
        {
            if (startingPoint == null || licznik < 1)
            {
                MessageBox.Show("Nie wybrano punktów");
            }
        }
    }
}
