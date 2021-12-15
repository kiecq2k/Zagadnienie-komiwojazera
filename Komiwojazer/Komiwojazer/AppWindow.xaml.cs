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
        private Points _startingPoint = null;
        private IList<Points> _points = new List<Points>();
        private int _counter = 0;
        private IList<Point> _intersections = new List<Point>();
        private IList<Point> _usedPoints = new List<Point>();
        private IList<IList<int>> _adjMatrix = new List<IList<int>>();

        public AppWindow()
        {
            InitializeComponent();
            FillIntersections();
            matrixFill();
            graphFill();
        }

        private void endButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CanvasImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var coord = e.GetPosition(this.CanvasImage);

            if (coord.IsOnRoad(Version.Full))
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
                    _counter++;
                }
            }
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

        private void startAlgorithmButton_Click(object sender, RoutedEventArgs e)
        {
            if (_startingPoint == null || _counter < 1)
            {
                MessageBox.Show("Nie wybrano punktów");
            }
            else { DrawPath(); }
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
            // 1 row
            _intersections.Add(new Point(11.200000000000003, 10));
            _intersections.Add(new Point(173.60000000000002, 10));
            _intersections.Add(new Point(187.20000000000002, 10));
            _intersections.Add(new Point(351.20000000000005, 10));
            _intersections.Add(new Point(472.8, 10));
            _intersections.Add(new Point(719.2, 10));
            // 2 row
            _intersections.Add(new Point(11.200000000000003, 54));
            _intersections.Add(new Point(65.60000000000001, 54));
            _intersections.Add(new Point(173.60000000000002, 54));
            _intersections.Add(new Point(187.20000000000002, 54));
            _intersections.Add(new Point(268.8, 54));
            _intersections.Add(new Point(351.20000000000005, 54));
            _intersections.Add(new Point(472.8, 54));
            _intersections.Add(new Point(599.2, 54));
            _intersections.Add(new Point(719.2, 54));
            // 3 row
            _intersections.Add(new Point(11.200000000000003, 98));
            _intersections.Add(new Point(65.60000000000001, 98));
            _intersections.Add(new Point(96, 98));
            _intersections.Add(new Point(173.60000000000002, 98));
            _intersections.Add(new Point(187.20000000000002, 98));
            _intersections.Add(new Point(268.8, 98));
            _intersections.Add(new Point(351.20000000000005, 98));
            _intersections.Add(new Point(472.8, 98));
            _intersections.Add(new Point(599.2, 98));
            // 4 row
            _intersections.Add(new Point(11.200000000000003, 142.8));
            _intersections.Add(new Point(96, 142.8));
            _intersections.Add(new Point(173.60000000000002, 142.8));
            _intersections.Add(new Point(187.20000000000002, 142.8));
            _intersections.Add(new Point(268.8, 142.8));
            _intersections.Add(new Point(351.20000000000005, 142.8));
            _intersections.Add(new Point(409.6, 142.8));
            _intersections.Add(new Point(472.8, 142.8));
            _intersections.Add(new Point(599.2, 142.8));
            _intersections.Add(new Point(719.2, 142.8));
            // 5 row
            _intersections.Add(new Point(11.200000000000003, 190));
            _intersections.Add(new Point(96, 190));
            _intersections.Add(new Point(173.60000000000002, 190));
            _intersections.Add(new Point(187.20000000000002, 190));
            _intersections.Add(new Point(268.8, 190));
            _intersections.Add(new Point(351.20000000000005, 190));
            _intersections.Add(new Point(408.8, 190));
            _intersections.Add(new Point(472.8, 190));
            _intersections.Add(new Point(599.2, 190));
            _intersections.Add(new Point(719.2, 190));
            // 6 row
            _intersections.Add(new Point(11.200000000000003, 232.4));
            _intersections.Add(new Point(96, 232.4));
            _intersections.Add(new Point(173.60000000000002, 232.4));
            _intersections.Add(new Point(187.20000000000002, 232.4));
            _intersections.Add(new Point(268.8, 232.4));
            _intersections.Add(new Point(351.20000000000005, 232.4));
            _intersections.Add(new Point(472.8, 232.4));
            _intersections.Add(new Point(599.2, 232.4));
            _intersections.Add(new Point(719.2, 232.4));
            // 7 row
            _intersections.Add(new Point(11.200000000000003, 281.2));
            _intersections.Add(new Point(96, 281.2));
            _intersections.Add(new Point(173.60000000000002, 281.2));
            _intersections.Add(new Point(187.20000000000002, 281.2));
            _intersections.Add(new Point(268.8, 281.2));
            _intersections.Add(new Point(351.20000000000005, 281.2));
            _intersections.Add(new Point(472.8, 281.2));
            _intersections.Add(new Point(599.2, 281.2));
            _intersections.Add(new Point(664.8000000000001, 281.2));
            // 8 row
            _intersections.Add(new Point(11.200000000000003, 329.20000000000005));
            _intersections.Add(new Point(96, 329.20000000000005));
            _intersections.Add(new Point(173.60000000000002, 329.20000000000005));
            _intersections.Add(new Point(187.20000000000002, 329.20000000000005));
            _intersections.Add(new Point(268.8, 329.20000000000005));
            _intersections.Add(new Point(351.20000000000005, 329.20000000000005));
            _intersections.Add(new Point(472.8, 329.20000000000005));
            _intersections.Add(new Point(599.2, 329.20000000000005));
            _intersections.Add(new Point(664.8000000000001, 329.20000000000005));
            _intersections.Add(new Point(719.2, 329.20000000000005));
            // 9 row
            _intersections.Add(new Point(11.200000000000003, 373.20000000000005));
            _intersections.Add(new Point(96, 373.20000000000005));
            _intersections.Add(new Point(173.60000000000002, 373.20000000000005));
            _intersections.Add(new Point(187.20000000000002, 373.20000000000005));
            _intersections.Add(new Point(268.8, 373.20000000000005));
            _intersections.Add(new Point(351.20000000000005, 373.20000000000005));
            _intersections.Add(new Point(411.20000000000005, 373.20000000000005));
            _intersections.Add(new Point(472.8, 373.20000000000005));
            _intersections.Add(new Point(543.2, 373.20000000000005));
            _intersections.Add(new Point(599.2, 373.20000000000005));
            _intersections.Add(new Point(719.2, 373.20000000000005));
            // 10 row
            _intersections.Add(new Point(96, 419.6));
            _intersections.Add(new Point(173.60000000000002, 419.6));
            _intersections.Add(new Point(187.20000000000002, 419.6));
            _intersections.Add(new Point(268.8, 419.6));
            _intersections.Add(new Point(351.20000000000005, 419.6));
            _intersections.Add(new Point(411.20000000000005, 419.6));
            _intersections.Add(new Point(472.8, 419.6));
            _intersections.Add(new Point(543.2, 419.6));
            _intersections.Add(new Point(599.2, 419.6));
            _intersections.Add(new Point(719.2, 419.6));
            // 11 row
            _intersections.Add(new Point(11.200000000000003, 462));
            _intersections.Add(new Point(96, 462));
            _intersections.Add(new Point(173.60000000000002, 462));
            _intersections.Add(new Point(187.20000000000002, 462));
            _intersections.Add(new Point(268.8, 462));
            _intersections.Add(new Point(351.20000000000005, 462));
            _intersections.Add(new Point(411.20000000000005, 462));
            _intersections.Add(new Point(472.8, 462));
            _intersections.Add(new Point(543.2, 462));
            _intersections.Add(new Point(599.2, 462));
            _intersections.Add(new Point(719.2, 462));
            // 12 row
            _intersections.Add(new Point(11.200000000000003, 506));
            _intersections.Add(new Point(96, 506));
            _intersections.Add(new Point(173.60000000000002, 506));
            _intersections.Add(new Point(187.20000000000002, 506));
            _intersections.Add(new Point(268.8, 506));
            _intersections.Add(new Point(351.20000000000005, 506));
            _intersections.Add(new Point(411.20000000000005, 506));
            _intersections.Add(new Point(472.8, 506));
            _intersections.Add(new Point(543.2, 506));
            _intersections.Add(new Point(599.2, 506));
            _intersections.Add(new Point(719.2, 506));
            // 13 row
            _intersections.Add(new Point(11.200000000000003, 550));
            _intersections.Add(new Point(96, 550));
            _intersections.Add(new Point(173.60000000000002, 550));
            _intersections.Add(new Point(187.20000000000002, 550));
            _intersections.Add(new Point(268.8, 550));
            _intersections.Add(new Point(351.20000000000005, 550));
            _intersections.Add(new Point(472.8, 550));
            _intersections.Add(new Point(599.2, 550));
            // 14 row
            _intersections.Add(new Point(11.200000000000003, 597.2));
            _intersections.Add(new Point(96, 597.2));
            _intersections.Add(new Point(173.60000000000002, 597.2));
            _intersections.Add(new Point(187.20000000000002, 597.2));
            _intersections.Add(new Point(268.8, 597.2));
            _intersections.Add(new Point(351.20000000000005, 597.2));
            _intersections.Add(new Point(472.8, 597.2));
            _intersections.Add(new Point(599.2, 597.2));
            _intersections.Add(new Point(719.2, 597.2));
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

            while (!AllPointsUsed())
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
                if (p != point && distance < minDistance && !_usedPoints.Contains(p))
                {
                    minDistance = distance;
                    minPoint = p;
                }
            }

            foreach (var p in _points)
            {
                double distance = GetDistance(point, p.Coor);
                if (p.Coor != point && distance < minDistance && !_usedPoints.Contains(p.Coor))
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
            var graph = new AdjacencyGraph<int, TaggedEdge<int, int>>();
            
            for (int i = 0; i < 140; i++)
            {
                graph.AddVertex(vertex[i]);
            }
            TaggedEdge<int, int>[] edge = new TaggedEdge<int, int>[1000];


            for (int i = 0; i < _adjMatrix.Count; i++)
            {
                for (int j = 0; j < _adjMatrix[i].Count; j++)
                {
                    if (_adjMatrix[i][j] != 0)
                    {
                        edge[counter++] = new TaggedEdge<int, int>(vertex[i], vertex[j], _adjMatrix[i][j]);
                    }
                }
            }

            foreach (var elem in edge)
            {
                if (elem != null)
                {
                    graph.AddEdge(elem);

                }
            }
        }

        private void matrixFill()
        {
            string[] lines = File.ReadAllLines("graph_matrix_full.txt");
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Replace(",", "").Split("\t");
                _adjMatrix.Add(new List<int>());

                foreach (var numberAsString in line)
                {
                    _adjMatrix[i].Add(int.Parse(numberAsString));
                }
            }
        }
    }
}
