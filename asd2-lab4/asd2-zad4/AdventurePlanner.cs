using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASD
{
    /// <summary>
    /// struktura przechowująca punkt
    /// </summary>
    [Serializable]
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point (int x, int y)
        {
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }

    public class AdventurePlanner: MarshalByRefObject
    {
        /// <summary>
        /// największy rozmiar tablicy, którą wyświetlamy
        /// ustaw na 0, żeby nic nie wyświetlać
        /// </summary>
        public int MaxToShow = 0;

      
        /// <summary>
        /// Znajduje optymalną pod względem liczby znalezionych skarbów ścieżkę,
        /// zaczynającą się w lewym górnym rogu mapy (0,0), a kończącą się w prawym
        /// dolnym rogu (X-Y-1).
        /// Za każdym razem możemy wykonać albo krok w prawo albo krok w dół.
        /// Pierwszym polem ścieżki powinno być (0,0), a ostatnim polem (X-1,Y-1).        
        /// </summary>
        /// <param name="treasure">liczba znalezionych skarbów</param>
        /// <param name="path">znaleziona ścieżka</param>
        /// <remarks>
        /// Złożoność rozwiązania to O(X * Y).
        /// </remarks>
        /// <returns></returns>
        public int FindPathThere(int[,] treasure, out List<Point> path)
        {
            int x = treasure.GetLength(0);
            int y = treasure.GetLength(1);

            path = new List<Point>();

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    int curVal = treasure[i, j];
                    if (i == 0 && j == 0) //Domek
                    {
                        continue;
                    }
                    else if (i == 0) //Sumujemy tylko z gory
                    {
                        int up = curVal + treasure[i, j - 1];
                        treasure[i, j] = Math.Max(curVal, up);
                    }
                    else if (j == 0) //Sumujemy tylko z lewa
                    {
                        int left = curVal + treasure[i - 1, j];
                        treasure[i, j] = Math.Max(curVal, left);
                    }
                    else //Sumujemy z lewa i z gory
                    {
                        int left = (x - 1) == 0 ? 0 : curVal + treasure[i - 1, j];
                        int up = (y - 1) == 0 ? 0 : curVal + treasure[i, j - 1];
                        treasure[i, j] = Math.Max(left, up);
                    }
                }
            }

            //GET PATH
            int a = x - 1;
            int b = y - 1;
            path.Add(new Point(x - 1, y - 1)); //Add ending point
            
            if ((x+y)-3 <= 0)
                return treasure[x - 1, y - 1];

            for (int len = 0; len < (x + y) - 2; len++)
            {
                int left = (a - 1 >= 0 && b>=0) ? treasure[a - 1, b] : -1;
                int up = (b - 1 >= 0 && a>=0) ? treasure[a, b - 1] : -1;

                bool isleft = left >= up;
                if (isleft)
                {
                    a--;
                    path.Add(new Point(a, b));
                }
                else
                {
                    b--;
                    path.Add(new Point(a, b));
                }
            }
            path.Reverse();
            return treasure[x-1,y-1];
        }

        /// <summary>
        /// Znajduje optymalną pod względem liczby znalezionych skarbów ścieżkę,
        /// zaczynającą się w lewym górnym rogu mapy (0,0), dochodzącą do prawego dolnego rogu (X-1,Y-1), a 
        /// następnie wracającą do lewego górnego rogu (0,0).
        /// W pierwszy etapie możemy wykonać albo krok w prawo albo krok w dół. Po osiągnięciu pola (x-1,Y-1)
        /// zacynamy wracać - teraz możemy wykonywać algo krok w prawo albo krok w górę.
        /// Pierwszym i ostatnim polem ścieżki powinno być (0,0).
        /// Możemy założyć, że X,Y >= 2.
        /// </summary>
        /// <param name="treasure">liczba znalezionych skarbów</param>
        /// <param name="path">znaleziona ścieżka</param>
        /// <remarks>
        /// Złożoność rozwiązania to O(X^2 * Y) lub O(X * Y^2).
        /// </remarks>
        /// <returns></returns>
        public int FindPathThereAndBack(int[,] treasure, out List<Point> path)
        {
            int x = treasure.GetLength(0);
            int y = treasure.GetLength(1);
            path = null;
            if (x == 1 && y == 1)
                return treasure[x - 1, y - 1];

            int[,,] v = new int[x, y, y];

            bool poX = true; //dluzszy bok - po nim (jego przekatnych) bedziemy iterowac

            if (y > x)
            {
                poX = false;
            }

            //ilosc przekatnych

            int it = x + y - 1;

            v[0, 0, 0] = treasure[0, 0];
            //Console.WriteLine("PRZYPADEK: " + x + " x " + y);
            //Console.WriteLine("Czy iteruje po X:" + poX);
            //Console.WriteLine("Czy y > x: " + (y > x));
            int secit = x;
            if (!poX)
                secit = y;
            for (int i = 0; i < x; i++) //iteruje po przekatnych (od 2, bo pierwsza jest jednoelementowa i do przedostatniej, bo ostatnia jest jednoelementowa)
            {
                for(int j = 0; j < y; j++) //pkt startowy
                {
                    for (int k = 0; k < y; k++) //pozostale pkt
                    {
                        if (x == 0 && y == 0)
                        {
                            continue;
                        }
                        else if (x == 0)
                        {

                        }
                        else if (y == 0)
                        {

                        }
                        else
                        {
                            int l = i + j - k;
                            //We're coming to these points from Left Left
                            int leftleft = i >= 1 ? treasure[i - 1, j] + treasure[l, k] : -1;
                            //We're coming to these points from Left Up
                            int leftup = k >= 1 ? treasure[i, j] + treasure[l, k - 1] : -1;
                            //We're coming to these points from Up Up
                            int upup = j >= 1 && k >= 1 ? treasure[i, j - 1] + treasure[l, k - 1] : -1;
                            //We're coming to these points from Up Left
                            int upleft = j >= 1 ? treasure[i, j - 1] + treasure[l, k] : -1;


                            int choice = Math.Max(Math.Max(leftleft, leftup), Math.Max(upup, upleft));
                            v[i, j, k] = choice;
                        }

                        //v[i, j, k] = choice;
                    }
                }
            }

            v[x - 1, y - 1, y - 1] = v[x - 2, y - 1, y - 2] + treasure[x - 1, y - 1]; //Dodaj ostatni pkt
            path = null;
            return v[x - 1, y - 1, y - 1];
        }
    }
}
