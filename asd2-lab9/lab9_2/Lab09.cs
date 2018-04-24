using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{

    // DEFINICJA
    // Skojarzeniem indukowanym grafu G nazywamy takie skojarzenie M,
    // ze żadne dwa konce roznych krawedzi z M nie sa polaczone krawedzia w G

    // Uwagi do obu metod
    // 1) Grafow bedacych parametrami nie wolno zmieniac
    // 2) Parametrami są zawsze grafy nieskierowane (nie trzeba tego sprawdzac)

    public class Lab09 : MarshalByRefObject
    {

        /// <summary>
        /// Funkcja znajduje dowolne skojarzenie indukowane o rozmiarze k w grafie graph
        /// </summary>
        /// <param name="graph">Badany graf nieskierowany</param>
        /// <param name="k">Rozmiar szukanego skojarzenia indukowanego</param>
        /// <param name="matching">Znalezione skojarzenie (lub null jeśli nie ma)</param>
        /// <returns>true jeśli znaleziono skojarzenie, false jesli nie znaleziono</returns>
        /// <remarks>
        /// Punktacja:  2 pkt, w tym
        ///     1.5  -  dzialajacy algorytm (testy podstawowe)
        ///     0.5  -  testy wydajnościowe
        /// </remarks>
        public bool InducedMatching(Graph graph, int k, out Edge[] matching)
        {
            //sanity check
            matching = null;
            if (k > graph.EdgesCount)
                return false;

            int cc;
            List<Edge> allEdges = new List<Edge>();
            List<Edge> ret = new List<Edge>();
            Graph temp = graph.Clone();

            Predicate<Edge> ve = delegate (Edge e)
            {
                if(e.From < e.To)
                    allEdges.Add(e);
                return true;
            };
            graph.GeneralSearchAll<EdgesStack>(null, null, ve, out cc);

            List<int> visitedVertices = new List<int>();
            if (findMatching(temp, k, allEdges, visitedVertices, ret, 0))
            {
                matching = ret.ToArray();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Funkcja znajduje skojarzenie indukowane o maksymalnej sumie wag krawedzi w grafie graph
        /// </summary>
        /// <param name="graph">Badany graf nieskierowany</param>
        /// <param name="matching">Znalezione skojarzenie (jeśli puste to tablica 0-elementowa)</param>
        /// <returns>Waga skojarzenia</returns>
        /// <remarks>
        /// Punktacja:  2 pkt, w tym
        ///     1.5  -  dzialajacy algorytm (testy podstawowe)
        ///     0.5  -  testy wydajnościowe
        /// </remarks>
        public double MaximalInducedMatching(Graph graph, out Edge[] matching)
        {
            //sanity check
            matching = null;

            int cc;
            List<Edge> allEdges = new List<Edge>();
            List<Edge> ret = new List<Edge>();
            Graph temp = graph.Clone();

            Predicate<Edge> ve = delegate (Edge e)
            {
                if (e.From < e.To)
                    allEdges.Add(e);
                return true;
            };
            graph.GeneralSearchAll<EdgesStack>(null, null, ve, out cc);

            List<int> visitedVertices = new List<int>();

            double max = 0;
            for(int i = graph.EdgesCount; i >= 1; i--)
            {
                double cur = 0;
                findMatchingMax(graph, i, allEdges, visitedVertices, ret, 0, 0, out cur);
                if (cur > max)
                {
                    max = cur;
                    matching = ret.ToArray();

                }
                ret = new List<Edge>();
                visitedVertices = new List<int>();
            }
            return max;
        }

        //funkcje pomocnicze
        bool canAdd(Edge e, List<int> visited, Graph g)
        {
            Graph temp = g.Clone();
            if (visited.Contains(e.From) || visited.Contains(e.To))
                return false;

            for(int i = 0; i < visited.Count; i++)
            { 
                int from = e.From;
                int cur = visited[i];

                if (temp.AddEdge(e.From, visited[i]) == false)
                    return false;

                if (temp.AddEdge(e.To, visited[i]) == false)
                    return false;
            }
            return true;
        }

        bool findMatchingMax(Graph g, int k, List<Edge> list, List<int> visited, List<Edge> matching, int start, double sum, out double max)
        {
            if (matching.Count == k)
            {
                max = sum;
                return true;
            }
            if (start >= list.Count)
            {
                max = 0;
                return false;
            }
                
            if (matching.Count > k)
            {
                max = 0;
                return false;
            }

            int newStart = -1;
            double newSum = -1;
            for (int j = start; j < list.Count; j++)
            {
                if (!canAdd(list[j], visited, g))
                    continue;

                matching.Add(list[j]);
                visited.Add(list[j].To);
                visited.Add(list[j].From);
                newSum = sum + list[j].Weight;
                newStart = j + 1;
                break;
            }
            if (newStart == -1)
            {
                max = 0;
                return false;
            }

            if (findMatchingMax(g, k, list, visited, matching, newStart, newSum, out max))
            {
                max = sum;
                return true;
            }

            max = 0;
            return false;
        }

        bool findMatching(Graph g, int k, List<Edge> list, List<int> visited, List<Edge> matching, int start)
        {
            if (matching.Count == k)
                return true;
            if (start >= list.Count)
                return false;
            if (matching.Count > k)
                return false;

            int newStart = -1;
            for(int j = start; j < list.Count; j++)
            {
                if (!canAdd(list[j], visited, g))
                    continue;

                matching.Add(list[j]);
                visited.Add(list[j].To);
                visited.Add(list[j].From);
                newStart = j + 1;
                break;
            }
            if (newStart == -1)
                return false;

            if(findMatching(g, k, list, visited, matching, newStart))
                return true;
            return false;
        }
    }
}



