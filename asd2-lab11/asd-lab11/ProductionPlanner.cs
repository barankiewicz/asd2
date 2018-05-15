using System;
using System.Linq;
using ASD.Graphs;

namespace ASD
{
    public class ProductionPlanner : MarshalByRefObject
    {
        /// <summary>
        /// Flaga pozwalająca na włączenie wypisywania szczegółów skonstruowanego planu na konsolę.
        /// Wartość <code>true</code> spoeoduje wypisanie planu.
        /// </summary>
        public bool ShowDebug { get; } = false;
        
        /// <summary>
        /// Część 1. zadania - zaplanowanie produkcji telewizorów dla pojedynczego kontrahenta.
        /// </summary>
        /// <remarks>
        /// Do przeprowadzenia testów wyznaczających maksymalną produkcję i zysk wymagane jest jedynie zwrócenie obiektu <see cref="PlanData"/>.
        /// Testy weryfikujące plan wymagają przypisania tablicy z planem do parametru wyjściowego <see cref="weeklyPlan"/>.
        /// </remarks>
        /// <param name="production">
        /// Tablica obiektów zawierających informacje o produkcji fabryki w kolejnych tygodniach.
        /// Wartości pola <see cref="PlanData.Quantity"/> oznaczają limit produkcji w danym tygodniu,
        /// a pola <see cref="PlanData.Value"/> - koszt produkcji jednej sztuki.
        /// </param>
        /// <param name="sales">
        /// Tablica obiektów zawierających informacje o sprzedaży w kolejnych tygodniach.
        /// Wartości pola <see cref="PlanData.Quantity"/> oznaczają maksymalną sprzedaż w danym tygodniu,
        /// a pola <see cref="PlanData.Value"/> - cenę sprzedaży jednej sztuki.
        /// </param>
        /// <param name="storageInfo">
        /// Obiekt zawierający informacje o magazynie.
        /// Wartość pola <see cref="PlanData.Quantity"/> oznacza pojemność magazynu,
        /// a pola <see cref="PlanData.Value"/> - koszt przechowania jednego telewizora w magazynie przez jeden tydzień.
        /// </param>
        /// <param name="weeklyPlan">
        /// Parametr wyjściowy, przez który powinien zostać zwrócony szczegółowy plan sprzedaży.
        /// </param>
        /// <returns>
        /// Obiekt <see cref="PlanData"/> opisujący wyznaczony plan.
        /// W polu <see cref="PlanData.Quantity"/> powinna znaleźć się maksymalna liczba wyprodukowanych telewizorów,
        /// a w polu <see cref="PlanData.Value"/> - wyznaczony maksymalny zysk fabryki.
        /// </returns>
        public PlanData CreateSimplePlan(PlanData[] production, PlanData[] sales, PlanData storageInfo,
            out SimpleWeeklyPlan[] weeklyPlan)
        {
            //Create graphs for Capacities and Costs
            Graph Capacities = new AdjacencyListsGraph<AVLAdjacencyList>(true, production.Length + 2);
            Graph Costs = new AdjacencyListsGraph<AVLAdjacencyList>(true, production.Length + 2);

            //Hard-wire IDs special vertices
            int source = production.Length;
            int target = production.Length + 1;

            //Populate graphs with edges
            for (int i = 0; i < production.Length; i++)
            {
                //For Capacities
                Capacities.AddEdge(source, i, production[i].Quantity);
                Capacities.AddEdge(i, target, sales[i].Quantity);

                //For Costs
                Costs.AddEdge(source, i, production[i].Value);
                Costs.AddEdge(i, target, -sales[i].Value);

                //Edge for magazine purposes
                if (i < production.Length - 1)
                {
                    Capacities.AddEdge(i, i + 1, storageInfo.Quantity);
                    Costs.AddEdge(i, i + 1, storageInfo.Value);
                }
            }

            //Generate the flow
            (double value, double cost, Graph flow) = MinCostFlowGraphExtender.MinCostFlow(Capacities, Costs, source, target);

            //Construct weekly plan
            weeklyPlan = new SimpleWeeklyPlan[production.Length];
            for(int i = 0; i < production.Length; i++)
            {
                int unitsStored = (int)flow.GetEdgeWeight(i, i + 1);
                int unitsSold = (int)flow.GetEdgeWeight(i, target);
                int unitsProduced = (int)flow.GetEdgeWeight(source, i);

                weeklyPlan[i].UnitsStored = unitsStored > 0 ?(int)flow.GetEdgeWeight(i, i+1) : 0;
                weeklyPlan[i].UnitsSold = unitsSold > 0 ? (int)flow.GetEdgeWeight(i, target) : 0;
                weeklyPlan[i].UnitsProduced = unitsProduced > 0 ? (int)flow.GetEdgeWeight(source, i) : 0;
            }

            return new PlanData {Quantity = (int)value, Value = -cost};
        }

        /// <summary>
        /// Część 2. zadania - zaplanowanie produkcji telewizorów dla wielu kontrahentów.
        /// </summary>
        /// <remarks>
        /// Do przeprowadzenia testów wyznaczających produkcję dającą maksymalny zysk wymagane jest jedynie zwrócenie obiektu <see cref="PlanData"/>.
        /// Testy weryfikujące plan wymagają przypisania tablicy z planem do parametru wyjściowego <see cref="weeklyPlan"/>.
        /// </remarks>
        /// <param name="production">
        /// Tablica obiektów zawierających informacje o produkcji fabryki w kolejnych tygodniach.
        /// Wartość pola <see cref="PlanData.Quantity"/> oznacza limit produkcji w danym tygodniu,
        /// a pola <see cref="PlanData.Value"/> - koszt produkcji jednej sztuki.
        /// </param>
        /// <param name="sales">
        /// Dwuwymiarowa tablica obiektów zawierających informacje o sprzedaży w kolejnych tygodniach.
        /// Pierwszy wymiar tablicy jest równy liczbie kontrahentów, zaś drugi - liczbie tygodni w planie.
        /// Wartości pola <see cref="PlanData.Quantity"/> oznaczają maksymalną sprzedaż w danym tygodniu,
        /// a pola <see cref="PlanData.Value"/> - cenę sprzedaży jednej sztuki.
        /// Każdy wiersz tablicy odpowiada jednemu kontrachentowi.
        /// </param>
        /// <param name="storageInfo">
        /// Obiekt zawierający informacje o magazynie.
        /// Wartość pola <see cref="PlanData.Quantity"/> oznacza pojemność magazynu,
        /// a pola <see cref="PlanData.Value"/> - koszt przechowania jednego telewizora w magazynie przez jeden tydzień.
        /// </param>
        /// <param name="weeklyPlan">
        /// Parametr wyjściowy, przez który powinien zostać zwrócony szczegółowy plan sprzedaży.
        /// </param>
        /// <returns>
        /// Obiekt <see cref="PlanData"/> opisujący wyznaczony plan.
        /// W polu <see cref="PlanData.Quantity"/> powinna znaleźć się optymalna liczba wyprodukowanych telewizorów,
        /// a w polu <see cref="PlanData.Value"/> - wyznaczony maksymalny zysk fabryki.
        /// </returns>
        public PlanData CreateComplexPlan(PlanData[] production, PlanData[,] sales, PlanData storageInfo,
            out WeeklyPlan[] weeklyPlan)
        {
            int c = sales.GetLength(0); //Contrahents no
            int n = sales.GetLength(1); //Weeks no

            //Create graphs for Capacities and Costs
            Graph Capacities = new AdjacencyListsGraph<AVLAdjacencyList>(true, 2*n + c + 2);
            Graph Costs = new AdjacencyListsGraph<AVLAdjacencyList>(true, 2*n + c + 2);

            //Vertices explanation:
            //n for n weeks
            //n for n magazines (one state for each week)
            //c for c contrahents
            //plus 2 extra vertices for source and target

            //Vertices definitions:

            //Source and targer
            int source = 2 * n + c + 1;
            int target = 2 * n + c;

            //Week main vertices
            int weeksStart = 0;

            //Week magazine vertices
            int weekMagStart = n;

            //Contrahents vertices
            int contrStart = 2 * n;

            //Console.WriteLine("Wierzcholki:");
            //Console.WriteLine("Source: " + source);
            //Console.WriteLine("Target: " + target);
            //string s = "Magazyny: ";
            //for(int i = 0; i < n; i++)
            //    s += (weekMagStart + i).ToString() + ", ";
            //Console.WriteLine(s);

            //string s1 = "tygodnie: ";
            //for (int i = 0; i < n; i++)
            //    s1 += (weeksStart + i).ToString() + ", ";
            //Console.WriteLine(s1);

            //string s2 = "kontrahenci: ";
            //for (int i = 0; i < c; i++)
            //    s2 += (contrStart + i).ToString() + ", ";
            //Console.WriteLine(s2);


            //Populate edges going out of / going into week magazine vertices:
            for (int i = 0; i < n; i++)
            {
                int v = weekMagStart + i; //current magazine week vertex

                //from source to v
                Capacities.AddEdge(source, v, production[i].Quantity);
                Costs.AddEdge(source, v, -production[i].Value);

                //from v to target
                Capacities.AddEdge(v, target, production[i].Quantity);
                Costs.AddEdge(v, target, -production[i].Value);

                int mult = 0;
                //from v to week vertices
                for(int j = i; j < n; j++)
                {
                    int w = weeksStart + j; //current week vertex
                    if(mult == 0)
                        Capacities.AddEdge(v, w, production[i].Quantity);
                    else
                        Capacities.AddEdge(v, w, storageInfo.Quantity);

                    Costs.AddEdge(v, w, mult * storageInfo.Value);
                    mult++;
                }
            }

            //Populate edges going out of week vertices into contrahents' vertices
            for(int i = 0; i < n; i++)
            {
                int w = weeksStart + i; //Week vertex
                for (int j = 0; j < c; j++)
                {
                    int v = contrStart + j; //Contrahent vertex
                    Capacities.AddEdge(w, v, sales[j,i].Quantity);
                    Costs.AddEdge(w, v, -sales[j, i].Value);
                }
            }

            //Populate edges going from contrahents to sink (probably could be embedded into one of the 'main' previous loops, just testing for now
            for(int i = 0; i < c; i++)
            {
                int v = contrStart + i;
                Capacities.AddEdge(v, target, int.MaxValue);
                Costs.AddEdge(v, target, 0);
            }

            //Generate the flow
            (double value, double cost, Graph flow) = MinCostFlowGraphExtender.MinCostFlow(Capacities, Costs, source, target);


            //Calculate quantity produced
            weeklyPlan = new WeeklyPlan[n];
            int quantity = 0;
            double profit = 0;
            for(int i = 0; i < n; i++)
            {
                int w = weekMagStart + i;
                int maxProd = production[i].Quantity;
                int actualProd = (int)flow.GetEdgeWeight(w, target);

                int unitsMagazined;
                if (i <= n - 1)
                    unitsMagazined = (int)flow.GetEdgeWeight(w, i + 1) > 0 ? (int)flow.GetEdgeWeight(w, i + 1) : 0;
                else
                    unitsMagazined = 0;

                double weeklyProdCost = production[i].Value;

                profit -= unitsMagazined * storageInfo.Value;
                profit -= (maxProd - actualProd) * weeklyProdCost;
                weeklyPlan[i].UnitsStored = unitsMagazined;
                weeklyPlan[i].UnitsProduced = maxProd - actualProd;
                weeklyPlan[i].UnitsSold = new int[c];
                for (int j = 0; j < c; j++)
                {
                    int v = contrStart + j;
                    int soldAmount = (int)flow.GetEdgeWeight(i, v);
                    double sellingPrice = sales[j, i].Value;
                    profit += soldAmount * sellingPrice;
                    weeklyPlan[i].UnitsSold[j] = soldAmount;
                }
                quantity += maxProd - actualProd;
            }
            GraphExport ge = new GraphExport();
            GraphExport ge2 = new GraphExport();
            //ge.Export(Capacities);
            //ge2.Export(Costs);
            ge.Export(flow);
            return new PlanData { Quantity = quantity, Value = profit };
        }

    }
}