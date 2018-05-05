
#define LAB

using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{

    public class CirculationsTestCase : TestCase
    {
        Graph GRef;
        double[] demandsRef;
        Graph G;
        double[] demands;

        bool expectedResult;
        Graph circulation;
        static int counter = 1;

        public CirculationsTestCase(double timeLimit, Exception expectedException, string description, Graph G, double[] demands, bool expectedResult)
                : base(timeLimit, expectedException, description)
        {
            this.G = G.Clone();
            this.GRef = G.Clone();
            this.demands = (double[])demands.Clone();
            this.demandsRef = (double[])demands.Clone();

            this.expectedResult = expectedResult;
        }

        public CirculationsTestCase(double timeLimit, string description, Graph G, double[] demands, bool expectedResult)
            :this(timeLimit,null,description,G,demands,expectedResult)
        {
        }

        public override void PerformTestCase(object prototypeObject)
        {
            ConstrainedFlows ap = (ConstrainedFlows)prototypeObject;
            circulation = ap.FindCirculation(G, demands);
        }

        public override void VerifyTestCase(out Result resultCode, out string message, object settings)
        {
            int n = G.VerticesCount;
#if LAB
            GraphExport ge = new GraphExport();
            if (circulation != null && ConstrainedFlows.circulationToDisplay.Contains(counter++))
            {
                string[] desc = new string[n];
                for (int v = 0; v < n; ++v)
                    desc[v] = $"{v} ({demandsRef[v]})";
                ge.Export(circulation, null, desc);
            }
#endif
            message = "";
            // sprawdzam, czy nie zmieniono danych wejściowych
            if (demandsRef.Length != demands.Length)
            {
                message = $"niedozwolona zmiana w tablicy demands (liczba elementów)";
                resultCode = Result.BadResult;
                return;
            }
            for (int v = 0; v < n; ++v)
                if (demands[v] != demandsRef[v])
                {
                    resultCode = Result.BadResult;
                    message = $"niedowolona zmiana w demands[{v}]";
                    return;
                }
            if (!G.IsEqual(GRef))
            {
                message = $"niedozwolona zmiana grafu wejściowego";
                resultCode = Result.BadResult;
                return;
            }

            // sprawdzanie, czy jest poprawna odpowiedź tak/nie
            if (!expectedResult && circulation == null)
            {
                message = $"OK, brak rozwiązania (time:{PerformanceTime,6:#0.000})";
                resultCode = Result.Success;
                return;
            }
            if (expectedResult && circulation == null)
            {
                message = $"zwrócono brak rozwiąznia, choć rozwiązanie istnieje";
                resultCode = Result.BadResult;
                return;
            }
            if (!expectedResult && circulation != null)
            {
                message = $"zwrócono rozwiązanie, choć rozwiązania nie ma, ";
//                resultCode = Result.BadResult;
//                return;
            }

            // sprawdzam, czy graf z rozwiązaniem ma poprawną postać i czy przepływy są z przedziału [0, capacity]
            if (circulation.VerticesCount != G.VerticesCount)
            {
                message += $"niewłaściwa liczba wierzchołków w grafie z wynikowym przepływem (jest {circulation.VerticesCount}, powinno być {G.VerticesCount})";
                resultCode = Result.BadResult;
                return;
            }
            if (circulation.EdgesCount != G.EdgesCount)
            {
                message += $"niewłaściwa liczba krawędzi w grafie z wynikowym przepływem (jest {circulation.EdgesCount}, powinno być {G.EdgesCount})";
                resultCode = Result.BadResult;
                return;
            }

            for (int v = 0; v < n; ++v)
                foreach (var e in circulation.OutEdges(v))
                {
                    double cap = G.GetEdgeWeight(v, e.To);
                    if (cap.IsNaN())
                    {
                        message += $"krawędź {e} istnieje w rozwiązaniu, a nie istnieje w grafie";
                        resultCode = Result.BadResult;
                        return;
                    }
                    if (e.Weight < 0 || e.Weight > cap)
                    {
                        message += $"krawędź {e} ma przepływ {e.Weight} przy przepustowości {cap}";
                        resultCode = Result.BadResult;
                        return;
                    }
                }

            // sprawdzam, czy spełnione są żądania
            double[] diffs = new double[n];

            for (int v = 0; v < n; ++v)
                foreach (var e in circulation.OutEdges(v))
                {
                    diffs[v] -= e.Weight;
                    diffs[e.To] += e.Weight;
                }
            for (int v = 0; v < n; ++v)
            {
                if (diffs[v] != demands[v])
                {
                    message += $"bilans w wierzchołku {v} wynosi {diffs[v]}, a powinien {demands[v]}";
                    resultCode = Result.BadResult;
                    return;
                }
            }
            message += $"OK (time:{PerformanceTime,6:#0.000})";
            resultCode = Result.Success;
        }
    }

    public class ConstrainedFlowTestCase : TestCase
    {
        Graph GRef;
        Graph lowerRef;
        Graph G;
        Graph lower;
        int source;
        int sink;

        bool expectedResult;
        Graph flow;
        static int counter = 1;

        public ConstrainedFlowTestCase(double timeLimit, Exception expectedException, string description,
            Graph G, Graph lower, int source, int sink, bool expectedResult)
                : base(timeLimit, expectedException, description)
        {
            this.G = G.Clone();
            this.GRef = G.Clone();
            this.lower = lower.Clone();
            this.lowerRef = lower.Clone();
            this.source = source;
            this.sink = sink;

            this.expectedResult = expectedResult;
        }

        public ConstrainedFlowTestCase(double timeLimit, string description, Graph G, Graph lower, int source, int sink, bool expectedResult)
            : this(timeLimit, null, description, G, lower, source, sink, expectedResult)
        {
        }

        public override void PerformTestCase(object prototypeObject)
        {
            ConstrainedFlows ap = (ConstrainedFlows)prototypeObject;
            flow = ap.FindConstrainedFlow(source, sink, G, lower);
        }

        public override void VerifyTestCase(out Result resultCode, out string message, object settings)
        {
            int n = G.VerticesCount;
#if LAB
            GraphExport ge = new GraphExport();
            if (flow != null && ConstrainedFlows.constrainedFlowToDisplay.Contains(counter++))
            {
                string[] desc = new string[n];
                for (int v = 0; v < n; ++v)
                    desc[v] = $"{v}";
                ge.Export(flow, null, desc);
            }
#endif
            message = "";
            // sprawdzam, czy nie zmieniono danych wejściowych
            if (!G.IsEqual(GRef))
            {
                message = $"niedozwolona zmiana grafu G";
                resultCode = Result.BadResult;
                return;
            }
            if (!lower.IsEqual(lowerRef))
            {
                message = $"niedozwolona zmiana grafu lower";
                resultCode = Result.BadResult;
                return;
            }

            // sprawdzanie, czy jest poprawna odpowiedź tak/nie
            if (!expectedResult && flow == null)
            {
                message = $"OK, brak rozwiązania (time:{PerformanceTime,6:#0.000})";
                resultCode = Result.Success;
                return;
            }
            if (expectedResult && flow == null)
            {
                message = $"zwrócono brak rozwiąznia, choć rozwiązanie istnieje";
                resultCode = Result.BadResult;
                return;
            }
            if (!expectedResult && flow != null)
            {
                message = $"zwrócono rozwiązanie, choć rozwiązania nie ma, ";
//                resultCode = Result.BadResult;
//                return;
            }

            // sprawdzam, czy graf z rozwiązaniem ma poprawną postać i czy przepływy są z przedziału [lower, capacity]
            if (flow.VerticesCount != G.VerticesCount)
            {
                message += $"niewłaściwa liczba wierzchołków w grafie z wynikowym przepływem (jest {flow.VerticesCount}, powinno być {G.VerticesCount})";
                resultCode = Result.BadResult;
                return;
            }
            if (flow.EdgesCount != G.EdgesCount)
            {
                message += $"niewłaściwa liczba krawędzi w grafie z wynikowym przepływem (jest {flow.EdgesCount}, powinno być {G.EdgesCount})";
                resultCode = Result.BadResult;
                return;
            }

            for (int v = 0; v < n; ++v)
                foreach (var e in flow.OutEdges(v))
                {
                    double cap = G.GetEdgeWeight(v, e.To);
                    double lo = lower.GetEdgeWeight(v, e.To);
                    if (cap.IsNaN())
                    {
                        message += $"krawędź {e} istnieje w rozwiązaniu, a nie istnieje w grafie";
                        resultCode = Result.BadResult;
                        return;
                    }
                    if (e.Weight < lo || e.Weight > cap)
                    {
                        message += $"krawędź {e} ma przepływ {e.Weight}, a powinien być w przedziale [{lo}, {cap}]";
                        resultCode = Result.BadResult;
                        return;
                    }
                }

            // sprawdzam warunek równowagi
            double[] diffs = new double[n];
            for (int v = 0; v < n; ++v)
                foreach (var e in flow.OutEdges(v))
                {
                    diffs[v] -= e.Weight;
                    diffs[e.To] += e.Weight;
                }
            for (int v = 0; v < n; ++v)
            {
                if ( v==source || v==sink ) continue;
                if (diffs[v] != 0)
                {
                    message += $"bilans w wierzchołku {v} wynosi {diffs[v]}, a powinien 0";
                    resultCode = Result.BadResult;
                    return;
                }
            }

            message += $"OK (time:{PerformanceTime,6:#0.000})";
            resultCode = Result.Success;
        }
    }

    class Lab10TestModule : TestModule
    {
        public override void PrepareTestSets()
        {
            TestSets["ClassCirculationTests"] = new TestSet(new ConstrainedFlows(), "Testy zajęciowe: circulation (2 pkt)");
            TestSets["ClassConstrainedFlowTests"] = new TestSet(new ConstrainedFlows(), "Testy zajęciowe: constrained flow (2 pkt)");
            PrepareClassCirculationTests();
            PrepareClassConstrainedFlowTests();
        }
        private void PrepareClassCirculationTests()
        {
            Graph G;
            double[] demands;

            List<Graph> graphsToTest = new List<Graph>();
            List<double[]> demandsToTest = new List<double[]>();
            List<bool> expectedResults = new List<bool>();
            List<string> descriptions = new List<string>();

            G = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 1);
            demands = new double[1] { 0};
            graphsToTest.Add(G.Clone());
            demandsToTest.Add((double[])demands.Clone());
            expectedResults.Add(true);
            descriptions.Add("graf z jednym wierzchołkiem i żądaniem 0");

            G = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 1);
            demands = new double[1] { 1 };
            graphsToTest.Add(G.Clone());
            demandsToTest.Add((double[])demands.Clone());
            expectedResults.Add(false);
            descriptions.Add("graf z jednym wierzchołkiem i żądaniem 1");

            G = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 4);
            G.AddEdge(0, 1, 3);
            G.AddEdge(0, 2, 3);
            G.AddEdge(1, 2, 2);
            G.AddEdge(1, 3, 2);
            G.AddEdge(2, 3, 2);
            demands = new double[4] { 0, 0, 0, 0 };

            graphsToTest.Add(G.Clone());
            demandsToTest.Add((double[])demands.Clone());
            expectedResults.Add(true);
            descriptions.Add("przepływ zerowy");

            G = new AdjacencyMatrixGraph(true, 4);
            G.AddEdge(0, 1, 3);
            G.AddEdge(0, 2, 3);
            G.AddEdge(1, 2, 2);
            G.AddEdge(1, 3, 2);
            G.AddEdge(2, 3, 2);
            demands = new double[4] { -3, -3, 2, 4 };

            graphsToTest.Add(G.Clone());
            demandsToTest.Add((double[])demands.Clone());
            expectedResults.Add(true);
            descriptions.Add("prosty graf o 4 wierzchołkach");

            G = new AdjacencyListsGraph<AVLAdjacencyList>(true, 4);
            G.AddEdge(0, 1, 3);
            G.AddEdge(0, 2, 3);
            G.AddEdge(1, 2, 2);
            G.AddEdge(1, 3, 2);
            G.AddEdge(2, 3, 2);
            demands = new double[4] { 1, 2, -3, 1 };

            graphsToTest.Add(G.Clone());
            demandsToTest.Add((double[])demands.Clone());
            expectedResults.Add(false);
            descriptions.Add("test niezrównoważony +");

            G = new AdjacencyListsGraph<SimpleAdjacencyList>(true, 4);
            G.AddEdge(0, 1, 3);
            G.AddEdge(0, 2, 3);
            G.AddEdge(1, 2, 2);
            G.AddEdge(1, 3, 2);
            G.AddEdge(2, 3, 2);
            demands = new double[4] { 0, -3, 1, 1 };

            graphsToTest.Add(G.Clone());
            demandsToTest.Add((double[])demands.Clone());
            expectedResults.Add(false);
            descriptions.Add("test niezrównoważony -");

            G = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 4);
            G.AddEdge(0, 1, 3);
            G.AddEdge(0, 2, 3);
            G.AddEdge(1, 2, 2);
            G.AddEdge(1, 3, 2);
            G.AddEdge(2, 3, 2);
            demands = new double[4] { 0, 4, -1, -3 };

            graphsToTest.Add(G.Clone());
            demandsToTest.Add((double[])demands.Clone());
            expectedResults.Add(false);
            descriptions.Add("test bez rozwiązania");

            G = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 6);
            G.AddEdge(0, 1, 2);
            G.AddEdge(0, 2, 3);
            G.AddEdge(1, 2, 4);
            G.AddEdge(1, 3, 6);
            G.AddEdge(2, 3, 6);
            G.AddEdge(2, 4, 4);
            G.AddEdge(3, 4, 4);
            G.AddEdge(3, 5, 3);
            G.AddEdge(4, 5, 2);

            demands = new double[6] { -3, -6, 0, 1, 5, 3 };

            graphsToTest.Add(G.Clone());
            demandsToTest.Add((double[])demands.Clone());
            expectedResults.Add(true);
            descriptions.Add("test na 6 wierzchołkach");

            int n = 300;
            G = new AdjacencyListsGraph<HashTableAdjacencyList>(true, n);
            demands = new double[n];
            for (int i = 0; i < n; ++i)
            {
                G.AddEdge(i, (i + 1) % n, 10);
                demands[i] = (i % 2 == 0) ? -1 : 1;
            }
            graphsToTest.Add(G.Clone());
            demandsToTest.Add((double[])demands.Clone());
            expectedResults.Add(true);
            descriptions.Add("duży cykl");

            for (int i = 0; i < graphsToTest.Count; ++i)
            {
                TestSets["ClassCirculationTests"].TestCases.Add(new CirculationsTestCase(i==graphsToTest.Count-1?10:1,descriptions[i], graphsToTest[i], demandsToTest[i], expectedResults[i]));
            }
        }

        private void PrepareClassConstrainedFlowTests()
        {
            Graph G;
            Graph lower;
            int source;
            int sink;

            List<Graph> graphsToTest = new List<Graph>();
            List<Graph> lowerToTest = new List<Graph>();
            List<int> sourcesToTest = new List<int>();
            List<int> sinksToTest = new List<int>();
            List<bool> expectedResults = new List<bool>();
            List<string> descriptions = new List<string>();

            G = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 4);
            lower = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 4);
            lower.AddEdge(0, 1, 0);
            G.AddEdge(0, 1, 3);

            lower.AddEdge(0, 2, 1);
            G.AddEdge(0, 2, 3);
            lower.AddEdge(1, 2, 2);
            G.AddEdge(1, 2, 5);
            lower.AddEdge(2, 3, 2);
            G.AddEdge(2, 3, 5);
            lower.AddEdge(1, 3, 1);
            G.AddEdge(1, 3, 2);
            source = 0;
            sink = 3;

            graphsToTest.Add(G.Clone());
            lowerToTest.Add(lower.Clone());
            sourcesToTest.Add(source);
            sinksToTest.Add(sink);
            expectedResults.Add(true);
            descriptions.Add("prosty test");

            G = new AdjacencyListsGraph<AVLAdjacencyList>(true, 4);
            lower = new AdjacencyListsGraph<AVLAdjacencyList>(true, 4);
            lower.AddEdge(0, 1, 0);
            G.AddEdge(0, 1, 3);
            lower.AddEdge(0, 2, 1);
            G.AddEdge(0, 2, 3);
            lower.AddEdge(1, 2, 2);
            G.AddEdge(1, 2, 5);
            lower.AddEdge(2, 3, 2);
            G.AddEdge(2, 3, 5);
            lower.AddEdge(1, 3, 1);
            G.AddEdge(1, 3, 2);
            lower.AddEdge(3, 0, 1);
            G.AddEdge(3, 0, 2);
            source = 0;
            sink = 3;

            graphsToTest.Add(G.Clone());
            lowerToTest.Add(lower.Clone());
            sourcesToTest.Add(source);
            sinksToTest.Add(sink);
            expectedResults.Add(true);
            descriptions.Add("test z krawędzią od ujścia do źródła");

            G = new AdjacencyListsGraph<SimpleAdjacencyList>(true, 4);
            lower = new AdjacencyListsGraph<SimpleAdjacencyList>(true, 4);
            lower.AddEdge(0, 1, 0);
            G.AddEdge(0, 1, 3);
            lower.AddEdge(0, 2, 100);
            G.AddEdge(0, 2, 100);
            lower.AddEdge(1, 2, 2);
            G.AddEdge(1, 2, 5);
            lower.AddEdge(2, 3, 2);
            G.AddEdge(2, 3, 5);
            lower.AddEdge(1, 3, 1);
            G.AddEdge(1, 3, 2);
            source = 0;
            sink = 3;

            graphsToTest.Add(G.Clone());
            lowerToTest.Add(lower.Clone());
            sourcesToTest.Add(source);
            sinksToTest.Add(sink);
            expectedResults.Add(false);
            descriptions.Add("test z absurdalnie dużym dolnym ograniczeniem");

            G = new AdjacencyMatrixGraph(true, 6);
            lower = new AdjacencyMatrixGraph(true, 6);
            lower.AddEdge(0, 1, 1);
            G.AddEdge(0, 1, 6);

            lower.AddEdge(0, 2, 1);
            G.AddEdge(0, 2, 1);

            lower.AddEdge(1, 2, 2);
            G.AddEdge(1, 2, 4);

            lower.AddEdge(1, 3, 0);
            G.AddEdge(1, 3, 6);

            lower.AddEdge(2, 3, 0);
            G.AddEdge(2, 3, 6);

            lower.AddEdge(2, 4, 0);
            G.AddEdge(2, 4, 4);

            lower.AddEdge(3, 4, 1);
            G.AddEdge(3, 4, 4);

            lower.AddEdge(3, 5, 1);
            G.AddEdge(3, 5, 5);

            lower.AddEdge(4, 5, 1);
            G.AddEdge(4, 5, 5);

            source = 0;
            sink = 5;
            graphsToTest.Add(G.Clone());
            lowerToTest.Add(lower.Clone());
            sourcesToTest.Add(source);
            sinksToTest.Add(sink);
            expectedResults.Add(true);
            descriptions.Add("test na 6 wierzchołkach");

            G = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 6);
            lower = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 6);
            lower.AddEdge(0, 1, 5);
            G.AddEdge(0, 1, 5);

            lower.AddEdge(0, 2, 1);
            G.AddEdge(0, 2, 1);

            lower.AddEdge(1, 2, 3);
            G.AddEdge(1, 2, 3);

            lower.AddEdge(1, 3, 2);
            G.AddEdge(1, 3, 2);

            lower.AddEdge(2, 3, 2);
            G.AddEdge(2, 3, 2);

            lower.AddEdge(2, 4, 2);
            G.AddEdge(2, 4, 2);

            lower.AddEdge(3, 4, 3);
            G.AddEdge(3, 4, 3);

            lower.AddEdge(3, 5, 1);
            G.AddEdge(3, 5, 1);

            lower.AddEdge(4, 5, 5);
            G.AddEdge(4, 5, 5);

            source = 0;
            sink = 5;
            graphsToTest.Add(G.Clone());
            lowerToTest.Add(lower.Clone());
            sourcesToTest.Add(source);
            sinksToTest.Add(sink);
            expectedResults.Add(true);
            descriptions.Add("test na 6 wierzchołkach bez żadnego wyboru");

            G = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 6);
            lower = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 6);
            lower.AddEdge(0, 1, 2);
            G.AddEdge(0, 1, 2);

            lower.AddEdge(0, 2, 1);
            G.AddEdge(0, 2, 3);

            lower.AddEdge(1, 2, 2);
            G.AddEdge(1, 2, 4);

            lower.AddEdge(1, 3, 3);
            G.AddEdge(1, 3, 6);

            lower.AddEdge(2, 3, 4);
            G.AddEdge(2, 3, 6);

            lower.AddEdge(2, 4, 0);
            G.AddEdge(2, 4, 4);

            lower.AddEdge(3, 4, 2);
            G.AddEdge(3, 4, 4);

            lower.AddEdge(3, 5, 1);
            G.AddEdge(3, 5, 3);

            lower.AddEdge(4, 5, 1);
            G.AddEdge(4, 5, 2);

            source = 0;
            sink = 5;
            graphsToTest.Add(G.Clone());
            lowerToTest.Add(lower.Clone());
            sourcesToTest.Add(source);
            sinksToTest.Add(sink);
            expectedResults.Add(false);
            descriptions.Add("test na 6 wierzchołkach bez rozwiązania");

            int n = 500;
            G = new AdjacencyListsGraph<HashTableAdjacencyList>(true, n);
            lower = new AdjacencyListsGraph<HashTableAdjacencyList>(true, n);

            for (int i = 0; i < n; ++i)
            {
                G.AddEdge(i, (i + 1) % n, 10);
                lower.AddEdge(i, (i + 1) % n, i%10);
            }
            source = 0;
            sink = n/2;

            graphsToTest.Add(G.Clone());
            lowerToTest.Add(lower.Clone());
            sourcesToTest.Add(source);
            sinksToTest.Add(sink);
            expectedResults.Add(true);
            descriptions.Add("duży cykl");

            for (int i = 0; i < graphsToTest.Count; ++i)
            {
                TestSets["ClassConstrainedFlowTests"].TestCases.Add(new ConstrainedFlowTestCase(i==graphsToTest.Count-1?15:1,
                    descriptions[i], graphsToTest[i], lowerToTest[i], sourcesToTest[i], sinksToTest[i], expectedResults[i]));
            }
        }

        public override double ScoreResult(out string message)
        {
            message = "OK";
            return 1;
        }
    }

    class Lab10
    {
        static void Main(string[] args)
        {
            Lab10TestModule lab10test = new Lab10TestModule();
            lab10test.PrepareTestSets();

            foreach (var ts in lab10test.TestSets)
            {
                ts.Value.PerformTests(verbose: true, checkTimeLimit: false);
            }
        }
    }
}
