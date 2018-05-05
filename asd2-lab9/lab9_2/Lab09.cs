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
        bool[] useless;

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

            bool[] visitedVertices = new bool[graph.VerticesCount];
            List<Edge> ret = new List<Edge>();

            if (findMatching(graph, k, visitedVertices, ret, 0))
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
            useless = new bool[graph.VerticesCount];
            matching = null;
            maxSum = 0;
            maxResult = new Edge[1];

            bool[] visitedVertices = new bool[graph.VerticesCount];
            double[] vertSums = new double[graph.VerticesCount];
            for (int i = 0; i < vertSums.Length; i++)
                vertSums[i] = -1;
            List<int> visited = new List<int>();
            List<Edge> ret = new List<Edge>();

            findMaxMatching(graph, visitedVertices, visited, ret, 0, 0, 0, vertSums, false);

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

        double findMaxMatching(Graph g, bool[] visited, List<int> visitedList, List<Edge> matching, int start, double curSum, double midSum, double[] vertSums, bool problem)
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
                    return findMaxMatching(g, visited, visitedList, matching, start + 1, curSum, midSum, vertSums, false);

                flag = g.AddEdge(e.To, start);
                if (flag)
                    g.DelEdge(e.To, start);
                else
                    return findMaxMatching(g, visited, visitedList, matching, start + 1, curSum, midSum, vertSums, false);
            }

            double max = -1;
            foreach (Edge e in g.OutEdges(start))
            {
                if (!canAdd(e, visited, visitedList, g))
                    continue;

                if (e.Weight < 0)
                    continue;

                //if (e.From >= e.To) continue;

                if (g.OutDegree(e.To) == 0 && curSum + e.Weight <= maxSum)
                    continue;
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

                if (findMaxMatching(g, visited, visitedList, matching, start + 1, curSum, 0, vertSums, false) != -1)
                    return maxSum;

                useless[start] = true;
                if (midSum > max)
                    max = midSum;
                visited[e.To] = false;
                visited[e.From] = false;
                matching.RemoveAt(matching.Count - 1);
                visitedList.RemoveAt(visitedList.Count - 1);
                visitedList.RemoveAt(visitedList.Count - 1);
                curSum -= e.Weight;
            }

            return findMaxMatching(g, visited, visitedList, matching, start + 1, curSum, 0, vertSums, true);
        }

        double findMaxMatchingTes(Graph g, bool[] visited, List<int> visitedList, List<Edge> matching, int start, double curSum, double midSum, double[] vertSums)
        {
            if (start >= visited.Length)
                return -1;
            if (visited[start])
                return -1;

            //First helper array; fill it with 'start' vertex' neighbours
            bool[] helper = (bool[])visited.Clone();
            helper[start] = true;
            foreach (Edge e in g.OutEdges(start))
                helper[e.To] = true;

            //Second helper array
            bool[] bigHelper = (bool[])helper.Clone();
            int nextStart = 0;

            double max = -1;
            foreach (Edge e in g.OutEdges(start))
            {
                if (visited[e.To])
                    continue;

                if (e.Weight < 0)
                    continue;

                //Fill second helper array with e.To's neighbours
                bigHelper = (bool[])helper.Clone();
                bigHelper[e.To] = true;
                bigHelper[e.From] = true;

                foreach (Edge et in g.OutEdges(e.To))
                    bigHelper[et.To] = true;
                matching.Add(e);

                curSum += e.Weight;
                if (curSum > maxSum)
                {
                    maxSum = curSum;
                    maxResult = new Edge[matching.Count];
                    matching.CopyTo(maxResult, 0);
                }

                //Search for next viable vertex
                for (nextStart = start; nextStart < bigHelper.Length; nextStart++)
                    if (!bigHelper[nextStart])
                        break;

                if (findMaxMatchingTes(g, bigHelper, visitedList, matching, nextStart, curSum, 0, vertSums) != -1)
                    return maxSum;

                //visited[e.To] = false;
                //visited[e.From] = false;
                matching.RemoveAt(matching.Count - 1);
                curSum -= e.Weight;
            }
            return findMaxMatchingTes(g, visited, visitedList, matching, start + 1, curSum, 0, vertSums);
        }
        bool findMatching(Graph g, int k, bool[] visited, List<Edge> matching, int start)
        {
            if (matching.Count == k)
                return true;
            if (start >= visited.Length)
                return false;
            if (visited[start])
                return false;
            if (matching.Count + (g.VerticesCount - start) < k)
                return false;

            //First helper array; fill it with 'start' vertex' neighbours
            bool[] helper = (bool[]) visited.Clone();
            helper[start] = true;
            foreach(Edge e in g.OutEdges(start))
                helper[e.To] = true;

            //Second helper array
            bool[] bigHelper = (bool[])helper.Clone();
            int nextStart = 0;

            foreach (Edge e in g.OutEdges(start))
            {
                if (visited[e.To])
                    continue;

                //Fill second helper array with e.To's neighbours
                bigHelper = (bool[])helper.Clone();
                bigHelper[e.To] = true;
                bigHelper[e.From] = true;

                foreach (Edge et in g.OutEdges(e.To))
                    bigHelper[et.To] = true;
                matching.Add(e);
                //Search for next viable vertex
                for (nextStart = 0; nextStart < visited.Length; nextStart++)
                    if (!bigHelper[nextStart])
                        break;
                if (findMatching(g, k, bigHelper, matching, nextStart))
                    return true;

                visited[e.To] = false;
                visited[e.From] = false;
                matching.RemoveAt(matching.Count - 1);
            }
            return findMatching(g, k, visited, matching, start + 1);
        }
    }
}



