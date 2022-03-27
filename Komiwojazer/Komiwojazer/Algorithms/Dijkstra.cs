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
        private static readonly int BRAK_RODZICA = -1;

        public Dijkstra(Version version, IList<IList<int>> g) : base(version, g)
        {
        }

        public Result[] dijkstra(int wierzchPoczatkowy)
        {
            // przypisanie do zmiennej ilosci wierzcholkow
            int iloscWierzcholkow = G.Count; 

            // inicjalizacja tablicy, ktora bedzie zawierala najkrotsza dlugosc trasy od wierzcholka 
            // startowego do wierzcholka o indeksie i
            int[] najkrotszeTrasy = new int[iloscWierzcholkow];

            // inicjalizacja tablicy wartosci logicznych
            // ktora na indeksie i bedzie zawierala wartosc true
            // gdy juz mamy wierzcholek i zapisany w tablicy najkrotszeTrasy
            bool[] dodane = new bool[iloscWierzcholkow];

            // inicjalizacja dlugosci tras na ustalona wartosc
            // ustawienie false w tablicy dodane
            for (int indexWierzcholka = 0; indexWierzcholka < iloscWierzcholkow;
                                                indexWierzcholka++)
            {
                najkrotszeTrasy[indexWierzcholka] = int.MaxValue - 1;
                dodane[indexWierzcholka] = false;
            }

            // ustawienie odleglosci od wierzcholka startowego 
            // do siebie samego, czyli wartosc 0
            najkrotszeTrasy[wierzchPoczatkowy] = 0;

            // inicjalizacja tablicy, ktora bedzie przechowywala
            // drzewo najkrotszych tras
            int[] rodzice = new int[iloscWierzcholkow];

            // wierzcholek poczatkowy nie ma rodzica
            // wiec ustawiamy wartosc -1
            rodzice[wierzchPoczatkowy] = BRAK_RODZICA;

            // algorytm znajdowania najkrotszej trasy
            // dla wszystkich wierzcholkow
            for (int i = 1; i < iloscWierzcholkow; i++)
            {

                // Pick the minimum distance vertex
                // from the set of vertices not yet
                // processed. nearestVertex is
                // always equal to startNode in
                // first iteration.

                // wybranie wierzcholka o najmniejszej odleglosci
                // ze zbioru wierzcholkow jeszcze nie przeprocesowanych
                int najblizszyWierzcholek = -1;
                int najmniejszaOdleglosc = int.MaxValue - 1;
                for (int indexWierzcholka = 0;
                        indexWierzcholka < iloscWierzcholkow;
                        indexWierzcholka++)
                {
                    if (!dodane[indexWierzcholka] &&
                        najkrotszeTrasy[indexWierzcholka] <
                        najmniejszaOdleglosc)
                    {
                        najblizszyWierzcholek = indexWierzcholka;
                        najmniejszaOdleglosc = najkrotszeTrasy[indexWierzcholka];
                    }
                }

                // ustawienie wybranego wierzcholka jako przeprocesowanego
                // czyli ustawienie flagi na true
                dodane[najblizszyWierzcholek] = true;

                // aktualizacja odleglosci w tablicy
                // dla wybranego wierzcholka
                for (int vertexIndex = 0;
                        vertexIndex < iloscWierzcholkow;
                        vertexIndex++)
                {
                    int edgeDistance = G[najblizszyWierzcholek][vertexIndex];

                    if (edgeDistance > 0
                        && ((najmniejszaOdleglosc + edgeDistance) <
                            najkrotszeTrasy[vertexIndex]))
                    {
                        rodzice[vertexIndex] = najblizszyWierzcholek;
                        najkrotszeTrasy[vertexIndex] = najmniejszaOdleglosc +
                                                        edgeDistance;
                    }
                }
            }

            // pomocnicza metoda do zwrocenia wartosci w czytelnej formie
            // czyli obiektu Result
            return GetResult(wierzchPoczatkowy, najkrotszeTrasy, rodzice);

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
            if (currentVertex == BRAK_RODZICA)
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

            while (!AllPointsUsed()) //tworzenie marszruty
            {
                var paths = dijkstra(startPoint);

                int minDistance = int.MaxValue;

                for (int i = STARTING_POINT + 1; i < N; i++) //szukanie najbliższego wierzchołka
                {
                    if (paths[i].Distance < minDistance && !_usedPoints.Contains(i)) //porównanie wag krawędzi
                    {
                        minDistance = paths[i].Distance;
                        startPoint = i;
                    }
                }

                _usedPoints.Add(startPoint); //dodanie wierzchołka do zbioru wierzchołków wykorzystanych

                for (int i = 0; i < paths[startPoint].Route.Count; i++) //dodanie krawedzi do rozwiązania
                    result.Add(paths[startPoint].Route[i]);
            }

            //dodanie drogi z ostatniego wierzchołka do wierzchołka początkowego
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
