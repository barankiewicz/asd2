using System;
using System.Linq;

namespace ASD
{
    public class WorkManager : MarshalByRefObject
    {
        /// <summary>
        /// Implementacja wersji 1
        /// W tablicy blocks zapisane s� wagi wszystkich blok�w do przypisania robotnikom.
        /// Ka�dy z nich powinien mie� przypisane bloki sumie wag r�wnej expectedBlockSum.
        /// Metoda zwraca tablic� przypisuj�c� ka�demu z blok�w jedn� z warto�ci:
        /// 1 - je�li blok zosta� przydzielony 1. robotnikowi
        /// 2 - je�li blok zosta� przydzielony 2. robotnikowi
        /// 0 - je�li blok nie zosta� przydzielony do �adnego robotnika
        /// Je�li wymaganego podzia�u nie da si� zrealizowa� metoda zwraca null.
        /// </summary>
        public int[] DivideWorkersWork(int[] blocks, int expectedBlockSum)
        {
            if (blocks.Length < 2)
                return null;
            int[] result = new int[blocks.Length];

            if (!problemSolver(blocks, result, 0, expectedBlockSum, expectedBlockSum))
                return null;
            else
                return result;
        }

        /// <summary>
        /// Implementacja wersji 2
        /// Parametry i wynik s� analogiczne do wersji 1.
        /// </summary>
        public int[] DivideWorkWithClosestBlocksCount(int[] blocks, int expectedBlockSum)
        {
            return new int[0];
        }

        // Mo�na dopisywa� pola i metody pomocnicze
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

