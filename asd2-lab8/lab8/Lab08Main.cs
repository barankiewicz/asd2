using ASD;
using System;
using System.Collections.Generic;

namespace Lab08
{

    class Lab08Main
    {
        static void Main(string[] args)
        {
            Lab08TestModule lab08test = new Lab08TestModule();
            lab08test.PrepareTestSets();

            foreach (var ts in lab08test.TestSets)
            {
                ts.Value.PerformTests(verbose: true, checkTimeLimit: ts.Key == "Hard");
            }
        }
    }

    class Lab08TestModule : TestModule
    {
        public override void PrepareTestSets()
        {
            Lab08 solver = new Lab08();

            TestSets["ElementsInDistance"] = new TestSet(solver, "Part 1 - CanPlaceElementsInDistance [0.5p]");
            TestSets["ElementsInDistance"].TestCases.Add(new CanPlaceElementsInDistanceTestCase(1, null, "", new int[] { 1, 2, 3 }, 3, 1, true));
            TestSets["ElementsInDistance"].TestCases.Add(new CanPlaceElementsInDistanceTestCase(1, null, "", new int[] { 1, 2, 4, 8, 9 }, 4, 10, false));
            TestSets["ElementsInDistance"].TestCases.Add(new CanPlaceElementsInDistanceTestCase(1, null, "", new int[] { 4, 5, 6, 7, 8, 9, 15, 16, 17, 18, 19, 20 }, 5, 5, false));
            TestSets["ElementsInDistance"].TestCases.Add(new CanPlaceElementsInDistanceTestCase(1, null, "", new int[] { 0, 2, 4, 6, 8, 10 }, 2, 4, true));
            TestSets["ElementsInDistance"].TestCases.Add(new CanPlaceElementsInDistanceTestCase(1, null, "", new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 3, 0, true));
            TestSets["ElementsInDistance"].TestCases.Add(new CanPlaceElementsInDistanceTestCase(1, null, "", new int[] { 0, 5, 6, 7, 8, 10 }, 3, 5, true));
            TestSets["ElementsInDistance"].TestCases.Add(new CanPlaceElementsInDistanceTestCase(1, null, "", new int[] { 2, 2, 5, 6, 7, 8, 9, 10 }, 6, 4, false));

            TestSets["Easy"] = new TestSet(solver, "Part 2 - Easy tests k=3 [1.0p]");
            TestSets["Easy"].TestCases.Add(new LargestMinDistanceTestCase(1, null, "", new int[] { 1, 2, 3 }, 3, 1));
            TestSets["Easy"].TestCases.Add(new LargestMinDistanceTestCase(1, null, "", new int[] { 1, 2, 4, 8, 9 }, 3, 3));
            TestSets["Easy"].TestCases.Add(new LargestMinDistanceTestCase(1, null, "", new int[] { 4, 5, 6, 7, 8, 9, 15, 16, 17, 18, 19, 20 }, 3, 5));
            TestSets["Easy"].TestCases.Add(new LargestMinDistanceTestCase(1, null, "", new int[] { 0, 2, 4, 6, 8, 10 }, 3, 4));
            TestSets["Easy"].TestCases.Add(new LargestMinDistanceTestCase(1, null, "", new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 3, 0));
            TestSets["Easy"].TestCases.Add(new LargestMinDistanceTestCase(1, null, "", new int[] { 0, 5, 6, 7, 8, 10 }, 3, 5));
            TestSets["Easy"].TestCases.Add(new LargestMinDistanceTestCase(1, null, "", new int[] { 2, 2, 5, 6, 7, 8, 9, 10 }, 3, 4));

            TestSets["Medium"] = new TestSet(solver, "Part 3 - Medium tests k <= 6 && k != 3 [1.0p]");
            TestSets["Medium"].TestCases.Add(new LargestMinDistanceTestCase(1, new ArgumentException(), "", new int[] { 2 }, 2, 0));
            TestSets["Medium"].TestCases.Add(new LargestMinDistanceTestCase(1, new ArgumentException(), "", new int[] { 5, 21, 44, 45 }, 5, 0));
            TestSets["Medium"].TestCases.Add(new LargestMinDistanceTestCase(1, new ArgumentException(), "", new int[] { 5, 21, 44, 45 }, 1, 0));
            TestSets["Medium"].TestCases.Add(new LargestMinDistanceTestCase(1, null, "", new int[] { 2, 2 }, 2, 0));
            TestSets["Medium"].TestCases.Add(new LargestMinDistanceTestCase(1, null, "", new int[] { 4, 5, 6, 7, 8, 11, 20, 21, 22, 33, 44, 45, 46, 99 }, 2, 95));
            TestSets["Medium"].TestCases.Add(new LargestMinDistanceTestCase(1, null, "", new int[] { 2, 2 }, 2, 0));
            TestSets["Medium"].TestCases.Add(new LargestMinDistanceTestCase(1, null, "", new int[] { 1, 2, 4, 5, 7, 8, 10, 43, 76, 99 }, 4, 23));
            TestSets["Medium"].TestCases.Add(new LargestMinDistanceTestCase(1, null, "", new int[] { 4, 5, 6, 7, 8, 11, 20, 21, 22, 33, 44, 45, 46, 99 }, 4, 18));
            TestSets["Medium"].TestCases.Add(new LargestMinDistanceTestCase(1, null, "", new int[] { 4, 5, 6, 7, 8, 11, 20, 21, 22, 33, 44, 45, 46, 99 }, 5, 13));
            TestSets["Medium"].TestCases.Add(new LargestMinDistanceTestCase(1, null, "", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 }, 5, 6));
            TestSets["Medium"].TestCases.Add(new LargestMinDistanceTestCase(1, null, "", new int[] { 2, 4, 5, 7, 9, 11, 20, 200 }, 6, 2));
            TestSets["Medium"].TestCases.Add(new LargestMinDistanceTestCase(1, null, "", new int[] { 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 6 }, 6, 1));

            TestSets["Hard"] = new TestSet(solver, "Part 4 - Performance tests [1.5p]");
            List<int> a = new List<int>();
            a.Add(100000000);
            for (int i = 0; i < 10000; i++) a.Add(100000000 + 2 * i);
            a.Add(300000000);
            TestSets["Hard"].TestCases.Add(new LargestMinDistanceTestCase(2, null, "", a.ToArray(), 2, 200000000));
            a.Add(200000000);
            a.Add(400000000);
            a.Sort();
            TestSets["Hard"].TestCases.Add(new LargestMinDistanceTestCase(2, null, "", a.ToArray(), 4, 100000000));
            a.Clear();

            Random rnd = new Random(999);
            for (int i = 0; i < 100; i++) a.Add(rnd.Next());
            a.Sort();
            TestSets["Hard"].TestCases.Add(new LargestMinDistanceTestCase(2, null, "", a.ToArray(), 10, 213697552));
            TestSets["Hard"].TestCases.Add(new LargestMinDistanceTestCase(2, null, "", a.ToArray(), 100, 291975));

            for (int i = 0; i < 9900; i++) a.Add(rnd.Next());
            a.Sort();
            TestSets["Hard"].TestCases.Add(new LargestMinDistanceTestCase(2, null, "", a.ToArray(), 100, 21486533));
            
            List<int> b = new List<int>();
            for (int i = 0; i < 100; i++) b.Add(999);
            for (int i = 0; i < 98; i++) b.Add(rnd.Next());
            b.Sort();
            TestSets["Hard"].TestCases.Add(new LargestMinDistanceTestCase(2, null, "", b.ToArray(), 100, 0));

            b.Add(rnd.Next());
            b.Sort();
            TestSets["Hard"].TestCases.Add(new LargestMinDistanceTestCase(2, null, "", b.ToArray(), 100, 133375));

            int elemsTo100k = 90000;
            for (int i = 0; i < elemsTo100k; i++) a.Add(rnd.Next(int.MaxValue - 1));
            a.Sort();
            TestSets["Hard"].TestCases.Add(new LargestMinDistanceTestCase(2, null, "", a.ToArray(), 1000, 2128520));
            a.Add(a[a.Count - 1] + 1);
            TestSets["Medium"].TestCases.Add(new LargestMinDistanceTestCase(2, new ArgumentException(), "", a.ToArray(), 1000, 2128520));
        }

        public override double ScoreResult(out string message)
        {
            message = "OK";
            return 1;
        }
    }

    public class LargestMinDistanceTestCase : TestCase
    {
        private int[] table;
        private int k;
        private int result;
        private List<int> exampleSolution;
        private int expectedResult;
        private Dictionary<int, int> elemsOccurences;
        public LargestMinDistanceTestCase(double timeLimit, Exception expectedException, string description, int[] a, int k, int result) : base(timeLimit, expectedException, description)
        {
            table = a;
            elemsOccurences = new Dictionary<int, int>();
            for (int i = 0; i < table.Length; i++)
            {
                if (elemsOccurences.ContainsKey(table[i]))
                    elemsOccurences[table[i]]++;
                else elemsOccurences.Add(table[i], 1);
            }
            this.k = k;
            expectedResult = result;
        }

        public override void PerformTestCase(object prototypeObject)
        {
            result = ((Lab08)prototypeObject).LargestMinDistance(table, k, out exampleSolution);
        }

        public override void VerifyTestCase(out Result resultCode, out string message, object settings = null)
        {
            if (exampleSolution == null || exampleSolution.Count == 0)
            {
                message = $"No solution";
                resultCode = Result.BadResult;
                return;
            }
            if (result != expectedResult)
            {
                message = $"incorrect result: {result} (expected: {expectedResult})";
                resultCode = Result.BadResult;
                return;
            }
            if (!CheckExampleSolution())
            {
                message = $"Elements in example solution do not match expected result: {result}";
                resultCode = Result.BadResult;
                return;
            }
            resultCode = Result.Success;
            message = $"OK, {PerformanceTime:#0.000}";
        }

        private bool CheckExampleSolution()
        {
            int minDistance = Math.Abs(exampleSolution[exampleSolution.Count - 1] - exampleSolution[0]);
            for (int i = 0; i < exampleSolution.Count; i++)
            {
                if (!elemsOccurences.ContainsKey(exampleSolution[i]) || elemsOccurences[exampleSolution[i]] == 0) return false;
                elemsOccurences[exampleSolution[i]]--;
                for (int j = i + 1; j < exampleSolution.Count; j++)
                {
                    if (!elemsOccurences.ContainsKey(exampleSolution[j]) || elemsOccurences[exampleSolution[j]] == 0) return false;
                    int dist = Math.Abs(exampleSolution[j] - exampleSolution[i]);
                    if (dist < minDistance) minDistance = dist;
                }
            }
            return minDistance == expectedResult;
        }
    }

    public class CanPlaceElementsInDistanceTestCase : TestCase
    {
        private int[] table;
        private int k;
        private int distance;
        private bool result;
        private List<int> exampleSolution;
        private bool expectedResult;
        private Dictionary<int, int> elemsOccurences;
        public CanPlaceElementsInDistanceTestCase(double timeLimit, Exception expectedException, string description, int[] a, int k, int distance, bool result) : base(timeLimit, expectedException, description)
        {
            table = a;
            elemsOccurences = new Dictionary<int, int>();
            for (int i = 0; i < table.Length; i++)
            {
                if (elemsOccurences.ContainsKey(table[i]))
                    elemsOccurences[table[i]]++;
                else elemsOccurences.Add(table[i], 1);
            }
            this.k = k;
            this.distance = distance;
            expectedResult = result;
        }

        public override void PerformTestCase(object prototypeObject)
        {
            result = ((Lab08)prototypeObject).CanPlaceElementsInDistance(table, distance, k, out exampleSolution);
        }

        public override void VerifyTestCase(out Result resultCode, out string message, object settings = null)
        {
            if (result != expectedResult)
            {
                message = $"incorrect result: {result} (expected: {expectedResult})";
                resultCode = Result.BadResult;
                return;
            }
            if (exampleSolution == null && expectedResult == true)
            {
                message = $"No elements in solution";
                resultCode = Result.BadResult;
                return;
            }
            if (exampleSolution != null && !CheckExampleSolution())
            {
                message = $"Elements in example solution do not match expected result: {result}";
                resultCode = Result.BadResult;
                return;
            }
            resultCode = Result.Success;
            message = $"OK, {PerformanceTime:#0.000}";
        }

        private bool CheckExampleSolution()
        {
            int minDistance = Math.Abs(exampleSolution[exampleSolution.Count - 1] - exampleSolution[0]);
            for (int i = 0; i < exampleSolution.Count; i++)
            {
                if (!elemsOccurences.ContainsKey(exampleSolution[i]) || elemsOccurences[exampleSolution[i]] == 0) return false;
                elemsOccurences[exampleSolution[i]]--;
                for (int j = i + 1; j < exampleSolution.Count; j++)
                {
                    if (!elemsOccurences.ContainsKey(exampleSolution[j]) || elemsOccurences[exampleSolution[j]] == 0) return false;
                    int dist = Math.Abs(exampleSolution[j] - exampleSolution[i]);
                    if (dist < minDistance) minDistance = dist;
                }
            }
            return minDistance == distance;
        }
    }
}
