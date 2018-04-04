using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
    public class IsTreeTestCase : TestCase
    {
        private bool expected;
        private bool actual;
        private Graph g;
        private Graph g1;

        public IsTreeTestCase(double timeLimit, Exception expectedException, string description,
            Graph g, bool expected) 
            : base(timeLimit, expectedException, description)
        {
            this.g = g;
            g1 = g.Clone();
            this.expected = expected;
        }

        public override void PerformTestCase(object prototypeObject)
        {
            actual = ((CyclesFinder)prototypeObject).IsTree(g1);
        }

        public override void VerifyTestCase(out Result resultCode, out string message, object settings)
        {
            if(expected != actual)
            {
                resultCode = Result.BadResult;
                message = $"Niepoprawna odpowiedź: {actual} (oczekiwano {expected})";
            }
            else
            {
                resultCode = Result.Success;
                message = "OK";
            }
        }
    }

    public class CyclesTestCase : TestCase
    {
        private Graph g;
        private Graph t;
        private Graph g1;
        private Graph t1;
        private Edge[][] cycles;
        private CycleChecker cycleChecker;

        public CyclesTestCase(double timeLimit, Exception expectedException, string description, Graph g, Graph t) 
            : base(timeLimit, expectedException, description)
        {
            this.g = g;
            this.t = t;
            cycleChecker = new CycleChecker(g);
            if (g != null)
                g1 = g.Clone();
            if (t != null)
                t1 = t.Clone();
        }

        public override void PerformTestCase(object prototypeObject)
        {
            CyclesFinder finder = (CyclesFinder)prototypeObject;
            cycles = finder.FindFundamentalCycles(g1, t1);
        }

        public override void VerifyTestCase(out Result resultCode, out string message, object settings)
        {
            if(cycles == null)
            {
                message = "Nie zwrócono wyniku";
                resultCode = Result.BadResult;
                return;
            }
            if(!AreEqual(g1, g) || !AreEqual(t1, t))
            {
                message = "Zmodyfikowano grafy";
                resultCode = Result.BadResult;
                return;
            }
            Graph tLookup = new AdjacencyListsGraph<HashTableAdjacencyList>(t);
            Graph gLookup = new AdjacencyListsGraph<HashTableAdjacencyList>(g);
            var test = gLookup.Clone();
            foreach (var e in tLookup.GetEdges())
                test.DelEdge(e);
            foreach (var cycle in cycles)
            {
                bool notInTree = false;
                bool[] vertices = new bool[g.VerticesCount];
                if(!cycleChecker.Check(cycle, out message))
                {
                    resultCode = Result.BadResult;
                    return;
                }
                foreach (var e in cycle)
                {
                    if (!tLookup.ContainsEdge(e))
                    {
                        if (notInTree)
                        {
                            message = "Wiele krawędzi spoza drzewa";
                            resultCode = Result.BadResult;
                            return;
                        }
                        notInTree = true;
                        if (!test.ContainsEdge(e))
                        {
                            message = $"Wiele cykli z krawędzią {e}";
                            resultCode = Result.BadResult;
                            return;
                        }
                        test.DelEdge(e);
                    }
                }
            }
            if(test.EdgesCount != 0)
            {
                var e = test.GetEdges().First();
                message = $"Brak cyklu z krawędzią {e}";
                resultCode = Result.BadResult;
                return;
            }
            resultCode = Result.Success;
            message = "OK";
        }

        private bool AreEqual(Graph g1, Graph g2)
        {
            if (g1.VerticesCount != g2.VerticesCount)
                return false;
            for(int i=0;i<g1.VerticesCount;++i)
            {
                var g1e = new HashSet<Edge>(g1.OutEdges(i));
                var g2e = new HashSet<Edge>(g2.OutEdges(i));
                if (!g1e.SetEquals(g2e))
                    return false;
            }
            return true;
        }
    }
    
    public class CycleChecker
    {
        Graph g;

        public CycleChecker(Graph g)
        {
            this.g = new AdjacencyListsGraph<HashTableAdjacencyList>(g);
        }

        public bool Check(Edge[] cycle, out string message)
        {
            var v = cycle.Select(e => e.From).GroupBy(c => c)
                .Where(grp => grp.Count() > 1).Select(grp => grp.Key);
            if (v.Any())
            {
                message = $"Wierzchołek {v.First()} występuje wielokrotnie";
                return false;
            }
            foreach(var e in cycle)
            {
                if(!g.ContainsEdge(e))
                {
                    message = $"Nieprawidłowa krawędź: {e}";
                    return false;
                }
            }
            for (int i = 0; i < cycle.Length; ++i)
                if(cycle[i].To != cycle[(i+1) % cycle.Length].From)
                {
                    message = $"{cycle[i]}, {cycle[(i+1) % cycle.Length]} nie są kolejnymi krawędziami cyklu";
                    return false;
                }
            message = "OK";
            return true;
        }
    }

    public class AddCyclesTestCase : TestCase
    {
        private Edge[] c1;
        private Edge[] c2;
        private List<int> expected;
        private Edge[] result;

        public AddCyclesTestCase(double timeLimit, Exception expectedException, string description,
            (Graph g, List<int> c1, List<int> c2, List<int> res) test)
            : base(timeLimit, expectedException, description)
        {
            c1 = ToEdges(test.c1);
            c2 = ToEdges(test.c2);
            expected = test.res;
        }

        private Edge[] ToEdges(List<int> vertices)
        {
            Edge[] edges = new Edge[vertices.Count];
            for (int i = 1; i < vertices.Count; ++i)
                edges[i - 1] = new Edge(vertices[i - 1], vertices[i]);
            edges[vertices.Count - 1] = new Edge(vertices[vertices.Count - 1], vertices[0]);
            return edges;
        }

        public override void PerformTestCase(object prototypeObject)
        {
            CyclesFinder finder = (CyclesFinder)prototypeObject;
            result = finder.AddFundamentalCycles((Edge[])c1.Clone(), (Edge[])c2.Clone());
        }

        public override void VerifyTestCase(out Result resultCode, out string message, object settings)
        {
            if(expected == null)
            {
                if(result == null)
                {
                    message = "Ok (null)";
                    resultCode = Result.Success;
                    return;
                }
                else
                {
                    message = "Zwrócono cykl, gdy oczekiwano null";
                    resultCode = Result.BadResult;
                    return;
                }
            }
            if (result == null)
            {
                message = "Zwrócono null, gdy wynik jest cyklem";
                resultCode = Result.BadResult;
                return;
            }
            if(result.Length != expected.Count)
            {
                message = $"Nieprawidłowa liczba krawędzi: {result.Length} (oczekiwano {expected.Count})";
                resultCode = Result.BadResult;
                return;
            }
            if(result[result.Length-1].To != result[0].From)
            {
                message = "Nie zwrócono cyklu";
                resultCode = Result.BadResult;
                return;
            }

            int start = 0;
            for (; start < expected.Count; ++start)
                if (result[0].From == expected[start])
                    break;

            List<int> tmp;
            if (expected[(start + 1) % expected.Count] != result[0].To)
                tmp = expected.Skip(start + 1).Concat(expected.Take(start + 1)).Reverse().ToList();
            else
                tmp = expected.Skip(start).Concat(expected.Take(start)).ToList();
            for (int i = 0; i < tmp.Count; ++i) {
                if (result[i].From != tmp[i])
                {
                    message = $"Niepoprawna krawędź: {result[i]}";
                    resultCode = Result.BadResult;
                    return;
                }
                if(result[i].To != tmp[(i+1)%tmp.Count])
                {
                    message = $"Niepoprawna krawędź {result[i]}";
                    resultCode = Result.BadResult;
                    return;
                }
            }
            resultCode = Result.Success;
            message = "OK";
        }
    }

    public class Lab05TestModule : TestModule
    {
        public override void PrepareTestSets()
        {
            Random random = new Random(20180310);
            RandomGraphGenerator rgg = new RandomGraphGenerator(2018);

            TestSets["LabIsTree"] = new TestSet(new CyclesFinder(), "Część 1 - czy graf jest drzewem (0.5 pkt)");
            TestSets["LabFindCyclesTests"] = new TestSet(new CyclesFinder(), "Część 2 - cykle fundamentalne (2.5 pkt)");
            TestSets["LabAddCyclesTests"] = new TestSet(new CyclesFinder(), "Część 3 - dodawanie cykli fundamentalnych (1 pkt)");

            List<(Graph g, Graph t)> testCases = new List<(Graph g, Graph t)>
            {
                Test01(), Test02(), Test03(), Test04(), Test05()
            };
            //testy losowe
            testCases.Add(FullGraph(rgg, 7));
            testCases.Add(FullGraph(rgg, 30));
            testCases.Add(FullGraph(rgg, 75));
            testCases.Add(RandomTest(random, 20, 4));
            testCases.Add(RandomTest(random, 30, 5));
            testCases.Add(RandomTest(random, 200, 7));

            TestSets["LabIsTree"].TestCases.AddRange(new[]{
                new IsTreeTestCase(1, null, "", testCases[2].t, true),
                new IsTreeTestCase(1, null, "Z cyklami", testCases[4].g, false),
                new IsTreeTestCase(1, null, "Bez krawędzi", testCases[6].g.IsolatedVerticesGraph(), false),
                new IsTreeTestCase(1, null, "Skojarzenie doskonałe", IsTreeTest01(), false),
                new IsTreeTestCase(1, null, "Niespójny", IsTreeTest02(), false),
                new IsTreeTestCase(1, null, "", testCases[9].t, true)
            });
            TestSets["LabIsTree"].TestCases.Add(new CyclesTestCase(1, new ArgumentException(), "Skierowany",
                rgg.DirectedGraph(typeof(AdjacencyListsGraph<AVLAdjacencyList>), 9, 0.3), null));

            List<string> desc = new List<string>
            {
                "cykl + krawędź", "3 cykle fund.", "drzewo", "krawędzie między poziomami",
                "głębokie drzewo", "pełny-7", "pełny-30", "pełny-75", "losowy-20", "losowy-30", "losowy-200"
            };

            for(int i= 0;i < testCases.Count;++i)
            {
                var (g, t) = testCases[i];
                TestSets["LabFindCyclesTests"].TestCases.Add(new CyclesTestCase(1, null, desc[i], g, t));
            }
            TestSets["LabFindCyclesTests"].TestCases.Add(new CyclesTestCase(1, new ArgumentException(), "Skierowany",
                rgg.DirectedGraph(typeof(AdjacencyListsGraph<AVLAdjacencyList>), 8, 0.7), null));

            var addCases = new List<(Graph g, List<int> c1, List<int> c2, List<int> c)>
            {
                AddTest01(), AddTest02(), AddTest03(), AddTest04(),
                AddTest05(), AddTest06(), AddTest07(), AddTest08(), AddTest09()
            };
            List<string> addDesc = new List<string>
            {
                "wspólny wierzchołek", "dłuższy cykl wynikowy", "krótszy cykl wynikowy",
                "krótszy cykl wynikowy 2", "dłuższy cykl wynikowy 2", "2 wspólne krawędzie",
                "3 wspólne krawędzie", "całkowicie rozłączne", "bez min i max"
            };
            for (int i = 0; i < addCases.Count; ++i)
                TestSets["LabAddCyclesTests"].TestCases.Add(new AddCyclesTestCase(1, null, addDesc[i], addCases[i]));
        }

        public override double ScoreResult(out string message)
        {
            message = "OK";
            return 1;
        }

        //skojarzenie doskonałe
        private Graph IsTreeTest01()
        {
            return new AdjacencyListsGraph<AVLAdjacencyList>(false, 10)
            {
                new Edge(0, 1), new Edge(2, 3), new Edge(4, 5),
                new Edge(6, 7), new Edge(8, 9)
            };
        }

        //graf niespójny
        private Graph IsTreeTest02()
        {
            var (g, t) = Test04();
            g.DelEdge(0, 2);
            g.DelEdge(3, 4);
            return g;
        }

        //cykl + krawędź
        private (Graph g, Graph t) Test01()
        {
            Graph t = new AdjacencyMatrixGraph(false, 4);
            t.AddEdge(0, 1);
            t.AddEdge(1, 2);
            t.AddEdge(0, 3);
            Graph g = t.Clone();
            g.AddEdge(1, 3);
            return (g, t);
        }

        //3 cykle
        private (Graph g, Graph t) Test02()
        {
            Graph t = new AdjacencyListsGraph<SimpleAdjacencyList>(false, 5);
            t.AddEdge(0, 1);
            t.AddEdge(0, 2);
            t.AddEdge(0, 3);
            t.AddEdge(0, 4);
            Graph g = new AdjacencyListsGraph<HashTableAdjacencyList>(t);
            g.AddEdge(1, 3);
            g.AddEdge(1, 4);
            g.AddEdge(1, 2);
            return (g, t);
        }

        //drzewo
        private (Graph g, Graph t) Test03()
        {
            Graph t = new AdjacencyListsGraph<SimpleAdjacencyList>(false, 10);
            t.AddEdge(0, 1);
            t.AddEdge(0, 2);
            t.AddEdge(0, 8);
            t.AddEdge(1, 9);
            t.AddEdge(1, 3);
            t.AddEdge(2, 4);
            t.AddEdge(2, 6);
            t.AddEdge(6, 7);
            t.AddEdge(4, 5);
            return (t.Clone(), t);
        }

        // krawędzie (spoza drzewa) między poziomami drzewa
        private (Graph g, Graph t) Test04()
        {
            var (g, t) = Test03();
            g.AddEdge(1, 8);
            g.AddEdge(8, 9);
            g.AddEdge(5, 6);
            g.AddEdge(5, 7);
            g.AddEdge(3, 4);
            return (g, t);
        }

        //głębokie drzewo
        private (Graph, Graph) Test05()
        {
            Graph t = new AdjacencyListsGraph<AVLAdjacencyList>(false, 16);
            t.AddEdge(0, 3);
            t.AddEdge(3, 5);
            t.AddEdge(5, 1);
            t.AddEdge(1, 6);
            t.AddEdge(6, 11);
            t.AddEdge(0, 7);
            t.AddEdge(7, 10);
            t.AddEdge(10, 15);
            t.AddEdge(10, 4);
            t.AddEdge(4, 8);
            t.AddEdge(0, 12);
            t.AddEdge(12, 13);
            t.AddEdge(13, 2);
            t.AddEdge(2, 9);
            t.AddEdge(9, 14);
            Graph g = t.Clone();
            g.AddEdge(3, 12);
            g.AddEdge(5, 7);
            g.AddEdge(10, 13);
            g.AddEdge(4, 13);
            g.AddEdge(1, 4);
            g.AddEdge(4, 2);
            return (g, t);
        }

        private (Graph, Graph) FullGraph(RandomGraphGenerator rgg, int n)
        {
            Graph g = rgg.UndirectedGraph(typeof(AdjacencyMatrixGraph), n, 1);
            Graph t = new AdjacencyListsGraph<SimpleAdjacencyList>(false, n);
            for (int i = 1; i < n; ++i)
                t.AddEdge(0, i);
            return (g, t);
        }

        private (Graph, Graph) RandomTest(Random r, int n, int maxLvlSize)
        {
            Graph t = new AdjacencyListsGraph<SimpleAdjacencyList>(false, n);
            Graph g = new AdjacencyListsGraph<AVLAdjacencyList>(false, n);
            int minLvl = 0, maxLvl = 0;
            while (maxLvl < n-1)
            {
                int lvlCnt = r.Next(1, Math.Min(maxLvlSize, n - maxLvl - 1));
                for (int i = 0; i < lvlCnt; ++i)
                {
                    int parent = r.Next(minLvl, maxLvl);
                    t.AddEdge(parent, maxLvl + i + 1);
                    g.AddEdge(parent, maxLvl + i + 1);
                    for (int j = 0; j < i; ++j)
                        if (r.Next(0, 2) == 1)
                            g.AddEdge(maxLvl + i + 1, maxLvl + j + 1);
                }
                minLvl = maxLvl + 1;
                maxLvl = maxLvl + lvlCnt;
            }
            return (g, t);
        }

        //wspólny wierzchołek
        private (Graph, List<int>, List<int>, List<int>) AddTest01()
        {
            var (g, _) = Test02();
            List<int> c1 = new List<int> { 0, 4, 3 },
                c2 = new List<int> { 0, 1, 2 };
            return (g, c1, c2, null);
        }

        //dłuższy cykl wynikowy
        private (Graph, List<int>, List<int>, List<int>) AddTest02()
        {
            var (g, _) = Test02();
            List<int> c1 = new List<int> { 0, 3, 1 },
                c2 = new List<int> { 0, 1, 2 },
                c = new List<int> { 0, 3, 1, 2 };
            return (g, c1, c2, c);
        }

        //krótszy cykl wynikowy
        private (Graph, List<int>, List<int>, List<int>) AddTest03()
        {
            var (g, _) = Test04();
            List<int> c1 = new List<int> { 6, 7, 5, 4, 2 },
                c2 = new List<int> { 6, 5, 4, 2 };
            var c = new List<int> { 6, 7, 5 };
            return (g, c1, c2, c);
        }

        //krótszy cykl wynikowy 2
        private (Graph, List<int>, List<int>, List<int>) AddTest04()
        {
            var (g, _) = Test04();
            List<int> c1 = new List<int> { 0, 1, 8 },
                c2 = new List<int> { 0, 1, 9, 8 };
            var c = new List<int> { 1, 9, 8 };
            return (g, c1, c2, c);
        }

        //dłuższy cykl wynikowy 2
        private (Graph, List<int>, List<int>, List<int>) AddTest05()
        {
            var (g, _) = Test05();
            List<int> c1 = new List<int> { 0, 3, 5, 7 },
                c2 = new List<int> { 0, 3, 12 };
            var c = new List<int> { 3, 5, 7, 0, 12 };
            return (g, c1, c2, c);
        }

        //2 wspólne krawędzie
        private (Graph, List<int>, List<int>, List<int>) AddTest06()
        {
            var (g, _) = Test05();
            List<int> c1 = new List<int> { 0, 3, 5, 1, 4, 10, 7 },
                c2 = new List<int> { 0, 7, 10, 13, 12 };
            var c = new List<int> { 0, 3, 5, 1, 4, 10, 13, 12 };
            return (g, c1, c2, c);
        }

        //3 wspólne krawędzie
        private (Graph, List<int>, List<int>, List<int>) AddTest07()
        {
            var (g, _) = Test05();
            List<int> c1 = new List<int> { 0, 3, 5, 7 },
                c2 = new List<int> { 0, 3, 5, 1, 4, 10, 7 },
                c = new List<int> { 5, 1, 4, 10, 7 };
            return (g, c1, c2, c);
        }

        //całkowicie rozłączne
        private (Graph, List<int>, List<int>, List<int>) AddTest08()
        {
            var (g, _) = Test04();
            List<int> c1 = new List<int> { 0, 1, 8 },
                c2 = new List<int> { 2, 6, 5, 4 };
            return (g, c1, c2, null);
        }

        //wynik nie zawiera wierzchołków o minimalnym i maksymalnym indeksie
        private (Graph, List<int>, List<int>, List<int>) AddTest09()
        {
            return (null,
                new List<int> { 4, 5, 0, 1, 2 },
                new List<int> { 4, 5, 0, 1, 3 },
                new List<int> { 4, 3, 1, 2});
        }

    }

    public class Lab05
    {
        static void Main(string[] args)
        {
            TestModule lab05test = new Lab05TestModule();
            lab05test.PrepareTestSets();

            foreach (var ts in lab05test.TestSets)
            {
                ts.Value.PerformTests(verbose: true, checkTimeLimit: false);
            }

        }
    }

    public static class SimpleGraphExtensions
    {
        public static bool ContainsEdge(this Graph g, Edge e)
        {
            return !double.IsNaN(g.GetEdgeWeight(e.From, e.To));
        }

        public static IEnumerable<Edge> GetEdges(this Graph g)
        {
            return Enumerable.Range(0, g.VerticesCount).SelectMany(i => g.OutEdges(i));
        }
    }
}
