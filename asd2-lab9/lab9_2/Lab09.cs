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
        double maxSum = -1; //przechowuje najmniejsza aktualnie znaleziona roznice (etap 2)
        Edge[] maxResult; //przechowuje najmniejsze aktualnie znalezione ustawienie (etap 2)

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

            double density = (double)(2 * graph.EdgesCount) / (double)(graph.VerticesCount * (graph.VerticesCount - 1));
            bool[] visitedVertices = new bool[graph.VerticesCount];
            List<int> visited = new List<int>();
            List<Edge> ret = new List<Edge>();
            int[] verticesDegSorted = new int[graph.VerticesCount];

            //MNIEJ NIZ DRZEWOOOOOO, MNIEJ NIZ DRZEWOOOOOOO
            //if (density < 0.1)
            //{
            //    int cc;
            //    List<Edge> allEdges = new List<Edge>();
            //    Predicate<Edge> ve = delegate (Edge e)
            //    {
            //        if (e.From < e.To)
            //            allEdges.Add(e);
            //        return true;
            //    };
            //    graph.GeneralSearchAll<EdgesStack>(null, null, ve, out cc);

            //    if (findMatchingEdges(graph, k, allEdges, visitedVertices, visited, ret, 0))
            //    {
            //        matching = ret.ToArray();
            //        return true;
            //    }
            //    return false;
            //}

            if (findMatching(graph, k, visitedVertices, visited, ret, 0))
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
            maxSum = 0;
            maxResult = new Edge[1];

            bool[] visitedVertices = new bool[graph.VerticesCount];
            double[] vertSums = new double[graph.VerticesCount];
            for (int i = 0; i < vertSums.Length; i++)
                vertSums[i] = -1;
            List<int> visited = new List<int>();
            List<Edge> ret = new List<Edge>();
            findMaxMatching(graph, visitedVertices, visited, ret, 0, 0, 0, vertSums);

            matching = maxResult;
            if (matching[0].From == matching[0].To)
                matching = new Edge[0];
            
            return maxSum;
        }

        //funkcje pomocnicze
        bool canAdd(Edge e, bool[] visited, List<int> visitedList, Graph g)
        {
            if (visited[e.From] || visited[e.To])
                return false;

            for(int i = 0; i < visitedList.Count; i++)
            {
                if (!visited[visitedList[i]])
                    continue;

                if (!g.AddEdge(e.From, visitedList[i]))
                    return false;
                g.DelEdge(e.From, visitedList[i]);

                if (!g.AddEdge(e.To, visitedList[i]))
                    return false;
                g.DelEdge(e.To, visitedList[i]);
            }
            return true;
        }

        bool canAddNoList(Edge e, bool[] visited, Graph g)
        {
            if (visited[e.From] || visited[e.To])
                return false;

            for (int i = 0; i < visited.Length; i++)
            {
                if (!visited[i])
                    continue;

                if (!g.AddEdge(e.From, i))
                    return false;
                g.DelEdge(e.From, i);

                if (!g.AddEdge(e.To, i))
                    return false;
                g.DelEdge(e.To, i);
            }

            return true;
        }

        double findMaxMatching(Graph g, bool[] visited, List<int> visitedList, List<Edge> matching, int start, double curSum, double midSum, double[] vertSums)
        {
            if (start >= visited.Length)
                return -1;
            if (visited[start])
                return -1;

            foreach (Edge e in matching)
            {
                bool flag = false;
                flag = g.AddEdge(e.From, start);
                if (flag)
                    g.DelEdge(e.From, start);
                else
                    return findMaxMatching(g, visited, visitedList, matching, start + 1, curSum, midSum, vertSums);

                flag = g.AddEdge(e.To, start);
                if (flag)
                    g.DelEdge(e.To, start);
                else
                    return findMaxMatching(g, visited, visitedList, matching, start + 1, curSum, midSum, vertSums);
            }

            double max = -1;
            foreach (Edge e in g.OutEdges(start))
            {
                if (!canAdd(e, visited, visitedList, g))
                    continue;

                //if (vertSums[e.To] != -1 && curSum + vertSums[e.To] < maxSum)
                //    continue;
                visited[e.To] = true;
                visited[e.From] = true;
                matching.Add(e);
                visitedList.Add(e.To);
                visitedList.Add(e.From);
                curSum += e.Weight;
                midSum += e.Weight;
                if(curSum > maxSum)
                {
                    maxSum = curSum;
                    maxResult = new Edge[matching.Count];
                    matching.CopyTo(maxResult, 0);
                }

                if (findMaxMatching(g, visited, visitedList, matching, start + 1, curSum, 0, vertSums) != -1)
                    return maxSum;

                if (midSum > max)
                    max = midSum;
                visited[e.To] = false;
                visited[e.From] = false;
                matching.RemoveAt(matching.Count - 1);
                visitedList.RemoveAt(visitedList.Count - 1);
                visitedList.RemoveAt(visitedList.Count - 1);
                curSum -= e.Weight;
                midSum -= e.Weight;
            }
            if(max!=-1)
                vertSums[start] = max;
            return findMaxMatching(g, visited, visitedList, matching, start + 1, curSum, midSum, vertSums);
        }

        bool findMatching(Graph g, int k, bool[] visited, List<int> visitedList, List<Edge> matching, int start)
        {
            if (matching.Count == k)
                return true;
            if (start >= visited.Length)
                return false;
            if (visited[start])
                return false;
            if (matching.Count > k)
                return false;
            if (matching.Count + (g.VerticesCount - start) < k)
                return false;
            foreach (Edge e in matching)
            {
                bool flag = false;
                flag = g.AddEdge(e.From, start);
                if (flag)
                    g.DelEdge(e.From, start);
                else
                    return findMatching(g, k, visited, visitedList, matching, start + 1);

                flag = g.AddEdge(e.To, start);
                if (flag)
                    g.DelEdge(e.To, start);
                else
                    return findMatching(g, k, visited, visitedList, matching, start + 1);
            }

            foreach (Edge e in g.OutEdges(start))
            {
                if (!canAdd(e, visited, visitedList, g))
                    continue;

                visited[e.To] = true;
                visited[e.From] = true;
                matching.Add(e);
                visitedList.Add(e.To);
                visitedList.Add(e.From);
                if (findMatching(g, k, visited, visitedList, matching, e.To + 1))
                    return true;

                visited[e.To] = false;
                visited[e.From] = false;
                matching.RemoveAt(matching.Count - 1);
                visitedList.RemoveAt(visitedList.Count - 1);
                visitedList.RemoveAt(visitedList.Count - 1);
            }
            return findMatching(g, k, visited, visitedList, matching, start + 2);
        }

        bool findMatchingEdges(Graph g, int k, List<Edge> list, bool[] visited, List<int> visitedList, List<Edge> matching, int start)
        {
            if (matching.Count == k)
                return true;
            if (start >= list.Count)
                return false;
            if (matching.Count > k)
                return false;

            if (start >= visited.Length)
                return false;
            if (visited[list[start].From])
                return false;
            if (visited[list[start].To])
                return false;
            if (matching.Count > k)
                return false;
            if (matching.Count + (g.EdgesCount - start) < k)
                return false;
            foreach (Edge e in matching)
            {
                bool flag = false;
                flag = g.AddEdge(e.From, list[start].From);
                if (flag)
                    g.DelEdge(e.From, list[start].From);
                else
                    return findMatching(g, k, visited, visitedList, matching, start + 1);

                flag = g.AddEdge(e.To, list[start].From);
                if (flag)
                    g.DelEdge(e.To, list[start].From);
                else
                    return findMatching(g, k, visited, visitedList, matching, start + 1);

                flag = g.AddEdge(e.From, list[start].To);
                if (flag)
                    g.DelEdge(e.From, list[start].To);
                else
                    return findMatching(g, k, visited, visitedList, matching, start + 1);

                flag = g.AddEdge(e.To, list[start].To);
                if (flag)
                    g.DelEdge(e.To, list[start].To);
                else
                    return findMatching(g, k, visited, visitedList, matching, start + 1);
            }

            for (int j = start; j < list.Count; j++)
            {
                if (!canAdd(list[j], visited, visitedList, g))
                    continue;

                matching.Add(list[j]);
                visited[list[j].To] = true;
                visited[list[j].From] = true;
                visitedList.Add(list[j].To);
                visitedList.Add(list[j].From);

                if (findMatchingEdges(g, k, list, visited, visitedList, matching, start + 1))
                    return true;
                matching.RemoveAt(matching.Count - 1);
                visited[list[j].To] = false;
                visited[list[j].From] = false;
                visitedList.RemoveAt(visitedList.Count - 1);
                visitedList.RemoveAt(visitedList.Count - 1);
            }

            return findMatchingEdges(g, k, list, visited, visitedList, matching, start + 1);
        }
    }
}



