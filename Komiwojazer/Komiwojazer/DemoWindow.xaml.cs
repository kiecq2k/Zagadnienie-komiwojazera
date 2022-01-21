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
    /// Interaction logic for DemoWindow.xaml
    /// </summary>
    public partial class DemoWindow : Window
    {
        
        private int _flag = -1;
        private IList<Line> _lines = new List<Line>();
        private Points _startingPoint = null;
        private IList<Points> _points = new List<Points>();
        private int _counter = 0;
        private IList<Point> _intersections = new List<Point>();
        private IList<Point> _dots = new List<Point>();
        private int _pointCounter = 27;
        private int _zIndexCounter = 1;

        private IList<IList<int>> _adjMatrix = new List<IList<int>>();
        private IList<IList<int>> _startingAdjMatrix = new List<IList<int>>();
        private IList<Tuple<int, int>> _verticalCross = new List<Tuple<int, int>>();

        private const int STARTING_POINT = 26;
        private const int SPEED = 50;
        private Version _version = Version.Demo;


        public DemoWindow()
        {
            InitializeComponent();
            FillIntersections();
            adjMatrixFill();
            addToMatrix();
            
            for (int i = 0; i < 26; i++)
            {
                _points.Add(new Points { Coor = _intersections[i] });
            }
            for (int i = 0; i < 27; i++)
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
                    if ((_adjMatrix[i][j] != 0 || _adjMatrix[j][i] != 0) && Math.Abs(i - j) != 1)
                    {
                        _verticalCross.Add(new Tuple<int, int>(i, j));
                    }
                }
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

        private bool isOnRoad(Point pos)
        {
            for (int i = 1; i < _intersections.Count; i++)
            {
                if (pos.X > _dots[i - 1].X && pos.X < _dots[i].X &&
                    Math.Abs(pos.Y - _dots[i - 1].Y) < 8)
                {
                    return true;
                }
            }
            for (int i = 0; i < _verticalCross.Count; i++)
            {
                if (pos.Y > _intersections[_verticalCross[i].Item1].Y &&
                    pos.Y < _intersections[_verticalCross[i].Item2].Y &&
                    Math.Abs(pos.X - _intersections[_verticalCross[i].Item1].X) < 10)
                {
                    return true;
                }

            }
            return false;
        }

        private void CanvasImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var coord = e.GetPosition(this.CanvasImage);

            if (/*coord.IsOnRoad(Version.Demo)*/ 
                isOnRoad(coord) &&
                crossCheck(coord) && pointCheck(coord))
            {
                if (_flag == 1)
                {

                    if (_startingPoint == null)
                    {
                        _startingPoint = new Points();
                        _startingPoint.point = new Ellipse();
                        _startingPoint.point.Width = 20;
                        _startingPoint.point.Height = 20;
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
                    _points[_pointCounter].point.Width = 18;
                    _points[_pointCounter].point.Height = 18;
                    _points[_pointCounter].point.Fill = Brushes.SaddleBrown;
                    Canvas.SetLeft(_points[_pointCounter].point, coord.X - 5);
                    Canvas.SetTop(_points[_pointCounter].point, coord.Y - 5);
                    CanvasImage.Children.Add(_points[_pointCounter].point);
                    _points[_pointCounter].Coor = coord;
                    Canvas.SetZIndex(_points[_pointCounter].point, _zIndexCounter++);
                    pointPosition(e, _flag);
                    _counter++;
                    startAlgorithmButton.IsEnabled = true;
                }
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
            _startingPoint = null;
            _flag = -1;
            _counter = 0;
            _pointCounter = 27;
            endPointsButton.IsEnabled = false;
            startAlgorithmButton.IsEnabled = false;
            startPointButton.IsEnabled = true;
            removePointsButton.IsEnabled = false;
            checkboxNN.IsChecked = false;
            checkboxBF.IsChecked = false;
            checkbox3.IsChecked = false;
            for (int i = 0; i < 27; i++)
            {
                _adjMatrix.Add(new List<int>());
                _adjMatrix[i] = new List<int>(_startingAdjMatrix[i]);
            }
            _dots = _intersections.ToList();
            for (int i = 0; i < 26; i++)
            {
                _points.Add(new Points { Coor = _intersections[i] });
            }
        }

        private void startAlgorithmButton_Click(object sender, RoutedEventArgs e)
        {

            if (_startingPoint == null || _counter < 1)
            {
                MessageBox.Show("Nie wybrano punktów");
                return;
            }
            int buttonCheck = 0;

            if (checkboxNN.IsChecked == true)
            {
                resultNN = NajblizszySasiad();
                DrawingFormNN();
                buttonCheck = 1;
            }

            if(checkboxBF.IsChecked == true)
            {
                resultBF = BruteForce();
                DrawingFormBF();
                buttonCheck = 1;
            }
            
            if(checkbox3.IsChecked == true)
            {
                resultGreedy = Greedy();
                DrawingFormGreedy();
                buttonCheck = 1;
            }
            if (buttonCheck == 1) removePointsButton.IsEnabled = false;
        }

        private void adjMatrixFill()
        {
            string[] lines = File.ReadAllLines("graph_matrix_demo.txt");
            for(int i=0; i<lines.Length; i++)
            {
                var line = lines[i].Replace(",","").Split(" ");
                _adjMatrix.Add(new List<int>());

                foreach (var numberAsString in line)
                {
                    _adjMatrix[i].Add(int.Parse(numberAsString));
                }
            }
            
        }


        private void FillIntersections()
        {
            // 1 row
            _intersections.Add(new Point(241.4, 16.8));
            _intersections.Add(new Point(551, 16.8));
            _intersections.Add(new Point(877.4, 16.8));

            // 2 row
            _intersections.Add(new Point(19.8, 144));
            _intersections.Add(new Point(241.4, 144));
            _intersections.Add(new Point(551, 144));
            _intersections.Add(new Point(684.6, 144));
            _intersections.Add(new Point(877.4, 144));

            // 3 row
            _intersections.Add(new Point(19.8, 271.2));
            _intersections.Add(new Point(241.4, 271.2));
            _intersections.Add(new Point(406.2, 271.2));
            _intersections.Add(new Point(551, 271.2));
            _intersections.Add(new Point(684.6, 271.2));
            _intersections.Add(new Point(877.4, 271.2));

            // 4 row
            _intersections.Add(new Point(19.8, 400.8));
            _intersections.Add(new Point(112.6, 400.8));
            _intersections.Add(new Point(241.4, 400.8));
            _intersections.Add(new Point(406.2, 400.8));
            _intersections.Add(new Point(551, 400.8));
            _intersections.Add(new Point(877.4, 400.8));

            // 5 row
            _intersections.Add(new Point(19.8, 535.2));
            _intersections.Add(new Point(111.8, 535.2));
            _intersections.Add(new Point(241.4, 535.2));
            _intersections.Add(new Point(406.2, 535.2));
            _intersections.Add(new Point(551, 535.2));
            _intersections.Add(new Point(877.4, 535.2));

            _dots = _intersections.ToList<Point>();
            
        }

        void pointPosition(MouseButtonEventArgs e, int flag)
        {
            var pos = e.GetPosition(this.CanvasImage);
            int cross1 = -1;
            int cross2 = -1;
            int road1 = 0;
            int road2 = 0;

            for (int i = 1; i < _intersections.Count; i++)
            {
                if (pos.X > _dots[i - 1].X && pos.X < _dots[i].X &&
                    Math.Abs(pos.Y - _dots[i - 1].Y) < 8)
                {
                    road1 = (int)(Math.Abs(_dots[i - 1].X - pos.X) * 0.4);
                    road2 = (int)(Math.Abs(_dots[i].X - pos.X) * 0.4);
                    cross1 = i - 1;
                    cross2 = i;
                }

            }
            if (cross1 != -1)
            {
                for (int i = 26; i < _dots.Count; i++)
                {
                    int dotsRoad = (int)(Math.Abs(_dots[i].X - pos.X) * 0.4);
                    if (Math.Abs(pos.Y - _dots[i].Y) < 20) 
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
                //double close1 = double.MaxValue;
                //double close2 = double.MaxValue;
                //for (int i = 0; i < _intersections.Count; i++)
                //{
                //    double x = Math.Abs(_dots[i].X - pos.X);
                //    double y = Math.Abs(_dots[i].Y - pos.Y);
                //    double sum = x + y;
                //    if (sum < close1)
                //    {
                //        close2 = close1;
                //        cross2 = cross1;
                //        road2 = road1;
                //        close1 = sum;
                //        cross1 = i;
                //        road1 = (int)(y * 0.4);
                //    }
                //    else if (sum < close2 && sum != close1)
                //    {
                //        close2 = sum;
                //        cross2 = i;
                //        road2 = (int)(y * 0.4);
                //    }
                //}
                //int tmpcross1 = cross1;
                //int tmpcross2 = cross2;

                for (int i = 0; i < _verticalCross.Count; i++)
                {
                    if (pos.Y > _intersections[_verticalCross[i].Item1].Y &&
                        pos.Y < _intersections[_verticalCross[i].Item2].Y &&
                        Math.Abs(pos.X - _intersections[_verticalCross[i].Item1].X) < 20)
                    {
                        cross1 = _verticalCross[i].Item1;
                        cross2 = _verticalCross[i].Item2;
                        road1 = (int)Math.Abs((_dots[_verticalCross[i].Item1].Y - pos.Y) * 0.4);
                        road2 = (int)Math.Abs((_dots[_verticalCross[i].Item2].Y - pos.Y) * 0.4);
                    }

                }
                if (cross1 == -1)
                {
                    MessageBox.Show("pionowe blad");
                }
                int tmpcross1 = cross1;
                int tmpcross2 = cross2;

                for (int i = 26; i < _dots.Count; i++)
                {
                    int dotsRoad = (int)(Math.Abs(_dots[i].Y - pos.Y) * 0.4);
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
                indexOfPoint = 26;
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

        private void addToMatrix()
        {
            foreach(var elem in _adjMatrix)
            {
                elem.Add(0);
            }
            _adjMatrix.Add(new List<int>());
            for (int i = 0; i < _adjMatrix.Count; i++)
            {
                _adjMatrix[_adjMatrix.Count-1].Add(0);
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
            for(int i =1;i<result.Count();i++)
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
                distance =Distance(droga);
                if (distance < minDistance)
                {
                    result.Clear();
                    result.AddRange(droga);
                    minDistance = distance;
                }
            }
            return result;
        }

        DispatcherTimer m_oTimer = new DispatcherTimer();
        DispatcherTimer m_oTimer2 = new DispatcherTimer();
        DispatcherTimer m_oTimer3 = new DispatcherTimer();

        public void DrawingFormNN()
        {
            m_oTimer = new DispatcherTimer();
            m_oTimer.Tick += m_oTimer_Tick1NN;
            m_oTimer.Interval = new TimeSpan(0,0,0,0,SPEED);
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
            if (increment2 < resultBF.Count - 1 && !m_oTimer.IsEnabled)
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
            else if(increment2 == resultBF.Count - 1)
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
            if (increment3 < resultGreedy.Count - 1 && !m_oTimer2.IsEnabled && !m_oTimer.IsEnabled)
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
                        Point2 = new System.Windows.Point(point2Coor.X-4 + 20, point2Coor.Y-4 + 10);
                        Point3 = new System.Windows.Point(point2Coor.X-4 + 20, point2Coor.Y-4 - 10);
                    }
                    else
                    {
                        Point2 = new System.Windows.Point(point2Coor.X-4 - 20, point2Coor.Y-4 + 10);
                        Point3 = new System.Windows.Point(point2Coor.X-4 - 20, point2Coor.Y-4 - 10);
                    }
                }
                else
                {
                    if (roznicaY > 0)
                    {
                        Point2 = new System.Windows.Point(point2Coor.X-4 - 10, point2Coor.Y-4 + 20);
                        Point3 = new System.Windows.Point(point2Coor.X-4 + 10, point2Coor.Y-4 + 20);
                    }
                    else
                    {
                        Point3 = new System.Windows.Point(point2Coor.X-4 + 10, point2Coor.Y-4 - 20);
                        Point2 = new System.Windows.Point(point2Coor.X-4 - 10, point2Coor.Y-4 - 20);
                    }
                }


                myPolygonGreedy = new Polygon();
                myPolygonGreedy.Stroke = System.Windows.Media.Brushes.Black;
                myPolygonGreedy.Fill = System.Windows.Media.Brushes.Green;
                myPolygonGreedy.StrokeThickness = 2;
                myPolygonGreedy.HorizontalAlignment = HorizontalAlignment.Left;
                myPolygonGreedy.VerticalAlignment = VerticalAlignment.Center;

                PointCollection myPointCollection = new PointCollection();
                System.Windows.Point Point1 = new System.Windows.Point(point2Coor.X-4, point2Coor.Y-4);
                myPointCollection.Add(Point1);
                myPointCollection.Add(Point2);
                myPointCollection.Add(Point3);
                myPolygonGreedy.Points = myPointCollection;
                CanvasImage.Children.Add(myPolygonGreedy);

                _lines.Add(line);
                CanvasImage.Children.Add(line);
                increment3++;
            }
            else if(increment3 == resultGreedy.Count - 1)
            {
                CanvasImage.Children.Remove(myPolygonGreedy);
                m_oTimer3.Stop();
                increment3 = 0;
            }

            if(!m_oTimer.IsEnabled && !m_oTimer2.IsEnabled && !m_oTimer3.IsEnabled)
            {
                removePointsButton.IsEnabled = true;
            }

        }
    }
}