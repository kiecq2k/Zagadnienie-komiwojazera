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

        public BruteForce(Version version, IList<IList<int>> g) : base(version, g)
        {
        }

        public List<IList<int>> GetPathBF2()
        {
            int tmp = STARTING_POINT + 1;
            int[] tab = new int[N - tmp];
            int i = 0;
            for (int j = tmp; j < N; i++, j++) //wypelnienie tablicy indeskami wierzchołków bez indeksu wierzchołka początkowego
            {
                tab[i] = j;
            }
            DoPermute(tab, 0, i - 1, _drogiBF); //utworzenie wszystkich permutacji
            foreach (var droga in _drogiBF) //dodanie na początku i na końcu każdej permutacji indeksu wierzchołka początkowego
            {
                droga.Insert(0, STARTING_POINT);
                droga.Add(STARTING_POINT);
            }
            var road = new List<int>();
            foreach (var droga in _drogiBF) //dodanie między wierzchołkami małego grafu indeksów wierzchołków pośrednich
            {
                road.Clear();
                for (int k = 1; k < droga.Count(); k++)
                {
                    var paths = new Dijkstra(Version, G).dijkstra(droga[k - 1]);
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
    }
}
