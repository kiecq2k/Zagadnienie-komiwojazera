using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Komiwojazer.Algorithms;
using Priority_Queue;
using QuickGraph;
using Point = System.Windows.Point;
using System.Windows.Threading;
using System.Threading.Tasks;

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
        private int _pointCounter = 133;
        private int _zIndexCounter = 1;


        private IList<Point> _intersections = new List<Point>();
        private IList<Point> _dots = new List<Point>();
        private IList<Point> _usedPoints = new List<Point>();
        private IList<IList<int>> _adjMatrix = new List<IList<int>>();
        private IList<IList<int>> _startingAdjMatrix = new List<IList<int>>();
        private IList<Tuple<int, int>> _verticalCross = new List<Tuple<int, int>>();

        public AppWindow()
        {
            InitializeComponent();
            FillIntersections();
            matrixFill();
            addToMatrix();
            for (int i = 0; i < 132; i++)
            {
                _points.Add(new Points { Coor = _intersections[i] });
            }
            for (int i = 0; i < 133; i++)
            {
                _startingAdjMatrix.Add(new List<int>());
                _startingAdjMatrix[i] = new List<int>(_adjMatrix[i]);
            }
            getVerticalRoads();
        }

        private void getVerticalRoads()
        {
            for (int i = 0; i < _adjMatrix.Count; i++)
            {
                for (int j = i; j < _adjMatrix.Count; j++)
                {
                    if((_adjMatrix[i][j]!= 0 || _adjMatrix[j][i] != 0) && Math.Abs(i - j) != 1)
                    {
                        _verticalCross.Add(new Tuple<int, int>(i, j));
                    }
                }
            }
        }


        private void addToMatrix()
        {
            foreach (var elem in _adjMatrix)
            {
                elem.Add(0);
            }
            _adjMatrix.Add(new List<int>());
            for (int i = 0; i < _adjMatrix.Count; i++)
            {
                _adjMatrix[_adjMatrix.Count - 1].Add(0);
            }
        }

        private void endButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private bool crossCheck(Point p)
        {
            foreach (var cross in _intersections)
            {
                if (cross.X + 20 > p.X && cross.X - 20 < p.X &&
                    cross.Y + 20 > p.Y && cross.Y - 20 < p.Y)
                    return false;
            }
            return true;
        }

        private bool pointCheck(Point p)
        {
            for (int i = 26; i < _dots.Count; i++)
            {
                if (Math.Abs(_dots[i].X - p.X) < 18 && Math.Abs(_dots[i].Y - p.Y) < 18)
                    return false;
            }
            return true;
        }
        private void CanvasImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var coord = e.GetPosition(this.CanvasImage);

            if (coord.IsOnRoad(Version.Full) &&
                crossCheck(coord) && pointCheck(coord))
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
                    Canvas.SetZIndex(_startingPoint.point, _zIndexCounter++);
                    pointPosition(e, _flag);
                    _startingPoint.Coor = coord;
                    _points.Add(new Points { Coor = _startingPoint.Coor });
                    endPointsButton.IsEnabled = true;
                    startPointButton.IsEnabled = false;
                    _flag = 2;

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
                    Canvas.SetZIndex(_points[_counter].point, _zIndexCounter++);
                    pointPosition(e, _flag);
                    _counter++;
                    startAlgorithmButton.IsEnabled = true;
                }
            }
        }



        void pointPosition(MouseButtonEventArgs e, int flag)
        {
            var pos = e.GetPosition(this.CanvasImage);
            int road1 = 0;
            int road2 = 0;
            int cross1 = -1;
            int cross2 = -1;
            for (int i = 1; i < _intersections.Count; i++)
            {
                if (pos.X > _dots[i - 1].X && pos.X < _dots[i].X &&
                    Math.Abs(pos.Y - _dots[i - 1].Y) < 11)
                {
                    road1 = (int)(Math.Abs(_dots[i - 1].X - pos.X) * 0.3125);
                    road2 = (int)(Math.Abs(_dots[i].X - pos.X) * 0.3125);
                    cross1 = i - 1;
                    cross2 = i;

                }
            }
            if (cross1 != -1)
            {
                for (int i = 132; i < _dots.Count; i++)
                {
                    int dotsRoad = (int)(Math.Abs(_dots[i].X - pos.X) * 0.3125);
                    if (Math.Abs(pos.Y - _dots[i].Y) < 11)
                    {
                        if (_dots[i].X < pos.X)
                        {
                            if (dotsRoad < road1)
                            {
                                road1 = dotsRoad;
                                cross1 = i;
                            }
                        }
                        else
                        {
                            if (dotsRoad < road2)
                            {
                                road2 = dotsRoad;
                                cross2 = i;
                            }

                        }
                    }
                }
            }
            if (cross1 == -1)
            {
                for (int i = 0; i < _verticalCross.Count; i++)
                {
                    if (pos.Y > _intersections[_verticalCross[i].Item1].Y &&
                        pos.Y < _intersections[_verticalCross[i].Item2].Y &&
                        Math.Abs(pos.X - _intersections[_verticalCross[i].Item1].X) < 10)
                    {
                        cross1 = _verticalCross[i].Item1;
                        cross2 = _verticalCross[i].Item2;
                        road1 = (int)Math.Abs((_dots[_verticalCross[i].Item1].Y - pos.Y) * 0.33);
                        road2 = (int)Math.Abs((_dots[_verticalCross[i].Item2].Y - pos.Y) * 0.33);
                    }
                    
                }
                if (cross1 == -1) 
                {
                    MessageBox.Show("pionowe blad");
                }
                int tmpcross1 = cross1;
                int tmpcross2 = cross2;
                for (int i = 132; i < _dots.Count; i++)
                {
                    int dotsRoad = (int)(Math.Abs(_dots[i].Y - pos.Y) * 0.33);
                    if (Math.Abs(pos.X - _dots[i].X) < 20)
                    {
                        if (_dots[i].Y < pos.Y)
                        {
                            if (cross1 < cross2)
                            {
                                if (dotsRoad < road1)
                                {
                                    road1 = dotsRoad;
                                    tmpcross1 = i;
                                }
                            }
                            else if (cross1 > cross2)
                            {
                                if (dotsRoad < road2)
                                {
                                    road2 = dotsRoad;
                                    tmpcross2 = i;
                                }
                            }
                        }
                        else
                        {
                            if (cross1 > cross2)
                            {
                                if (dotsRoad < road1)
                                {
                                    road1 = dotsRoad;
                                    tmpcross1 = i;
                                }
                            }
                            else
                            {
                                if (dotsRoad < road2)
                                {
                                    road2 = dotsRoad;
                                    tmpcross2 = i;
                                }
                            }

                        }
                    }
                }
                cross1 = tmpcross1;
                cross2 = tmpcross2;
            }
            if (road1 == 0) road1 = 1;
            if (road2 == 0) road2 = 1;
            //add point to matrix and delete roads between crossroads
            int indexOfPoint;
            if (flag == 1)
            {
                indexOfPoint = 132;
                _dots.Add(pos);
            }
            else
            {
                indexOfPoint = _pointCounter++;
                _dots.Add(pos);
                addToMatrix();
            }
            int check = 0;
            if (_adjMatrix[cross1][cross2] != 0)
            {
                _adjMatrix[cross1][cross2] = 0;
                _adjMatrix[cross1][indexOfPoint] = road1;
                _adjMatrix[indexOfPoint][cross2] = road2;
                check = 1;
            }
            if (_adjMatrix[cross2][cross1] != 0)
            {
                _adjMatrix[cross2][cross1] = 0;
                _adjMatrix[cross2][indexOfPoint] = road2;
                _adjMatrix[indexOfPoint][cross1] = road1;
                check = 1;
            }
            if (check == 0)
            {
                MessageBox.Show("Blad dodawania punktow");
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

            
            _dots = _intersections.ToList<Point>();

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
                    p.Visited = true;
            }

            _usedPoints.Add(minPoint);

            return minPoint;
        }

        private double GetDistance(Point p1, Point p2) => Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));

        private bool AllPointsUsed()
        {
            foreach (var point in _points)
            {
                if (!point.Visited)
                    return false;
            }

            return true;
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
