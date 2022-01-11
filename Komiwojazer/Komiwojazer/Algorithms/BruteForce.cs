using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Komiwojazer.Algorithms
{
    public class BruteForce : BaseAlgorithm
    {
        public List<IList<int>> _drogiBF = new List<IList<int>>();
        public List<IList<int>> _drogiBF2 = new List<IList<int>>();

        public BruteForce(IList<IList<int>> g) : base(g)
        {
        }

        public List<IList<int>> GetPathBF2()
        {
            int[] tab = new int[N - 27];
            int i = 0;
            for (int j = 27; j < N; i++, j++)
            {
                tab[i] = j;
            }
            DoPermute(tab, 0, i - 1, _drogiBF);
            foreach (var droga in _drogiBF)
            {
                droga.Insert(0, 26);
                droga.Add(26);
            }
            var road = new List<int>();
            foreach (var droga in _drogiBF)
            {
                road.Clear();
                for (int k = 1; k < droga.Count(); k++)
                {
                    var paths = new Dijkstra(G).dijkstra(droga[k - 1]);
                    for (int j = 0; j < paths[droga[k]].Route.Count; j++)
                    {
                        road.Add(paths[droga[k]].Route[j]);
                    }
                }
                _drogiBF2.Add(road.RemoveDuplication().ToList());
            }




            return _drogiBF2;
        }
        static void DoPermute(int[] nums, int start, int end, IList<IList<int>> list)
        {
            if (start == end)
            {
                list.Add(new List<int>(nums));
            }
            else
            {
                for (var i = start; i <= end; i++)
                {
                    Swap(ref nums[start], ref nums[i]);
                    DoPermute(nums, start + 1, end, list);
                    Swap(ref nums[start], ref nums[i]);
                }
            }
        }

        static void Swap(ref int a, ref int b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        /*
public IList<int> GetPathBF()
{
    var result = new List<int>();
    var result2 = new List<int>();
    int startPoint = 26;
    var paths = dijkstra(startPoint);
    for (int i = 27; i < N; i++)
    {

        _usedPoints.Clear();
        result2.Clear();
        startPoint = 26;
        paths = dijkstra(startPoint);

        for (int j = 0; j < paths[i].Route.Count(); j++)
        {
            result2.Add(paths[i].Route[j]);
        }

        DFS(i, result2);

    }
    var lastVisitedNode = result[result.Count - 1];
    var cbPath = dijkstra(lastVisitedNode);
    for (int i = 0; i < cbPath[26].Route.Count; i++)
        result.Add((cbPath[26].Route[i]));

    return result;
}

public void DFS(int startPoint, List<int> result2)
{
    _usedPoints.Add(startPoint);
    var paths = dijkstra(startPoint);
    for (int i = 27; i < N; i++)
    {
        if (!_usedPoints.Contains(i))
        {
            for (int j = 1; j < paths[i].Route.Count(); j++)
            {
                result2.Add(paths[i].Route[j]);
            }
            if (!AllPointsUsed())
            {
                DFS(i, result2);
            }
        }
    }
}
*/
    }
}
