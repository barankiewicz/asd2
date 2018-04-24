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
        public int initialStart = 0;

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

            Predicate<Edge> ve = delegate (Edge e)
            {
                if(e.From < e.To)
                    allEdges.Add(e);
                return true;
            };
            graph.GeneralSearchAll<EdgesStack>(null, null, ve, out cc);

            bool[] visitedVertices = new bool[graph.VerticesCount];
            if (findMatching(graph, k, allEdges, visitedVertices, ret, 0))
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
            matching = null;
            return 0;
        }

        //funkcje pomocnicze
        bool canAdd(Edge e, bool[] visited, Graph g)
        {
            if (visited[e.From] || visited[e.To])
                return false;

            for(int i = 0; i < visited.Length; i++)
            {
                if (!visited[i])
                    continue;

                if (g.AddEdge(e.From, i) == false)
                {
                    return false;
                }
                g.DelEdge(e.From, i);

                if (g.AddEdge(e.To, i) == false)
                {
                    return false;
                }
                g.DelEdge(e.To, i);
            }

            return true;
        }

        bool findMatching(Graph g, int k, List<Edge> list, bool[] visited, List<Edge> matching, int start)
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
                visited[list[j].To] = true;
                visited[list[j].From] = true;
                newStart = j + 1;
                break;
            }
            if (newStart == -1)
            {
                matching = new List<Edge>();
                visited = new bool[g.VerticesCount];
                initialStart++;

                if (findMatching(g, k, list, visited, matching, initialStart))
                    return true;
                return false;
            }
                //return false;

            if(findMatching(g, k, list, visited, matching, newStart))
                return true;
            return false;
        }
    }
}



