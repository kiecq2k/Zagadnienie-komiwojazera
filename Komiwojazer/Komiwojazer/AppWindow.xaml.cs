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
        private IList<Tuple<int, int>> _betweenCross = new List<Tuple<int, int>>();
        private const int SPEED = 50;
        private Version _version = Version.Full;

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
            betweenFill();
            getVerticalRoads();
        }

        private void betweenFill()
        {
            for (int i = 0; i < 132; i++)
            {
                _betweenCross.Add(new Tuple<int, int>(-1, -1));
            }
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
                if (cross.X + 10 > p.X && cross.X-10 < p.X &&
                    cross.Y + 10 > p.Y && cross.Y-10 < p.Y)
                    return false;
            }
            return true;
        }

        private bool pointCheck(Point p)
        {
            for (int i = 132; i < _dots.Count; i++)
            {
                if (Math.Abs(_dots[i].X - p.X) < 10 && Math.Abs(_dots[i].Y - p.Y) < 20)
                    return false;
            }
            return true;
        }



        private bool chybaCheck(Point pos)
        {
            for (int i = 1; i < _intersections.Count; i++)
            {
                if (pos.X > _dots[i - 1].X && pos.X < _dots[i].X &&
                    Math.Abs(pos.Y - _dots[i - 1].Y) < 5)
                {
                    return true;

                }
            }
            for (int i = 0; i < _verticalCross.Count; i++)
            {
                if (pos.Y > _intersections[_verticalCross[i].Item1].Y &&
                    pos.Y < _intersections[_verticalCross[i].Item2].Y &&
                    Math.Abs(pos.X - _intersections[_verticalCross[i].Item1].X) < 4)
                {
                    return true;
                }
            }
            return false;

        }
        private void CanvasImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var coord = e.GetPosition(this.CanvasImage);


            if (chybaCheck(coord) &&
                crossCheck(coord) && pointCheck(coord) && _counter <7)
            {
                if (_flag == 1)
                {
                    if (_startingPoint == null)
                    {
                        _startingPoint = new Points();
                        _startingPoint.point = new Ellipse();
                        _startingPoint.point.Width = 15;
                        _startingPoint.point.Height = 15;
                        _startingPoint.point.Fill = Brushes.Black;
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
                    removePointsButton.IsEnabled = true;
                    _flag = 2;

                }
                else if (_flag == 2)
                {
                    _points.Add(new Points());
                    _points[_pointCounter].point = new Ellipse();
                    _points[_pointCounter].point.Width = 15;
                    _points[_pointCounter].point.Height = 15;
                    _points[_pointCounter].point.Fill = Brushes.SaddleBrown;
                    Canvas.SetLeft(_points[_pointCounter].point, coord.X - 5);
                    Canvas.SetTop(_points[_pointCounter].point, coord.Y - 5);
                    CanvasImage.Children.Add(_points[_pointCounter].point);
                    _points[_pointCounter].Coor = coord;
                    Canvas.SetZIndex(_points[_pointCounter].point, _zIndexCounter++);
                    pointPosition(e, _flag);
                    _counter++;
                    startAlgorithmButton.IsEnabled = true;
                    number.Content = $"{_counter}";
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
            int defcross1 = -1;
            int defcross2 = -1;

            for (int i = 1; i < _intersections.Count; i++)
            {
                if (pos.X > _dots[i - 1].X && pos.X < _dots[i].X &&
                    Math.Abs(pos.Y - _dots[i - 1].Y) < 11)
                {
                    road1 = (int)(Math.Abs(_dots[i - 1].X - pos.X) * 0.3125);
                    road2 = (int)(Math.Abs(_dots[i].X - pos.X) * 0.3125);
                    cross1 = i - 1;
                    cross2 = i;
                    defcross1 = cross1;
                    defcross2 = cross2;
                }
            }
            if (cross1 != -1)
            {
                for (int i = 132; i < _dots.Count; i++)
                {
                    int dotsRoad = (int)(Math.Abs(_dots[i].X - pos.X) * 0.3125);
                    if ((_betweenCross[i].Item1 == defcross1 && _betweenCross[i].Item2 == defcross2) ||
                        (_betweenCross[i].Item1 == defcross2 && _betweenCross[i].Item2 == defcross1)) 
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
                        defcross1 = cross1;
                        defcross2 = cross2;
                        road1 = (int)Math.Abs((_dots[_verticalCross[i].Item1].Y - pos.Y) * 0.33);
                        road2 = (int)Math.Abs((_dots[_verticalCross[i].Item2].Y - pos.Y) * 0.33);
                    }
                    
                }
                int tmpcross1 = cross1;
                int tmpcross2 = cross2;
                for (int i = 132; i < _dots.Count; i++)
                {
                    int dotsRoad = (int)(Math.Abs(_dots[i].Y - pos.Y) * 0.33);
                    if ((_betweenCross[i].Item1 == defcross1 && _betweenCross[i].Item2 == defcross2) ||
                        (_betweenCross[i].Item1 == defcross2 && _betweenCross[i].Item2 == defcross1))
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
                _betweenCross.Add(new Tuple<int, int>(0, 0));
                _dots.Add(pos);
            }
            else
            {
                indexOfPoint = _pointCounter++;
                _dots.Add(pos);
                _betweenCross.Add(new Tuple<int, int>(0, 0));
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
            _betweenCross[indexOfPoint] = new Tuple<int, int>(defcross1, defcross2);
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
            if (_startingPoint == null)
            {
                MessageBox.Show("Najpierw wybierz punkt startowy");
            }
            else
            {
                _flag = 2;

            }
        }

        private void removePointsButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var line in _lines)
            {
                CanvasImage.Children.Remove(line);
            }
            foreach (var point in _points)
            {
                CanvasImage.Children.Remove(point.point);
            }
            CanvasImage.Children.Remove(_startingPoint.point);
            road_alg1.Clear();
            road_alg2.Clear();
            road_alg3.Clear();
            _points.Clear();
            _adjMatrix.Clear();
            _betweenCross.Clear();
            _startingPoint = null;
            _flag = -1;
            _counter = 0;
            _pointCounter = 133;
            endPointsButton.IsEnabled = false;
            startAlgorithmButton.IsEnabled = false;
            removePointsButton.IsEnabled = false;
            startPointButton.IsEnabled = true;
            radioButtonNN.IsChecked = false;
            radioButtonBF.IsChecked = false;
            radioButton3.IsChecked = false;
            for (int i = 0; i < 133; i++)
            {
                _adjMatrix.Add(new List<int>());
                _adjMatrix[i] = new List<int>(_startingAdjMatrix[i]);
            }
            _dots = _intersections.ToList();
            for (int i = 0; i < 132; i++)
            {
                _points.Add(new Points { Coor = _intersections[i] });
            }
            betweenFill();
            number.Content = $"{_counter}";
        }

        private void startAlgorithmButton_Click(object sender, RoutedEventArgs e)
        {
            if (_startingPoint == null || _counter < 1)
            {
                MessageBox.Show("Nie wybrano punktów");
                return;
            }
            int buttonCheck = 0;
            if (radioButtonNN.IsChecked == true)
            {
                resultNN = NajblizszySasiad();
                DrawingFormNN();
                buttonCheck = 1;
            }
            else if(radioButtonBF.IsChecked == true)
            {
                resultBF = BruteForce();
                DrawingFormBF();
                buttonCheck = 1;
            }
            else if(radioButton3.IsChecked == true)
            {
                resultGreedy = Greedy();
                DrawingFormGreedy();
                buttonCheck = 1;
            }
            if (buttonCheck == 1)
            {
                removePointsButton.IsEnabled = false;
                startAlgorithmButton.IsEnabled = false;
                endPointsButton.IsEnabled = false;
                _flag = -1;
            }
        }


     
        private IList<int> NajblizszySasiad()
        {
            var dijkstra = new Dijkstra(_version, _adjMatrix);
            var result = dijkstra.GetPath();
            road_alg1.Text += Distance(result);
            return result;
        }

        public IList<int> BruteForce()
        {
            var bruteForce = new BruteForce(_version, _adjMatrix);
            var result = bruteForce.GetPathBF2();
            var result2 = DistanceBF(result);
            road_alg2.Text += Distance(result2);
            return result2;
        }

        public IList<int> Greedy()
        {
            var greedy = new Greedy(_version, _adjMatrix);
            var result = greedy.NajmniejszaKrawedz();
            road_alg3.Text += Distance(result);
            return result;
        }

        public int Distance(IList<int> result)
        {
            int distance = 0;
            for (int i = 1; i < result.Count(); i++)
            {
                distance += _adjMatrix[result[i - 1]][result[i]];
            }
            return distance;
        }

        public IList<int> DistanceBF(List<IList<int>> drogiBF)
        {
            var result = new List<int>();
            int distance = int.MaxValue;
            int minDistance = int.MaxValue;
            foreach (var droga in drogiBF)
            {
                distance = Distance(droga);
                if (distance < minDistance)
                {
                    result.Clear();
                    result.AddRange(droga);
                    minDistance = distance;
                }
            }
            return result;
        }

        private void FillIntersections()
        {
            // 1 row
            _intersections.Add(new Point(8.2, 15.6));
            _intersections.Add(new Point(222.6, 15.6));
            _intersections.Add(new Point(239.4, 15.6));
            _intersections.Add(new Point(457.8, 15.6));
            _intersections.Add(new Point(614.6, 15.6));
            _intersections.Add(new Point(937.8, 15.6));
            // 2 row
            _intersections.Add(new Point(8.2, 74.8));
            _intersections.Add(new Point(79.4, 74.8));
            _intersections.Add(new Point(222.6, 74.8));
            _intersections.Add(new Point(239.4, 74.8));
            _intersections.Add(new Point(347.4, 74.8));
            _intersections.Add(new Point(457.8, 74.8));
            _intersections.Add(new Point(614.6, 74.8));
            _intersections.Add(new Point(781, 74.8));
            _intersections.Add(new Point(937.8, 74.8));
            // 3 row
            _intersections.Add(new Point(8.2, 133.2));
            _intersections.Add(new Point(79.4, 133.2));
            _intersections.Add(new Point(120.2, 133.2));
            _intersections.Add(new Point(222.6, 133.2));
            _intersections.Add(new Point(239.4, 133.2));
            _intersections.Add(new Point(347.4, 133.2));
            _intersections.Add(new Point(457.8, 133.2));
            _intersections.Add(new Point(614.6, 133.2));
            _intersections.Add(new Point(781, 133.2));
            // 4 row
            _intersections.Add(new Point(8.2, 194.8));
            _intersections.Add(new Point(120.2, 194.8));
            _intersections.Add(new Point(222.6, 194.8));
            _intersections.Add(new Point(239.4, 194.8));
            _intersections.Add(new Point(347.4, 194.8));
            _intersections.Add(new Point(457.8, 194.8));
            _intersections.Add(new Point(529.8, 194.8));
            _intersections.Add(new Point(614.6, 194.8));
            _intersections.Add(new Point(781, 194.8));
            _intersections.Add(new Point(937.8, 194.8));
            // 5 row
            _intersections.Add(new Point(8.2, 250.8));
            _intersections.Add(new Point(120.2, 250.8));
            _intersections.Add(new Point(222.6, 250.8));
            _intersections.Add(new Point(239.4, 250.8));
            _intersections.Add(new Point(347.4, 250.8));
            _intersections.Add(new Point(457.8, 250.8));
            _intersections.Add(new Point(531.4, 250.8));
            _intersections.Add(new Point(614.6, 250.8));
            _intersections.Add(new Point(781, 250.8));
            _intersections.Add(new Point(937.8, 250.8));
            // 6 row
            _intersections.Add(new Point(8.2, 311.6));
            _intersections.Add(new Point(120.2, 311.6));
            _intersections.Add(new Point(222.6, 311.6));
            _intersections.Add(new Point(239.4, 311.6));
            _intersections.Add(new Point(347.4, 311.6));
            _intersections.Add(new Point(457.8, 311.6));
            _intersections.Add(new Point(614.6, 311.6));
            _intersections.Add(new Point(781, 311.6));
            _intersections.Add(new Point(937.8, 311.6));
            // 7 row
            _intersections.Add(new Point(8.2, 374));
            _intersections.Add(new Point(120.2, 374));
            _intersections.Add(new Point(222.6, 374));
            _intersections.Add(new Point(239.4, 374));
            _intersections.Add(new Point(347.4, 374));
            _intersections.Add(new Point(457.8, 374));
            _intersections.Add(new Point(614.6, 374));
            _intersections.Add(new Point(781, 374));
            _intersections.Add(new Point(864.2, 374));
            // 8 row
            _intersections.Add(new Point(8.2, 435.6));
            _intersections.Add(new Point(119.4, 435.6));
            _intersections.Add(new Point(222.6, 435.6));
            _intersections.Add(new Point(239.4, 435.6));
            _intersections.Add(new Point(347.4, 435.6));
            _intersections.Add(new Point(457.8, 435.6));
            _intersections.Add(new Point(614.6, 435.6));
            _intersections.Add(new Point(781, 435.6));
            _intersections.Add(new Point(867.4, 435.6));
            _intersections.Add(new Point(937.8, 435.6));
            // 9 row
            _intersections.Add(new Point(8.2, 495.6));
            _intersections.Add(new Point(120.2, 495.6));
            _intersections.Add(new Point(222.6, 495.6));
            _intersections.Add(new Point(239.4, 496.4));
            _intersections.Add(new Point(347.4, 495.6));
            _intersections.Add(new Point(457.8, 495.6));
            _intersections.Add(new Point(534.6, 495.6));
            _intersections.Add(new Point(614.6, 495.6));
            _intersections.Add(new Point(704.2, 495.6));
            _intersections.Add(new Point(781, 495.6));
            _intersections.Add(new Point(937.8, 495.6));
            // 10 row
            _intersections.Add(new Point(121.8, 553.2));
            _intersections.Add(new Point(222.6, 553.2));
            _intersections.Add(new Point(239.4, 553.2));
            _intersections.Add(new Point(347.4, 553.2));
            _intersections.Add(new Point(457.8, 553.2));
            _intersections.Add(new Point(533.8, 553.2));
            _intersections.Add(new Point(614.6, 553.2));
            _intersections.Add(new Point(708.2, 553.2));
            _intersections.Add(new Point(781, 553.2));
            _intersections.Add(new Point(937.8, 553.2));
            // 11 row
            _intersections.Add(new Point(8.2, 613.2));
            _intersections.Add(new Point(123.4, 613.2));
            _intersections.Add(new Point(222.6, 613.2));
            _intersections.Add(new Point(239.4, 613.2));
            _intersections.Add(new Point(347.4, 613.2));
            _intersections.Add(new Point(457.8, 613.2));
            _intersections.Add(new Point(535.4, 613.2));
            _intersections.Add(new Point(614.6, 613.2));
            _intersections.Add(new Point(709, 613.2));
            _intersections.Add(new Point(781, 613.2));
            _intersections.Add(new Point(937.8, 613.2));
            // 12 row
            _intersections.Add(new Point(8.2, 674));
            _intersections.Add(new Point(123.4, 674));
            _intersections.Add(new Point(222.6, 674));
            _intersections.Add(new Point(239.4, 674));
            _intersections.Add(new Point(347.4, 674));
            _intersections.Add(new Point(457.8, 674));
            _intersections.Add(new Point(536.2, 674));
            _intersections.Add(new Point(614.6, 674));
            _intersections.Add(new Point(710.6, 674));
            _intersections.Add(new Point(781, 674));
            _intersections.Add(new Point(937.8, 674));
            // 13 row
            _intersections.Add(new Point(8.2, 731.6));
            _intersections.Add(new Point(123.4, 731.6));
            _intersections.Add(new Point(222.6, 731.6));
            _intersections.Add(new Point(239.4, 731.6));
            _intersections.Add(new Point(347.4, 731.6));
            _intersections.Add(new Point(457.8, 731.6));
            _intersections.Add(new Point(614.6, 731.6));
            _intersections.Add(new Point(781, 731.6));
            // 14 row
            _intersections.Add(new Point(8.2, 794));
            _intersections.Add(new Point(125.8, 794));
            _intersections.Add(new Point(222.6, 794));
            _intersections.Add(new Point(239.4, 794));
            _intersections.Add(new Point(347.4, 794));
            _intersections.Add(new Point(457.8, 794));
            _intersections.Add(new Point(614.6, 794));
            _intersections.Add(new Point(781, 794));
            _intersections.Add(new Point(937.8, 794));


            _dots = _intersections.ToList<Point>();

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

        DispatcherTimer m_oTimer = new DispatcherTimer();
        DispatcherTimer m_oTimer2 = new DispatcherTimer();
        DispatcherTimer m_oTimer3 = new DispatcherTimer();
        public void DrawingFormNN()
        {
            m_oTimer = new DispatcherTimer();
            m_oTimer.Tick += m_oTimer_Tick1NN;
            m_oTimer.Interval = new TimeSpan(0, 0, 0, 0, SPEED);
            m_oTimer.Start();
        }

        public void DrawingFormBF()
        {
            m_oTimer2 = new DispatcherTimer();
            m_oTimer2.Tick += m_oTimer_Tick1BF;
            m_oTimer2.Interval = new TimeSpan(0, 0, 0, 0, SPEED);
            m_oTimer2.Start();
        }

        public void DrawingFormGreedy()
        {
            m_oTimer3 = new DispatcherTimer();
            m_oTimer3.Tick += m_oTimer_Tick1Greedy;
            m_oTimer3.Interval = new TimeSpan(0, 0, 0, 0, SPEED);
            m_oTimer3.Start();
        }

        private IList<int> resultBF = null;
        private IList<int> resultNN = null;
        private IList<int> resultGreedy = null;

        private int increment = 0;
        private int increment2 = 0;
        private int increment3 = 0;

        Polygon myPolygonBF = null;
        Polygon myPolygonNN = null;
        Polygon myPolygonGreedy = null;

        void m_oTimer_Tick1NN(object sender, EventArgs e)
        {
            if (increment < resultNN.Count - 1)
            {

                if (myPolygonNN != null)
                {
                    CanvasImage.Children.Remove(myPolygonNN);
                }
                var point1Index = resultNN[increment];
                var point1Coor = _points[point1Index].Coor;
                var point2Index = resultNN[increment + 1];
                var point2Coor = _points[point2Index].Coor;

                var line = new Line
                {
                    Stroke = System.Windows.Media.Brushes.OrangeRed,
                    Fill = System.Windows.Media.Brushes.OrangeRed,
                    X1 = point1Coor.X,
                    Y1 = point1Coor.Y,
                    X2 = point2Coor.X,
                    Y2 = point2Coor.Y,
                    StrokeThickness = 4,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom

                };
                double roznicaX = point1Coor.X - point2Coor.X;
                double roznicaY = point1Coor.Y - point2Coor.Y;

                System.Windows.Point Point2;
                System.Windows.Point Point3;


                if (Math.Abs(roznicaX) > Math.Abs(roznicaY))
                {
                    if (roznicaX > 0)
                    {
                        Point2 = new System.Windows.Point(point2Coor.X + 20, point2Coor.Y + 10);
                        Point3 = new System.Windows.Point(point2Coor.X + 20, point2Coor.Y - 10);
                    }
                    else
                    {
                        Point2 = new System.Windows.Point(point2Coor.X - 20, point2Coor.Y + 10);
                        Point3 = new System.Windows.Point(point2Coor.X - 20, point2Coor.Y - 10);
                    }
                }
                else
                {
                    if (roznicaY > 0)
                    {
                        Point2 = new System.Windows.Point(point2Coor.X - 10, point2Coor.Y + 20);
                        Point3 = new System.Windows.Point(point2Coor.X + 10, point2Coor.Y + 20);
                    }
                    else
                    {
                        Point3 = new System.Windows.Point(point2Coor.X + 10, point2Coor.Y - 20);
                        Point2 = new System.Windows.Point(point2Coor.X - 10, point2Coor.Y - 20);
                    }
                }


                myPolygonNN = new Polygon();
                myPolygonNN.Stroke = System.Windows.Media.Brushes.Black;
                myPolygonNN.Fill = System.Windows.Media.Brushes.OrangeRed;
                myPolygonNN.StrokeThickness = 2;
                myPolygonNN.HorizontalAlignment = HorizontalAlignment.Left;
                myPolygonNN.VerticalAlignment = VerticalAlignment.Center;

                PointCollection myPointCollection = new PointCollection();
                System.Windows.Point Point1 = new System.Windows.Point(point2Coor.X, point2Coor.Y);
                myPointCollection.Add(Point1);
                myPointCollection.Add(Point2);
                myPointCollection.Add(Point3);
                myPolygonNN.Points = myPointCollection;
                CanvasImage.Children.Add(myPolygonNN);

                _lines.Add(line);
                CanvasImage.Children.Add(line);
                increment++;
            }
            else
            {
                CanvasImage.Children.Remove(myPolygonNN);
                m_oTimer.Stop();
                increment = 0;
            }
            if (!m_oTimer.IsEnabled && !m_oTimer2.IsEnabled && !m_oTimer3.IsEnabled)
            {
                removePointsButton.IsEnabled = true;
            }

        }

        void m_oTimer_Tick1BF(object sender, EventArgs e)
        {
            if (increment2 < resultBF.Count - 1)
            {

                if (myPolygonBF != null)
                {
                    CanvasImage.Children.Remove(myPolygonBF);
                }
                var point1Index = resultBF[increment2];
                var point1Coor = _points[point1Index].Coor;
                var point2Index = resultBF[increment2 + 1];
                var point2Coor = _points[point2Index].Coor;

                var line = new Line
                {
                    Stroke = System.Windows.Media.Brushes.DeepSkyBlue,
                    Fill = System.Windows.Media.Brushes.DeepSkyBlue,
                    X1 = point1Coor.X + 4,
                    Y1 = point1Coor.Y + 4,
                    X2 = point2Coor.X + 4,
                    Y2 = point2Coor.Y + 4,
                    StrokeThickness = 4,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom

                };

                double roznicaX = point1Coor.X - point2Coor.X;
                double roznicaY = point1Coor.Y - point2Coor.Y;

                System.Windows.Point Point2;
                System.Windows.Point Point3;

                if (Math.Abs(roznicaX) > Math.Abs(roznicaY))
                {
                    if (roznicaX > 0)
                    {
                        Point2 = new System.Windows.Point(point2Coor.X + 4 + 20, point2Coor.Y + 4 + 10);
                        Point3 = new System.Windows.Point(point2Coor.X + 4 + 20, point2Coor.Y + 4 - 10);
                    }
                    else
                    {
                        Point2 = new System.Windows.Point(point2Coor.X + 4 - 20, point2Coor.Y + 4 + 10);
                        Point3 = new System.Windows.Point(point2Coor.X + 4 - 20, point2Coor.Y + 4 - 10);
                    }
                }
                else
                {
                    if (roznicaY > 0)
                    {
                        Point2 = new System.Windows.Point(point2Coor.X + 4 - 10, point2Coor.Y + 4 + 20);
                        Point3 = new System.Windows.Point(point2Coor.X + 4 + 10, point2Coor.Y + 4 + 20);
                    }
                    else
                    {
                        Point3 = new System.Windows.Point(point2Coor.X + 4 + 10, point2Coor.Y + 4 - 20);
                        Point2 = new System.Windows.Point(point2Coor.X + 4 - 10, point2Coor.Y + 4 - 20);
                    }
                }

                myPolygonBF = new Polygon();
                myPolygonBF.Stroke = System.Windows.Media.Brushes.Black;
                myPolygonBF.Fill = System.Windows.Media.Brushes.DeepSkyBlue;
                myPolygonBF.StrokeThickness = 2;
                myPolygonBF.HorizontalAlignment = HorizontalAlignment.Left;
                myPolygonBF.VerticalAlignment = VerticalAlignment.Center;

                PointCollection myPointCollection = new PointCollection();
                System.Windows.Point Point1 = new System.Windows.Point(point2Coor.X + 4, point2Coor.Y + 4);
                myPointCollection.Add(Point1);
                myPointCollection.Add(Point2);
                myPointCollection.Add(Point3);
                myPolygonBF.Points = myPointCollection;
                CanvasImage.Children.Add(myPolygonBF);

                _lines.Add(line);
                CanvasImage.Children.Add(line);
                increment2++;
            }
            else if (increment2 == resultBF.Count - 1)
            {
                CanvasImage.Children.Remove(myPolygonBF);
                m_oTimer2.Stop();
                increment2 = 0;
            }
            if (!m_oTimer.IsEnabled && !m_oTimer2.IsEnabled && !m_oTimer3.IsEnabled)
            {
                removePointsButton.IsEnabled = true;
            }
        }

        void m_oTimer_Tick1Greedy(object sender, EventArgs e)
        {
            if (increment3 < resultGreedy.Count - 1)
            {
                if (myPolygonGreedy != null)
                {
                    CanvasImage.Children.Remove(myPolygonGreedy);
                }
                var point1Index = resultGreedy[increment3];
                var point1Coor = _points[point1Index].Coor;
                var point2Index = resultGreedy[increment3 + 1];
                var point2Coor = _points[point2Index].Coor;

                var line = new Line
                {
                    Stroke = System.Windows.Media.Brushes.Green,
                    Fill = System.Windows.Media.Brushes.Green,
                    X1 = point1Coor.X - 4,
                    Y1 = point1Coor.Y - 4,
                    X2 = point2Coor.X - 4,
                    Y2 = point2Coor.Y - 4,
                    StrokeThickness = 4,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom

                };
                double roznicaX = point1Coor.X - point2Coor.X;
                double roznicaY = point1Coor.Y - point2Coor.Y;
                System.Windows.Point Point2;
                System.Windows.Point Point3;
                if (Math.Abs(roznicaX) > Math.Abs(roznicaY))
                {
                    if (roznicaX > 0)
                    {
                        Point2 = new System.Windows.Point(point2Coor.X - 4 + 20, point2Coor.Y - 4 + 10);
                        Point3 = new System.Windows.Point(point2Coor.X - 4 + 20, point2Coor.Y - 4 - 10);
                    }
                    else
                    {
                        Point2 = new System.Windows.Point(point2Coor.X - 4 - 20, point2Coor.Y - 4 + 10);
                        Point3 = new System.Windows.Point(point2Coor.X - 4 - 20, point2Coor.Y - 4 - 10);
                    }
                }
                else
                {
                    if (roznicaY > 0)
                    {
                        Point2 = new System.Windows.Point(point2Coor.X - 4 - 10, point2Coor.Y - 4 + 20);
                        Point3 = new System.Windows.Point(point2Coor.X - 4 + 10, point2Coor.Y - 4 + 20);
                    }
                    else
                    {
                        Point3 = new System.Windows.Point(point2Coor.X - 4 + 10, point2Coor.Y - 4 - 20);
                        Point2 = new System.Windows.Point(point2Coor.X - 4 - 10, point2Coor.Y - 4 - 20);
                    }
                }


                myPolygonGreedy = new Polygon();
                myPolygonGreedy.Stroke = System.Windows.Media.Brushes.Black;
                myPolygonGreedy.Fill = System.Windows.Media.Brushes.Green;
                myPolygonGreedy.StrokeThickness = 2;
                myPolygonGreedy.HorizontalAlignment = HorizontalAlignment.Left;
                myPolygonGreedy.VerticalAlignment = VerticalAlignment.Center;

                PointCollection myPointCollection = new PointCollection();
                System.Windows.Point Point1 = new System.Windows.Point(point2Coor.X - 4, point2Coor.Y - 4);
                myPointCollection.Add(Point1);
                myPointCollection.Add(Point2);
                myPointCollection.Add(Point3);
                myPolygonGreedy.Points = myPointCollection;
                CanvasImage.Children.Add(myPolygonGreedy);

                _lines.Add(line);
                CanvasImage.Children.Add(line);
                increment3++;
            }
            else if (increment3 == resultGreedy.Count - 1)
            {
                CanvasImage.Children.Remove(myPolygonGreedy);
                m_oTimer3.Stop();
                increment3 = 0;
            }
            if (!m_oTimer.IsEnabled && !m_oTimer2.IsEnabled && !m_oTimer3.IsEnabled)
            {
                removePointsButton.IsEnabled = true;
            }

        }
    }
}
