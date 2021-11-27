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
        private IList<Line> _lines = new List<Line>();
        private int _linesCounter = 0;
        //private IList<Ellipse> _points = new List<Ellipse>();
        private Points _startingPoint = null;
        private IList<Points> _points = new List<Points>();
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

            if (coord.IsOnRoad())
            {
                if (_flag == 1)
                {
                    if (_startingPoint == null)
                    {
                        _startingPoint = new Points();
                        _startingPoint.point = new Ellipse();
                        _startingPoint.point.Width = 10;
                        _startingPoint.point.Height = 10;
                        _startingPoint.point.Fill = Brushes.Red;
                        CanvasImage.Children.Add(_startingPoint.point);
                    }
                    Canvas.SetLeft(_startingPoint.point, coord.X - 5);
                    Canvas.SetTop(_startingPoint.point, coord.Y - 5);
                    _startingPoint.Coor = coord;
                    int col = 0, row = 0;
                    fun(ref col, ref row);
                    _startingPoint.Column = col;
                    _startingPoint.Row = row;
                }
                else if (_flag == 2)
                {
                    _points.Add(new Points());
                    _points[_counter].point = new Ellipse();
                    _points[_counter].point.Width = 10;
                    _points[_counter].point.Height = 10;
                    _points[_counter].point.Fill = Brushes.Blue;
                    Canvas.SetLeft(_points[_counter].point, coord.X - 5);
                    Canvas.SetTop(_points[_counter].point, coord.Y - 5);
                    CanvasImage.Children.Add(_points[_counter].point);
                    _points[_counter].Coor = coord;
                    int col = 0, row = 0;
                    fun(ref col, ref row);
                    _points[_counter].Column = col;
                    _points[_counter].Row = row;
                    _counter++;
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
                CanvasImage.Children.Remove(point.point);

            }
            _counter = 0;
        }

        private void lineVert(int x, int y)
        {

            _lines.Add(new Line
            {
                Stroke = System.Windows.Media.Brushes.Red,
                Fill = System.Windows.Media.Brushes.Red,
                X1 = 0,
                X2 = 500,
                StrokeThickness = 4,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom

            });
            imageGrid.Children.Add(_lines[_linesCounter]);
            Grid.SetRow(_lines[_linesCounter], x);
            Grid.SetColumn(_lines[_linesCounter++], y);
        }
        private void lineHorDo(int x, int y)
        {
            _lines.Add(new Line
            {
                Stroke = System.Windows.Media.Brushes.Red,
                Fill = System.Windows.Media.Brushes.Red,
                Y1 = 0,
                Y2 = 500,
                StrokeThickness = 4,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom

            });
            imageGrid.Children.Add(_lines[_linesCounter]);
            Grid.SetRow(_lines[_linesCounter], x);
            Grid.SetColumn(_lines[_linesCounter++], y);
        }
        private void lineHorUp(int x, int y)
        {
            _lines.Add(new Line
            {
                Stroke = System.Windows.Media.Brushes.Red,
                Fill = System.Windows.Media.Brushes.Red,
                Y1 = 0,
                Y2 = 500,
                StrokeThickness = 4,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom

            });
            imageGrid.Children.Add(_lines[_linesCounter]);
            Grid.SetRow(_lines[_linesCounter], x);
            Grid.SetColumn(_lines[_linesCounter++], y);
        }

        void fun(ref int col, ref int row)
        {
            var point = Mouse.GetPosition(imageGrid);

            
            double accumulatedHeight = 0.0;
            double accumulatedWidth = 0.0;

            // calc row mouse was over
            foreach (var rowDefinition in imageGrid.RowDefinitions)
            {
                accumulatedHeight += rowDefinition.ActualHeight;
                if (accumulatedHeight >= point.Y)
                    break;
                row++;
            }

            // calc col mouse was over
            foreach (var columnDefinition in imageGrid.ColumnDefinitions)
            {
                accumulatedWidth += columnDefinition.ActualWidth;
                if (accumulatedWidth >= point.X)
                    break;
                col++;
            }
            //MessageBox.Show($"row{row}  col{col}");
        }

        private void startAlgorithmButton_Click(object sender, RoutedEventArgs e)
        {
            if (_startingPoint == null || _counter < 1)
            {
                //MessageBox.Show("Nie wybrano punktów");
            }
            int row = _startingPoint.Row, col = _startingPoint.Column;
            for (int i = 0; i < _counter; i++)
            {
                while (row != _points[i].Row)
                {
                    if (row < _points[i].Row)
                    {
                        lineHorDo(row, col);
                        row++;
                        if (row == _points[i].Row) lineHorDo(row, col);
                    }
                    else if (row > _points[i].Row)
                    {
                        lineHorUp(row, col);
                        row--;
                    }
                }

                //lineHorDo(row, col);
                //row--;
                //col++;
                while (col != _points[i].Column)
                {
                    if (col < _points[i].Column)
                    {

                        col++;
                        lineVert(row, col);
                    }
                    else if (col > _points[i].Column)
                    {
                        lineVert(row, col);
                        col--;
                    }
                }
                //MessageBox.Show($"row{row}  col{col}");
                lineVert(row, col);
                row = _points[i].Row;
                col = _points[i].Column;
            }
            //while (row != _startingPoint.Row)
            //{
            //    if (row < _startingPoint.Row)
            //    {
            //        lineHorDo(row, col);
            //        row++;
            //        if (row == _startingPoint.Row) lineHorDo(row, col);
            //    }
            //    else if (row > _startingPoint.Row)
            //    {
            //        lineHorUp(row, col);
            //        row--;
            //    }
            //}

            //lineHorDo(row, col);
            //row--;
            //col++;
            //while (col != _startingPoint.Column)
            //{
            //    if (col < _startingPoint.Column)
            //    {

            //        col++;
            //        lineVert(row, col);
            //    }
            //    else if (col > _startingPoint.Column)
            //    {
            //        lineVert(row, col);
            //        col--;
            //    }
            //}




        }

        private void imageGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //var point = Mouse.GetPosition(imageGrid);

            //int row = 0;
            //int col = 0;
            //double accumulatedHeight = 0.0;
            //double accumulatedWidth = 0.0;

            //// calc row mouse was over
            //foreach (var rowDefinition in imageGrid.RowDefinitions)
            //{
            //    accumulatedHeight += rowDefinition.ActualHeight;
            //    if (accumulatedHeight >= point.Y)
            //        break;
            //    row++;
            //}

            //// calc col mouse was over
            //foreach (var columnDefinition in imageGrid.ColumnDefinitions)
            //{
            //    accumulatedWidth += columnDefinition.ActualWidth;
            //    if (accumulatedWidth >= point.X)
            //        break;
            //    col++;
            //}
            //MessageBox.Show($"row{row}  col{col}");
            int col = 0;
            int row = 0;
            fun(ref col, ref row);

            //lineVert(row, col++);
            //lineVert(row++, col);
            //lineHor(row++, col);
            //lineHor(row, col++);
            //lineVert(row, col++);
            //lineVert(row, col++);

        }

        private void removeRoadButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var line in _lines)
            {
                imageGrid.Children.Remove(line);

            }
            //_linesCounter = 0;
        }
    }
}
