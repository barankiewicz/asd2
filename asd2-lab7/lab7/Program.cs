using System;

namespace ASD
{
    class Program
    {
        static void Main(string[] args)
        {
            Lab07TestModule lab07test = new Lab07TestModule();
            lab07test.PrepareTestSets();

            lab07test.TestSets["Lvl1"].PerformTests(verbose: true, checkTimeLimit: false);
            lab07test.TestSets["Lvl2"].PerformTests(verbose: true, checkTimeLimit: false);
            if (lab07test.TestSets["Lvl1"].FailedCount == 0)
                lab07test.TestSets["Lvl1perf"].PerformTests(verbose: true, checkTimeLimit: true);
            if (lab07test.TestSets["Lvl2"].FailedCount == 0)
                lab07test.TestSets["Lvl2perf"].PerformTests(verbose: true, checkTimeLimit: true);
        }
    }

    class DivideWorkersWorkTestCase : TestCase
    {
        private bool resultExists;
        private int[] result;
        private int[] blocks;
        private int expectedBlockSum;

        public DivideWorkersWorkTestCase(double timeLimit, Exception expectedException, string description, int[] blocks, int expectedBlockSum, bool resultExists)
            : base(timeLimit, expectedException, description)
        {
            this.blocks = (int[])blocks.Clone();
            this.expectedBlockSum = expectedBlockSum;
            this.resultExists = resultExists;
        }
        public override void PerformTestCase(object prototypeObject)
        {
            var workManager = (WorkManager)prototypeObject;
            result = workManager.DivideWorkersWork(blocks, expectedBlockSum);
        }

        public override void VerifyTestCase(out Result resultCode, out string message, object settings)
        {
            bool actualResultExists = result != null;

            if (resultExists != actualResultExists)
            {
                resultCode = Result.BadResult;
                message = $"incorrect result: {(actualResultExists ? "Exists but should be null" : "Does not exist but it should")}";
                return;
            }


            if (result != null)
            {
                if (result.Length != blocks.Length)
                {
                    resultCode = Result.BadResult;
                    message = $"incorrect result: Result array length should be equal to blocks length";
                    return;
                }

                int w1sum = 0, w2sum = 0;
                for (int i = 0; i < result.Length; i++)
                {
                    switch (result[i])
                    {
                        case 0:
                            break;
                        case 1:
                            w1sum += blocks[i];
                            break;
                        case 2:
                            w2sum += blocks[i];
                            break;
                        default:
                            resultCode = Result.BadResult;
                            message = $"invalid value in result: {result[i]}";
                            return;
                    }
                }
                if (w1sum != expectedBlockSum || w2sum != expectedBlockSum)
                {
                    resultCode = Result.BadResult;
                    message = $"invalid sum, should be: {expectedBlockSum} != {w1sum} != {w2sum}";
                    return;
                }
            }

            resultCode = Result.Success;
            message = $"OK, {PerformanceTime:#0.00}";
        }
    }

    class DivideWorkersWorkWithEqualSumTestCase : TestCase
    {
        private bool resultExists;
        private int[] result;
        private int[] blocks;
        private int expectedBlockSum;
        private int expectedDiff;

        public DivideWorkersWorkWithEqualSumTestCase(double timeLimit, Exception expectedException, string description, int[] blocks, int expectedBlockSum, int expectedDiff, bool resultExists)
            : base(timeLimit, expectedException, description)
        {
            this.blocks = (int[])blocks.Clone();
            this.expectedBlockSum = expectedBlockSum;
            this.resultExists = resultExists;
            this.expectedDiff = expectedDiff;
        }
        public override void PerformTestCase(object prototypeObject)
        {
            var workManager = (WorkManager)prototypeObject;
            result = workManager.DivideWorkWithClosestBlocksCount(blocks, expectedBlockSum);
        }

        public override void VerifyTestCase(out Result resultCode, out string message, object settings)
        {
            bool actualResultExists = result != null;

            if (resultExists != actualResultExists)
            {
                resultCode = Result.BadResult;
                message = $"incorrect result: {(actualResultExists ? "Exists but should be null" : "Does not exist but it should")}";
                return;
            }


            if (result != null)
            {
                if (result.Length != blocks.Length)
                {
                    resultCode = Result.BadResult;
                    message = $"incorrect result: Result array length should be equal to blocks length";
                    return;
                }

                int w1sum = 0, w2sum = 0;
                int w1blocks = 0, w2blocks = 0;
                for (int i = 0; i < result.Length; i++)
                {
                    switch (result[i])
                    {
                        case 0:
                            break;
                        case 1:
                            w1sum += blocks[i];
                            w1blocks++;
                            break;
                        case 2:
                            w2sum += blocks[i];
                            w2blocks++;
                            break;
                        default:
                            resultCode = Result.BadResult;
                            message = $"invalid value in result: {result[i]}";
                            return;
                    }
                }
                if (w1sum != expectedBlockSum || w2sum != expectedBlockSum)
                {
                    resultCode = Result.BadResult;
                    message = $"invalid sum, should be: {expectedBlockSum} != {w1sum} != {w2sum}";
                    return;
                }
                if (Math.Abs(w1blocks - w2blocks) != expectedDiff)
                {
                    resultCode = Result.BadResult;
                    message = $"incorrect result: Difference between blocks is {Math.Abs(w1blocks - w2blocks)} (should be {expectedDiff})";
                    return;
                }
            }

            resultCode = Result.Success;
            message = $"OK, {PerformanceTime:#0.00}";
        }
    }

    class Lab07TestModule : TestModule
    {
        Random rand = new Random(2018);

        public override void PrepareTestSets()
        {
            // Performance Level 1
            int[] threeRelevant = new int[7000];
            for (int i = 0; i < threeRelevant.Length; i++)
                threeRelevant[i] = rand.Next(100000, 500000);
            threeRelevant[10] = 10;
            threeRelevant[threeRelevant.Length / 2] = 10;
            threeRelevant[threeRelevant.Length - 10] = 20;

            int[] ones = new int[6000];
            for (int i = 0; i < ones.Length; i++)
                ones[i] = 1;

            int[] bigOnes = new int[100000];
            for (int i = 0; i < bigOnes.Length; i++)
                bigOnes[i] = 1;

            int[] bigNumbers = new int[18];
            for (int i = 0; i < bigNumbers.Length; i++)
                bigNumbers[i] = 2000002;
            bigNumbers[1] = 200000002;
            bigNumbers[3] = 100000001;
            bigNumbers[16] = 700000007;
            bigNumbers[17] = 300000003;

            // Performance Level 2
            int[] relevantAtBeginning = new int[100000];
            for (int i = 5; i < relevantAtBeginning.Length; i++)
                relevantAtBeginning[i] = rand.Next(61, 1000);
            relevantAtBeginning[0] = 40;
            relevantAtBeginning[1] = 20;
            relevantAtBeginning[2] = 60;
            relevantAtBeginning[3] = 30;
            relevantAtBeginning[4] = 30;

            int[] twoPower = new int[9 * 2 - 1];
            int actNum = 1;
            for (int i = 0; i < twoPower.Length; i += 2)
            {
                twoPower[twoPower.Length - 1 - i] = actNum;
                if (i + 1 < twoPower.Length)
                    twoPower[twoPower.Length - 2 - i] = actNum;
                actNum <<= 1;
            }


            TestSets["Lvl1"] = new TestSet(new WorkManager(), "Workers with blocks of equal sum");
            TestSets["Lvl1"].TestCases.Add(new DivideWorkersWorkTestCase(1, null, "", new int[] { 2 }, 2, false));
            TestSets["Lvl1"].TestCases.Add(new DivideWorkersWorkTestCase(1, null, "", new int[] { 1, 1 }, 1, true));
            TestSets["Lvl1"].TestCases.Add(new DivideWorkersWorkTestCase(1, null, "", new int[] { 1, 2, 2, 2, 7, 1, 3, 2 }, 3, true));
            TestSets["Lvl1"].TestCases.Add(new DivideWorkersWorkTestCase(1, null, "", new int[] { 2, 4, 8, 6, 7 }, 2, false));
            TestSets["Lvl1"].TestCases.Add(new DivideWorkersWorkTestCase(1, null, "", new int[] { 2, 4, 8, 6, 7 }, 10, true));
            TestSets["Lvl1"].TestCases.Add(new DivideWorkersWorkTestCase(1, null, "", new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 18, true));
            TestSets["Lvl1"].TestCases.Add(new DivideWorkersWorkTestCase(1, null, "", new int[] { 1, 2, 3, 4, 5, 6 }, 11, false));
            TestSets["Lvl1"].TestCases.Add(new DivideWorkersWorkTestCase(1, null, "", new int[] { 1, 1, 1, 1, 1, 1 }, 3, true));
            TestSets["Lvl1"].TestCases.Add(new DivideWorkersWorkTestCase(1, null, "", new int[] { 1, 1, 1, 1, 1, 1, 1 }, 4, false));
            TestSets["Lvl1"].TestCases.Add(new DivideWorkersWorkTestCase(1, null, "", new int[] { 5, 6, 2, 7 }, 10, false));

            TestSets["Lvl1perf"] = new TestSet(new WorkManager(), "Workers with blocks of equal sum - Performance tests");
            TestSets["Lvl1perf"].TestCases.Add(new DivideWorkersWorkTestCase(1, null, "3 numbers below expected sum", threeRelevant, 20, true));
            TestSets["Lvl1perf"].TestCases.Add(new DivideWorkersWorkTestCase(1, null, "Array of ones", ones, ones.Length / 2, true));
            TestSets["Lvl1perf"].TestCases.Add(new DivideWorkersWorkTestCase(35, null, "", bigNumbers, 300000003, true));
            TestSets["Lvl1perf"].TestCases.Add(new DivideWorkersWorkTestCase(30, null, "", GenerateRandomArray(660,12345671), 4520, true));
            TestSets["Lvl1perf"].TestCases.Add(new DivideWorkersWorkTestCase(30, null, "", GenerateRandomArray(2276,345679), 1303, false));

            TestSets["Lvl2"] = new TestSet(new WorkManager(), "Workers with closest block count");
            TestSets["Lvl2"].TestCases.Add(new DivideWorkersWorkWithEqualSumTestCase(1, null, "", new int[] { 2, 1, 8, 1, 2, 2 }, 2, 0, true));
            TestSets["Lvl2"].TestCases.Add(new DivideWorkersWorkWithEqualSumTestCase(1, null, "", new int[] { 2, 1, 8, 1, 2, 2 }, 8, 4, true));
            TestSets["Lvl2"].TestCases.Add(new DivideWorkersWorkWithEqualSumTestCase(1, null, "", new int[] { 4, 3, 1, 1, 1, 1, 1, 1, 1, 1 }, 4, 0, true));
            TestSets["Lvl2"].TestCases.Add(new DivideWorkersWorkWithEqualSumTestCase(1, null, "", new int[] { 1, 3, 2, 4 }, 4, 1, true));
            TestSets["Lvl2"].TestCases.Add(new DivideWorkersWorkWithEqualSumTestCase(1, null, "", new int[] { 1, 3, 2, 4 }, 5, 0, true));
            TestSets["Lvl2"].TestCases.Add(new DivideWorkersWorkWithEqualSumTestCase(1, null, "", new int[] { 1, 2, 3 }, 4, 0, false));

            TestSets["Lvl2perf"] = new TestSet(new WorkManager(), "Workers with closest block count - Performance tests");
            TestSets["Lvl2perf"].TestCases.Add(new DivideWorkersWorkWithEqualSumTestCase(1, null, "Answer in 5 first elements of array", relevantAtBeginning, 60, 0, true));
            TestSets["Lvl2perf"].TestCases.Add(new DivideWorkersWorkWithEqualSumTestCase(25, null, "...16 16 4 4 2 2 1 1", twoPower, twoPower[0], 1, true));
            TestSets["Lvl2perf"].TestCases.Add(new DivideWorkersWorkWithEqualSumTestCase(1, null, "Array of ones, expected sum not achievable", bigOnes, bigOnes.Length / 2 + 1, 0, false));
            TestSets["Lvl2perf"].TestCases.Add(new DivideWorkersWorkWithEqualSumTestCase(100, null, "", GenerateRandomArray(122,12345673), 15434, 0, false));
            TestSets["Lvl2perf"].TestCases.Add(new DivideWorkersWorkWithEqualSumTestCase(60, null, "", GenerateRandomArray(900,12345674, 100), 1555, 0, true));
        }

        public override double ScoreResult(out string message)
        {
            message = "OK";
            return 1;
        }

        private int[] GenerateRandomArray(int size, int seed, int max = 100000)
        {
            Random rnd = new Random(seed);
            var arr = new int[size];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = rnd.Next(max);
            }
            return arr;
        }
    }

}
