
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ASD.Graphs
{

    public class TSPHelper : System.MarshalByRefObject
    {
        Graph g;
        int n; //Liczba wierzcholkow grafu
        double tweight; //koszt najlepszego rozwiazania
        
        int[] fwdptr; //krawedzie wprzod (w rozwiazaniu jeszcze niezakonczonym)
        int[] backptr; //krawedzie w tyl (w rozwiazaniu jeszcze niezakonczonym)
        int[] route; 
        int[] best; //najlepsze dotychczas znalezione rozwiazanie
        double[][] A; //macierz reprezentujaca graf
        double[][] A2; //macierz reprezentujaca graf
        int actRow = 0;
        int actCol = 0;
        int[] outDeg;
        int[] inDeg;

        (double[] rowValues, double[] colValues, double cost) Reduce(int[] row, int[] col, int edgesNo)
        {
            double cost = 0;
            double[] rowValues = new double[n];
            double[] colValues = new double[n];

            //po wierszach...
            for (int i = 0; i < n - edgesNo; ++i)
            {
                actRow = row[i];
                double temp = double.PositiveInfinity;
                for (int j = 0; j < n - edgesNo; j++) //znajdz pole w wierszu, ktore ma najmniejszy koszt
                {
                    actCol = col[j];

                    if (A[actRow][actCol] == 0) //jesli jest juz 0, to nie trzeba dalej szukac
                    {
                        temp = 0;
                        break;
                    }
                    else if (A[actRow][actCol] < temp)
                        temp = A[actRow][actCol];
                }

                if (temp > 0 && temp != double.PositiveInfinity)
                {
                    for (int j = 0; j < n - edgesNo; j++) //odejmujemy od kazdego pola w danym wierszu
                        A[actRow][col[j]] -= temp;
                    cost += temp;
                }
                rowValues[i] = temp;
            }
            //i po kolumnach
            for (int j = 0; j < n - edgesNo; ++j)
            {
                actCol = col[j];
                double temp = double.PositiveInfinity;
                for (int i = 0; i < n - edgesNo; i++) //znajdz pole w wierszu, ktore ma najmniejszy koszt
                {
                    actRow = row[i];
                    if (A[actRow][actCol] == 0)
                    {
                        temp = 0;
                        break;
                    }
                    else if (A[actRow][actCol] < temp)
                        temp = A[actRow][actCol];
                }

                if (temp > 0 && temp != double.PositiveInfinity)
                {
                    for (int i = 0; i < n - edgesNo; i++) //odejmujemy od kazdego pola w danym wierszu
                        A[row[i]][actCol] -= temp;
                    cost += temp;
                }
                colValues[j] = temp;
            }
            return (rowValues, colValues, cost);
        }

        public (int r, int c, double most) ChooseArc(int[] row, int[] col)
        {
            int r = -1;
            int c = -1;
            int actualRow;
            int actualCol;

            double actualVal;
            double[] rowValues = new double[n];
            double[] colValues = new double[n];

            for (int i = 0; i < n; i++)
            {
                rowValues[i] = -1;
                colValues[i] = -1;
            }

            double most = double.NegativeInfinity;
            for (int i = 0; i < n && row[i] != -1; i++)
                for (int j = 0; j < n && col[j] != -1; j++)
                    if (A[row[i]][col[j]] == 0)
                    {
                        int ttttRow = row[i];
                        int ttttCol = col[j];
                        double minR = double.PositiveInfinity;
                        double minC = double.PositiveInfinity;
                        //Wyznaczam najmniejszy element w wierszu i rozny od G[i,j]
                        if (rowValues[row[i]] == -1)
                        {
                            for (int k = 0; k < n && col[k] != -1; k++)
                            {
                                if(col[k] != col[j] && A[row[i]][col[k]] == 0)
                                {
                                    minR = 0;
                                    break;
                                }
                                actualRow = row[i];
                                actualCol = col[k];
                                actualVal = A[row[i]][col[k]];
                                if (A[row[i]][col[k]] < minR && col[k] != col[j])
                                    minR = A[row[i]][col[k]];
                            }
                            rowValues[row[i]] = minR;
                        }
                        else
                            minR = rowValues[row[i]]; //mamy juz value, to po co obliczac ponownie?

                        //Wyznaczam najmniejszy element w kolumnie j rozny od G[i,j]
                        if (colValues[col[j]] == -1)
                        {
                            for (int k = 0; k < n && row[k] != -1; k++)
                            {
                                actualRow = row[k];
                                actualCol = col[j];
                                actualVal = A[row[k]][col[j]];
                                if (A[row[k]][col[j]] == 0 && actualRow != ttttRow)
                                {
                                    minC = 0;
                                    break;
                                }
                                if (A[row[k]][col[j]] < minC && actualRow != ttttRow)
                                    minC = A[row[k]][col[j]];
                            }
                            colValues[col[j]] = minC;
                        }
                        else
                            minC = colValues[col[j]];

                        if (double.IsPositiveInfinity(minC))
                            minC = 0;
                        if (double.IsPositiveInfinity(minR))
                            minR = 0;

                        double total = minR + minC;
                        if (outDeg[row[i]] == 1 || inDeg[col[j]] == 1)
                        {
                            most = total;
                            r = i;
                            c = j;
                            return (r, c, most);
                        }
                        if (total >= most)
                        {
                            most = total;
                            r = i;
                            c = j;
                        }
                    }
            return (r, c, most);
        }

        public int[] UpdateIndex(int[] arr, int index, int trimNo)
        {
            //Ta metoda sluzy do przesuwania tablicy wierszy i kolumn w celu zmniejszenia (logicznego) macierzy grafu
            int[] ret = new int[arr.Length];

            for (int i = 0; i < index; i++)
                ret[i] = arr[i];

            if(index != (arr.Length - 1))
                Array.Copy(arr, index + 1, ret, index, arr.Length - 1 - index);
            
            ret[arr.Length - 1] = -1;
            return ret;
        }

        public void Explore(int edgesNo, double cost, int[] rows, int[] columns)
        {
            //(double[] rowVals, double[] colVals, double reduceCost, HashSet<(int, int)> zeroes) = Reduce(A, rows, columns);
            (double[] rowVals, double[] colVals, double reduceCost) = Reduce(rows, columns, edgesNo);
            cost += reduceCost;

            if (cost < tweight)
            {
                if(edgesNo == n - 2) //Doprowadzilismy do macierzy 2x2, wiec wystarczy juz tylko dodac dwie pozostale krawedzie
                {
                    double leftUpper = A[rows[0]][columns[0]];
                    double leftLower = A[rows[0]][columns[1]];
                    double rightUpper = A[rows[1]][columns[0]];
                    double rightLower = A[rows[1]][columns[1]];

                    if ((double.IsInfinity(leftUpper) && double.IsInfinity(leftLower)) ||
                       (double.IsInfinity(leftUpper) && double.IsInfinity(rightUpper)) ||
                       (double.IsInfinity(leftLower) && double.IsInfinity(rightLower)) ||
                       (double.IsInfinity(rightUpper) && double.IsInfinity(rightLower))
                        )
                        return;

                    for (int i = 0; i < n; i++)
                    {
                        best[i] = fwdptr[i];
                        if (best[i] != -1 && double.IsNaN(g.GetEdgeWeight(i, best[i])))
                            return;
                    }
                        
                    if (double.IsPositiveInfinity(A[rows[0]][columns[0]]))
                    {
                        best[rows[0]] = columns[1];
                        best[rows[1]] = columns[0];
                    }
                    else
                    {
                        best[rows[0]] = columns[0];
                        best[rows[1]] = columns[1];
                    }
                    //sprawdzam, czy dana sciezka nie zwiodla mnie na manowce - czasami sa przypadki gdzie algorytm dodaje krawedz, ktorej nie powinna i nie da sie tego wykryc (a przynajmniej mi sie nie udalo)
                    if (double.IsNaN(g.GetEdgeWeight(rows[0], best[rows[0]])) || double.IsNaN(g.GetEdgeWeight(rows[1], best[rows[1]])))
                        return;

                    tweight = cost;
                        
                }
                else //Macierz jest wieksza niz 2x2 - trzeba rekurowac dalej
                {
                    (int r, int c, double most) = ChooseArc(rows, columns); //wybierz najlepszy łuk do podziału
                    if (r == -1) //w przypadku grafow rzadkich czasami nie ma juz krawedzi do dalszej pracy
                        return;
                    int actualRow = rows[r];
                    int actualCol = columns[c];
                    double lowerBound = cost + most; //nie czuje gdy rymuje

                    fwdptr[rows[r]] = columns[c];
                    backptr[columns[c]] = rows[r];

                    //ZABEZPIECZYC PRZED CYKLEM!!!! 
                    int last = columns[c];
                    while (fwdptr[last] != -1)
                        last = fwdptr[last];
                    int first = rows[r];
                    while (backptr[first] != -1)
                        first = backptr[first];

                    double RowColumnValue = A[last][first];
                    A[last][first] = double.PositiveInfinity;

                    outDeg[actualCol]--;

                    //Zmniejsz macierz
                    int[] newRows = UpdateIndex(rows, r, edgesNo);
                    int[] newCols = UpdateIndex(columns, c, edgesNo);

                    Explore(edgesNo + 1, cost, newRows, newCols); //lewe drzewo

                    outDeg[actualCol]++;
                    A[last][first] = RowColumnValue;
                    backptr[columns[c]] = -1;
                    fwdptr[rows[r]] = -1;
                    if (lowerBound < tweight && outDeg[actualRow] != 1 && inDeg[actualCol] != 1) //Prawe poddrzewo o potencjalnie mniejszym koszcie niz znalezione idac ciagle na lewo
                    {
                        A[rows[r]][columns[c]] = double.PositiveInfinity;
                        Explore(edgesNo, cost, rows, columns); //Po zejsciu na prawo, rekurujemy dalej
                        A[rows[r]][columns[c]] = 0;
                    }
                }
            }

            //Unreduce matrix
            if (reduceCost != 0)
                for (int i = 0; i < n && rows[i] != -1; i++)
                    for (int j = 0; j < n && columns[j] != -1; j++)
                        A[rows[i]][columns[j]] += rowVals[i] + colVals[j];
        }

        public double TSP(Graph g, out Edge[] cycle)
        {
            cycle = null;  // zmienić
            int cc;
            DFSGraphExtender.DFSearchAll(g, null, null, out cc, null);
            if (cc != 1) return double.NaN; // wiecej niz 1 spojna skladowa

            n = g.VerticesCount;
            this.g = g;

            A = new double[n][];
            for (int i = 0; i < n; i++)
                A[i] = new double[n];
            A = g.ToArray(); //zamien graf na macierz sasiedztwa
            A2 = g.ToArray();

            if(A == null)
                return double.NaN;

            //Zainicjuj potrzebne globalne tablice
            int [] rows = new int[n];
            int [] columns = new int[n];
            fwdptr = new int[n];
            backptr = new int[n];
            route = new int[n];
            best = new int[n];
            tweight = double.PositiveInfinity;
            outDeg = new int[n];
            inDeg = new int[n];
            for (int i = 0; i < n; i++)
            {
                rows[i] = i;
                columns[i] = i;
                backptr[i] = -1;
                fwdptr[i] = -1;
                outDeg[i] = g.OutDegree(i);
                inDeg[i] = g.InDegree(i);
            }

            Explore(0, 0, rows, columns);

            if (double.IsPositiveInfinity(tweight))
                return double.NaN;
            //Skonstruuj rozwiazanie
            Edge[] res = new Edge[n];
            int index = 0;
            for(int i = 0; i < n; i++)
            {
                route[i] = index;
                index = best[index];
                if(i != 0)
                {
                    res[i - 1] = new Edge(route[i - 1], route[i], A[route[i - 1]][route[i]]);
                    if (double.IsNaN(res[i - 1].Weight))
                        return double.NaN;
                }
            }
            res[n - 1] = new Edge(route[n - 1], route[0], A[route[n - 1]][route[0]]);
            cycle = res;
            return tweight;
        }

        public static void DoTest(Graph g, TSPHelper tsp)
        {
            double goodWeight = 0;
            Edge[] goodPath = new Edge[0];
            Stopwatch s1 = new Stopwatch();
            s1.Start();
            (goodWeight, goodPath) = g.BranchAndBoundTSP();
            s1.Stop();
            var elapsedMsBiblio = s1.ElapsedTicks;

            double myWeight = 0;
            Edge[] myPath = new Edge[0];
            Stopwatch s2 = new Stopwatch();
            s2.Start();
            myWeight = tsp.TSP(g, out myPath);
            s2.Stop();
            var elapsedMsMy = s2.ElapsedTicks;

            Console.WriteLine("Rozwiazanie biblioteczne: " + goodWeight);
            if (goodPath != null)
                foreach (Edge e in goodPath)
                    Console.Write("(" + e.From + ", " + e.To + "), ");
            Console.WriteLine();

            Console.WriteLine("Rozwiazanie moje: " + myWeight.ToString());
            
            if (myPath != null)
                foreach (Edge e in myPath)
                    Console.Write("(" + e.From + ", " + e.To + "), ");

            Console.WriteLine();
            Console.WriteLine("Czas biblio: " + elapsedMsBiblio.ToString());
            Console.WriteLine("Czas moj: " + elapsedMsMy.ToString());
            if ((double.IsNaN(goodWeight) && double.IsNaN(myWeight)) || goodWeight == myWeight)
                Console.WriteLine("OK");
            else
                Console.WriteLine("ZLE");

            Console.WriteLine();
        }

        public static void DrBrodkaTest(TSPHelper tsp)
        {
            Graph g = new AdjacencyMatrixGraph(true, 6);
            g.AddEdge(0, 1, 3);
            g.AddEdge(0, 2, 93);
            g.AddEdge(0, 3, 13);
            g.AddEdge(0, 4, 33);
            g.AddEdge(0, 5, 9);

            g.AddEdge(1, 0, 4);
            g.AddEdge(1, 2, 77);
            g.AddEdge(1, 3, 42);
            g.AddEdge(1, 4, 21);
            g.AddEdge(1, 5, 16);

            g.AddEdge(2, 0, 45);
            g.AddEdge(2, 1, 17);
            g.AddEdge(2, 3, 36);
            g.AddEdge(2, 4, 16);
            g.AddEdge(2, 5, 28);

            g.AddEdge(3, 0, 39);
            g.AddEdge(3, 1, 90);
            g.AddEdge(3, 2, 80);
            g.AddEdge(3, 4, 56);
            g.AddEdge(3, 5, 7);

            g.AddEdge(4, 0, 28);
            g.AddEdge(4, 1, 46);
            g.AddEdge(4, 2, 88);
            g.AddEdge(4, 3, 33);
            g.AddEdge(4, 5, 25);

            g.AddEdge(5, 0, 3);
            g.AddEdge(5, 1, 88);
            g.AddEdge(5, 2, 18);
            g.AddEdge(5, 3, 46);
            g.AddEdge(5, 4, 92);

            var watchBiblio = System.Diagnostics.Stopwatch.StartNew();
            (double goodWeight, Edge[] goodPath) = g.BranchAndBoundTSP();
            watchBiblio.Stop();
            var elapsedMsBiblio = watchBiblio.ElapsedTicks;

            Edge[] myPath;
            var myWatch = System.Diagnostics.Stopwatch.StartNew();
            double myWeight = tsp.TSP(g, out myPath);
            myWatch.Stop();
            var elapsedMsMy = myWatch.ElapsedTicks;

            Console.WriteLine("Rozwiazanie biblioteczne: " + goodWeight);
            if (goodPath != null)
                foreach (Edge e in goodPath)
                    Console.Write("(" + e.From + ", " + e.To + "), ");
            Console.WriteLine();

            Console.WriteLine("Rozwiazanie moje: " + myWeight.ToString());

            if (myPath != null)
                foreach (Edge e in myPath)
                    Console.Write("(" + e.From + ", " + e.To + "), ");


            Console.WriteLine("Czas biblio: " + elapsedMsBiblio.ToString());
            Console.WriteLine("Czas moj: " + elapsedMsMy.ToString());
            if ((double.IsNaN(goodWeight) && double.IsNaN(myWeight)) || goodWeight == myWeight)
                Console.WriteLine("OK");
            else
                Console.WriteLine("ZLE");

            Console.WriteLine();
        }

        public static void DoTests()
        {
            TSPHelper tsp = new TSPHelper();
            int N = 20; //ilosc testow
            int M = 30; //rozmiar testow
            int seed = 3377;
            RandomGraphGenerator rgg = new RandomGraphGenerator(seed);
            for (int i = 0; i < N; i++)
            {
                Graph g4 = rgg.UndirectedGraph(typeof(AdjacencyMatrixGraph), M, 0.3, 1, 100, true);
                //Graph g4 = rgg.UndirectedCycle(typeof(AdjacencyMatrixGraph), M, 1, 5, true);
                DoTest(g4, tsp);
            }
            //DrBrodkaTest(tsp);
        }

        static void Main(string[] args)
        {
            DoTests();
        }
    }

    public static class GraphExtender
    {
        public static double[][] ToArray(this Graph g)
        {
            int n = g.VerticesCount;
            double[][] ret = new double[n][];

            for (int i = 0; i < n; i++)
                ret[i] = new double[n];

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                    ret[i][j] = double.PositiveInfinity;
                int deg = 0; //stopien aktualnego wierzcholka
                foreach (Edge e in g.OutEdges(i))
                {
                    if (e.Weight < 0)
                        throw new ArgumentException(); //ujemna krawedz - zwracamy null
                    ret[e.From][e.To] = e.Weight;
                    deg++; //inkrementuj stopien
                }

                if (deg == 0)
                    return null; // istnieje wierzcholek stopnia 0
                ret[i][i] = double.PositiveInfinity;
            }
            return ret;
        }
    }
}

