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
        private IList<Point> _usedPoints = new List<Point>();

        private IList<IList<int>> _adjMatrix = new List<IList<int>>();



        public DemoWindow()
        {
            InitializeComponent();
            FillIntersections();
            graphFillMatrix();
            
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

        private void graphFillMatrix()
        {
            int counter = 0;
            TaggedEdge<int, int>[] edge1 = new TaggedEdge<int, int>[300];
            adjMatrixFill();
            int[] vertex = new int[100];
            for (int i = 0; i < 26; i++)
            {
                vertex[i] = i;
            }
            var graph = new AdjacencyGraph<int, TaggedEdge<int, int>>();
            for (int i = 0; i < 26; i++)
            {
                graph.AddVertex(vertex[i]);
            }

            for (int i = 0; i < _adjMatrix.Count; i++)
            {
                for (int j = 0; j < _adjMatrix[i].Count; j++)
                {
                    if (_adjMatrix[i][j] != 0)
                    {
                        edge1[counter++] = new TaggedEdge<int, int>(vertex[i], vertex[j], _adjMatrix[i][j]);
                    }
                }
            }

            foreach (var elem in edge1)
            {
                if(elem != null)
                {
                    graph.AddEdge(elem);
                }
            }
            
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
            _intersections.Add(new Point(677.9000000000001, 97.60000000000001));

            // 3 row
            _intersections.Add(new Point(16.300000000000036, 184.8));
            _intersections.Add(new Point(185.90000000000003, 184.8));
            _intersections.Add(new Point(313.1000000000001, 184.8));
            _intersections.Add(new Point(425.1000000000001, 184.8));
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
        }


    }
}




 
