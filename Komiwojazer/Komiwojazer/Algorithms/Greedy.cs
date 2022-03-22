using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Komiwojazer.Algorithms
{
    public class Path
    {
        public int From { get; set; }
        public int To { get; set; }
        public int Distance { get; set; }
    }

    public class Greedy : BaseAlgorithm
    {
        private readonly List<Path> _paths;

        public Greedy(Version version, IList<IList<int>> g) : base(version, g)
        {
            _paths = new List<Path>();
        }

        public IList<int> NajmniejszaKrawedz()
        {
            for(int i= STARTING_POINT; i < N; i++)
            {
                var dijkstra = new Dijkstra(Version, G);
                var distances = dijkstra.dijkstra(i);
                for(int j=STARTING_POINT; j < distances.Length; j++) //zapisanie wszytskich wag krawędzi małego grafu w liście
                {
                    if(distances[j].Distance != 0)
                    {
                        _paths.Add(new Path()
                        {
                            From = i,
                            To = j,
                            Distance = distances[j].Distance
                        });
                    }
                }
            }
            var sortedPaths = _paths.OrderBy(p => p.Distance).ToList(); //sortowanie listy z wagami krawędzi w kolejności rosnącej
            int n = N - STARTING_POINT;
            var dict = new Dictionary<int, int>(); //stworzenie miejsca do dodawania krawędzi do rozwiązania
            dict.Add(sortedPaths[0].From, sortedPaths[0].To); //dodanie pierwszej krawędzi z listy do rozwiązania

            for (int i = 1; i < sortedPaths.Count; i++) //dodawanie krawędzi do rozwiązania
            {
                if (!dict.ContainsKey(sortedPaths[i].From) &&
                    !dict.ContainsValue(sortedPaths[i].To)) //sprawdzamy czy w rozwiązaniu nie ma krawędzi z tym samym wierzołkiem na początku lub końcu krawędzi
                {
                    int value;
                    if(n > 2)
                    {
                        if ((!dict.TryGetValue(sortedPaths[i].To, out value) ||
                            value != sortedPaths[i].From) && KrawedzOK(dict, sortedPaths, i)) //sprawdzenie czy możemy dołączyć krawędź do rozwiązania
                        {
                            dict.Add(sortedPaths[i].From, sortedPaths[i].To);
                        }
                            
                    }
                    else
                    {
                        dict.Add(sortedPaths[i].From, sortedPaths[i].To); //dodanie krawędzi do rozwiązania
                    }
                    
                }   
            }
            var list = new List<int>();
            int index = STARTING_POINT;
            while(dict.Count > 0) //edytujemy listę aby krawedzie laczyly się ze sobą po kolei
            {
                list.Add(index);
                var to = dict[index];
                dict.Remove(index);
                index = to;
            }
            var result = new List<int>();
            for(int i = 0; i < list.Count - 1; i++) //dodanie do rozwiązania punktów pośrednich
            {
                var dijkstra1 = new Dijkstra(Version, G).dijkstra(list[i]);
                
                result.AddRange(dijkstra1[list[i + 1]].Route);
            }
            RemoveDuplication(ref result);
            var dijkstra2 = new Dijkstra(Version, G).dijkstra(result[result.Count - 1]);
            result.AddRange(dijkstra2[STARTING_POINT].Route);
            return result.RemoveDuplication();
        }

        private bool KrawedzOK(Dictionary<int,int> dict, List<Path> sortedPaths, int i)
        {
            var dictCopy = new Dictionary<int,int>(dict);
            dictCopy.Add(sortedPaths[i].From, sortedPaths[i].To);

            for (int j = i+1; j < sortedPaths.Count; j++)
            {
                if (!dictCopy.ContainsKey(sortedPaths[j].From) &&
                    !dictCopy.ContainsValue(sortedPaths[j].To))
                {
                    int value;
                    if (!dictCopy.TryGetValue(sortedPaths[j].To, out value) ||
                       value != sortedPaths[j].From)
                        dictCopy.Add(sortedPaths[j].From, sortedPaths[j].To);
                }
            }

            var list = new List<int>();
            int index = STARTING_POINT;
            int dictCopySize = dictCopy.Count;

            try
            {
                while (dictCopy.Count > 0)
                {
                    list.Add(index);
                    var to = dictCopy[index];
                    dictCopy.Remove(index);
                    index = to;
                }
            }
            catch
            {
                return false;
            }
            

            int n = N - STARTING_POINT;
            return (dictCopySize == n);
        }

        private void RemoveDuplication(ref List<int> result)
        {
            int n = N - STARTING_POINT;
            var temp = new List<int>();
            int index = result.Count - 1;

            for(int i = 0;i < result.Count; i++)
            {
                if(result[i] >= STARTING_POINT && !temp.Contains(result[i]))
                    temp.Add(result[i]);

                if (temp.Count == n)
                {
                    index = i;
                    break;
                }
                    
            }

            result = result.Take(index + 1).ToList();
        }
        
    }
}
