
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{

public interface IPriorityQueue
    {
    void Put(int p);     // wstawia element do kolejki
    int GetMax();        // pobiera maksymalny element z kolejki (element jest usuwany z kolejki)
    int ShowMax();       // pokazuje maksymalny element kolejki (element pozostaje w kolejce)
    int Count { get; }   // liczba elementów kolejki
    }


public class LazyPriorityQueue  : MarshalByRefObject, IPriorityQueue
    {
        List<int> el;
        int maxVal;
        int maxInd;

    public LazyPriorityQueue()
        {
            el = new List<int>();
            maxVal = int.MinValue;
            maxInd = 0;
        }

    public void Put(int p)
        {
            el.Add(p);
            if (p > maxVal)
            {
                maxVal = p;
                maxInd = el.Count - 1;
            }
        }

    public int GetMax()
        {
            if (el.Count == 0)
            {
                throw new InvalidOperationException("Access to empty queue");
            }
            int max = maxVal;
            el.RemoveAt(maxInd);

            maxVal = int.MinValue;
            for (int i = 0; i < el.Count; i++)
            {
                if (el[i] > maxVal)
                {
                    maxVal = el[i];
                    maxInd = i;
                }
            }
            return max;
        }

    public int ShowMax()
        {
            if (el.Count == 0)
            {
                throw new InvalidOperationException("Access to empty queue");
            }
            return maxVal;
        }

    public int Count
        {
        get {
            return el.Count;
            }
        }
    } // LazyPriorityQueue




public class EagerPriorityQueue : MarshalByRefObject, IPriorityQueue
    {
        List<int> el;

    public EagerPriorityQueue()
        {
            el = new List<int>();
        }

    public void Put(int p)
        {
            int index = 0; //smallest

            while(index < el.Count)
            {
                if (p < el[index])
                {
                    break;
                }
                index++;
            }
            el.Insert(index,p);
        }

    public int GetMax()
        {
            if (el.Count == 0)
            {
                throw new InvalidOperationException("Access to empty queue");
            }
            int max = el.Last();
            el.RemoveAt(Count - 1);
            return max;
        }

    public int ShowMax()
        {
            if (el.Count == 0)
            {
                throw new InvalidOperationException("Access to empty queue");
            }
            return el.Last();
        }

    public int Count
        {
        get {
            return el.Count;
            }
        }

    } // EagerPriorityQueue


public class HeapPriorityQueue : MarshalByRefObject, IPriorityQueue
    {
        List<int> el;

    public HeapPriorityQueue()
        {
            el = new List<int>();
        }

    public void Put(int p)
        {
            el.Add(p);
            int ci = el.Count - 1; //Starting index
            while (ci > 0)
            {
                int pi = (ci - 1) / 2; //Get parent index
                if (el[pi] >= el[ci])
                    break;
                int tmp = el[ci];
                el[ci] = el[pi];
                el[pi] = tmp;
                ci = pi;
            }
        }

    public int GetMax()
        {
            if (el.Count == 0)
            {
                throw new InvalidOperationException("Access to empty queue");
            }

            int hl = el.Count - 1; //Tree height
            int max = el[0];
            el[0] = el[hl];
            el.RemoveAt(hl);
            --hl;

            //Bubble down:
            int pi = 0; //Parent index
            while (true)
            {
                int lc = pi * 2 + 1; //Left child index
                int rc = lc + 1; //Right child index
                if (lc > hl) break;

                if (rc <= hl && el[rc] > el[lc])
                    lc = rc;
                if (el[pi] > el[lc]) break;

                int tmp = el[pi];
                el[pi] = el[lc];
                el[lc] = tmp;
                pi = lc;
            }
            return max;
        }

    public int ShowMax()
        {
            if (el.Count == 0)
            {
                throw new InvalidOperationException("Access to empty queue");
            }
            return el[0];
        }

    public int Count
        {
        get {
            return el.Count;
            }
        }

    } // HeapPriorityQueue

}
