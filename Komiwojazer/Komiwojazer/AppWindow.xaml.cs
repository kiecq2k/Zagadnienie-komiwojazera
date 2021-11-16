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
        private int _flag = -1;

        private IList<Ellipse> _points = new List<Ellipse>();
        private Ellipse _startingPoint = null;
        private int _counter = 0;

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
                if (_flag == 1)
                {
                    if (_startingPoint == null)
                    {
                        _startingPoint = new Ellipse();
                        _startingPoint.Width = 10;
                        _startingPoint.Height = 10;
                        _startingPoint.Fill = Brushes.Red;
                        CanvasImage.Children.Add(_startingPoint);
                    }
                    Canvas.SetLeft(_startingPoint, coord.X - 5);
                    Canvas.SetTop(_startingPoint, coord.Y - 5);
                }
                else if (_flag == 2)
                {
                    _points.Add(new Ellipse());
                    _points[_counter].Width = 10;
                    _points[_counter].Height = 10;
                    _points[_counter].Fill = Brushes.Blue;
                    Canvas.SetLeft(_points[_counter], coord.X - 5);
                    Canvas.SetTop(_points[_counter], coord.Y - 5);
                    CanvasImage.Children.Add(_points[_counter++]);
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
            _flag = 1;
        }

        private void endPointsButton_Click(object sender, RoutedEventArgs e)
        {
            _flag = 2;
        }

        private void removePointsButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var point in _points)
            {
                CanvasImage.Children.Remove(point);
            }
            _counter = 0;
        }

        private void startAlgorithmButton_Click(object sender, RoutedEventArgs e)
        {
            if (_startingPoint == null || _counter < 1)
            {
                MessageBox.Show("Nie wybrano punktów");
            }
        }
    }
}
