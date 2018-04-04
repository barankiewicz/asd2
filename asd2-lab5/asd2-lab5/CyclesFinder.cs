using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASD
{
    public class CyclesFinder : MarshalByRefObject
    {
        /// <summary>
        /// Sprawdza czy graf jest drzewem
        /// </summary>
        /// <param name="g">Graf</param>
        /// <returns>true jeśli graf jest drzewem</returns>
        public bool IsTree(Graph g)
        {
            if (g.Directed)
                throw new ArgumentException();

            int skl = 0;
            bool czy = g.GeneralSearchAll<EdgesStack>(null, null, null, out skl);
            return skl == 1 && g.VerticesCount == g.EdgesCount + 1;
        }

        /// <summary>
        /// Wyznacza cykle fundamentalne grafu g względem drzewa t.
        /// Każdy cykl fundamentalny zawiera dokadnie jedną krawędź spoza t.
        /// </summary>
        /// <param name="g">Graf</param>
        /// <param name="t">Drzewo rozpinające grafu g</param>
        /// <returns>Tablica cykli fundamentalnych</returns>
        /// <remarks>W przypadku braku cykli zwracać pustą (0-elementową) tablicę, a nie null</remarks>
        public Edge[][] FindFundamentalCycles(Graph g, Graph t)
        {
            if (g.Directed)
                throw new ArgumentException();

            if (!IsTree(t))
                throw new ArgumentException();

            List<Edge> treeEdges = new List<Edge>(); //lista krawedzi drzewa (uzyta, poniewaz z jakiegos powodu nie dziala ContainsEdge
            Predicate<Edge> treeEdgesDelegate = delegate (Edge e)
            {
                treeEdges.Add(e);
                return true;
            };
            t.GeneralSearchFrom<EdgesStack>(0, null, null, treeEdgesDelegate); //Add all edges of the tree to treeEdges list

            foreach(Edge e in treeEdges)
                if (g.AddEdge(e))
                    throw new ArgumentException();
            //Koniec sprawdzania czy drzewo jest poprawne

            List<Edge[]> main = new List<Edge[]>(); //glowna lista ktora zostanie zwrocona na koncu
            List<Edge> visited = new List<Edge>(); //odwiedzone krawedzie
            //List<Edge[]> toRemove = new List<Edge[]>(); //listy, ktore nalezy usunac na koncu (ktore mialy potencjalnie cykl, ale nie udalo sie go znalezc)

            Predicate<Edge> ve = delegate (Edge e)
            {
                if (visited.Contains(e) || treeEdges.Contains(e))
                    return true;

                Graph temp = t.Clone();
                temp.AddEdge(e); //Make a graph containing a tree and that single additional edge
                bool hasCycle = false;
                
                bool[] visitedVerticesFrom = new bool[temp.VerticesCount];
                List<Edge> cycle = new List<Edge>();
                List<Edge> properCycle = new List<Edge>();

                List<Edge> toRemove = new List<Edge>();
                Predicate<Edge> findCycle = delegate (Edge ed)
                {
                    if (cycle.Contains(ed) || cycle.Contains(new Edge(ed.To, ed.From)))
                        return true;
                    int edg = temp.EdgesCount;
                    if (visitedVerticesFrom[ed.To])
                    {
                        
                        cycle.Add(ed);
                        hasCycle = true;
                        int iter = cycle.Count - 1;
                        Edge cur = cycle[iter];
                        Edge checkAgainst = cycle[iter];
                        Edge prev = cycle[iter - 1];
                        
                        bool del = false;
                        while (iter >= 0)
                        {
                            if (del)
                            {
                                toRemove.Add(prev);
                                iter--;
                                if (iter >= 0)
                                    prev = cycle[iter];
                                continue;
                            }
                            if (iter == 0)
                                break;

                            if (prev.From == ed.To)
                            {
                                del = true;
                                iter--;
                                prev = cycle[iter];
                                continue;
                            }

                            if (cur.From == prev.To)
                            {
                                cur = cycle[iter];
                                iter--;
                                prev = cycle[iter];
                                continue;
                            }
                            else
                            {
                                toRemove.Add(prev);
                                iter--;
                                prev = cycle[iter];
                            }
                        }

                        foreach (Edge et in toRemove)
                            cycle.Remove(et);

                        return false;
                    }
                    //properCycle.Reverse();
                    cycle.Add(ed);
                    visitedVerticesFrom[e.From] = true;
                    return true;
                };
                temp.GeneralSearchFrom<EdgesStack>(e.From, null, null, findCycle);
                if (hasCycle)
                {
                    main.Add(cycle.ToArray());
                }
                visited.Add(e);
                visited.Add(new Edge(e.To, e.From));
                return true;
            };

            g.GeneralSearchFrom<EdgesMaxPriorityQueue>(0, null, null, ve); //Search for potential cycles (edges that are in G but not in the tree
            return main.ToArray();
        }

        /// <summary>
        /// Dodaje 2 cykle fundamentalne
        /// </summary>
        /// <param name="c1">Pierwszy cykl</param>
        /// <param name="c2">Drugi cykl</param>
        /// <returns>null, jeśli wynikiem nie jest cykl i suma cykli, jeśli wynik jest cyklem</returns>
        public Edge[] AddFundamentalCycles(Edge[] c1, Edge[] c2)
        {
            int maxVertex = 0;
            var edges = new List<Edge>();
            bool does = true;
            int max = 0;
            var l1 = c1.ToList();
            var l2 = c2.ToList();
            var toRemove = new List<Edge>();

            foreach (Edge e1 in l1)
            {
                does = true;
                foreach (Edge e2 in l2)
                {
                    if (e1 == e2 || e1 == new Edge(e2.To, e2.From))
                    {
                        toRemove.Add(e1);
                        does = false;
                    }
                }
                if (does)
                {
                    max = Math.Max(e1.To, e1.From);
                    if (max > maxVertex)
                        maxVertex = max;
                    edges.Add(e1);
                }
            }

            foreach(Edge e in toRemove)
            {
                l1.Remove(e);
                l2.Remove(e);
            }
                

            does = true;
            foreach (Edge e2 in c2)
            {
                does = true;
                foreach (Edge e1 in c1)
                {
                    if (e2 == e1 || e2 == new Edge(e1.To, e1.From))
                    {
                        does = false;
                    }
                }
                if (does)
                {
                    max = Math.Max(e2.To, e2.From);
                    if (max > maxVertex)
                        maxVertex = max;
                    edges.Add(e2);
                }
            }
            
            if (edges.Count == 0)
                return null;

            Graph g = new AdjacencyListsGraph<AVLAdjacencyList>(false, maxVertex + 1);
            foreach (Edge e in edges)
                g.AddEdge(e);

            var cycle = new Edge[edges.Count];
            int iterator = 0;

            Predicate<Edge> ve = delegate (Edge e)
            {
                if(cycle.Contains(e) || cycle.Contains(new Edge(e.To, e.From)))
                {
                    return true;
                }
                cycle[iterator++] = e;
                return true;
            };
            g.GeneralSearchFrom<EdgesStack>(maxVertex, null, null, ve);
            return cycle[0].From == cycle.Last().To ? cycle : null;
        }
    }
}
