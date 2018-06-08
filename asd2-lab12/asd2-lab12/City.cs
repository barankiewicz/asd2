using System;
using System.Collections.Generic;
using ASD.Graphs;

namespace asd2
{
    public class City : MarshalByRefObject
    {
        /// <summary>
        /// Sprawdza przecięcie zadanych ulic-odcinków. Zwraca liczbę punktów wspólnych.
        /// </summary>
        /// <returns>0 - odcinki rozłączne, 
        /// 1 - dokładnie jeden punkt wspólny, 
        /// int.MaxValue - odcinki częściowo pokrywają się (więcej niż 1 punkt wspólny)</returns>

       public bool OnRectangle(Point q, Point p1, Point p2)
        {
            // pi=(xi,yi), q=(x,y)
            double x1 = p1.x;
            double x2 = p2.x;
            double y1 = p1.y;
            double y2 = p2.y;

            double x = q.x;
            double y = q.y;

            return Math.Min(x1, x2) <= x && x <= Math.Max(x1, x2) && Math.Min(y1, y2) <= y && y <= Math.Max(y1, y2);
        }

        public (double A, double B, double C) GetLine(Point p1, Point p2)
        {
            double a = 0;
            double b = 0;
            double c = 0;
            if (p1.x != p2.x) //nie sa dziwne
            {
                a = -(p1.y - p2.y) / (p1.x - p2.x);
                c = -(p1.y - (-a)*p1.x);
                b = 1;
            }
            else
            {
                b = 0;
                c = -p1.x;
                a = 1;
            }

            //Console.WriteLine("Prosta: {0}*x + {1}*y + {2}", a, b, c);
            return (a, b, c);
        }
        public int CheckIntersection(Street s1, Street s2)
        {
            Point p1 = s1.p1;
            Point p2 = s1.p2;
            Point p3 = s2.p1;
            Point p4 = s2.p2;

            double d1 = Point.CrossProduct((p4 - p3), (p1 - p3));
            double d2 = Point.CrossProduct((p4 - p3), (p2 - p3));
            double d3 = Point.CrossProduct((p2 - p1), (p3 - p1));
            double d4 = Point.CrossProduct((p2 - p1), (p4 - p1));

            double d12 = d1 * d2;
            double d34 = d3 * d4;

            if (d12 > 0 || d34 > 0)
                return 0;

            if (d12 < 0 || d34 < 0)
                return 1;

            if (d1 == 0 && d2 == 0 && d3 == 0 && d4==0) //sa wspolliniowe
            {
                if (!OnRectangle(p1, p3, p4) && !OnRectangle(p2, p3, p4) && !OnRectangle(p3, p1, p2) && !OnRectangle(p4, p1, p2)) //Nie maja czesci wspolnej
                    return 0;
                if ((p1 == p3 && !OnRectangle(p4, p1, p2) && !OnRectangle(p2, p3, p4)) ||
                    (p2 == p4 && !OnRectangle(p3, p1, p2) && !OnRectangle(p1, p3, p4)) ||
                    (p1 == p4 && !OnRectangle(p3, p1, p2) && !OnRectangle(p2, p3, p4)) ||
                    (p2 == p3 && !OnRectangle(p4, p1, p2) && !OnRectangle(p1, p3, p4))) //wspolne konce (i tylko konce)
                    return 1;
                else
                    return int.MaxValue;
            }
            return 1;
        }


        /// <summary>
        /// Sprawdza czy dla podanych par ulic możliwy jest przejazd między nimi (z użyciem być może innych ulic). 
        /// </summary>
        /// <returns>Lista, w której na i-tym miejscu jest informacja czy przejazd między ulicami w i-tej parze z wejścia jest możliwy</returns>
        public bool[] CheckStreetsPairs(Street[] streets, int[] streetsToCheck1, int[] streetsToCheck2)
        {
            int n = streetsToCheck1.Length;
            UnionFind union = new UnionFind(streets.Length);
            bool[] result = new bool[n];

            for(int i = 0; i < streets.Length; i++)
                for(int j = i + 1; j < streets.Length; j++)
                {
                    int points = CheckIntersection(streets[i], streets[j]);
                    if (points > 1)
                        throw new ArgumentException();
                    else if (points > 0)
                        union.Union(i, j);
                }

            for(int i = 0; i < n; i++)
                result[i] = (union.Find(streetsToCheck1[i]) == union.Find(streetsToCheck2[i]));

            return result;
        }


        /// <summary>
        /// Zwraca punkt przecięcia odcinków s1 i s2.
        /// W przypadku gdy nie ma jednoznacznego takiego punktu rzuć wyjątek ArgumentException
        /// </summary>
        public Point GetIntersectionPoint(Street s1, Street s2)
        {
            //znajdź współczynniki a i b prostych y=ax+b zawierających odcinki s1 i s2
            //uwaga na proste równoległe do osi y
            //uwaga na odcinki równoległe o wspólnych końcu
            //porównaj równania prostych, aby znaleźć ich punkt wspólny

            if (CheckIntersection(s1, s2) != 1)
                throw new ArgumentException();

            Point p1 = s1.p1;
            Point p2 = s1.p2;
            Point p3 = s2.p1;
            Point p4 = s2.p2;

            double x = 0;
            double y = 0;

            if (p1 == p3 || p1 == p4) // wspolny koniec
                return p1;

            if (p2 == p3 || p2 == p4)
                return p2;

            (double a1, double b1, double c1) = GetLine(p1, p2);
            (double a2, double b2, double c2) = GetLine(p3, p4);

            if(a1 != 0 && b1 != 0 && a2 != 0 && b2 != 0)
            {
                double W = a1 * b2 - a2 * b1;
                double Wx = (-c1) * b2 - (-c2) * b1;
                double Wy = a1 * (-c2) - a2 * (-c1);

                x = Wx / W;
                y = Wy / W;
            }
            else if((a1 == 0)&&(a2 != 0 && b2 != 0))
            {
                y = -c1;
                x = (-c2 - b2 * y) / a2;
            }
            else if ((a2 == 0) && (a1 != 0 && b1 != 0))
            {
                y = -c2;
                x = (-c1 - b1 * y) / a1;
            }
            else if ((b1 == 0) && (a2 != 0 && b2 != 0))
            {
                x = -c1;
                y = (-c2 - a2 * x) / b2;
            }
            else if ((b2 == 0) && (a1 != 0 && b1 != 0))
            {
                x = -c2;
                y = (-c1 - a1 * x) / b1;
            }
            else if ((a1 == 0) && (b2 == 0))
            {
                y = -c1;
                x = -c2;
            }
            else if ((a2 == 0) && (b1 == 0))
            {
                y = -c2;
                x = -c1;
            }

            return new Point(x, y);
        }


        /// <summary>
        /// Sprawdza możliwość przejazdu między dzielnicami-wielokątami district1 i district2,
        /// tzn. istnieją para ulic, pomiędzy którymi jest przejazd 
        /// oraz fragment jednej ulicy należy do obszaru jednej z dzielnic i fragment drugiej należy do obszaru drugiej dzielnicy
        /// </summary>
        /// <returns>Informacja czy istnieje przejazd między dzielnicami</returns>
        public bool CheckDistricts(Street[] streets, Point[] district1, Point[] district2, out List<int> path, out List<Point> intersections)
        {
            AdjacencyListsGraph<SimpleAdjacencyList> g = new AdjacencyListsGraph<SimpleAdjacencyList>(false, streets.Length + 2);
            path = new List<int>();
            intersections = new List<Point>();

            int s = streets.Length;
            int d1 = streets.Length + 1;
            int d2 = streets.Length;
            //Construct graph
            for (int i = 0; i < s; i++)
            {
                for(int j = 0; j < district1.Length; j++)
                {
                    Street temp = new Street(district1[j], district1[(j + 1)%district1.Length]);
                    if(CheckIntersection(temp, streets[i])==1)
                        g.AddEdge(i, d1);
                }

                for (int j = 0; j < district2.Length; j++)
                {
                    Street temp = new Street(district2[j], district2[(j + 1) % district2.Length]);
                    if (CheckIntersection(temp, streets[i]) == 1)
                        g.AddEdge(i, d2);
                }

                for(int j = i + 1; j < s; j++)
                {
                    if(CheckIntersection(streets[i], streets[j]) == 1)
                        g.AddEdge(i, j);
                }
            }

            PathsInfo[] d = new PathsInfo[s + 2];
            g.DijkstraShortestPaths(d1, out d);
            if (double.IsNaN(d[d2].Dist))
                return false;

            Edge[] edges = PathsInfo.ConstructPath(d1, d2, d);
            for(int i = 0; i < edges.Length-1; i++)
            {
                path.Add(edges[i].To);
                if(i > 0)
                    intersections.Add(GetIntersectionPoint(streets[edges[i].From], streets[edges[i].To]));
            }
            return true;
        }

    }

    [Serializable]
    public struct Point
    {
        public double x;
        public double y;

        public Point(double px, double py) { x = px; y = py; }

        public static Point operator +(Point p1, Point p2) { return new Point(p1.x + p2.x, p1.y + p2.y); }

        public static Point operator -(Point p1, Point p2) { return new Point(p1.x - p2.x, p1.y - p2.y); }

        public static bool operator ==(Point p1, Point p2) { return p1.x == p2.x && p1.y == p2.y; }

        public static bool operator !=(Point p1, Point p2) { return !(p1 == p2); }

        public override bool Equals(object obj) { return base.Equals(obj); }

        public override int GetHashCode() { return base.GetHashCode(); }

        public static double CrossProduct(Point p1, Point p2) { return p1.x * p2.y - p2.x * p1.y; }

        public override string ToString() { return String.Format("({0},{1})", x, y); }
    }

    [Serializable]
    public struct Street
    {
        public Point p1;
        public Point p2;

        public Street(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }
    }
}