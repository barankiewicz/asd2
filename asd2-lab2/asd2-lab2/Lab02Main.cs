
namespace ASD
{
    using System;

    public class CarpentersBenchTestCase : TestCase
    {

        private (int length, int width) sheet;
        private (int length, int width, int price)[] elements1;
        private (int length, int width, int price)[] elements2;
        private int expectedResult;
        private int result;
        private Cut cuts;

        public CarpentersBenchTestCase(double timeLimit, Exception expectedException, string description, (int length, int width) sheet, (int length, int width, int price)[] elements, int result)
               : base(timeLimit, expectedException, description)
        {
            this.sheet = sheet;
            this.elements1 = ((int length, int width, int price)[])elements.Clone();
            this.elements2 = ((int length, int width, int price)[])elements.Clone();
            this.expectedResult = result;
        }

        public override void PerformTestCase()
        {
            result = new CarpentersBench().Cut(sheet, ((int length, int width, int price)[])elements2, out cuts);
        }

        public override void VerifyTestCase(out Result resultCode, out string message, object settings = null)
        {
            if (result != expectedResult)
            {
                resultCode = Result.BadResult;
                message = $"incorrect result: {result} (expected: {expectedResult})";
                return;
            }
            for (int i = 0; i < elements1.Length; ++i)
                if (!elements1[i].Equals(elements2[i]))
                {
                    resultCode = Result.BadResult;
                    message = $"illegal parameter change: elements[{i}]";
                    return;
                }
            if ((bool)settings)
            {
                if (cuts == null || sheet.length != cuts.length || sheet.width != cuts.width || result != cuts.price || !VerifyCutting(cuts))
                {
                    resultCode = Result.BadResult;
                    message = $"invalid cutting description";
                    return;
                }
            }
            resultCode = Result.Success;
            message = "OK";
        }

        private bool VerifyCutting(Cut cuts)
        {
            if (cuts.n == 0)
            {
                if (cuts.topleft != null || cuts.bottomright != null) return false;
                for (int i = 0; i < elements1.Length; ++i)
                    if (elements1[i].length == cuts.length && elements1[i].width == cuts.width)
                        return elements1[i].price == cuts.price;
                return cuts.price == 0;
            }
            if (cuts.topleft == null || cuts.bottomright == null) return false;
            if (cuts.topleft.price + cuts.bottomright.price != cuts.price) return false;
            if (cuts.vertical)
            {
                if (cuts.n < 1 || cuts.n >= cuts.width) return false;
                if (cuts.topleft.width != cuts.n || cuts.bottomright.width != cuts.width - cuts.n) return false;
                if (cuts.topleft.length != cuts.length || cuts.bottomright.length != cuts.length) return false;
            }
            else
            {
                if (cuts.n < 1 || cuts.n > cuts.length) return false;
                if (cuts.topleft.length != cuts.n || cuts.bottomright.length != cuts.length - cuts.n) return false;
                if (cuts.topleft.width != cuts.width || cuts.bottomright.width != cuts.width) return false;
            }
            return VerifyCutting(cuts.topleft) && VerifyCutting(cuts.bottomright);
        }

    }

    class CarpentersBenchTestModule : TestModule
    {

        public override void PrepareTestSets()
        {
            TestSets["LabResultOnlyTests"] = new TestSet("Part A - result only tests", null, false);
            TestSets["LabResultOnlyTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (5, 5), new(int length, int width, int price)[] { (5, 5, 1) }, 1));
            TestSets["LabResultOnlyTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (5, 5), new(int length, int width, int price)[] { (2, 3, 1) }, 2));
            TestSets["LabResultOnlyTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (5, 11), new(int length, int width, int price)[] { (4, 4, 21), (3, 3, 13), (2, 4, 10), (5, 1, 6), (1, 2, 2) }, 71));
            TestSets["LabResultOnlyTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (2, 3), new(int length, int width, int price)[] { (4, 5, 30) }, 0));
            TestSets["LabResultOnlyTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (2, 3), new(int length, int width, int price)[] { (4, 5, 30), (1, 1, 2), (2, 1, 1) }, 12));
            TestSets["LabResultOnlyTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (2, 3), new(int length, int width, int price)[] { (2, 1, 1), (1, 2, 2) }, 5));
            TestSets["LabResultOnlyTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (1, 11), new(int length, int width, int price)[] { (1, 3, 5), (1, 2, 3), (1, 1, 1) }, 18));
            TestSets["LabResultOnlyTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (11, 1), new(int length, int width, int price)[] { (3, 1, 5), (2, 1, 3), (1, 1, 1) }, 18));
            TestSets["LabResultOnlyTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (500, 500), new(int length, int width, int price)[] { (1, 1, 2), (10, 10, 11), (100, 100, 100), (500, 500, 100) }, 500000));

            TestSets["LabFullTests"] = new TestSet("Part B - full tests", null, true);
            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (5, 5), new(int length, int width, int price)[] { (5, 5, 1) }, 1));
            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (5, 5), new(int length, int width, int price)[] { (2, 3, 1) }, 2));
            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (5, 11), new(int length, int width, int price)[] { (4, 4, 21), (3, 3, 13), (2, 4, 10), (5, 1, 6), (1, 2, 2) }, 71));
            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (2, 3), new(int length, int width, int price)[] { (4, 5, 30) }, 0));
            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (2, 3), new(int length, int width, int price)[] { (4, 5, 30), (1, 1, 2), (2, 1, 1) }, 12));
            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (2, 3), new(int length, int width, int price)[] { (2, 1, 1), (1, 2, 2) }, 5));
            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (1, 11), new(int length, int width, int price)[] { (1, 3, 5), (1, 2, 3), (1, 1, 1) }, 18));
            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (11, 1), new(int length, int width, int price)[] { (3, 1, 5), (2, 1, 3), (1, 1, 1) }, 18));
            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (500, 500), new(int length, int width, int price)[] { (1, 1, 2), (10, 10, 11), (100, 100, 100), (500, 500, 100) }, 500000));
        }

        public override double ScoreResult(out string message)
        {
            message = "OK";
            return 1;
        }

    }

    class Lab02Main
    {

        public static void Main()
        {
            CarpentersBenchTestModule carpentersBenchTest = new CarpentersBenchTestModule();
            carpentersBenchTest.PrepareTestSets();

            foreach (var ts in carpentersBenchTest.TestSets)
            {
                Console.WriteLine($"\n{ts.Value.Description}");
                ts.Value.PerformTests(verbose: true, checkTimeLimit: false);
            }

            //double grade = carpentersBenchTest.ScoreResult(out string _);
            //Console.Out.WriteLine($"\nOcena: {grade}\n");
        }

    }

}