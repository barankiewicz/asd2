using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab08
{

    public class Lab08 : MarshalByRefObject
    {

        /// <summary>
        /// funkcja do sprawdzania czy da się ustawić k elementów w odległości co najmniej dist od siebie
        /// </summary>
        /// <param name="a">posortowana tablica elementów</param>
        /// <param name="dist">zadany dystans</param>
        /// <param name="k">liczba elementów do wybrania</param>
        /// <param name="exampleSolution">Wybrane elementy</param>
        /// <returns>true - jeśli zadanie da się zrealizować</returns>
        public bool CanPlaceElementsInDistance(int[] a, int dist, int k, out List<int> exampleSolution)
        {
            exampleSolution = new List<int>();
            int prevDist = a[0];
            
            exampleSolution.Add(a[0]);
            for (int i = 1; i < a.Length; i++)
            {
                int curDist = a[i];
                if (a[i] - prevDist >= dist)
                {
                    exampleSolution.Add(a[i]);
                    prevDist = curDist;
                }

                if (exampleSolution.Count >= k)
                    return true;
            }
            //If not found, set exampleSolution to null
            exampleSolution = null;
            return false;
        }

        /// <summary>
        /// Funkcja wybiera k elementów tablicy a, tak aby minimalny dystans pomiędzy dowolnymi dwiema liczbami (spośród k) był maksymalny
        /// </summary>
        /// <param name="a">posortowana tablica elementów</param>
        /// <param name="k">liczba elementów do wybrania</param>
        /// <param name="exampleSolution">Wybrane elementy</param>
        /// <returns>Maksymalny możliwy dystans między wybranymi elementami</returns>
        public int LargestMinDistance(int[] a, int k, out List<int> exampleSolution)
        {
            //Sanity tests
            if (a.Length > 100000 || a.Length < 1)
                throw new ArgumentException();

            if (k > a.Length || k < 2)
                throw new ArgumentException();

            long diff = a[a.Length - 1] - a[0];

            var ret = new List<int>();
            var rett = new List<int>();
            long start = 0;
            long end = diff/(k-1) + 1;
            long inter = 0;
            while (start < end)
            {
                inter = (end + start) / 2;
                if (CanPlaceElementsInDistance(a, (int)inter, k, out ret))
                {
                    rett = ret;
                    start = inter + 1;
                    diff = inter;
                }
                else
                {
                    end = inter;
                }
            }
            exampleSolution = rett;
            return (int)diff;
        }

    }

}
