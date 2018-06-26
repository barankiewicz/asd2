using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD.Graphs
{

    public class CyclesTestCase : TestCase
    {
        Graph g;
        private Edge[] expectedCycle;
        private Edge[] cycle;
        private CycleChecker cycleChecker;
        private double result;
        private double expectedResult;

        public CyclesTestCase(double timeLimit, Exception expectedException, string description, Graph g, Edge[] cycle, Edge[] expectedCycle, double result, double expectedResult)
            : base(timeLimit, expectedException, description)
        {
            this.g = g;
            cycleChecker = new CycleChecker(g);
            this.expectedCycle = expectedCycle;
            this.result = result;
            this.expectedResult = expectedResult;
            this.cycle = cycle;
        }

        public override void PerformTestCase(object prototypeObject)
        {
            TSPHelper finder = (TSPHelper)prototypeObject;
            result = finder.TSP(g, out cycle);
        }

        public override void VerifyTestCase(out Result resultCode, out string message, object settings)
        {
            if (cycle == null)
            {
                message = "Nie zwrócono cyklu";
                resultCode = Result.BadResult;
                return;
            }

            if(!cycleChecker.Check(cycle, out message))
            {
                resultCode = Result.BadResult;
                return;
            }

            if(result != expectedResult)
            {
                message = "Zly wynik, powinno byc: " + expectedResult + ", a jest: " + result + "\n";
                resultCode = Result.BadResult;
                return;
            }


            resultCode = Result.Success;
            message = "OK";
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
            foreach (var e in cycle)
            {
                if (!g.ContainsEdge(e))
                {
                    message = $"Nieprawidłowa krawędź: {e}";
                    return false;
                }
            }
            for (int i = 0; i < cycle.Length; ++i)
                if (cycle[i].To != cycle[(i + 1) % cycle.Length].From)
                {
                    message = $"{cycle[i]}, {cycle[(i + 1) % cycle.Length]} nie są kolejnymi krawędziami cyklu";
                    return false;
                }
            message = "OK";
            return true;
        }
    }

    public class Lab05TestModule : TestModule
    {
        public override void PrepareTestSets()
        {
            TestSets["LabFindCyclesTests"] = new TestSet(new TSPHelper(), "TESTY TSP BRANCH AND BOUND");

            List<(Graph g, double res, Edge[] cycle)> testCases = new List<(Graph g, double res, Edge[] cycle)>
            {
                TestAbdula()
            };
            List<string> desc = new List<string>
            {
                "Graf ABDULA XD", "dziwny graf z messengera - wszystkie wagi rowne", "dziwny graf z messengera - waga srodkowa duza", "dziwny graf z messengera - waga srodkowa mala"
            };

            for (int i = 0; i < testCases.Count; ++i)
            {
                var (g, res, cyc) = testCases[i];
                TestSets["LabFindCyclesTests"].TestCases.Add(new CyclesTestCase(1, null, desc[i], g, null, cyc, 0, res));
            }
        }

        public override double ScoreResult(out string message)
        {
            message = "OK";
            return 1;
        }

        //cykl + krawędź
        private (Graph g, double res, Edge[] cycle) TestAbdula()
        {
            Graph g = new AdjacencyMatrixGraph(true, 4);
            List<Edge> cycle = new List<Edge>();
            g.AddEdge(0, 1, 4);
            g.AddEdge(0, 2, 12);
            g.AddEdge(0, 3, 7);
            

            g.AddEdge(1, 3, 18);
            g.AddEdge(1, 0, 5);

            g.AddEdge(2, 3, 6);
            g.AddEdge(2, 0, 11);

            g.AddEdge(3, 0, 10);
            g.AddEdge(3, 1, 2);
            g.AddEdge(3, 2, 3);

            cycle.Add(new Edge(0, 2, 12));
            cycle.Add(new Edge(2, 3, 6));
            cycle.Add(new Edge(3, 1, 2));
            cycle.Add(new Edge(1, 0, 5));
            double res = 25;
            return (g, res, cycle.ToArray());
        }



        private (Graph g, double res, Edge[] cycle) Test02()
        {
            Graph g = new AdjacencyMatrixGraph(true, 5);
            List<Edge> cycle = new List<Edge>();
            g.AddEdge(0, 1);
            g.AddEdge(1, 2);
            g.AddEdge(2, 0);
            g.AddEdge(2, 3);
            g.AddEdge(3, 4);
            g.AddEdge(4, 5);
            g.AddEdge(5, 2);


            cycle.Add(new Edge(0, 1));
            cycle.Add(new Edge(1, 3));
            cycle.Add(new Edge(3, 4));
            cycle.Add(new Edge(4, 5));
            cycle.Add(new Edge(5, 0));


            double res = 5;
            return (g, res, cycle.ToArray());
        }

        private (Graph g, double res, Edge[] cycle) Test03()
        {
            Graph g = new AdjacencyMatrixGraph(true, 5);
            List<Edge> cycle = new List<Edge>();
            g.AddEdge(0, 1);
            g.AddEdge(1, 2, 10);
            g.AddEdge(2, 0);
            g.AddEdge(2, 3);
            g.AddEdge(3, 4);
            g.AddEdge(4, 5);
            g.AddEdge(5, 2);


            cycle.Add(new Edge(0, 1));
            cycle.Add(new Edge(1, 3));
            cycle.Add(new Edge(3, 4));
            cycle.Add(new Edge(4, 5));
            cycle.Add(new Edge(5, 0));


            double res = 5;
            return (g, res, cycle.ToArray());
        }

        private (Graph g, double res, Edge[] cycle) Test04()
        {
            Graph g = new AdjacencyMatrixGraph(true, 5);
            List<Edge> cycle = new List<Edge>();
            g.AddEdge(0, 1);
            g.AddEdge(1, 2, 0.1);
            g.AddEdge(2, 0);
            g.AddEdge(2, 3);
            g.AddEdge(3, 4);
            g.AddEdge(4, 5);
            g.AddEdge(5, 2);


            cycle.Add(new Edge(0, 1));
            cycle.Add(new Edge(1, 3));
            cycle.Add(new Edge(3, 4));
            cycle.Add(new Edge(4, 5));
            cycle.Add(new Edge(5, 0));


            double res = 5;
            return (g, res, cycle.ToArray());
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
