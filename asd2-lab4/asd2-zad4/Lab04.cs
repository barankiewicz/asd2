using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASD
{   
   
    public class PathThereTestCase : TestCase
    {
        int[,] M;
        int expectedResult;
        public int X { get { return M.GetLength(0); } }
        public int Y { get { return M.GetLength(1); } }

        int result;
        List<Point> path;

        public PathThereTestCase(double timeLimit, Exception expectedException, string description, int[,] M, int expectedResult)
                : base(timeLimit, expectedException, description)
        {
            this.M = (int[,])M.Clone();
            this.expectedResult = expectedResult;
        }

        public override void PerformTestCase(object prototypeObject)
        {
            AdventurePlanner ap = (AdventurePlanner)prototypeObject;
            result = ap.FindPathThere((int[,])M.Clone(), out path);
            if (path!=null && X <= ap.MaxToShow && Y <= ap.MaxToShow)
            {
                Lab04.PrintSol(M, path);
            }
        }

        public override void VerifyTestCase(out Result resultCode, out string message, object settings)
        {
            if (result != expectedResult)
            {
                message = $"zwraca {result}, a nie {expectedResult}";
                resultCode = Result.BadResult;
                return;
            }
            message = "wynik OK, ";                

            if (path == null)
            {
                message += "brak drogi";
                resultCode = Result.BadResult;
                return;
            }                

            int res = 0;
            Point home = new Point(0, 0);
            Point target = new Point(X - 1, Y - 1);
            if (!path[0].Equals(home))
            {
                message += "droga nie zaczyna się z (0,0)";
                resultCode = Result.BadResult;
                return;                                
            }
            if (!(path[path.Count - 1].Equals(target)))
            {
                message+="droga nie kończy się w (X-1,Y-1)";
                resultCode = Result.BadResult;
                return;
            }
            Point prev = new Point(-1,-1);
            foreach (var p in path)
            {

                if (p.X < 0 || p.X >= X || p.Y < 0 || p.Y >= Y)
                {
                    message += "pole spoza zakresu";
                    resultCode = Result.BadResult;
                    return;
                }
                res += M[p.X, p.Y];
                M[p.X, p.Y] = 0;
                if (prev.X ==-1)
                {
                    prev = p;
                    continue;
                }
                if (!
                     (
                     (prev.X == p.X && prev.Y + 1 == p.Y) ||
                     (prev.X + 1 == p.X && prev.Y == p.Y)))
                {
                    message += "nieprawidłowy ruch";
                    resultCode = Result.BadResult;
                    return;
                }
                prev = p;
            }
            if (res != expectedResult)
            {
               message+=$"droga o wartości {res}, a nie {expectedResult}";
                resultCode = Result.BadResult;
                return;
            }
           message+="droga OK";
            resultCode = Result.Success;
        }
    }
    
    public class PathThereAndBackTestCase : TestCase
    {
        int[,] M;
        int expectedResult;
        public int X { get { return M.GetLength(0); } }
        public int Y { get { return M.GetLength(1); } }

        int result;
        List<Point> path;

        public PathThereAndBackTestCase(double timeLimit, Exception expectedException, string description, int[,] M, int expectedResult)
                : base(timeLimit, expectedException, description)
        {
            this.M = (int[,])M.Clone();
            this.expectedResult = expectedResult;
        }

        public override void PerformTestCase(object prototypeObject)
        {
            AdventurePlanner ap = (AdventurePlanner)prototypeObject;
            result = ap.FindPathThereAndBack((int[,])M.Clone(), out path);
            if (path != null && X <= ap.MaxToShow && Y <= ap.MaxToShow)
            {
                Lab04.PrintSol(M, path);
            }
        }

        public override void VerifyTestCase(out Result resultCode, out string message, object settings)
        {
            if (result != expectedResult)
            {
                message = $"zwraca {result}, a nie {expectedResult}";
                resultCode = Result.BadResult;
                return;
            }
            message = "wynik OK, ";

            if (path == null)
            {
                message += "brak drogi";
                resultCode = Result.BadResult;
                return;
            }

            int res = 0;
            Point home = new Point(0, 0);
            Point target = new Point(X - 1, Y - 1);
            if (!path[0].Equals(home))
            {
                message+="droga nie zaczyna się z (0,0)";
                resultCode = Result.BadResult;
                return;
            }
            if (!(path[path.Count - 1].Equals(home)))
            {
                message+="droga nie kończy się w (0,0)";
                resultCode = Result.BadResult;
                return;
            }
            if (!path.Contains(target))
            {
                message+="droga nie zawiera (X-1,Y-1)";
                resultCode = Result.BadResult;
                return;
            }
            Point prev = new Point(-1,-1);
            bool firsthalf = true;
            foreach (var p in path)
            {
                if (p.X < 0 || p.X >= X || p.Y < 0 || p.Y >= Y)
                {
                    message+="pole spoza zakresu";
                    resultCode = Result.BadResult;
                    return;
                }
                res += M[p.X, p.Y];
                M[p.X, p.Y] = 0;
                if (prev.X == -1)
                {
                    prev = p;
                    continue;
                }

                if (firsthalf && !
                     (
                     (prev.X == p.X && prev.Y + 1 == p.Y) ||
                     (prev.X + 1 == p.X && prev.Y == p.Y)))
                {
                   message+="nieprawidłowy ruch tam";
                    resultCode = Result.BadResult;
                    return;
                }
                if (!firsthalf && !
                     (
                     (prev.X == p.X && prev.Y - 1 == p.Y) ||
                     (prev.X - 1 == p.X && prev.Y == p.Y)))
                {
                    message+="nieprawidłowy ruch z powrotem";
                    resultCode = Result.BadResult;
                    return;
                }
                prev = p;
                if (p.Equals(target))
                    firsthalf = false;
            }            
            if (res != expectedResult)
            {
                message += $"droga o wartości {res}, a nie {expectedResult}";
                resultCode = Result.BadResult;
                return;
            }
            message += "droga OK";
            resultCode = Result.Success;
        }
    }

    class Lab04TestModule : TestModule
    {
        public override void PrepareTestSets()
        {
            List<int[,]> tables = new List<int[,]>();
            List<int> thereRes = new List<int>();
            List<int> thereAndBackRes = new List<int>();
            List<string> descriptions = new List<string>();
            int[,] M;

            // test bez żadnych utrudnień
            M = new int[5, 5];
            for (int i = 0; i < 5; ++i)
            {
                M[0, i] = M[i, 4] = 2;
                M[i, 0] = M[4, i] = 1;
            }
            M[0, 0] = M[4, 4] = 0;
            tables.Add(M);
            thereRes.Add(14);
            thereAndBackRes.Add(21);
            descriptions.Add("dwie ścieżki, żadnego wyboru, żadnych utrudnień");

            // test bez skarbów
            M = new int[10, 10];
            tables.Add(M);
            thereRes.Add(0);
            thereAndBackRes.Add(0);
            descriptions.Add("test bez skarbów");

            // test, który sprawdza czy poprawnie zbieramy 
            // skarby ze startu i mety
            M = new int[5, 5];
            for (int i = 0; i < 5; ++i)
            {
                M[0, i] = M[i, 4] = 2;
                M[i, 0] = M[4, i] = 1;
            }
            M[0, 0] = 10;
            M[4, 4] = 10;
            tables.Add(M);
            thereRes.Add(34);
            thereAndBackRes.Add(41);
            descriptions.Add("test, który sprawdza czy poprawnie zbieramy  skarby ze startu i mety");

            // test zdegenerowany - 1 element
            M = new int[1, 1] { { 1} };
            tables.Add(M);
            thereRes.Add(1);
            thereAndBackRes.Add(1);
            descriptions.Add("test zdegenerowany: 1 element");

            // test zdegenerowany  w jednym wymiarze
            M = new int[1, 5] { { 1, 1,1,1,1 } };
            tables.Add(M);
            thereRes.Add(5);
            thereAndBackRes.Add(5);
            descriptions.Add("test zdegenerowany: 1 x 5");

            // test zdegenerowany w drugim wymiarze
            M = new int[5, 1] { { 1 }, { 1 }, { 1 }, { 1 }, { 1 } };
            tables.Add(M);
            thereRes.Add(5);
            thereAndBackRes.Add(5);
            descriptions.Add("test zdegenerowany: 5 x 1");


            // test niesymetryczny X<Y
            M = new int[5, 10];
            for (int i = 0; i < 5; ++i)
                for (int j = 0; j < 10; ++j)
                    M[i, j] = (i + j) % 5;

            tables.Add(M);
            thereRes.Add(26);
            thereAndBackRes.Add(49);
            descriptions.Add("test niesymetryczny: 5 x 10");

            // test niesymetryczny X>Y
            M = new int[10, 5];
            for (int i = 0; i < 10; ++i)
                for (int j = 0; j < 5; ++j)
                    M[i, j] = (i + j) % 6;

            tables.Add(M);
            thereRes.Add(31);
            thereAndBackRes.Add(61);
            descriptions.Add("test niesymetryczny: 10 x 5");

            // test w którym nie opłaca się dwa razy szukać optymalnej ścieżki, ale trzeba szukać ich naraz
            tables.Add(new int[3, 3] { { 5, 2, 9 }, { 1, 10, 1 }, { 0, 1, 5 } });
            thereRes.Add(23);
            thereAndBackRes.Add(34);
            descriptions.Add("test, w którym nie opłaca się dwa razy szukać optymalnej ścieżki, ale trzeba szukać ich naraz");
        

            // kilka większych testów losowych
            Random rnd = new Random(2018);
            int Z = 20;
            M = new int[Z, Z];
            for (int i = 0; i < Z; ++i)
                for (int j = 0; j < Z; ++j)
                    M[i, j] = rnd.Next() % 20;
            tables.Add(M);
            thereRes.Add(542);
            thereAndBackRes.Add(1004);
            descriptions.Add("test losowy 20 x 20");

            Z = 50;
            M = new int[Z, Z];
            for (int i = 0; i < Z; ++i)
                for (int j = 0; j < Z; ++j)
                    M[i, j] = rnd.Next() % 20;
            tables.Add(M);
            thereRes.Add(1368);
            thereAndBackRes.Add(2673);
            descriptions.Add("test losowy 50 x 50");

            Z = 70;
            M = new int[Z, Z];
            for (int i = 0; i < Z; ++i)
                for (int j = 0; j < Z; ++j)
                    M[i, j] = rnd.Next() % 20;
            tables.Add(M);
            thereRes.Add(1937);
            thereAndBackRes.Add(3769);
            descriptions.Add("test losowy 70 x 70");

            Z = 100;
            M = new int[Z, Z];
            for (int i = 0; i < Z; ++i)
                for (int j = 0; j < Z; ++j)
                    M[i, j] = rnd.Next() % 20;
            tables.Add(M);
            thereRes.Add(2869);
            thereAndBackRes.Add(5644);
            descriptions.Add("test losowy 100 x 100");


            TestSets["LabPathThereTests"] = new TestSet(new AdventurePlanner(), "Part 1 - path there [1.5 + 0.5]", null, false);
            TestSets["LabPathThereAndBackTests"] = new TestSet(new AdventurePlanner(), "Part 2 - path there and back [1.5 + 0.5]", null, false);

            for (int i = 0; i < tables.Count; ++i)
            {
                TestSets["LabPathThereTests"].TestCases.Add(new PathThereTestCase(1, null, "", tables[i], thereRes[i]));
                TestSets["LabPathThereAndBackTests"].TestCases.Add(new PathThereAndBackTestCase(1, null, "", tables[i], thereAndBackRes[i]));
            }
        }
        public override double ScoreResult(out string message)
        {
            message = "OK";
            return 1;
        }
    }


    class Lab04
    {
        public static void PrintSol(int[,] M, List<Point> path)
        {
            int X = M.GetLength(0);
            int Y = M.GetLength(1);

            for (int y = 0; y < Y; ++y)
            {
                Console.WriteLine();
                Console.Write("| ");
                for (int x = 0; x < X; ++x)
                {
                    Point p = new Point(x, y);
                    if (path.Contains(p))
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                    }
                    Console.Write("{0,2} ", M[x, y]);
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                Console.Write(" |");
            }
            Console.WriteLine();
        }


        static void Main(string[] args)
        {
            

            Lab04TestModule lab04test = new Lab04TestModule();
            lab04test.PrepareTestSets();

            foreach (var ts in lab04test.TestSets)
            {
                ts.Value.PerformTests(verbose: true, checkTimeLimit: false);
            }


        }

        

    }
}
