using System;

namespace ASD
{

    public class CarpentersBench : System.MarshalByRefObject
    {

        /// <summary>
        /// Metoda pomocnicza - wymagana przez system
        /// </summary>
        public int Cut(int length, int width, int[,] elements, out Cut cuts)
        {
            (int length, int width, int price)[] _elements = new(int length, int width, int price)[elements.GetLength(0)];
            for (int i = 0; i < _elements.Length; ++i)
            {
                _elements[i].length = elements[i, 0];
                _elements[i].width = elements[i, 1];
                _elements[i].price = elements[i, 2];
            }
            return Cut((length, width), _elements, out cuts);
        }

        /// <summary>
        /// Wyznaczanie optymalnego sposobu pocięcia płyty
        /// </summary>
        /// <param name="sheet">Rozmiary płyty</param>
        /// <param name="elements">Tablica zawierająca informacje o wymiarach i wartości przydatnych elementów</param>
        /// <param name="cuts">Opis cięć prowadzących do uzyskania optymalnego rozwiązania</param>
        /// <returns>Maksymalna sumaryczna wartość wszystkich uzyskanych w wyniku cięcia elementów</returns>
        public int Cut((int length, int width) sheet, (int length, int width, int price)[] elements, out Cut cuts)
        {
            int[,] tab = new int[sheet.length + 1, sheet.width + 1];

            //Initialize
            int len = tab.GetLength(0);
            int wid = tab.GetLength(1);
            for (int i = 0; i < elements.Length; i++)
            {
                if(elements[i].length < len && elements[i].width < wid)
                {
                    if(elements[i].price > tab[elements[i].length, elements[i].width])
                        tab[elements[i].length, elements[i].width] = elements[i].price;
                }
            }
            
            //Fill
            for (int i = 1; i < sheet.length + 1; i++)
            {
                for (int j = 1; j < sheet.width + 1; j++)
                {
                    tab[i, j] = getMax(i, j, tab);
                }
            }

            cuts = new ASD.Cut(sheet.length, sheet.width, tab[sheet.length, sheet.width]);
            getMaxCut(cuts, tab, elements);
            return tab[sheet.length, sheet.width];    // zmienic w czesci A
        }

        public int getMax(int len, int wid, int[,] tab)
        {
            int maxVal = 0;

            //Cutting horizontally
            for(int i = len; i > 0; i--)
            {
                if (tab[i, wid] + tab[len - i, wid] > maxVal)
                    maxVal = tab[i, wid] + tab[len - i, wid];
            }

            //Cutting vertically
            for (int j = wid; j > 0; j--)
            {
                if (tab[len, j] + tab[len, wid - j] > maxVal)
                    maxVal = tab[len, j] + tab[len, wid - j];
            }

            return maxVal;
        }

        public void getMaxCut(Cut main, int[,] tab, (int length, int width, int price)[] elements)
        {
            int len = main.length;
            int wid = main.width;

            //Check if the cut is already one of the elements
            for(int i = 0; i < elements.Length; i++)
            {
                if (elements[i].length == len && elements[i].width == wid && tab[len, wid] == elements[i].price)
                {
                    main.n = 0;
                    main.bottomright = null;
                    main.topleft = null;
                    return;
                }
            }

            //Check if the cut is worthless
            if (main.price == 0)
            {
                main.n = 0;
                main.bottomright = null;
                main.topleft = null;
                return;
            }

            //Maximum value for a cut
            int maxVal = 0;

            //Values of 2 new cuts
            int valTopLeft = 0, valBottomRight = 0;

            //Distance from edge for a new cut:
            int dist = 0;
            bool vertical = false;

            //Sizes of 2 new cuts
            int bottomrightSizeY = 0;
            int bottomrightSizeX = 0;
            int topleftSizeY = 0;
            int topleftSizeX = 0;

            //Cutting horizontally
            for (int i = len; i > 0; i--)
            {
                if (tab[i, wid] + tab[len - i, wid] == tab[len, wid])
                {
                    if (len - i == 0 || wid == 0 || i == 0 || wid == 0)
                        continue;

                    vertical = false;
                    dist = i;
                    bottomrightSizeY = len - i;
                    bottomrightSizeX = wid;
                    topleftSizeY = i;
                    topleftSizeX = wid;

                    valTopLeft = tab[i, wid];
                    valBottomRight = tab[len - i, wid];
                    maxVal = valTopLeft + valBottomRight;
                }
            }

            //Cutting vertically
            for (int j = wid; j > 0; j--)
            {
                if (tab[len, j] + tab[len, wid - j] == tab[len, wid])
                {
                    if (len == 0 || wid - j == 0 || len == 0 || j == 0)
                        continue;

                    vertical = true;
                    dist = j;
                    bottomrightSizeY = len;
                    bottomrightSizeX = wid - j;
                    topleftSizeY = len;
                    topleftSizeX = j;

                    valTopLeft = tab[len, j];
                    valBottomRight = tab[len, wid - j];
                    maxVal = valTopLeft + valBottomRight;
                }
            }

            if (maxVal == 0)
            {
                main.bottomright = null;
                main.topleft = null;
                main.n = 0;
                return;
            }

            if ((!vertical && dist == len) || (vertical && dist == wid))
            {
                main.bottomright = null;
                main.topleft = null;
                main.n = 0;
                return;
            }

            main.n = dist;
            main.vertical = vertical;
            main.price = maxVal;
            main.bottomright = new ASD.Cut(bottomrightSizeY, bottomrightSizeX, valBottomRight, vertical, 1, null, null);
            main.topleft = new ASD.Cut(topleftSizeY, topleftSizeX, valTopLeft, vertical, 1, null, null);

            getMaxCut(main.bottomright, tab, elements);
            getMaxCut(main.topleft, tab, elements);
        }
    }

}
