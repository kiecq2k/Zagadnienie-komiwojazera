using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using QuickGraph;
using QuickGraph.Algorithms;

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
        private IList<Point> _intersections = new List<Point>();
        private IList<Point> _usedPoints = new List<Point>();

        public AppWindow()
        {
            InitializeComponent();
            FillIntersections();
            graphFill();
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
            DrawPath();

            //radze nie zagladac
            {
                /*
                //if (_startingPoint == null || _counter < 1)
                //{
                //    //MessageBox.Show("Nie wybrano punktów");
                //}
                //int row = _startingPoint.Row, col = _startingPoint.Column;
                //for (int i = 0; i < _counter; i++)
                //{
                //    while (row != _points[i].Row)
                //    {
                //        if (row < _points[i].Row)
                //        {
                //            lineHorDo(row, col);
                //            row++;
                //            if (row == _points[i].Row) lineHorDo(row, col);
                //        }
                //        else if (row > _points[i].Row)
                //        {
                //            lineHorUp(row, col);
                //            row--;
                //        }
                //    }
                //    //lineHorDo(row, col);
                //    //row--;
                //    //col++;
                //    while (col != _points[i].Column)
                //    {
                //        if (col < _points[i].Column)
                //        {
                //            col++;
                //            lineVert(row, col);
                //        }
                //        else if (col > _points[i].Column)
                //        {
                //            lineVert(row, col);
                //            col--;
                //        }
                //    }
                //    //MessageBox.Show($"row{row}  col{col}");
                //    lineVert(row, col);
                //    row = _points[i].Row;
                //    col = _points[i].Column;
                //}
                ////while (row != _startingPoint.Row)
                ////{
                ////    if (row < _startingPoint.Row)
                ////    {
                ////        lineHorDo(row, col);
                ////        row++;
                ////        if (row == _startingPoint.Row) lineHorDo(row, col);
                ////    }
                ////    else if (row > _startingPoint.Row)
                ////    {
                ////        lineHorUp(row, col);
                ////        row--;
                ////    }
                ////}
                ////lineHorDo(row, col);
                ////row--;
                ////col++;
                ////while (col != _startingPoint.Column)
                ////{
                ////    if (col < _startingPoint.Column)
                ////    {
                ////        col++;
                ////        lineVert(row, col);
                ////    }
                ////    else if (col > _startingPoint.Column)
                ////    {
                ////        lineVert(row, col);
                ////        col--;
                ////    }
                ////}
                */
            }

        }

        private void imageGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            int col = 0;
            int row = 0;
            fun(ref col, ref row);
        }

        private void removeRoadButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var line in _lines)
            {
                imageGrid.Children.Remove(line);
                CanvasImage.Children.Remove(line);
            }
        }

        private void FillIntersections()
        {
            _intersections.Add(new Point(2.300000000000068, 10));
            _intersections.Add(new Point(92.70000000000007, 9.200000000000003));
            _intersections.Add(new Point(176.70000000000007, 8.400000000000002));
            _intersections.Add(new Point(191.10000000000008, 8.400000000000002));
            _intersections.Add(new Point(281.50000000000006, 8.400000000000002));
            _intersections.Add(new Point(371.1000000000001, 8.400000000000002));
            _intersections.Add(new Point(500.70000000000005, 6.800000000000001));
            _intersections.Add(new Point(637.5000000000001, 6));
            _intersections.Add(new Point(763.9000000000001, 5.200000000000001));
            _intersections.Add(new Point(2.300000000000068, 52.400000000000006));
            _intersections.Add(new Point(94.30000000000007, 54));
            _intersections.Add(new Point(179.9000000000001, 54.8));
            _intersections.Add(new Point(194.30000000000007, 54));
            _intersections.Add(new Point(279.9000000000001, 52.400000000000006));
            _intersections.Add(new Point(372.7000000000001, 52.400000000000006));
            _intersections.Add(new Point(503.10000000000014, 52.400000000000006));
            _intersections.Add(new Point(638.3000000000001, 50));
            _intersections.Add(new Point(763.9000000000001, 53.2));
            _intersections.Add(new Point(2.300000000000068, 98));
            _intersections.Add(new Point(95.10000000000008, 98));
            _intersections.Add(new Point(181.50000000000009, 98));
            _intersections.Add(new Point(193.50000000000009, 97.2));
            _intersections.Add(new Point(279.9000000000001, 97.2));
            _intersections.Add(new Point(372.7000000000001, 96.4));
            _intersections.Add(new Point(501.5000000000001, 96.4));
            _intersections.Add(new Point(637.5000000000001, 96.4));
            _intersections.Add(new Point(763.1000000000001, 94.80000000000001));
            _intersections.Add(new Point(1.5000000000000693, 142));
            _intersections.Add(new Point(97.50000000000007, 141.20000000000002));
            _intersections.Add(new Point(181.50000000000009, 140.4));
            _intersections.Add(new Point(195.10000000000008, 142));
            _intersections.Add(new Point(280.7000000000001, 141.20000000000002));
            _intersections.Add(new Point(373.5000000000001, 142));
            _intersections.Add(new Point(502.30000000000007, 140.4));
            _intersections.Add(new Point(637.5000000000001, 140.4));
            _intersections.Add(new Point(763.9000000000001, 141.20000000000002));
            _intersections.Add(new Point(4.70000000000007, 186.8));
            _intersections.Add(new Point(96.70000000000007, 186.8));
            _intersections.Add(new Point(180.70000000000007, 185.20000000000002));
            _intersections.Add(new Point(195.10000000000008, 184.4));
            _intersections.Add(new Point(282.30000000000007, 186));
            _intersections.Add(new Point(372.7000000000001, 186));
            _intersections.Add(new Point(502.30000000000007, 184.4));
            _intersections.Add(new Point(638.3000000000001, 184.4));
            _intersections.Add(new Point(762.3000000000001, 184.4));
            _intersections.Add(new Point(4.70000000000007, 230.8));
            _intersections.Add(new Point(96.70000000000007, 231.60000000000002));
            _intersections.Add(new Point(182.30000000000007, 230.8));
            _intersections.Add(new Point(191.9000000000001, 229.20000000000002));
            _intersections.Add(new Point(280.7000000000001, 229.20000000000002));
            _intersections.Add(new Point(372.7000000000001, 227.60000000000002));
            _intersections.Add(new Point(502.30000000000007, 226.8));
            _intersections.Add(new Point(639.1000000000001, 228.4));
            _intersections.Add(new Point(763.9000000000001, 229.20000000000002));
            _intersections.Add(new Point(4.70000000000007, 278.8));
            _intersections.Add(new Point(95.90000000000008, 278));
            _intersections.Add(new Point(179.9000000000001, 278));
            _intersections.Add(new Point(195.9000000000001, 277.2));
            _intersections.Add(new Point(283.1000000000001, 276.40000000000003));
            _intersections.Add(new Point(373.5000000000001, 276.40000000000003));
            _intersections.Add(new Point(501.5000000000001, 276.40000000000003));
            _intersections.Add(new Point(639.9000000000001, 274.8));
            _intersections.Add(new Point(762.3000000000001, 276.40000000000003));
            _intersections.Add(new Point(4.70000000000007, 326));
            _intersections.Add(new Point(98.30000000000007, 326));
            _intersections.Add(new Point(183.9000000000001, 326));
            _intersections.Add(new Point(195.9000000000001, 324.40000000000003));
            _intersections.Add(new Point(282.30000000000007, 326));
            _intersections.Add(new Point(374.30000000000007, 326));
            _intersections.Add(new Point(502.30000000000007, 325.20000000000005));
            _intersections.Add(new Point(639.9000000000001, 323.6));
            _intersections.Add(new Point(762.3000000000001, 322));
            _intersections.Add(new Point(3.100000000000069, 370));
            _intersections.Add(new Point(95.90000000000008, 370));
            _intersections.Add(new Point(181.50000000000009, 369.20000000000005));
            _intersections.Add(new Point(195.10000000000008, 369.20000000000005));
            _intersections.Add(new Point(283.1000000000001, 369.20000000000005));
            _intersections.Add(new Point(375.1000000000001, 369.20000000000005));
            _intersections.Add(new Point(503.10000000000014, 368.40000000000003));
            _intersections.Add(new Point(641.5000000000001, 366.8));
            _intersections.Add(new Point(761.5000000000001, 367.6));
            _intersections.Add(new Point(3.9000000000000696, 413.20000000000005));
            _intersections.Add(new Point(98.30000000000007, 414));
            _intersections.Add(new Point(183.10000000000008, 413.20000000000005));
            _intersections.Add(new Point(197.50000000000009, 412.40000000000003));
            _intersections.Add(new Point(283.1000000000001, 412.40000000000003));
            _intersections.Add(new Point(375.1000000000001, 412.40000000000003));
            _intersections.Add(new Point(503.9000000000001, 413.20000000000005));
            _intersections.Add(new Point(640.7000000000002, 410.8));
            _intersections.Add(new Point(762.3000000000001, 414.8));
            _intersections.Add(new Point(3.9000000000000696, 458));
            _intersections.Add(new Point(98.30000000000007, 458));
            _intersections.Add(new Point(182.30000000000007, 458.8));
            _intersections.Add(new Point(196.70000000000007, 458.8));
            _intersections.Add(new Point(283.1000000000001, 458.8));
            _intersections.Add(new Point(377.5000000000001, 458.8));
            _intersections.Add(new Point(503.10000000000014, 456.40000000000003));
            _intersections.Add(new Point(641.5000000000001, 456.40000000000003));
            _intersections.Add(new Point(763.9000000000001, 457.20000000000005));
            _intersections.Add(new Point(5.500000000000071, 502.80000000000007));
            _intersections.Add(new Point(98.30000000000007, 502.80000000000007));
            _intersections.Add(new Point(181.50000000000009, 503.6));
            _intersections.Add(new Point(195.10000000000008, 503.6));
            _intersections.Add(new Point(285.50000000000006, 503.6));
            _intersections.Add(new Point(375.1000000000001, 502));
            _intersections.Add(new Point(503.10000000000014, 503.6));
            _intersections.Add(new Point(642.3000000000001, 501.20000000000005));
            _intersections.Add(new Point(762.3000000000001, 500.40000000000003));
            _intersections.Add(new Point(4.70000000000007, 546.8000000000001));
            _intersections.Add(new Point(96.70000000000007, 547.6));
            _intersections.Add(new Point(182.30000000000007, 549.2));
            _intersections.Add(new Point(198.30000000000007, 548.4));
            _intersections.Add(new Point(284.7000000000001, 547.6));
            _intersections.Add(new Point(375.1000000000001, 547.6));
            _intersections.Add(new Point(503.9000000000001, 546.8000000000001));
            _intersections.Add(new Point(642.3000000000001, 546.8000000000001));
            _intersections.Add(new Point(763.9000000000001, 545.2));

        }

        private void DrawPath()
        {
            var np1 = GetNearestPoint(_startingPoint.Coor);
            var line = new Line
            {
                Stroke = System.Windows.Media.Brushes.Red,
                Fill = System.Windows.Media.Brushes.Red,
                X1 = _startingPoint.Coor.X,
                Y1 = _startingPoint.Coor.Y,
                X2 = np1.X,
                Y2 = np1.Y,
                StrokeThickness = 4,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom

            };

            _lines.Add(line);
            CanvasImage.Children.Add(line);
            _usedPoints.Add(_startingPoint.Coor);

            Point last = np1;

            while(!AllPointsUsed())
            {
                var line2 = new Line
                {
                    Stroke = System.Windows.Media.Brushes.Red,
                    Fill = System.Windows.Media.Brushes.Red,
                    X1 = last.X,
                    Y1 = last.Y,
                    StrokeThickness = 4,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom
                };

                last = GetNearestPoint(last);
                line2.X2 = last.X;
                line2.Y2 = last.Y;

                _lines.Add(line2);
                CanvasImage.Children.Add(line2);
            }
        }

        private Point GetNearestPoint(Point point)
        {
            double minDistance = 1000000;
            Point minPoint;

            foreach (var p in _intersections)
            {
                double distance = GetDistance(point, p);
                if(p != point && distance < minDistance && !_usedPoints.Contains(p))
                {
                    minDistance = distance;
                    minPoint = p;
                }
            }

            foreach (var p in _points)
            {
                double distance = GetDistance(point, p.Coor);
                if(p.Coor != point && distance < minDistance && !_usedPoints.Contains(p.Coor))
                {
                    minDistance = distance;
                    minPoint = p.Coor;
                }
            }

            foreach (var p in _points)
            {
                if (p.Coor == minPoint)
                    p.Used = true;
            }

            _usedPoints.Add(minPoint);

            return minPoint;
        }

        private double GetDistance(Point p1, Point p2) => Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));

        private bool AllPointsUsed()
        {
            foreach (var point in _points)
            {
                if (!point.Used)
                    return false;
            }

            return true;
        }

        private void graphFill()
        {
            int counter = 0;
            int[] vertex = new int[200];
            for (int i = 0; i < 200; i++)
            {
                vertex[i] = i;
            }
            var graph = new AdjacencyGraph<int, Edge<int>>();
            for (int i = 0; i < 125; i++)
            {
                graph.AddVertex(vertex[i]);
            }
            Edge<int>[] edge = new Edge<int>[1000];



            //reczny graf
            {
                /*
                 row 0
                // */
                ////vertex 0
                //edge[counter++] = new Edge<int>(vertex[0], vertex[9]);
                ////vertex 1
                //edge[counter++] = new Edge<int>(vertex[1], vertex[0]);
                ////vertex 2
                //edge[counter++] = new Edge<int>(vertex[2], vertex[1]);
                //edge[counter++] = new Edge<int>(vertex[2], vertex[11]);
                ////vertex 3
                //edge[counter++] = new Edge<int>(vertex[3], vertex[2]);
                ////vertex 4
                //edge[counter++] = new Edge<int>(vertex[4], vertex[3]);
                //edge[counter++] = new Edge<int>(vertex[4], vertex[13]);
                ////vertex 5
                //edge[counter++] = new Edge<int>(vertex[5], vertex[4]);
                ////vertex 6
                //edge[counter++] = new Edge<int>(vertex[6], vertex[5]);
                //edge[counter++] = new Edge<int>(vertex[6], vertex[15]);
                ////vertex 7
                //edge[counter++] = new Edge<int>(vertex[7], vertex[6]);
                ////vertex 8
                //edge[counter++] = new Edge<int>(vertex[8], vertex[7]);
                //edge[counter++] = new Edge<int>(vertex[8], vertex[17]);
                /*
                  row 1
                 */
                ////vertex 9
                //edge[counter++] = new Edge<int>(vertex[9], vertex[18]);
                //edge[counter++] = new Edge<int>(vertex[9], vertex[10]);
                ////vertex 10
                //edge[counter++] = new Edge<int>(vertex[10], vertex[11]);
                //edge[counter++] = new Edge<int>(vertex[10], vertex[1]);
                ////vertex 11 
                //edge[counter++] = new Edge<int>(vertex[11], vertex[12]);
                //edge[counter++] = new Edge<int>(vertex[11], vertex[20]);
                ////vertex 12
                //edge[counter++] = new Edge<int>(vertex[12], vertex[3]);
                //edge[counter++] = new Edge<int>(vertex[12], vertex[13]);
                ////vertex 13
                //edge[counter++] = new Edge<int>(vertex[13], vertex[22]);
                //edge[counter++] = new Edge<int>(vertex[13], vertex[14]);
                ////vertex 14
                //edge[counter++] = new Edge<int>(vertex[14], vertex[5]);
                //edge[counter++] = new Edge<int>(vertex[14], vertex[15]);
                ////vertex 15
                //edge[counter++] = new Edge<int>(vertex[15], vertex[24]);
                //edge[counter++] = new Edge<int>(vertex[15], vertex[16]);
                ////vertex 16
                //edge[counter++] = new Edge<int>(vertex[16], vertex[7]);
                //edge[counter++] = new Edge<int>(vertex[16], vertex[17]);
                ////vertex 17
                //edge[counter++] = new Edge<int>(vertex[17], vertex[8]);
                //edge[counter++] = new Edge<int>(vertex[17], vertex[26]);
                ///*
                // row 2
                // */
                ////vertex 18
                //edge[counter++] = new Edge<int>(vertex[18], vertex[27]);
                ////vertex 19
                //edge[counter++] = new Edge<int>(vertex[19], vertex[10]);
                //edge[counter++] = new Edge<int>(vertex[19], vertex[18]);
                ////vertex 20
                //edge[counter++] = new Edge<int>(vertex[20], vertex[19]);
                //edge[counter++] = new Edge<int>(vertex[20], vertex[29]);
                ////vertex 21
                //edge[counter++] = new Edge<int>(vertex[21], vertex[20]);
                //edge[counter++] = new Edge<int>(vertex[21], vertex[12]);
                ////vertex 22
                //edge[counter++] = new Edge<int>(vertex[22], vertex[21]);
                //edge[counter++] = new Edge<int>(vertex[22], vertex[31]);
                ////vertex 23
                //edge[counter++] = new Edge<int>(vertex[23], vertex[22]);
                //edge[counter++] = new Edge<int>(vertex[23], vertex[14]);
                ////vertex 24
                //edge[counter++] = new Edge<int>(vertex[24], vertex[23]);
                //edge[counter++] = new Edge<int>(vertex[24], vertex[33]);
                ////vertex 25 
                //edge[counter++] = new Edge<int>(vertex[25], vertex[24]);
                //edge[counter++] = new Edge<int>(vertex[25], vertex[16]);
                ////vertex 26
                //edge[counter++] = new Edge<int>(vertex[26], vertex[25]);
                //edge[counter++] = new Edge<int>(vertex[26], vertex[17]);
                //edge[counter++] = new Edge<int>(vertex[26], vertex[35]);
            }
            
            
            
            
            int vert_counter = 0;
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if(i%2 != 0)
                    {
                        if (j != 8)
                        {
                            edge[counter++] = new Edge<int>(vertex[vert_counter], vertex[vert_counter + 1]);
                        }
                    }
                    else
                    {
                        if (j != 0)
                        {
                            edge[counter++] = new Edge<int>(vertex[vert_counter], vertex[vert_counter - 1]);
                        }
                    }
                    vert_counter++;
                }
            }
            edge[counter++] = new Edge<int>(vertex[vert_counter], vertex[++vert_counter]);

            for (int i = 0; i < 7; i++)
            {

                edge[counter++] = new Edge<int>(vertex[vert_counter], vertex[vert_counter+1]);
                edge[counter++] = new Edge<int>(vertex[vert_counter], vertex[vert_counter-1]);
                vert_counter++;
            }
            edge[counter++] = new Edge<int>(vertex[vert_counter], vertex[vert_counter - 1]);
            vert_counter++;
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (i % 2 == 0)
                    {
                        if (j != 8)
                        {
                            edge[counter++] = new Edge<int>(vertex[vert_counter], vertex[vert_counter + 1]);
                        }
                    }
                    else
                    {
                        if (j != 0)
                        {
                            edge[counter++] = new Edge<int>(vertex[vert_counter], vertex[vert_counter - 1]);
                        }
                    }
                    vert_counter++;
                }
            }
            edge[counter++] = new Edge<int>(vertex[vert_counter], vertex[++vert_counter]);

            for (int i = 0; i < 7; i++)
            {

                edge[counter++] = new Edge<int>(vertex[vert_counter], vertex[vert_counter + 1]);
                edge[counter++] = new Edge<int>(vertex[vert_counter], vertex[vert_counter - 1]);
                vert_counter++;
            }
            edge[counter++] = new Edge<int>(vertex[vert_counter], vertex[vert_counter - 1]);

            /////vertical

            vert_counter = 0;
            for (int i = 0; i < 8; i++)
		    {
                for (int j = 0; j < 13; j++)
                {
                    if (i % 2 == 0)
                    {
                        edge[counter++] = new Edge<int>(vertex[vert_counter], vertex[vert_counter + 9]);
                        vert_counter += 9;
                    }
                    else
                    {
                        edge[counter++] = new Edge<int>(vertex[vert_counter], vertex[vert_counter - 9]);
                        vert_counter -= 9;
                    }
                }
                vert_counter++;
            }
            vert_counter = 8;
            edge[counter++] = new Edge<int>(vertex[vert_counter], vertex[vert_counter + 9]);
            vert_counter += 9;
            for (int i = 0; i < 12; i++)
            {
                edge[counter++] = new Edge<int>(vertex[vert_counter], vertex[vert_counter - 9]);
                edge[counter++] = new Edge<int>(vertex[vert_counter], vertex[vert_counter + 9]);
                vert_counter += 9;
            }
            edge[counter++] = new Edge<int>(vertex[vert_counter], vertex[vert_counter - 9]);
            counter++;
        }


    }
}
