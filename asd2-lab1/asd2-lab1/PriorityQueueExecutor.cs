
using System;

namespace ASD
{

[Serializable]
public struct OperationInfo
    {
    public char oper;
    public int val;
    public int count;
    public Exception ex;
    public OperationInfo(char oper, int val, int count, Exception ex)
        {
        this.oper = oper;
        this.val = val;
        this.count = count;
        this.ex = ex;
        }
    }

public abstract class PriorityQueueExecutor : MarshalByRefObject
    {

    protected abstract IPriorityQueue TestedQueue { get; }

    public OperationInfo[] Execute(OperationInfo[] operations)
        {
        OperationInfo[] results = new OperationInfo[operations.Length];
        for ( int i=0 ; i<operations.Length ; ++i )
            try
                {
                switch ( operations[i].oper )
                    {
                    case 'p':
                        TestedQueue.Put(operations[i].val);
                        break;
                    case 'g':
                        results[i].val = TestedQueue.GetMax();
                        break;
                    case 's':
                        results[i].val = TestedQueue.ShowMax();
                        break;
                    }
                results[i].count = TestedQueue.Count;
                }
            catch ( InvalidOperationException ex ) when ( operations[i].ex!=null )
                {
                results[i].ex = ex;
                results[i].count = TestedQueue.Count;
                }
        return results;
        }

    }

public class LazyPriorityQueueExecutor : PriorityQueueExecutor
    {
    protected override IPriorityQueue TestedQueue { get; } = new LazyPriorityQueue();
    }

public class EagerPriorityQueueExecutor : PriorityQueueExecutor
    {
    protected override IPriorityQueue TestedQueue { get; } = new EagerPriorityQueue();
    }

public class HeapPriorityQueueExecutor : PriorityQueueExecutor
    {
    protected override IPriorityQueue TestedQueue { get; } = new HeapPriorityQueue();
    }

}