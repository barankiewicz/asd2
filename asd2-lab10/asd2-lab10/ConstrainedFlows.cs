using ASD.Graphs;
using System;
using System.Collections.Generic;

namespace ASD
{
    public class ConstrainedFlows : System.MarshalByRefObject
    {
        // testy, dla których ma być generowany obrazek
        // graf w ostatnim teście ma bardzo dużo wierzchołków, więc lepiej go nie wyświetlać
        public static int[] circulationToDisplay = {  };
        public static int[] constrainedFlowToDisplay = { 1 };

        /// <summary>
        /// Metoda znajdująca cyrkulację w grafie, z określonymi żądaniami wierzchołków.
        /// Żądania opisane są w tablicy demands. Szukamy funkcji, która dla każdego wierzchołka będzie spełniała warunek:
        /// suma wartości na krawędziach wchodzących - suma wartości na krawędziach wychodzących = demands[v]
        /// </summary>
        /// <param name="G">Graf wejściowy, wagi krawędzi oznaczają przepustowości</param>
        /// <param name="demands">Żądania wierzchołków</param>
        /// <returns>Graf reprezentujący wynikową cyrkulację.
        /// Reprezentacja cyrkulacji jest analogiczna, jak reprezentacja przepływu w innych funkcjach w bibliotece.
        /// Należy zwrócić kopię grafu G, gdzie wagi krawędzi odpowiadają przepływom na tych krawędziach.
        /// Zwróć uwagę na rozróżnienie sytuacji, kiedy mamy zerowy przeływ na krawędzi (czyli istnieje
        /// krawędź z wagą 0) od sytuacji braku krawędzi.
        /// Jeśli żądana cyrkulacja nie istnieje, zwróć null.
        /// </returns>
        /// <remarks>
        /// Nie można modyfikować danych wejściowych!
        /// Złożoność metody powinna być asymptotycznie równa złożoności metody znajdującej największy przeływ (z biblioteki).
        /// </remarks>
        public Graph FindCirculation(Graph G, double[] demands)
        {
            Graph network = G.IsolatedVerticesGraph(true, G.VerticesCount + 2);

            //Check sanity
            double sumPositive = 0;
            double sumNegative = 0;
            for (int i = 0; i < demands.Length; i++)
                if (demands[i] >= 0)
                    sumPositive += demands[i];
                else
                    sumNegative += demands[i];
                
            if (sumPositive + sumNegative != 0)
                return null;

            //Add all edges to network
            for(int i = 0; i < G.VerticesCount; i++)
                foreach (Edge e in G.OutEdges(i))
                    network.AddEdge(e);

            int s = network.VerticesCount - 1; //starting vertex
            int t = network.VerticesCount - 2; //ending vertex

            //Add edges to ending vertex and from starting vertex
            for(int n = 0; n < network.VerticesCount - 2; n++)
                if (demands[n] < 0)
                    network.AddEdge(s, n, (-1) * demands[n]);
                else if (demands[n] > 0)
                    network.AddEdge(n, t, demands[n]);

            //Generate flow
            (double val, Graph flow) = network.PushRelabelMaxFlow(s, t);

            //Check if the flow is OK
            if (sumPositive != val)
                return null;

            //Flow generated, now generate a return graph without starting and ending vertices
            Graph ret = G.IsolatedVerticesGraph();
            for(int i = 0; i < flow.VerticesCount; i++)
                foreach(Edge e in flow.OutEdges(i))
                {
                    if (e.From == s || e.To == t)
                        continue;
                    ret.AddEdge(e);
                }
            return ret;
        }

        /// <summary>
        /// Funkcja zwracająca przepływ z ograniczeniami, czyli przepływ, który dla każdej z krawędzi
        /// ma wartość pomiędzy dolnym ograniczeniem a górnym ograniczeniem.
        /// Zwróć uwagę, że interesuje nas *jakikolwiek* przepływ spełniający te ograniczenia.
        /// </summary>
        /// <param name="source">źródło</param>
        /// <param name="sink">ujście</param>
        /// <param name="G">graf wejściowy, wagi krawędzi oznaczają przepustowości (górne ograniczenia)</param>
        /// <param name="lowerBounds">kopia grafu G, wagi krawędzi oznaczają dolne ograniczenia przepływu</param>
        /// <returns>Graf reprezentujący wynikowy przepływ (analogicznie do poprzedniej funkcji i do reprezentacji
        /// przepływu w funkcjach z biblioteki.
        /// Jeśli żądany przepływ nie istnieje, zwróć null.
        /// </returns>
        /// <remarks>
        /// Nie można modyfikować danych wejściowych!
        /// Złożoność metody powinna być asymptotycznie równa złożoności metody znajdującej największy przeływ (z biblioteki).
        /// </remarks>
        /// <hint>Wykorzystaj poprzednią część zadania.
        /// </hint>
        public Graph FindConstrainedFlow(int source, int sink, Graph G, Graph lowerBounds)
        {

            //Generate middle graph which has all capacities calculated as: G.GetEdgeWeight(e) - lowerBounds.GetEdgeWeight(e)
            Graph middle = G.Clone();
            Predicate<Edge> generateMiddle = delegate (Edge e)
            {
                double val = -lowerBounds.GetEdgeWeight(e.From, e.To);
                middle.ModifyEdgeWeight(e.From, e.To, val);
                return true;
            };
            middle.GeneralSearchFrom<EdgesStack>(0, null, null, generateMiddle);

            Graph inter = G.Clone();
            //Generate demands array based on lowerBounds graph
            double[] newDemands = new double[G.VerticesCount];
            double[] positiveDemands = new double[G.VerticesCount];
            double[] negativeDemands = new double[G.VerticesCount];
            List<int> fromSource = new List<int>();
            List<int> toSink = new List<int>();
            Predicate<Edge> ve = delegate (Edge e)
            {
                if (e.From == source)
                {
                    //newDemands[e.To] -= lowerBounds.GetEdgeWeight(source, e.To);
                    fromSource.Add(e.To);
                    return true;
                }

                if (e.To == sink)
                {
                    //newDemands[e.From] += lowerBounds.GetEdgeWeight(e.From, sink);
                    toSink.Add(e.From);
                    return true;
                }

                newDemands[e.From] += e.Weight;
                newDemands[e.To] -= e.Weight;
                return true;
            };
            lowerBounds.GeneralSearchFrom<EdgesStack>(0, null, null, ve);

            //Get source's demand
            foreach(int i in fromSource)
                if(newDemands[i]>0)
                    newDemands[source] -= newDemands[i];

            foreach (int i in toSink)
                if (newDemands[i] > 0)
                    newDemands[sink] += newDemands[i];

            newDemands[source] = -2;
            newDemands[sink] = 2;
            Graph helper = FindCirculation(middle, newDemands);

            //Add lower bounds to the graph

            
            //return G;
            return helper;
        }

    }
}