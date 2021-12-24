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

    public class DijkstraAlgorithm
    {
        private readonly int N;
        private readonly IList<IList<int>> G;
        private IList<int> _usedPoints;

        public DijkstraAlgorithm(IList<IList<int>> g)
        {
            N = g.Count;
            G = g;
            _usedPoints = new List<int>();
        }

        public Result[] dijkstra(int startnode)
        {
            var cost = new int[N, N];
            var distance = new int[N];
            var pred = new int[N];
            var visited = new int[N];
            int count, mindistance, nextnode =0, i, j;
            for (i = 0; i < N; i++)
                for (j = 0; j < N; j++)
                    if (G[i][j] == 0)
                        cost[i, j] = 9999;
                    else
                        cost[i, j] = G[i][j];
            for (i = 0; i < N; i++)
            {
                distance[i] = cost[startnode, i];
                pred[i] = startnode;
                visited[i] = 0;
            }
            distance[startnode] = 0;
            visited[startnode] = 1;
            count = 1;
            while (count < N - 1)
            {
                mindistance = 9999;
                for (i = 0; i < N; i++)
                    if (distance[i] < mindistance && visited[i] == 0)
                    {
                        mindistance = distance[i];
                        nextnode = i;
                    }
                visited[nextnode] = 1;
                for (i = 0; i < N; i++)
                    if (visited[i] == 0)
                        if (mindistance + cost[nextnode, i] < distance[i])
                        {
                            distance[i] = mindistance + cost[nextnode, i];
                            pred[i] = nextnode;
                        }
                count++;
            }

            var result = new Result[N];
            result[startnode] = new Result();

            for (i = 0; i < N; i++)
                if (i != startnode)
                {
                    result[i] = new Result();
                    result[i].Distance = distance[i];
                    result[i].Route.Add(i);
                    j = i;
                    do
                    {
                        j = pred[j];
                        result[i].Route.Add(j);
                    } while (j != startnode);
                }

            return result;
        }

        public IList<int> GetPath()
        {
            var result = new List<int>();
            int startPoint = 26;

            while (!AllPointsUsed())
            {
                var paths = dijkstra(startPoint);

                int minDistance = int.MaxValue;
                for(int i = 27; i < N; i++)
                {
                    if(paths[i].Distance < minDistance && !_usedPoints.Contains(i))
                    {
                        minDistance = paths[i].Distance;
                        startPoint = i;
                    }
                }

                _usedPoints.Add(startPoint);
                for(int i = paths[startPoint].Route.Count - 1; i >= 0; i--)
                    result.Add(paths[startPoint].Route[i]);
            }

            // comeback path
            var lastVisitedNode = result[result.Count - 1];
            var cbPath = dijkstra(lastVisitedNode);
            for (int i = cbPath[26].Route.Count - 1; i >= 0; i--)
                result.Add((cbPath[26].Route[i]));

            return result.RemoveDuplication();
        }

        public bool AllPointsUsed()
        {
            for(int i = 27; i < N; i++)
            {
                if (!_usedPoints.Contains(i))
                    return false;
            }

            return true;
        }


    }

    public static class ListExtensions
    {
        public static IList<int> RemoveDuplication(this IList<int> list)
        {
            var resultList = new List<int>();
            resultList.Add(list[0]);

            for(int i = 1; i < list.Count; i++)
            {
                if (resultList[resultList.Count - 1] != list[i])
                    resultList.Add(list[i]);
            }

            return resultList;
        }
    }
}
