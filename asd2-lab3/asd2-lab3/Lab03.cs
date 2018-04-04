using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{

    // Klasy Lab03Helper NIE WOLNO ZMIENIAĆ !!!
    public class Lab03Helper : System.MarshalByRefObject
    {
        public Graph SquareOfGraph(Graph graph) => graph.SquareOfGraph();
        public Graph LineGraph(Graph graph, out (int x, int y)[] names) => graph.LineGraph(out names);
        public Graph LineGraph(Graph graph, out int [,] names)
            {
            Graph g = graph.LineGraph(out (int x, int y)[] _names);
            if ( _names==null )
                names = null;
            else
                {
                names = new int[_names.Length,2];
                for ( int i=0 ; i<_names.Length ; ++i )
                    {
                    names[i,0] = _names[i].x;
                    names[i,1] = _names[i].y;
                    }
                }
            return g;
            }
        public int VertexColoring(Graph graph, out int[] colors) => graph.VertexColoring(out colors);
        public int StrongEdgeColoring(Graph graph, out Graph coloredGraph) => graph.StrongEdgeColoring(out coloredGraph);
    }

    // Uwagi do wszystkich metod
    // 1) Grafy wynikowe powinny być reprezentowane w taki sam sposób jak grafy będące parametrami
    // 2) Grafów będących parametrami nie wolno zmieniać
    static class Lab03
    {

        // 0.5 pkt
        // Funkcja zwracajaca kwadrat grafu graph.
        // Kwadratem grafu nazywamy graf o takim samym zbiorze wierzcholkow jak graf bazowy,
        // 2 wierzcholki polaczone sa krawedzia jesli w grafie bazowym byly polaczone krawedzia badz sciezka zlozona z 2 krawedzi
        public static Graph SquareOfGraph(this Graph graph)
        {
            Graph ret = graph.Clone();
            for(int i = 0; i < ret.VerticesCount; i++)
            {
                foreach(Edge e in graph.OutEdges(i))
                {
                    foreach (Edge r in graph.OutEdges(e.To))
                    {
                        if(i != r.To)
                        {
                            bool val = ret.AddEdge(i, r.To);
                        }
                    }
                }
            }
            return ret;
        }

        // 2 pkt
        // Funkcja zwracająca Graf krawedziowy grafu graph
        // Wierzcholki grafu krawedziwego odpowiadaja krawedziom grafu bazowego,
        // 2 wierzcholki grafu krawedziwego polaczone sa krawedzia
        // jesli w grafie bazowym z krawędzi odpowiadającej pierwszemu z nich można przejść 
        // na krawędź odpowiadającą drugiemu z nich przez wspólny wierzchołek.
        //
        // (w grafie skierowanym: 2 wierzcholki grafu krawedziwego polaczone sa krawedzia
        // jesli wierzcholek koncowy krawedzi odpowiadajacej pierwszemu z nich
        // jest wierzcholkiem poczatkowym krawedzi odpowiadajacej drugiemu z nich)
        //
        // do tablicy names nalezy wpisac numery wierzcholkow grafu krawedziowego,
        // np. dla wierzcholka powstalego z krawedzi <0,1> do tabeli zapisujemy krotke (0, 1) - przyda się w dalszych etapach
        //
        // UWAGA: Graf bazowy może być skierowany lub nieskierowany, graf krawędziowy zawsze jest nieskierowany.

        public static Graph LineGraph(this Graph graph, out (int x, int y)[] names)
        {
            int n = graph.VerticesCount;
            int m = graph.EdgesCount;
            var nam = new List<(int, int)>();
            Graph ret = graph.IsolatedVerticesGraph(false, m);

            //Initialize helper
            int no = 0;
            Graph helper = graph.Clone();

            //Initialize array
            var ord = new int[n, n];
            for(int i = 0; i < n; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    ord[i, j] = -1;
                }
            }

            Predicate<Edge> ve = delegate (Edge e)
            {
                int value = 0;
                if(ord[e.From, e.To] == -1 && ord[e.To, e.From] == -1)
                {
                    value = no;
                    helper.ModifyEdgeWeight(e.From, e.To, no);
                    ord[e.From, e.To] = value;
                    ord[e.To, e.From] = value;
                    no++;
                }
                else if (ord[e.From, e.To] == -1 && ord[e.To, e.From] != -1)
                {
                    value = ord[e.To, e.From];
                    ord[e.From, e.To] = value;
                }
                else
                {
                    value = ord[e.From, e.To];
                    ord[e.To, e.From] = value;
                }
                return true;
            };

            Predicate<Edge> veDirected = delegate (Edge e)
            {
                int value = 0;
                if (ord[e.From, e.To] == -1)
                {
                    value = no;
                    helper.ModifyEdgeWeight(e.From, e.To, no);
                    ord[e.From, e.To] = value;
                    no++;
                }
                return true;
            };
            int cc;

            if (graph.Directed)
                helper.GeneralSearchAll<EdgesStack>(null, null, veDirected, out cc, null);
            else
                helper.GeneralSearchAll<EdgesStack>(null, null, ve, out cc, null);
            
            //Main algorithm
            for(int i = 0; i < n; i++)
            {
                foreach(Edge e in graph.OutEdges(i))
                {
                    int u = e.To;
                    foreach(Edge ed in graph.OutEdges(u))
                    {
                        int w = ed.To;
                        if (i != w)
                        {
                            int from = (int)helper.GetEdgeWeight(i, u) - 1;
                            int to = (int)helper.GetEdgeWeight(u, w) - 1;
                            ret.AddEdge(from, to);
                            nam.Add((from, to));
                        }
                    }
                }
            }
            names = nam.ToArray();
            return ret;
        }

        // 1 pkt
        // Funkcja znajdujaca poprawne kolorowanie wierzcholkow grafu graph
        // Kolorowanie wierzcholkow jest poprawne, gdy kazde dwa sasiadujace wierzcholki maja rozne kolory
        // Funkcja ma szukać kolorowania wedlug nastepujacego algorytmu zachlannego:
        //
        // Dla wszystkich wierzcholkow (od 0 do n-1) 
        //      pokoloruj wierzcholek v na najmniejszy mozliwy kolor (czyli taki, na ktory nie sa pomalowani jego sasiedzi)
        //
        // Nalezy zwrocic liczbe kolorow, a w tablicy colors zapamietac kolory dla poszczegolnych wierzcholkow
        //
        // UWAGA: Dla grafów skierowanych metoda powinna zgłaszać wyjątek ArgumentException
        public static int VertexColoring(this Graph graph, out int[] colors)
        {
            if (graph.Directed)
                throw new ArgumentException();

            int amt = 1;
            colors = new int[graph.VerticesCount];

            //Initialize array
            for (int j = 0; j < colors.Length; j++)
                colors[j] = -1;

            for (int i = 0; i < graph.VerticesCount; i++)
            {
                //Temp list for neighbors' colors
                var neighcol = new List<int>();
                foreach(Edge e in graph.OutEdges(i))
                {
                    if(colors[e.To] != -1) //if the vertex is colored, add its' color to temp list
                        neighcol.Add(colors[e.To]);
                }
                if (!neighcol.Any())
                {
                    colors[i] = 0;
                    continue;
                }

                if(neighcol.Count == amt) //if there's as many neighbours as there are colors, you have to use a new color
                {
                    colors[i] = amt;
                    amt++;
                }
                else
                {
                    int max = neighcol.Max();
                    int min = neighcol.Min();
                    colors[i] = min == 0 ? colors[i] = max + 1: colors[i] = min - 1;
                }
            }
            return amt;
        }

        // 0.5 pkt
        // Funkcja znajdujaca silne kolorowanie krawedzi grafu graph
        // Silne kolorowanie krawedzi grafu jest poprawne gdy kazde dwie krawedzie, ktore sa ze soba sasiednie
        // albo sa polaczone inna krawedzia, maja rozne kolory.
        //
        // Nalezy zwrocic nowy graf, ktory bedzie kopia zadanego grafu, ale w wagach krawedzi zostana zapisane znalezione kolory
        // 
        // Wskazowka - to bardzo proste. Nalezy tu wykorzystac wszystkie poprzednie funkcje. 
        // Zastanowic sie co mozemy powiedziec o kolorowaniu wierzcholkow kwadratu grafu krawedziowego - jak sie ma do silnego kolorowania krawedzi grafu bazowego
        public static int StrongEdgeColoring(this Graph graph, out Graph coloredGraph)
        {
            (int,int)[] names;
            int[] SquareLineGraphColoring;
            Graph ret = graph.Clone();
            

            Graph SquareLineGraph = graph.LineGraph(out names).SquareOfGraph();
            int SquareLineGraphColorsAmt = SquareLineGraph.VertexColoring( out SquareLineGraphColoring);

            Console.WriteLine();
            Console.WriteLine("Graf bazowy ma " + graph.EdgesCount + " krawedzi, a kwadrat grafu krawedziowego ma " + SquareLineGraph.VerticesCount + " wierzcholkow");
            Console.WriteLine("kwadrat grafu krawedziowego da się pokolorowac na " + SquareLineGraphColorsAmt + " kolorow");

            coloredGraph = ret;
            return SquareLineGraphColorsAmt;
        }
    }
}
