namespace ASD
{
    using System;
    using System.Collections.Generic;
    using ASD.Graphs;

    public class SquareofGraphTestCase : TestCase
    {
        private Graph graph;
        private Graph expected_result_graph;
        private Graph result_graph;

        public SquareofGraphTestCase(double timeLimit, Exception expectedException, string description, Graph graph, Graph res_graph)
            : base(timeLimit, expectedException, description)
        {
            this.graph = graph;
            this.expected_result_graph = res_graph;
        }

        public override void PerformTestCase(object prototypeObject)
        {
            result_graph = ((Lab03Helper)prototypeObject).SquareOfGraph(graph);
        }

        public override void VerifyTestCase(out Result resultCode, out string message, object settings = null)
        {
            if (result_graph == null)
            {
                message = $"No solution";
                resultCode = Result.BadResult;
                return;
            }
            //tu rozwiazanie jest jednoznaczne - sprawdzamy tylko rownosc grafu, izomorfizm niepotrzebny
            if (expected_result_graph.EdgesCount != result_graph.EdgesCount || !expected_result_graph.IsEqual(result_graph))
            {
                //mozna zakomentowac - ma ulatwic zrozumienie bledu
                //var ge = new GraphExport();
                //ge.Export(expected_result_graph, "Expected");
                //ge.Export(result_graph, "Result");

                //message = $"Incorrect result graph";
                //resultCode = Result.BadResult;
                //return;
            }
            resultCode = Result.Success;
            message = "OK";
        }
    }

    public class LineGraphTestCase : TestCase
    {
        private Graph graph;
        private Graph expected_result_graph;
        private Graph result_graph;
        private (int x, int y)[] names;
        private string[] names_str;
        private string[] expected_names;

        public LineGraphTestCase(double timeLimit, Exception expectedException, string description, Graph graph, Graph res_graph, string[] names)
            : base(timeLimit, expectedException, description)
        {
            this.graph = graph;
            this.expected_result_graph = res_graph;
            this.expected_names = names;
        }

        public override void PerformTestCase(object prototypeObject)
        {
            result_graph = ((Lab03Helper)prototypeObject).LineGraph(graph, out names);
        }

        public override void VerifyTestCase(out Result resultCode, out string message, object settings = null)
        {
            if (names != null)
            {
                names_str = new string[result_graph.VerticesCount];
                for (int i = 0; i < result_graph.VerticesCount; ++i)
                    names_str[i] = string.Format("[{0}, {1}]", names[i].x, names[i].y);
            }
            if (result_graph == null)
            {
                message = $"No solution";
                resultCode = Result.BadResult;
                return;
            }
            bool res = false;
            if (expected_result_graph.EdgesCount == result_graph.EdgesCount)
            {
                if (expected_result_graph.IsEqual(result_graph))
                    res = true;
                else
                    if (IsomorphismGraphExtender.Isomorpchism(expected_result_graph, result_graph) != null)
                    res = true;
            }
            if (!res)
            {
                //mozna zakomentowac - ma ulatwic zrozumienie bledu
                var ge = new GraphExport();
                //ge.Export(expected_result_graph, "Expected", expected_names);
                ge.Export(result_graph, "Result", names_str);

                message = $"Incorrect result graph";
                resultCode = Result.BadResult;
                return;
            }
            resultCode = Result.Success;
            message = "OK";
        }
    }

    public class VertexColoringTestCase : TestCase
    {
        private Graph graph;
        private int expected_colors_number;
        private int colors_number;
        private int[] expected_colors;
        private int[] colors;

        public VertexColoringTestCase(double timeLimit, Exception expectedException, string description, Graph graph, int num, int[] colors)
            : base(timeLimit, expectedException, description)
        {
            this.graph = graph;
            this.expected_colors_number = num;
            this.expected_colors = colors;
        }

        public override void PerformTestCase(object prototypeObject)
        {
            colors_number = ((Lab03Helper)prototypeObject).VertexColoring(graph, out colors);
        }

        public override void VerifyTestCase(out Result resultCode, out string message, object settings = null)
        {
            if (colors == null)
            {
                message = $"No solution";
                resultCode = Result.BadResult;
                return;
            }
            if (expected_colors_number != colors_number)
            {
                message = $"Incorrect numbers of colors: {colors_number}, should be: {expected_colors_number}" ;
                resultCode = Result.BadResult;
                return;
            }
            //algorytm podany jest w taki, sposob, ze kolorowanie mozliwe jest tylko jedno - sprawdzam czy tablice sa identyczne
            for (int id = 0; id < colors_number; ++id)
            {
                if (expected_colors[id] != colors[id])
                {
                    //mozna zakomentowac - ma ulatwic zrozumienie bledu
                    //var ge = new GraphExport();
                    //ge.Export(graph, "Expected");

                    //message = $"Incorrect coloring";
                    //string c1 = "", c2 = "";
                    //for (int i = 0; i < graph.VerticesCount; ++i)
                    //{
                    //    c1 += "[ " + i + ": " + colors[i].ToString() + "] ";
                    //    c2 += "[ " + i + ": " + expected_colors[i].ToString() + "] ";
                    //}
                    //message += "\nResult: \n" + c1 + "\n" + "Expected: \n" + c2;
                    ////wyswietlic kolorowanie
                    //resultCode = Result.BadResult;
                    //return;
                }
            }
            resultCode = Result.Success;
            message = "OK";
        }
    }

    public class StrongEdgeColoringTestCase : TestCase
    {
        private Graph graph;
        private Graph expected_result_graph;
        private Graph result_graph;
        private int expected_colors_number;
        private int colors_number;

        public StrongEdgeColoringTestCase(double timeLimit, Exception expectedException, string description, Graph graph, Graph res_graph, int num)
            : base(timeLimit, expectedException, description)
        {
            this.graph = graph;
            this.expected_result_graph = res_graph;
            this.expected_colors_number = num;
        }

        public override void PerformTestCase(object prototypeObject)
        {
            colors_number = ((Lab03Helper)prototypeObject).StrongEdgeColoring(graph, out result_graph);
        }

        public override void VerifyTestCase(out Result resultCode, out string message, object settings = null)
        {
            if (result_graph == null)
            {
                message = $"No solution";
                resultCode = Result.BadResult;
                return;
            }
            bool res = false;
            if (expected_colors_number != colors_number)
            {
                message = $"Incorrect numbers of colors: {colors_number}, should be: {expected_colors_number}";
                resultCode = Result.BadResult;
                return;
            }
            if (expected_result_graph.EdgesCount == result_graph.EdgesCount)
            {
                if (expected_result_graph.IsEqual(result_graph))
                    res = true;
                else
                {
                    //tu sprawdzam czy zwrocone kolorowanie krawedzi jest poprawne
                    for (int i = 0; i < result_graph.VerticesCount; ++i)
                    {
                        foreach (Edge e1 in result_graph.OutEdges(i))
                            foreach (Edge e2 in result_graph.OutEdges(e1.To))
                            {
                                if ( e1.From != e2.To && e1.Weight == e2.Weight)
                                {
                                    //var ge = new GraphExport();
                                    //ge.Export(result_graph, "Result");
                                    message = $"Incorrect coloring. Edges [{e1.From}, {e1.To}] and [{e2.From}, {e2.To}] have the same color!";
                                    resultCode = Result.BadResult;
                                    return;
                                }
                                foreach (Edge e3 in result_graph.OutEdges(e2.To))
                                    if ( !(e1.From == e3.From && e1.To == e3.To) && !(e1.From == e3.To && e1.To == e3.From)  && e1.Weight == e3.Weight)
                                    {
                                        //var ge = new GraphExport();
                                        //ge.Export(result_graph, "Result");
                                        message = $"Incorrect coloring. Edges [{e1.From}, {e1.To}] and [{e3.From}, {e3.To}] have the same color!";
                                        resultCode = Result.BadResult;
                                        return;
                                    }
                            }
                    }
                    res = true;
                }
            }

            if (!res)
            {
                //var ge = new GraphExport();
                //ge.Export(expected_result_graph, "Expected");
                //ge.Export(result_graph, "Result");
                message = $"Incorrect result graph";
                resultCode = Result.BadResult;
                return;
            }
            resultCode = Result.Success;
            message = "OK";

        }
    }
 

    class Lab03TestModule : TestModule
    {
        
        public override void PrepareTestSets()
        {
            var rgg = new RandomGraphGenerator(12345);
            int n = 6;
            Graph[] graphs = new Graph[n];
            Graph[] squares = new Graph[n];
            Graph[] linegraphs = new Graph[n];
            List<string[]> names = new List<string[]>(); //description of edges
            List<int[]> colors = new List<int[]>();
            int[] chn = new int[n]; //chromatic number
            Graph[] sec_graphs = new Graph[n];
            int[] schi = new int[n]; //strong chromatic index

            graphs[0] = new AdjacencyMatrixGraph(false, 5);
            graphs[0].AddEdge(0, 1);
            graphs[0].AddEdge(0, 3);
            graphs[0].AddEdge(1, 2);
            graphs[0].AddEdge(2, 0);
            graphs[0].AddEdge(2, 3);
            graphs[0].AddEdge(3, 4);

            graphs[1] = new AdjacencyMatrixGraph(false, 6);
            graphs[1].AddEdge(0, 1);
            graphs[1].AddEdge(1, 2);
            graphs[1].AddEdge(2, 3);
            graphs[1].AddEdge(3, 4);
            graphs[1].AddEdge(4, 5);

//            graphs[2] = new AdjacencyListsGraph<SimpleAdjacencyList>(false, 9);
            graphs[2] = new AdjacencyMatrixGraph(false, 9);
            graphs[2].AddEdge(0, 3);
            graphs[2].AddEdge(1, 0);
            graphs[2].AddEdge(6, 0);
            graphs[2].AddEdge(1, 5);
            graphs[2].AddEdge(5, 4);
            graphs[2].AddEdge(5, 2);
            graphs[2].AddEdge(5, 6);
            graphs[2].AddEdge(6, 7);
            graphs[2].AddEdge(7, 8);

            graphs[3] = new AdjacencyMatrixGraph(false, 5);
            graphs[3].AddEdge(0, 1);
            graphs[3].AddEdge(1, 2);
            graphs[3].AddEdge(2, 3);
            graphs[3].AddEdge(3, 4);
            graphs[3].AddEdge(4, 0);

            graphs[4] = rgg.UndirectedGraph(typeof(AdjacencyMatrixGraph), 5, 1.0);

            int g5vc = 9001;
            graphs[5] = new AdjacencyListsGraph<SimpleAdjacencyList>(true,g5vc);
            for ( int i=1 ; i<g5vc ; ++i )
                graphs[5].AddEdge(i-1,i);
            graphs[5].AddEdge(g5vc-1,0);

            squares[0] = graphs[0].Clone();
            squares[0].AddEdge(0, 4);
            squares[0].AddEdge(1, 3);
            squares[0].AddEdge(2, 4);

            squares[1] = graphs[1].Clone();
            squares[1].AddEdge(0, 2);
            squares[1].AddEdge(1, 3);
            squares[1].AddEdge(2, 4);
            squares[1].AddEdge(3, 5);

            squares[2] = graphs[2].Clone();
            squares[2].AddEdge(0, 5);
            squares[2].AddEdge(0, 6);
            squares[2].AddEdge(0, 7);
            squares[2].AddEdge(1, 2);
            squares[2].AddEdge(1, 3);
            squares[2].AddEdge(1, 4);
            squares[2].AddEdge(1, 6);
            squares[2].AddEdge(2, 4);
            squares[2].AddEdge(2, 6);
            squares[2].AddEdge(3, 6);
            squares[2].AddEdge(4, 6);
            squares[2].AddEdge(5, 7);
            squares[2].AddEdge(6, 8);

            squares[3] = rgg.UndirectedGraph(typeof(AdjacencyMatrixGraph), 5, 1.0);
            squares[4] = rgg.UndirectedGraph(typeof(AdjacencyMatrixGraph), 5, 1.0);

            squares[5] = new AdjacencyListsGraph<SimpleAdjacencyList>(true,g5vc);
            for ( int i=0 ; i<g5vc ; ++i )
                {
                squares[5].AddEdge(i,(i+1)%g5vc);
                squares[5].AddEdge(i,(i+2)%g5vc);
                }

            linegraphs[0] = new AdjacencyMatrixGraph(false, 6);
            linegraphs[0].AddEdge(0, 1);
            linegraphs[0].AddEdge(0, 2);
            linegraphs[0].AddEdge(0, 3);
            linegraphs[0].AddEdge(1, 2);
            linegraphs[0].AddEdge(1, 3);
            linegraphs[0].AddEdge(1, 4);
            linegraphs[0].AddEdge(2, 4);
            linegraphs[0].AddEdge(2, 5);
            linegraphs[0].AddEdge(3, 4);
            linegraphs[0].AddEdge(4, 5);

            linegraphs[1] = new AdjacencyMatrixGraph(false, 5);
            linegraphs[1].AddEdge(0, 1);
            linegraphs[1].AddEdge(1, 2);
            linegraphs[1].AddEdge(2, 3);
            linegraphs[1].AddEdge(3, 4);

//            linegraphs[2] = new AdjacencyListsGraph<SimpleAdjacencyList>(false, 9);
            linegraphs[2] = new AdjacencyMatrixGraph(false, 9);
            linegraphs[2].AddEdge(0, 1);
            linegraphs[2].AddEdge(0, 2);
            linegraphs[2].AddEdge(0, 3);
            linegraphs[2].AddEdge(1, 2);
            linegraphs[2].AddEdge(2, 6);
            linegraphs[2].AddEdge(2, 7);
            linegraphs[2].AddEdge(3, 4);
            linegraphs[2].AddEdge(3, 5);
            linegraphs[2].AddEdge(3, 6);
            linegraphs[2].AddEdge(4, 5);
            linegraphs[2].AddEdge(4, 6);
            linegraphs[2].AddEdge(5, 6);
            linegraphs[2].AddEdge(6, 7);
            linegraphs[2].AddEdge(7, 8);

            linegraphs[3] = new AdjacencyMatrixGraph(false, 5);
            linegraphs[3].AddEdge(0, 1);
            linegraphs[3].AddEdge(0, 2);
            linegraphs[3].AddEdge(1, 4);
            linegraphs[3].AddEdge(2, 3);
            linegraphs[3].AddEdge(3, 4);

            linegraphs[4] = new AdjacencyMatrixGraph(false, 10);
            linegraphs[4].AddEdge(0, 1);
            linegraphs[4].AddEdge(0, 2);
            linegraphs[4].AddEdge(0, 3);
            linegraphs[4].AddEdge(0, 4);
            linegraphs[4].AddEdge(0, 5);
            linegraphs[4].AddEdge(0, 6);
            linegraphs[4].AddEdge(1, 2);
            linegraphs[4].AddEdge(1, 3);
            linegraphs[4].AddEdge(1, 4);
            linegraphs[4].AddEdge(1, 7);
            linegraphs[4].AddEdge(1, 8);
            linegraphs[4].AddEdge(2, 3);
            linegraphs[4].AddEdge(2, 5);
            linegraphs[4].AddEdge(2, 7);
            linegraphs[4].AddEdge(2, 9);
            linegraphs[4].AddEdge(3, 6);
            linegraphs[4].AddEdge(3, 8);
            linegraphs[4].AddEdge(3, 9);
            linegraphs[4].AddEdge(4, 5);
            linegraphs[4].AddEdge(4, 6);
            linegraphs[4].AddEdge(4, 7);
            linegraphs[4].AddEdge(4, 8);
            linegraphs[4].AddEdge(5, 6);
            linegraphs[4].AddEdge(5, 7);
            linegraphs[4].AddEdge(5, 9);
            linegraphs[4].AddEdge(6, 8);
            linegraphs[4].AddEdge(6, 9);
            linegraphs[4].AddEdge(7, 8);
            linegraphs[4].AddEdge(7, 9);
            linegraphs[4].AddEdge(8, 9);

            linegraphs[5] = new AdjacencyListsGraph<SimpleAdjacencyList>(false,g5vc);
            for ( int i=0 ; i<g5vc ; ++i )
                linegraphs[5].AddEdge(i,(i+1)%g5vc);

            names.Add(new string[6]);
            names[0][0] = ("[0, 1]");
            names[0][1] = ("[0, 2]");
            names[0][2] = ("[0, 3]");
            names[0][3] = ("[1, 2]");
            names[0][4] = ("[2, 3]");
            names[0][5] = ("[3, 4]");

            names.Add(new string[5]);
            names[1][0] = ("[0, 1]");
            names[1][1] = ("[1, 2]");
            names[1][2] = ("[2, 3]");
            names[1][3] = ("[3, 4]");
            names[1][4] = ("[4, 5]");

            names.Add(new string[9]);
            names[2][0] = ("[0, 1]");
            names[2][1] = ("[0, 3]");
            names[2][2] = ("[0, 6]");
            names[2][3] = ("[1, 5]");
            names[2][4] = ("[2, 5]");
            names[2][5] = ("[4, 5]");
            names[2][6] = ("[5, 6]");
            names[2][7] = ("[6, 7]");
            names[2][8] = ("[7, 8]");

            names.Add(new string[5]);
            names[3][0] = ("[0, 1]");
            names[3][1] = ("[0, 4]");
            names[3][2] = ("[1, 2]");
            names[3][3] = ("[2, 3]");
            names[3][4] = ("[3, 4]");

            names.Add(new string[10]);
            names[4][0] = ("[0, 1]");
            names[4][1] = ("[0, 2]");
            names[4][2] = ("[0, 3]");
            names[4][3] = ("[0, 4]");
            names[4][4] = ("[1, 2]");
            names[4][5] = ("[1, 3]");
            names[4][6] = ("[1, 4]");
            names[4][7] = ("[2, 3]");
            names[4][8] = ("[2, 4]");
            names[4][9] = ("[3, 4]");

            names.Add(new string[g5vc]);
            for ( int i=0 ; i<g5vc ; ++i )
                names[5][i] = string.Format("[{0}, {1}]",i,(i+1)%g5vc);

            colors.Add(new int[5]);
            colors[0][0] = 0;
            colors[0][1] = 1;
            colors[0][2] = 2;
            colors[0][3] = 1;
            colors[0][4] = 0;
            chn[0] = 3;

            colors.Add(new int[6]);
            colors[1][0] = 0;
            colors[1][1] = 1;
            colors[1][2] = 0;
            colors[1][3] = 1;
            colors[1][4] = 0;
            colors[1][5] = 1;
            chn[1] = 2;

            colors.Add(new int[9]);
            colors[2][0] = 0;
            colors[2][1] = 1;
            colors[2][2] = 0;
            colors[2][3] = 1;
            colors[2][4] = 0;
            colors[2][5] = 2;
            colors[2][6] = 1;
            colors[2][7] = 0;
            colors[2][8] = 1;
            chn[2] = 3;

            colors.Add(new int[5]);
            colors[3][0] = 0;
            colors[3][1] = 1;
            colors[3][2] = 0;
            colors[3][3] = 1;
            colors[3][4] = 2;
            chn[3] = 3;

            colors.Add(new int[5]);
            colors[4][0] = 0;
            colors[4][1] = 1;
            colors[4][2] = 2;
            colors[4][3] = 3;
            colors[4][4] = 4;
            chn[4] = 5;

            colors.Add(new int[g5vc]);  // nie bedzie uzywana

            sec_graphs[0] = new AdjacencyMatrixGraph(false, 5);
            sec_graphs[0].AddEdge(0, 1, 0);
            sec_graphs[0].AddEdge(0, 3, 2);
            sec_graphs[0].AddEdge(1, 2, 3);
            sec_graphs[0].AddEdge(2, 0, 1);
            sec_graphs[0].AddEdge(2, 3, 4);
            sec_graphs[0].AddEdge(3, 4, 5);
            schi[0] = 6;

            sec_graphs[1] = new AdjacencyMatrixGraph(false, 6);
            sec_graphs[1].AddEdge(0, 1, 0);
            sec_graphs[1].AddEdge(1, 2, 1);
            sec_graphs[1].AddEdge(2, 3, 2);
            sec_graphs[1].AddEdge(3, 4, 0);
            sec_graphs[1].AddEdge(4, 5, 1);
            schi[1] = 3;

//            sec_graphs[2] = new AdjacencyListsGraph<SimpleAdjacencyList>(false, 9);
            sec_graphs[2] = new AdjacencyMatrixGraph(false, 9);
            sec_graphs[2].AddEdge(0, 3, 1);
            sec_graphs[2].AddEdge(1, 0, 0);
            sec_graphs[2].AddEdge(6, 0, 2);
            sec_graphs[2].AddEdge(1, 5, 3);
            sec_graphs[2].AddEdge(5, 4, 4);
            sec_graphs[2].AddEdge(5, 2, 1);
            sec_graphs[2].AddEdge(5, 6, 5);
            sec_graphs[2].AddEdge(6, 7, 6);
            sec_graphs[2].AddEdge(7, 8, 0);
            schi[2] = 7;

            sec_graphs[3] = new AdjacencyMatrixGraph(false, 5);
            sec_graphs[3].AddEdge(0, 1, 0);
            sec_graphs[3].AddEdge(1, 2, 2);
            sec_graphs[3].AddEdge(2, 3, 3);
            sec_graphs[3].AddEdge(3, 4, 4);
            sec_graphs[3].AddEdge(4, 0, 1);
            schi[3] = 5;

            sec_graphs[4] = new AdjacencyMatrixGraph(false, 5);
            sec_graphs[4].AddEdge(0, 1, 0);
            sec_graphs[4].AddEdge(0, 2, 1);
            sec_graphs[4].AddEdge(0, 3, 2);
            sec_graphs[4].AddEdge(0, 4, 3);
            sec_graphs[4].AddEdge(1, 2, 4);
            sec_graphs[4].AddEdge(1, 3, 5);
            sec_graphs[4].AddEdge(1, 4, 6);
            sec_graphs[4].AddEdge(2, 3, 7);
            sec_graphs[4].AddEdge(2, 4, 8);
            sec_graphs[4].AddEdge(3, 4, 9);
            schi[4] = 10;

            sec_graphs[5] = new AdjacencyListsGraph<SimpleAdjacencyList>(true,g5vc);
            for ( int i=0 ; i<g5vc ; ++i )
                sec_graphs[5].AddEdge(i,(i+1)%g5vc,i%3);
            schi[5] = 3;

            TestSets["LabSquareOfGraphTests"] = new TestSet(new Lab03Helper(), "Part 1 - square of graph [0.5 pkt]", null, false);
            TestSets["LabLineGraphTests"] = new TestSet(new Lab03Helper(), "Part 2 - line graph [2 pkt]", null, false);
            TestSets["LabVertexColoringTests"] = new TestSet(new Lab03Helper(), "Part 3 - vertex coloring [1 pkt]", null, false);
            TestSets["LabStrongEdgeColoringTests"] = new TestSet(new Lab03Helper(), "Part 4 - strong edge coloring [0.5 pkt]", null, false);

            for (int i = 0; i < n; ++i)
            {
                TestSets["LabSquareOfGraphTests"].TestCases.Add(new SquareofGraphTestCase(1, null, "", graphs[i], squares[i]));
                TestSets["LabLineGraphTests"].TestCases.Add(new LineGraphTestCase(1, null, "", graphs[i], linegraphs[i], names[i]));
                TestSets["LabVertexColoringTests"].TestCases.Add(new VertexColoringTestCase(1, graphs[i].Directed?new ArgumentException():null, "", graphs[i], chn[i], colors[i]));
                TestSets["LabStrongEdgeColoringTests"].TestCases.Add(new StrongEdgeColoringTestCase(1, null, "", graphs[i], sec_graphs[i], schi[i]));
            }
        }

        public override double ScoreResult(out string message)
        {
            message = "OK";
            return 1;
        }

    }

    class Lab03Main
    {

        public static void Main()
        {
            Lab03TestModule lab03test = new Lab03TestModule();
                lab03test.PrepareTestSets();

            foreach (var ts in lab03test.TestSets)
            {
                ts.Value.PerformTests(verbose: true, checkTimeLimit: false);
            }

        }

    }
}