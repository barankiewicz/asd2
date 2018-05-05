namespace ASD
{
    using System;
    using System.Collections.Generic;
    using ASD.Graphs;

    class Lab09Main
    {

        public static void Main()
        {
            Lab09TestModule lab09test = new Lab09TestModule();
            lab09test.PrepareTestSets();

            lab09test.TestSets["LabInducedMatchingTests"].PerformTests(verbose: true, checkTimeLimit: false);
            lab09test.TestSets["LabMaximalInducedMatchingTests"].PerformTests(verbose: true, checkTimeLimit: false);
            if (lab09test.TestSets["LabInducedMatchingTests"].FailedCount == 0)
                lab09test.TestSets["LabInducedMatchingPerformanceTests"].PerformTests(verbose: true, checkTimeLimit: true);
            if (lab09test.TestSets["LabMaximalInducedMatchingTests"].FailedCount == 0)
                lab09test.TestSets["LabMaximalInducedMatchingPerformanceTests"].PerformTests(verbose: true, checkTimeLimit: true);
        }

    }

    class Lab09TestModule : TestModule
    {
        public override void PrepareTestSets()
        {
            var rgg = new RandomGraphGenerator(2018);
            int n = 14;
            int n1 = 8;
            Graph[] graphs = new Graph[n];
            string[] descriptions = new string[n];

            // do czesci 1
            int[] sizes = new int[n];
            bool[] bool_results = new bool[n];
            int[] limits1 = new int[n];
            // do czesci 2 
            double[] maximal_results = new double[n];
            int[] limits2 = new int[n];
            

            //przyklady z zajec
            //sciezka P10
            graphs[0] = new AdjacencyMatrixGraph(false, 10);
            graphs[0].AddEdge(0, 1, 5);
            graphs[0].AddEdge(1, 2, 4);
            graphs[0].AddEdge(2, 3, 3);
            graphs[0].AddEdge(3, 4, 2);
            graphs[0].AddEdge(4, 5, 3);
            graphs[0].AddEdge(5, 6, 1);
            graphs[0].AddEdge(6, 7, 6);
            graphs[0].AddEdge(7, 8, 2);
            graphs[0].AddEdge(8, 9, 1);
            descriptions[0] = "Sciezka P10";
            sizes[0] = 3;
            bool_results[0] = true;
            maximal_results[0] = 13;

            //cykl C5
            graphs[1] = new AdjacencyMatrixGraph(false, 5);
            graphs[1].AddEdge(0, 1, 1);
            graphs[1].AddEdge(1, 2, 2);
            graphs[1].AddEdge(2, 3, 3);
            graphs[1].AddEdge(3, 4, 4);
            graphs[1].AddEdge(4, 0, 0);
            descriptions[1] = "Cykl C5";
            sizes[1] = 2;
            bool_results[1] = false;
            maximal_results[1] = 4;

            //cykl C10
            graphs[2] = new AdjacencyMatrixGraph(false, 10);
            graphs[2].AddEdge(0, 1, 1.08);
            graphs[2].AddEdge(1, 2, 6.52);
            graphs[2].AddEdge(2, 3, 3);
            graphs[2].AddEdge(3, 4, 4);
            graphs[2].AddEdge(4, 5, 0);
            graphs[2].AddEdge(5, 6, 1.99);
            graphs[2].AddEdge(6, 7, 2.56);
            graphs[2].AddEdge(7, 8, 3.9);
            graphs[2].AddEdge(8, 9, 4.12);
            graphs[2].AddEdge(9, 0, 0);
            descriptions[2] = "Cykl C10";
            sizes[2] = 2;
            bool_results[2] = true;
            maximal_results[2] = 12.63;

            //klika K5
            graphs[3] = rgg.UndirectedGraph(typeof(AdjacencyMatrixGraph), 5, 1,-9,-1);
            descriptions[3] = "Klika K5";
            sizes[3] = 1;
            bool_results[3] = true;
            maximal_results[3] = 0;

            //prawie klika K10 (bez kilku krawedzi)
            graphs[4] = rgg.UndirectedGraph(typeof(AdjacencyMatrixGraph), 10, 1,-9,99);
            descriptions[4] = "Klika K10 bez kilku krawedzi";
            graphs[4].DelEdge(0, 1);
            graphs[4].DelEdge(0, 2);
            graphs[4].DelEdge(0, 3);
            graphs[4].DelEdge(4, 5);
            graphs[4].DelEdge(4, 6);
            graphs[4].DelEdge(4, 7);
            sizes[4] = 1;
            bool_results[4] = true;
            maximal_results[4] = 98;

            //jakis sobie graf 
            graphs[5] = new AdjacencyListsGraph<SimpleAdjacencyList>(false, 9);
            graphs[5].AddEdge(0, 1, 0.33);
            graphs[5].AddEdge(1, 3, 1.5);
            graphs[5].AddEdge(1, 6, 10.1);
            graphs[5].AddEdge(2, 3, 1);
            graphs[5].AddEdge(2, 4, 1);
            graphs[5].AddEdge(2, 5, 1);
            graphs[5].AddEdge(2, 8, 3);
            graphs[5].AddEdge(3, 7, 3.33);
            graphs[5].AddEdge(4, 5, 3.7);
            graphs[5].AddEdge(7, 8, 0.7);
            descriptions[5] = "Pewien graf o 9 wierzcholkach";
            sizes[5] = 3;
            bool_results[5] = true;
            maximal_results[5] = 14.5;

            //graf dwudzielny
            graphs[6] = new AdjacencyListsGraph<SimpleAdjacencyList>(false, 10);
            graphs[6].AddEdge(0, 5, 3);
            graphs[6].AddEdge(0, 6, 3);
            graphs[6].AddEdge(1, 7, 4);
            graphs[6].AddEdge(1, 8, 4);
            graphs[6].AddEdge(2, 6, 1);
            graphs[6].AddEdge(2, 8, 1);
            graphs[6].AddEdge(2, 9, 1);
            graphs[6].AddEdge(3, 9, 2);
            graphs[6].AddEdge(4, 9, 3);
            descriptions[6] = "Graf dwudzielny o 10 wierzcholkach";
            sizes[6] = 3;
            bool_results[6] = true;
            maximal_results[6] = 10;

            //gwiazda
            int g7vc = 200;
            graphs[7] = new AdjacencyListsGraph<AVLAdjacencyList>(false, g7vc);
            for (int i = 1; i < g7vc; ++i)
                graphs[7].AddEdge(0, i, i);
            descriptions[7] = "Nieskierowana gwiazda";
            sizes[7] = 2;
            bool_results[7] = false;
            maximal_results[7] = 199;


            //Wieksze testy, z wydajnoscia
            //gesty duzy graf losowy nr 1
            int g8vc = 250;
            rgg.SetSeed(123451);
            graphs[8] = rgg.UndirectedGraph(typeof(AdjacencyMatrixGraph), g8vc, 0.9,-9,9);
            descriptions[8] = "Gesty graf losowy nr 1";
            sizes[8] = 3;
            bool_results[8] = true;
            maximal_results[8] = 21;
            limits1[8] = 600;
            limits2[8] = 600;

            //gesty duzy graf losowy nr 2
            int g9vc = 200;
            rgg.SetSeed(123452);
            graphs[9] = rgg.UndirectedGraph(typeof(AdjacencyMatrixGraph), g9vc, 0.95,-9,99);
            descriptions[9] = "Gesty graf losowy nr 2";
            sizes[9] = 3;
            bool_results[9] = false;
            maximal_results[9] = 194;
            limits1[9] = 12;
            limits2[9] = 12;

            //rzadki graf losowy nr 1
            int g10vc = 45;
            rgg.SetSeed(123453);
            graphs[10] = rgg.UndirectedGraph(typeof(AdjacencyListsGraph<SimpleAdjacencyList>), g10vc, 0.1,-9,99);
            descriptions[10] = "Rzadki graf losowy nr 1";
            sizes[10] = 5;
            bool_results[10] = true;
            maximal_results[10] = 743;
            limits1[10] = 1;
            limits2[10] = 65;

            //rzadki graf losowy nr 2
            int g11vc = 45;
            rgg.SetSeed(123454);
            graphs[11] = rgg.UndirectedGraph(typeof(AdjacencyListsGraph<SimpleAdjacencyList>), g11vc, 0.04,-9,99);
            descriptions[11] = "Rzadki graf losowy nr 2";
            sizes[11] = 7;
            bool_results[11] = true;
            maximal_results[11] = 675;
            limits1[11] = 1;
            limits2[11] = 12;

            //cykl C35
            int g12vc = 35;
            graphs[12] = new AdjacencyListsGraph<HashTableAdjacencyList>(false, g12vc);
            for (int i = 1; i < g12vc; ++i)
                graphs[12].AddEdge(i - 1, i, i);
            graphs[12].AddEdge(g12vc - 1, 0, 1);
            descriptions[12] = "Cykl C35";
            sizes[12] = 11;
            bool_results[12] = true;
            maximal_results[12] = 209;
            limits1[12] = 378903433;
            limits2[12] = 22;

            //duza klika K200
            int g13vc = 200;
            rgg.SetSeed(123455);
            graphs[13] = rgg.UndirectedGraph(typeof(AdjacencyMatrixGraph), g13vc, 1,-9,99);
            descriptions[13] = "Klika K200";
            sizes[13] = 100;
            bool_results[13] = false;
            maximal_results[13] = 99;
            limits1[13] = 10;
            limits2[13] = 10;

            TestSets["LabInducedMatchingTests"] = new TestSet(new Lab09(), "Part 1 - induced matching with given size [1.5]");
            TestSets["LabMaximalInducedMatchingTests"] = new TestSet(new Lab09(), "Part 2 - maximal induced matching [1.5]");
            TestSets["LabInducedMatchingPerformanceTests"] = new TestSet(new Lab09(), "Part 1 - induced matching with given size - performance tests[0.5]");
            TestSets["LabMaximalInducedMatchingPerformanceTests"] = new TestSet(new Lab09(), "Part 2 - maximal induced matching - performance tests[0.5]");

            for (int i = 0; i < n; ++i)
            {
                if (i < n1)
                {
                    //TestSets["LabInducedMatchingTests"].TestCases.Add(new InducedMatchingTestCase(1, null, descriptions[i], graphs[i], sizes[i], bool_results[i]));
                    TestSets["LabMaximalInducedMatchingTests"].TestCases.Add(new MaximalInducedMatchingTestCase(1, null, descriptions[i], graphs[i], maximal_results[i]));
                }
                else
                {
                    //TestSets["LabInducedMatchingPerformanceTests"].TestCases.Add(new InducedMatchingTestCase(limits1[i], null, descriptions[i], graphs[i], sizes[i], bool_results[i]));
                    TestSets["LabMaximalInducedMatchingPerformanceTests"].TestCases.Add(new MaximalInducedMatchingTestCase(limits2[i], null, descriptions[i], graphs[i], maximal_results[i]));
                }
            }
        }

        public override double ScoreResult(out string message)
        {
            message = "OK";
            return 1;
        }

    }

    public class InducedMatchingTestCase : TestCase
    { 
        private Graph input_graph;
        private Graph graph;
        private Edge[] result_maching;
        private int k;
        private bool expected_result;
        private bool result;

        public InducedMatchingTestCase(double timeLimit, Exception expectedException, string description, Graph graph, int k, bool result)
            : base(timeLimit, expectedException, description)
        {
            this.graph = graph;
            input_graph = graph.Clone(); //zapamietuje graf wejsciowy, zeby potem sprawzic czy algorytm go nie zmienia
            this.k = k;
            expected_result = result;
        }

        public override void PerformTestCase(object prototypeObject)
        {
            result = ((Lab09)prototypeObject).InducedMatching(graph, k, out result_maching);
        }

        public override void VerifyTestCase(out Result resultCode, out string message, object settings = null)
        {
            if (!input_graph.IsEqual(graph))
            {
                message = $"Changed input graph";
                resultCode = Result.BadResult;
                return;
            }
            if (result != expected_result)
            {
                message = $"Wrong result: {result},  should be: {expected_result}";
                resultCode = Result.BadResult;
                return;
            }
            if ( !expected_result )
                {
                if ( result_maching!=null )
                    {
                    message = $"Should be null";
                    resultCode = Result.BadResult;
                    return;
                    }
                else
                    {
                    resultCode = Result.Success;
                    message = $"OK (time:{PerformanceTime,6:#0.000})";
                    return;
                    }
                }

            if ( result_maching==null )
                {
                message = $"null";
                resultCode = Result.BadResult;
                return;
                }
            //sprawdzam rozmiar zwroconego skojarzenia indukowanego
            if (result_maching.Length != k)
            {
                message = $"Wrong size";
                resultCode = Result.BadResult;
                return;
            }
            //sprawdzam poprawnosc zwroconego skojarzenia indukowanego
            if (!TestHelper.TestEdges(input_graph, result_maching))
            {
                message = $"It is not an induced matching!";
                resultCode = Result.BadResult;
                return;
            }
            resultCode = Result.Success;
            message = $"OK (time:{PerformanceTime,6:#0.000})";
        }
    }
    public class MaximalInducedMatchingTestCase : TestCase
    {
        private Graph input_graph;
        private Graph graph;
        private Edge[] result_maching;
        private double expected_result;
        private double result;
        private double eps = 1e-10;

        public MaximalInducedMatchingTestCase(double timeLimit, Exception expectedException, string description, Graph graph, double result)
            : base(timeLimit, expectedException, description)
        {
            this.graph = graph;
            input_graph = graph.Clone(); //zapamietuje graf wejsciowy, zeby potem sprawzic czy algorytm go nie zmienia
            expected_result = result;
        }

        public override void PerformTestCase(object prototypeObject)
        {
            result = ((Lab09)prototypeObject).MaximalInducedMatching(graph, out result_maching);
        }

        public override void VerifyTestCase(out Result resultCode, out string message, object settings = null)
        {
            if (result_maching == null)
            {
                message = $"null";
                resultCode = Result.BadResult;
                return;
            }
            if (!input_graph.IsEqual(graph))
            {
                message = $"Changed input graph";
                resultCode = Result.BadResult;
                return;
            }
            if (Math.Abs(result - expected_result) > eps)
            {
                message = $"Wrong result: {result},  should be: {expected_result}";
                resultCode = Result.BadResult;
                return;
            }
            //sprawdzam czy zwrocona waga to rzeczywiscie waga zwroconego skojarzenia
            if (!TestHelper.TestWeight(result_maching, expected_result, eps))
            {
                message = $"Result induced matching has an incorrect weight";
                resultCode = Result.BadResult;
                return;
            }
            //sprawdzam poprawnosc zwroconego skojarzenia indukowanego
            if (!TestHelper.TestEdges(input_graph, result_maching))
            {
                message = $"It is not an induced matching!";
                resultCode = Result.BadResult;
                return;
            }
            resultCode = Result.Success;
            message = $"OK (time:{PerformanceTime,6:#0.000})";
        }
    }

    public static class TestHelper
    {
        public static bool TestEdges(Graph g, Edge[] tab)
        {
        try
            {
            for (int i = 0; i < tab.Length; ++i)
                for (int j = i + 1; j < tab.Length; ++j)
                    if (!g.GetEdgeWeight(tab[i].From, tab[j].From).IsNaN() ||
                        !g.GetEdgeWeight(tab[i].From, tab[j].To).IsNaN() ||
                        !g.GetEdgeWeight(tab[i].To, tab[j].From).IsNaN() ||
                        !g.GetEdgeWeight(tab[i].To, tab[j].To).IsNaN() )
                        return false;
            }
        catch ( Exception )
            {
            return false;
            }
        return true;
        }

        public static bool TestWeight(Edge[]tab, double weight, double eps)
        {
            double sum = 0;
            for (int i = 0; i < tab.Length; ++i)
                sum += tab[i].Weight;
            return Math.Abs(sum - weight) < eps;
        }
    }

}
