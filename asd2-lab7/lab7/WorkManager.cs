using System;
using System.Linq;

namespace ASD
{
    public class WorkManager : MarshalByRefObject
    {
        int difference = int.MaxValue;
        int[] minResult;

        /// <summary>
        /// Implementacja wersji 1
        /// W tablicy blocks zapisane s¹ wagi wszystkich bloków do przypisania robotnikom.
        /// Ka¿dy z nich powinien mieæ przypisane bloki sumie wag równej expectedBlockSum.
        /// Metoda zwraca tablicê przypisuj¹c¹ ka¿demu z bloków jedn¹ z wartoœci:
        /// 1 - jeœli blok zosta³ przydzielony 1. robotnikowi
        /// 2 - jeœli blok zosta³ przydzielony 2. robotnikowi
        /// 0 - jeœli blok nie zosta³ przydzielony do ¿adnego robotnika
        /// Jeœli wymaganego podzia³u nie da siê zrealizowaæ metoda zwraca null.
        /// </summary>
        public int[] DivideWorkersWork(int[] blocks, int expectedBlockSum)
        {
            int[] result = new int[blocks.Length];

            if (!problemSolver(blocks, result, 0, expectedBlockSum, expectedBlockSum))
                return null;
            else
                return result;
        }

        /// <summary>
        /// Implementacja wersji 2
        /// Parametry i wynik s¹ analogiczne do wersji 1.
        /// </summary>
        public int[] DivideWorkWithClosestBlocksCount(int[] blocks, int expectedBlockSum)
        {
            difference = int.MaxValue;
            minResult = new int[blocks.Length];
            int[] result = new int[blocks.Length];
            if (blocks.Sum() < 2 * expectedBlockSum)
                return null;

            problemSolverSec(blocks, result, 0, expectedBlockSum, expectedBlockSum, 0, 0);

            int sum = minResult.Sum();
            if (sum == 0)
                return null;
            else
                return minResult;
        }

        bool problemSolverSec(int[] blocks, int[] result, int startingPoint, int remainingSumA, int remainingSumB, int blockCountA, int blockCountB)
        {
            //Znalezione poprawne mapowanie
            if (remainingSumA == 0 && remainingSumB == 0)
            {
                //Jesli mniejsze, nalezy nadpisac minimalne rozwiazane
                if (Math.Abs(blockCountA - blockCountB) < difference)
                {
                    difference = Math.Abs(blockCountA - blockCountB);
                    result.CopyTo(minResult, 0);
                    return false;
                }

                //Jesli 0, nic lepszego juz sie nie znajdzie
                if (difference == 0)
                    return true;
            }
            //Jesli startingPoint wychodzi poza tablice
            if (startingPoint >= blocks.Length)
                return false;
            //Jesli pozostala suma jest ujemna
            if (remainingSumA < 0 || remainingSumB < 0)
                return false;

            result[startingPoint] = 1;
            if (problemSolverSec(blocks, result, startingPoint + 1, remainingSumA - blocks[startingPoint], remainingSumB, blockCountA + 1, blockCountB))
                return true;
            else
            {
                result[startingPoint] = 2;
                if (problemSolverSec(blocks, result, startingPoint + 1, remainingSumA, remainingSumB - blocks[startingPoint], blockCountA, blockCountB + 1))
                    return true;
                else
                {
                    result[startingPoint] = 0;
                    if (problemSolverSec(blocks, result, startingPoint + 1, remainingSumA, remainingSumB, blockCountA, blockCountB))
                        return true;
                    else
                        return false;
                }
            }
        }

        bool problemSolver(int[] blocks, int[] result, int startingPoint, int remainingSumA, int remainingSumB)
        {
            if (remainingSumA == 0 && remainingSumB == 0)
                return true;
            if (startingPoint >= blocks.Length)
                return false;
            if (remainingSumA < 0 || remainingSumB < 0)
                return false;

            result[startingPoint] = 1;
            if (problemSolver(blocks, result, startingPoint + 1, remainingSumA - blocks[startingPoint], remainingSumB))
                return true;
            else
            {
                result[startingPoint] = 2;
                if (problemSolver(blocks, result, startingPoint + 1, remainingSumA, remainingSumB - blocks[startingPoint]))
                    return true;
                else
                {
                    result[startingPoint] = 0;
                    if (problemSolver(blocks, result, startingPoint + 1, remainingSumA, remainingSumB))
                        return true;
                    else
                        return false;
                }
            }
        }
    }
}

