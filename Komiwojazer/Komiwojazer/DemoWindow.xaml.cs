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

namespace Komiwojazer
{
    /// <summary>
    /// Interaction logic for DemoWindow.xaml
    /// </summary>
    public partial class DemoWindow : Window
    {
        private int _startupCounter = 0;
        private int _flag = -1;
        private IList<Line> _lines = new List<Line>();
        private Points _startingPoint = null;
        private IList<Points> _points = new List<Points>();
        private int _counter = 0;
        private IList<Point> _intersections = new List<Point>();
        private IList<Point> _usedPoints = new List<Point>();
        private int _pointCounter = 27;

        public IList<IList<int>> _adjMatrix = new List<IList<int>>();
        private AdjacencyGraph<int, TaggedEdge<int, int>> _graph = new AdjacencyGraph<int, TaggedEdge<int, int>>();
        private const int STARTING_POINT = 26;
 


        public DemoWindow()
        {
            InitializeComponent();
            //DrawingForm();
            if (_adjMatrix.Count == 0) startup();
            //DijkstraAlgorithm _dj = new DijkstraAlgorithm(_adjMatrix);
            //_dijkstra = _dj;
        }

        private void startup()
        {

            _startupCounter++;
            FillIntersections();
            graphFillMatrixStartup();
            for (int i = 0; i < 26; i++)
            {
                _points.Add(new Points { Coor = _intersections[i] });
            }
        }

        private void endButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CanvasImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var coord = e.GetPosition(this.CanvasImage);

            if (coord.IsOnRoad(Version.Demo))
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
                    
                    pointPosition(e, _flag);
                    _startingPoint.Coor = coord;
                    _points.Add(new Points { Coor = _startingPoint.Coor });
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
                    
                    pointPosition(e, _flag);
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
                return;
            }
            AlgorithmsPick ap = new AlgorithmsPick();

            if (ap.checkboxNN.IsChecked == true)
            {
                resultNN = NajblizszySasiad();
                DrawingFormNN();
            }
            if (ap.checkboxBF.IsChecked == true)
            {
                resultBF = BruteForce();
                DrawingFormBF();
            }
            if (ap.checkbox3.IsChecked == true)
            {

            }

            var greedy = new Greedy(_adjMatrix);
            var res = greedy.NajmniejszaKrawedz();
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

        private void graphFillMatrixStartup()
        {
           
            IList<TaggedEdge<int, int>> edge = new List<TaggedEdge<int, int>>();
            adjMatrixFill();
            IList<int> vertex = new List<int>();
            for (int i = 0; i < _adjMatrix.Count; i++)
            {
                vertex.Add(i);
            }
            var graph = new AdjacencyGraph<int, TaggedEdge<int, int>>();
            for (int i = 0; i < _adjMatrix.Count; i++)
            {
                graph.AddVertex(vertex[i]);
            }

            for (int i = 0; i < _adjMatrix.Count; i++)
            {
                for (int j = 0; j < _adjMatrix[i].Count; j++)
                {
                    if (_adjMatrix[i][j] != 0)
                    {
                        edge.Add(new TaggedEdge<int, int>(vertex[i], vertex[j], _adjMatrix[i][j]));
                    }
                }
            }

            foreach (var elem in edge)
            {
                if(elem != null)
                {
                    graph.AddEdge(elem);
                }
            }
            _graph = graph;
            addToMatrix();
        }

        private void FillIntersections()
        {
            // 1 row
            _intersections.Add(new Point(185.90000000000003, 10.400000000000002));
            _intersections.Add(new Point(425.1000000000001, 10.400000000000002));
            _intersections.Add(new Point(677.9000000000001, 10.400000000000002));

            // 2 row
            _intersections.Add(new Point(14.700000000000035, 97.60000000000001));
            _intersections.Add(new Point(185.90000000000003, 97.60000000000001));
            _intersections.Add(new Point(425.1000000000001, 97.60000000000001));
            _intersections.Add(new Point(527.5, 97.60000000000001));
            _intersections.Add(new Point(677.9000000000001, 97.60000000000001));

            // 3 row
            _intersections.Add(new Point(16.300000000000036, 184.8));
            _intersections.Add(new Point(185.90000000000003, 184.8));
            _intersections.Add(new Point(313.1000000000001, 184.8));
            _intersections.Add(new Point(425.1000000000001, 184.8));
            _intersections.Add(new Point(527.5, 184.8));
            _intersections.Add(new Point(677.9000000000001, 184.8));

            // 4 row
            _intersections.Add(new Point(16.300000000000036, 275.2));
            _intersections.Add(new Point(87.50000000000004, 275.2));
            _intersections.Add(new Point(185.90000000000003, 275.2));
            _intersections.Add(new Point(313.1000000000001, 275.2));
            _intersections.Add(new Point(425.1000000000001, 275.2));
            _intersections.Add(new Point(677.9000000000001, 275.2));

            // 5 row
            _intersections.Add(new Point(16.300000000000036, 369.6));
            _intersections.Add(new Point(85.90000000000003, 369.6));
            _intersections.Add(new Point(185.90000000000003, 369.6));
            _intersections.Add(new Point(315.50000000000006, 369.6));
            _intersections.Add(new Point(425.1000000000001, 369.6));
            _intersections.Add(new Point(677.9000000000001, 369.6));


            //foreach (var elem in _intersections)
            //{
            //    _startingPoint = new Points();
            //    _startingPoint.point = new Ellipse();
            //    _startingPoint.point.Width = 10;
            //    _startingPoint.point.Height = 10;
            //    _startingPoint.point.Fill = Brushes.Red;
            //    CanvasImage.Children.Add(_startingPoint.point);

            //    Canvas.SetLeft(_startingPoint.point, elem.X - 5);
            //    Canvas.SetTop(_startingPoint.point, elem.Y - 5);
            //}
        

            
        
        }


        void pointPosition(MouseButtonEventArgs e, int flag)
        {
            var pos = e.GetPosition(this.CanvasImage);
            int cross1 = -1;
            int cross2 = -1;
            double x1;
            double x2;
            //double y;
            int road1 = 0;
            int road2 = 0;

            for (int i = 1; i < _intersections.Count; i++)
            {
                if (pos.X > _intersections[i - 1].X && pos.X < _intersections[i].X &&
                    Math.Abs(pos.Y - _intersections[i - 1].Y) < 7)
                {
                    road1 = (int)(Math.Abs(_intersections[i - 1].X - pos.X) * 0.4);
                    road2 = (int)(Math.Abs(_intersections[i].X - pos.X) * 0.4);
                    cross1 = i - 1;
                    cross2 = i;
                }
            }
            if (cross1 == -1)
            {

                double close1 = double.MaxValue;
                double close2 = double.MaxValue;
                for (int i = 0; i < _intersections.Count; i++)
                {
                    double x = Math.Abs(_intersections[i].X - pos.X);
                    double y = Math.Abs(_intersections[i].Y - pos.Y);
                    double sum = x + y;
                    if (sum < close1)
                    {
                        close2 = close1;
                        cross2 = cross1;
                        road2 = road1;
                        close1 = sum;
                        cross1 = i;
                        road1 = (int)(y * 0.4);
                    }
                    else if (sum < close2 && sum != close1)
                    {
                        close2 = sum;
                        cross2 = i;
                        road2 = (int)(y * 0.4);
                    }
                }
                
            }
            


            //add point to matrix and delete roads between crossroads
            int indexOfPoint;
            if(flag == 1)
            {
                indexOfPoint = 26;
            }
            else
            {
                indexOfPoint = _pointCounter++;
                addToMatrix();
            }
            if (_adjMatrix[cross1][cross2] != 0)
            {
                _adjMatrix[cross1][cross2] = 0;
                _adjMatrix[cross1][indexOfPoint] = road1;
                _adjMatrix[indexOfPoint][cross2] = road2;
            }
            if (_adjMatrix[cross2][cross1] != 0)
            {
                _adjMatrix[cross2][cross1] = 0;
                _adjMatrix[cross2][indexOfPoint] = road2;
                _adjMatrix[indexOfPoint][cross1] = road1;
            }
            matrixToGraph();
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

        private void matrixToGraph()
        {
           
            IList<TaggedEdge<int, int>> edge = new List<TaggedEdge<int, int>>();
            IList<int> vertex = new List<int>();
            for (int i = 0; i < _adjMatrix.Count; i++)
            {
                vertex.Add(i);
            }
            var graphTMP = new AdjacencyGraph<int, TaggedEdge<int, int>>();
            for (int i = 0; i < _adjMatrix.Count; i++)
            {
                graphTMP.AddVertex(vertex[i]);
            }

            for (int i = 0; i < _adjMatrix.Count; i++)
            {
                for (int j = 0; j < _adjMatrix[i].Count; j++)
                {
                    if (_adjMatrix[i][j] != 0)
                    {
                        edge.Add(new TaggedEdge<int, int>(vertex[i], vertex[j], _adjMatrix[i][j]));
                    }
                }
            }

            foreach (var elem in edge)
            {
                if (elem != null)
                {
                    graphTMP.AddEdge(elem);
                }
            }
            _graph = graphTMP;
            
        }

        private IList<int> NajblizszySasiad()
        {
            var dijkstra = new Dijkstra(_adjMatrix);
            var result = dijkstra.GetPath();
         
            road_alg1.Text += Disatnce(result);
            return result;
        }

        public IList<int> BruteForce()
        {
            var bruteForce = new BruteForce(_adjMatrix);
            var result = bruteForce.GetPathBF2();
            var result2 = DistanceBF(result);
            road_alg2.Text += Disatnce(result2);
            return result2;
        }

        private void DrawPath(IList<int> result)
        {
            for(int i = 0; i < result.Count - 1; i++)
            {
                var point1Index = result[i];
                var point1Coor = _points[point1Index].Coor;
                var point2Index = result[i + 1];
                var point2Coor = _points[point2Index].Coor;

                var line = new Line
                {
                    Stroke = System.Windows.Media.Brushes.Red,
                    Fill = System.Windows.Media.Brushes.Red,
                    X1 = point1Coor.X,
                    Y1 = point1Coor.Y,
                    X2 = point2Coor.X,
                    Y2 = point2Coor.Y,
                    StrokeThickness = 4,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom

                };

                _lines.Add(line);
                CanvasImage.Children.Add(line);
            }
        }

        private void removeRoadButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var line in _lines)
            {
                CanvasImage.Children.Remove(line);
            }
        }

        private bool AllPointsVisited()
        {
            for(int i=27; i <_points.Count; i++)
            {
                if (!_points[i].Visited)
                    return false;
            }

            return true;
        }

        public int Disatnce(IList<int> result)
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
                distance =Disatnce(droga);
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

        public void DrawingFormNN()
        {
            //InitializeComponent();

            m_oTimer.Tick += m_oTimer_Tick1NN;
            m_oTimer.Interval = new TimeSpan(0,0,0,0,300);
            
            m_oTimer.Start();
        }

        // Enable the timer and call m_oTimer.Start () when
        // you're ready to draw your lines.
        private IList<int> resultBF = null;
        private IList<int> resultNN = null;

        private int increment = 0;
        private int increment2 = 0;

        Polygon myPolygonBF = null;
        Polygon myPolygonNN = null;

        void m_oTimer_Tick1NN(object sender, EventArgs e)
        {
            // Draw the next line here; disable
            // the timer when done with drawing.
            //m_oTimer.Start();
            //for (int i = 0; i < result.Count - 1; i++)
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
                    X1 = point1Coor.X + (increment * 0.05),
                    Y1 = point1Coor.Y + (increment * 0.05),
                    X2 = point2Coor.X + (increment * 0.05),
                    Y2 = point2Coor.Y + (increment * 0.05),
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
            }
            
        }


        public void DrawingFormBF()
        {
            //InitializeComponent();

            m_oTimer2.Tick += m_oTimer_Tick1BF;
            m_oTimer2.Interval = new TimeSpan(0, 0, 0, 0, 300);
            //m_oTimer.Enabled = false;
            m_oTimer2.Start();
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
                    X1 = point1Coor.X + 4 + (increment2 * 0.05),
                    Y1 = point1Coor.Y + 4 + (increment2 * 0.05),
                    X2 = point2Coor.X + 4 + (increment2 * 0.05),
                    Y2 = point2Coor.Y + 4 + (increment2 * 0.05),
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
            else
            {
                CanvasImage.Children.Remove(myPolygonBF);
                m_oTimer2.Stop();
            }
            //m_oTimer.Stop();
        }

        private double GetDistance(Point p1, Point p2) => Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
    }
}




 
