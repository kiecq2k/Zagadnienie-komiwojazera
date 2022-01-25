using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Komiwojazer.Algorithms
{

    public class Result
    {
        public int Distance { get; set; }
        public List<int> Route { get; set; }


        public Result()
        {
            Route = new List<int>();
        }
    }

    public class Dijkstra : BaseAlgorithm
    {
        private static readonly int NO_PARENT = -1;

        public Dijkstra(Version version, IList<IList<int>> g) : base(version, g)
        {
        }

        public Result[] dijkstra(int startVertex)
        {
            int nVertices = G.Count;

            // shortestDistances[i] will hold the
            // shortest distance from src to i
            int[] shortestDistances = new int[nVertices];

            // added[i] will true if vertex i is
            // included / in shortest path tree
            // or shortest distance from src to
            // i is finalized
            bool[] added = new bool[nVertices];

            // Initialize all distances as
            // INFINITE and added[] as false
            for (int vertexIndex = 0; vertexIndex < nVertices;
                                                vertexIndex++)
            {
                shortestDistances[vertexIndex] = int.MaxValue - 1;
                added[vertexIndex] = false;
            }

            // Distance of source vertex from
            // itself is always 0
            shortestDistances[startVertex] = 0;

            // Parent array to store shortest
            // path tree
            int[] parents = new int[nVertices];

            // The starting vertex does not
            // have a parent
            parents[startVertex] = NO_PARENT;

            // Find shortest path for all
            // vertices
            for (int i = 1; i < nVertices; i++)
            {

                // Pick the minimum distance vertex
                // from the set of vertices not yet
                // processed. nearestVertex is
                // always equal to startNode in
                // first iteration.
                int nearestVertex = -1;
                int shortestDistance = int.MaxValue - 1;
                for (int vertexIndex = 0;
                        vertexIndex < nVertices;
                        vertexIndex++)
                {
                    if (!added[vertexIndex] &&
                        shortestDistances[vertexIndex] <
                        shortestDistance)
                    {
                        nearestVertex = vertexIndex;
                        shortestDistance = shortestDistances[vertexIndex];
                    }
                }

                // Mark the picked vertex as
                // processed
                added[nearestVertex] = true;

                // Update dist value of the
                // adjacent vertices of the
                // picked vertex.
                for (int vertexIndex = 0;
                        vertexIndex < nVertices;
                        vertexIndex++)
                {
                    int edgeDistance = G[nearestVertex][vertexIndex];

                    if (edgeDistance > 0
                        && ((shortestDistance + edgeDistance) <
                            shortestDistances[vertexIndex]))
                    {
                        parents[vertexIndex] = nearestVertex;
                        shortestDistances[vertexIndex] = shortestDistance +
                                                        edgeDistance;
                    }
                }
            }

            //printSolution(startVertex, shortestDistances, parents);
            return GetResult(startVertex, shortestDistances, parents);

        }

        private static Result[] GetResult(int startVertex,
                                    int[] distances,
                                    int[] parents)
        {
            int nVertices = distances.Length;
            var result = new Result[nVertices];
            Console.Write("Vertex\t Distance\tPath");

            for (int vertexIndex = 0; vertexIndex < nVertices; vertexIndex++)
            {
                if (vertexIndex != startVertex)
                {
                    var path = new List<int>();
                    GetPath(vertexIndex, parents, path);

                    result[vertexIndex] = new Result()
                    {
                        Distance = distances[vertexIndex],
                        Route = path
                    };
                }
                else
                    result[vertexIndex] = new Result();
            }

            return result;
        }

        // Function to print shortest path
        // from source to currentVertex
        // using parents array
        private static void GetPath(int currentVertex, int[] parents, IList<int> path)
        {

            // Base case : Source node has
            // been processed
            if (currentVertex == NO_PARENT)
            {
                return;
            }
            GetPath(parents[currentVertex], parents, path);
            path.Add(currentVertex);
        }

        public IList<int> GetPath()
        {
            var result = new List<int>();
            int startPoint = STARTING_POINT;

            while (!AllPointsUsed())
            {
                var paths = dijkstra(startPoint);

                int minDistance = int.MaxValue;

                for (int i = STARTING_POINT + 1; i < N; i++)
                {
                    if (paths[i].Distance < minDistance && !_usedPoints.Contains(i))
                    {
                        minDistance = paths[i].Distance;
                        startPoint = i;
                    }
                }

                _usedPoints.Add(startPoint);

                for (int i = 0; i < paths[startPoint].Route.Count; i++)
                    result.Add(paths[startPoint].Route[i]);
            }

            // comeback path
            var lastVisitedNode = result[result.Count - 1];
            var cbPath = dijkstra(lastVisitedNode);
            for (int i = 0; i < cbPath[STARTING_POINT].Route.Count; i++)
                result.Add((cbPath[STARTING_POINT].Route[i]));

            return result.RemoveDuplication();
        }

    }

    public static class ListExtensions
    {
        public static IList<int> RemoveDuplication(this IList<int> list)
        {
            var resultList = new List<int>();
            resultList.Add(list[0]);

            for (int i = 1; i < list.Count; i++)
            {
                if (resultList[resultList.Count - 1] != list[i])
                    resultList.Add(list[i]);
            }

            return resultList;
        }
    }
}
