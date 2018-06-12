using System;
using System.Collections.Generic;
using System.Text;

namespace ASD
{
    public class LZ77 : MarshalByRefObject
    {
        /// <summary>
        /// Odkodowywanie napisu zakodowanego algorytmem LZ77. Dane kodowanie jest poprawne (nie trzeba tego sprawdzać).
        /// </summary>

        public string Decode(List<EncodingTriple> encoding)
        {
            int capacity = 0;
            for (int i = 0; i < encoding.Count; i++)
                capacity += encoding[i].c;
            StringBuilder sb = new StringBuilder(capacity);
            for (int i = 0; i < encoding.Count; i++)
            {
                if (encoding[i].c != 0)
                {
                    int startingIndex = sb.Length - encoding[i].p - 1;
                    int length = sb.Length - startingIndex;
                    for (int j = 0; j < encoding[i].c; j++)
                        sb.Append(sb[startingIndex + (j % length)]);
                }
                sb.Append(encoding[i].s);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Kodowanie napisu s algorytmem LZ77
        /// </summary>
        /// <returns></returns>
        public List<EncodingTriple> Encode(string s, int maxP)
        {
            List<EncodingTriple> ret = new List<EncodingTriple>();
            int l = s.Length;

            if(l == 1)
            {
                ret.Add(new EncodingTriple(0, 0, s[0]));
                return ret;
            } else if (l == 2)
            {
                ret.Add(new EncodingTriple(0, 0, s[0]));
                ret.Add(new EncodingTriple(0, 0, s[1]));
                return ret;
            } else if (l == 3)
            {
                ret.Add(new EncodingTriple(0, 0, s[0]));
                if(s[1] == s[0])
                {
                    ret.Add(new EncodingTriple(0, 1, s[2]));
                } else
                {
                    ret.Add(new EncodingTriple(0, 0, s[1]));
                    ret.Add(new EncodingTriple(0, 0, s[2]));
                }
                return ret;
            }
            ret.Add(new EncodingTriple(0, 0, s[0]));
            int searchStart = 0;
            int pointer = 1;
            int w = -1;

            int j, c;
            while (pointer != s.Length)
            {
                if(pointer == s.Length - 1)
                {
                    ret.Add(new EncodingTriple(0, 0, s[s.Length - 1]));
                    break;
                }
                searchStart = (pointer - (maxP + 1) > 0) ? pointer - (maxP + 1) : 0;
                w = pointer - searchStart;

                //Krok 3
                
                (j, c) = SuperKMP(searchStart, pointer, l - 1, s, maxP);
                //(j, c) = KMP2(searchStart, pointer, l - 1, s, maxP);
                //var costam = KMP(s.Substring(searchStart, w), s.Substring(pointer, windowEnd - pointer));

                //Krok 4
                ret.Add(new EncodingTriple(w - (j + 1), c, s[pointer + c]));

                //Krok 5
                pointer += c + 1;
            }



            return ret;
        }

        /// <summary>
        /// Wyszukiwanie wzorca w tekście algorytmem Knutha-Morrisa-Pratta (wersja podstawowa)
        /// </summary>
        /// <param name="y">Badany tekst</param>
        /// <param name="x">Szukany wzorzec</param>
        /// <returns>Lista zawierająca początkowe indeksy wystąpień wzorca x w tekście y</returns>
        public static (int j, int c) SuperKMP(int searchStart, int pointer, int windowEnd, string s, int maxP)
        {

            int n, m, i, j;

            n = windowEnd - searchStart;
            
            m = windowEnd - pointer;
            int max = -1;
            int maxI = -1;
            int[] P = new int[m + 1];
            int t;
            P[0] = P[1] = t = 0;
            int lastCalc = 1;

            int end = (maxP > (n - m)) ? n - m : maxP;
            for (j = i = 0; i <= end && i < pointer; i += (j == 0 ? 1 : j - P[j]))
            {
                for (j = P[j]; j < m && s[searchStart + i + j] == s[pointer + j]; ++j)
                {
                    if (j + 1 > lastCalc && j + 1 < m && s[searchStart + i + j + 1] == s[pointer + j + 1]) //Check if Im gonna need P[j + 1] for next iteration
                    {
                        t = P[j];
                        while (t > 0 && s[pointer + t] != s[pointer + j])
                            t = P[t];
                        if (s[pointer + t] == s[pointer + j]) ++t;  // czy można wydłużyć prefikso-sufiks ?
                        P[j + 1] = t;
                        lastCalc++;
                    }
                }

                if (max < j && i < pointer)
                {
                    max = j;
                    maxI = i;
                }

                if (j > lastCalc) //Check if Im gonna need P[j + 1] for next iteration
                {
                    t = P[j - 1];
                    while (t > 0 && s[pointer + t] != s[pointer + j - 1])
                        t = P[t];
                    if (s[pointer + t] == s[pointer + j - 1]) ++t;  // czy można wydłużyć prefikso-sufiks ?
                    P[j] = t;
                    lastCalc++;
                }
            }
            return (maxI, max);
        }

    [Serializable]
    public struct EncodingTriple
    {
        public int p, c;
        public char s;

        public EncodingTriple(int p, int c, char s)
        {
            this.p = p;
            this.c = c;
            this.s = s;
        }
    }
}
