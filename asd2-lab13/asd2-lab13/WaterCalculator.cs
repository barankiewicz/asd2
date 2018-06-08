using System;
using System.Collections.Generic;

namespace ASD
{
    public class WaterCalculator : MarshalByRefObject
    {

        /*
         * Metoda sprawdza, czy przechodząc p1->p2->p3 skręcamy w lewo 
         * (jeżeli idziemy prosto, zwracany jest fałsz).
         */
        private bool leftTurn(Point p1, Point p2, Point p3)
        {
            Point w1 = new Point(p2.x - p1.x, p2.y - p1.y);
            Point w2 = new Point(p3.x - p2.x, p3.y - p2.y);
            double vectProduct = w1.x * w2.y - w2.x * w1.y;
            return vectProduct > 0;
        }


        /*
         * Metoda wyznacza punkt na odcinku p1-p2 o zadanej współrzędnej y.
         * Jeżeli taki punkt nie istnieje (bo cały odcinek jest wyżej lub niżej), zgłaszany jest wyjątek ArgumentException.
         */
        private Point getPointAtY(Point p1, Point p2, double y)
        {
            if (p1.y != p2.y)
            {
                double newX = p1.x + (p2.x - p1.x) * (y - p1.y) / (p2.y - p1.y);
                if ((newX - p1.x) * (newX - p2.x) > 0)
                    throw new ArgumentException("Odcinek p1-p2 nie zawiera punktu o zadanej współrzędnej y!");
                return new Point(p1.x + (p2.x - p1.x) * (y - p1.y) / (p2.y - p1.y), y);
            }
            else
            {
                if (p1.y != y)
                    throw new ArgumentException("Odcinek p1-p2 nie zawiera punktu o zadanej współrzędnej y!");
                return new Point((p1.x + p2.x) / 2, y);
            }
        }


        /// <summary>
        /// Funkcja zwraca tablice t taką, że t[i] jest głębokością, na jakiej znajduje się punkt points[i].
        /// 
        /// Przyjmujemy, że pierwszy punkt z tablicy points jest lewym krańcem, a ostatni - prawym krańcem łańcucha górskiego.
        /// </summary>
        public double[] PointDepths(Point[] points)
        {
            double[] ret = new double[points.Length];
            bool[] sunk = new bool[points.Length];
            double[] minLevel = new double[points.Length];
            int left = 0;
            int right = points.Length - 1;


            //loop 1
            for(int i = 1; i < points.Length; i++)
            {
                if (points[i].x - points[i - 1].x < 0)
                {
                    left = -1;
                    continue;
                }

                if (left == -1)
                    left = i - 1;

                if (i == left + 1 && points[i].y >= points[left].y) //jestem obok granicy
                {
                    left = i;
                }
                else if(points[i].y >= points[left].y)
                {
                    for (int j = left + 1; j < i; j++)
                        sunk[j] = true;
                    left = i;
                }
            }

            //loop 2
            for (int i = points.Length - 2; i >= 0; i--)
            {
                if (points[i].x  - points[i + 1].x > 0)
                {
                    right = -1;
                    continue;
                }
                if (right == -1)
                    right = i + 1;

                if (i == right - 1 && points[i].y >= points[right].y) //jestem obok granicy
                {
                    right = i;
                }
                else if (points[i].y >= points[right].y)
                {
                    for (int j = right - 1; j > i; j--)
                        sunk[j] = true;
                    right = i;
                }
            }

            //loop 3
            left = 0;
            right = int.MaxValue;
            bool sinking = false;
            for (int i = 1; i < points.Length; i++)
            {
                if (sunk[i])
                {
                    sinking = true;
                    continue;
                }

                if (right == int.MaxValue)
                {
                    if (!sinking)
                    {
                        left = i;
                        continue;
                    }

                    right = i;
                    double minimum = Math.Min(points[right].y, points[left].y);

                    for (int j = left + 1; j < right; ++j)
                    {
                        ret[j] = minimum - points[j].y;
                    }

                    sinking = false;
                    left = i;
                    right = int.MaxValue;
                }
            }

            return ret;
        }

        /// <summary>
        /// Funkcja zwraca objętość wody, jaka zatrzyma się w górach.
        /// 
        /// Przyjmujemy, że pierwszy punkt z tablicy points jest lewym krańcem, a ostatni - prawym krańcem łańcucha górskiego.
        /// </summary>
        public double WaterVolume(Point[] points)
        {
            double ret = 0;
            double[] depths = PointDepths(points);
            int left = -1;
            int right = -1;
            double maximum = double.MaxValue;
            Point temp;

            double h = 0;
            double a = 0;
            double b = 0;

            for (int i = 1; i < points.Length; ++i)
            {
                if (depths[i] > 0 && left == -1)
                {
                    temp = getPointAtY(points[i], points[i - 1], points[i].y + depths[i]);
                    h = depths[i];
                    a = -temp.x;
                    b = points[i].x;
                    
                    left = i;
                }
                else if (depths[i] == 0 && left != 0)
                {

                    temp = getPointAtY(points[i - 1], points[i], points[i - 1].y + depths[i - 1]);
                    h = depths[i - 1];
                    a = -points[i - 1].x;
                    b = temp.x;

                    left = -1;
                }
                else if (depths[i] > 0)
                {
                    a = depths[i];
                    b = depths[i - 1];
                    h = points[i].x - points[i - 1].x;
                    
                    right = i;
                }

                ret += (a + b) * h * 0.5;
                if (points[i].y > maximum)
                    maximum = points[i].y;
            }
            return ret;
        }
    }

    [Serializable]
    public struct Point
    {
        public double x, y;
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
