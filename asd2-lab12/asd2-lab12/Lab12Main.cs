using ASD;
using System;
using System.Collections.Generic;

namespace asd2
{
    public class CheckIntersectionTestCase : TestCase
    {
        readonly Street s1;
        readonly Street s2;
        readonly int expectedResult;

        int result;

        public CheckIntersectionTestCase(double timeLimit, Exception expectedException, string description, Street s1, Street s2, int expectedResult) : base(timeLimit, expectedException, description)
        {
            this.s1 = s1;
            this.s2 = s2;
            this.expectedResult = expectedResult;
        }

        public override void PerformTestCase(object prototypeObject)
        {
            City city = (City)prototypeObject;
            result = city.CheckIntersection(s1, s2);
        }

        public override void VerifyTestCase(out Result resultCode, out string message, object settings)
        {
            if (result != expectedResult)
            {
                message = $"Zły wynik, oczekiwano {expectedResult}, zwrócono {result}";
                resultCode = Result.BadResult;
                return;
            }

            message = $"OK (czas:{PerformanceTime,6:#0.000} jednostek)";
            resultCode = Result.Success;
        }
    }


    public class CheckStreetsPairsTestCase : TestCase
    {
        readonly Street[] streets;
        readonly int[] streetsToCheck1;
        readonly int[] streetsToCheck2;
        readonly bool[] expectedResult;

        bool[] result;

        public CheckStreetsPairsTestCase(double timeLimit, Exception expectedException, string description, Street[] streets, int[] streetsToCheck1, int[] streetsToCheck2, bool[] expectedResult) : base(timeLimit, expectedException, description)
        {
            this.streets = streets;
            this.streetsToCheck1 = streetsToCheck1;
            this.streetsToCheck2 = streetsToCheck2;
            this.expectedResult = expectedResult;
        }

        public override void PerformTestCase(object prototypeObject)
        {
            City city = (City)prototypeObject;
            result = city.CheckStreetsPairs(streets, streetsToCheck1, streetsToCheck2);
        }

        public override void VerifyTestCase(out Result resultCode, out string message, object settings)
        {
            if (result == null || result.Length == 0)
            {
                resultCode = Result.BadResult;
                message = "Pusta lista wynikowa";
                return;
            }

            if (result.Length != expectedResult.Length)
            {
                resultCode = Result.BadResult;
                message = "Nieprawidłowa liczba elementów w zwróconej tablicy";
                return;
            }

            for (int i = 0; i < result.Length; i++)
                if (result[i] != expectedResult[i])
                {
                    resultCode = Result.BadResult;
                    message = $"Zły wynik. Na miejscu {i} oczekiwano {expectedResult[i]} a zwrócono {result[i]}";
                    return;
                }

            message = $"OK (czas:{PerformanceTime,6:#0.000} jednostek)";
            resultCode = Result.Success;
        }
    }


    public class GetIntersectionPointTestCase : TestCase
    {
        readonly Street s1;
        readonly Street s2;
        readonly Point expectedResult;

        Point result;

        public GetIntersectionPointTestCase(double timeLimit, Exception expectedException, string description, Street s1, Street s2, Point expectedResult) : base(timeLimit, expectedException, description)
        {
            this.s1 = s1;
            this.s2 = s2;
            this.expectedResult = expectedResult;
        }

        public override void PerformTestCase(object prototypeObject)
        {
            City city = (City)prototypeObject;
            result = city.GetIntersectionPoint(s1, s2);
        }

        public override void VerifyTestCase(out Result resultCode, out string message, object settings)
        {
            double EPS = 0.00001;
            if (Math.Abs(result.x - expectedResult.x) > EPS || Math.Abs(result.y - expectedResult.y) > EPS)
            {
                message = $"Zły wynik, oczekiwano {expectedResult}, zwrócono {result}";
                resultCode = Result.BadResult;
                return;
            }

            message = $"OK (czas:{PerformanceTime,6:#0.000} jednostek)";
            resultCode = Result.Success;
        }
    }

    public class CheckDistrictsTestCase : TestCase
    {
        readonly Street[] streets;
        readonly Point[] district1;
        readonly Point[] district2;
        readonly bool expectedResult;
        readonly int expectedPathLength;

        bool result;
        List<int> path;
        List<Point> intersections;
        List<int> streetsToDistrict1;
        List<int> streetsToDistrict2;

        public CheckDistrictsTestCase(double timeLimit, Exception expectedException, string description, Street[] streets,
            Point[] district1, Point[] district2, bool expectedResult, int expectedPathLength, List<int> streetsToDistrict1,
            List<int> streetsToDistrict2) : base(timeLimit, expectedException, description)
        {
            this.streets = streets;
            this.district1 = district1;
            this.district2 = district2;
            this.expectedResult = expectedResult;
            this.expectedPathLength = expectedPathLength;
            this.streetsToDistrict1 = streetsToDistrict1;
            this.streetsToDistrict2 = streetsToDistrict2;
        }

        public override void PerformTestCase(object prototypeObject)
        {
            City city = (City)prototypeObject;
            result = city.CheckDistricts(streets, district1, district2, out path, out intersections);
        }

        public override void VerifyTestCase(out Result resultCode, out string message, object settings)
        {
            double EPS = 0.00001;

            if (result != expectedResult)
            {
                message = $"Zły wynik, oczekiwano {expectedResult}, zwrócono {result}";
                resultCode = Result.BadResult;
                return;
            }
            if (path == null || intersections == null)
            {
                message = $"Wynik OK, pusta ścieżka lub lista przecięć";
                resultCode = Result.BadResult;
                return;
            }
            if (path.Count != expectedPathLength)
            {
                message = $"Wynik OK, nieprawidłowa długość znalezionej ścieżki: oczekiwano {expectedPathLength}, zwrócono {path.Count}";
                resultCode = Result.BadResult;
                return;
            }
            if (path.Count > 0 && path.Count != intersections.Count + 1)
            {
                message = $"Wynik OK, nieprawidłowa długość listy przecięć";
                resultCode = Result.BadResult;
                return;
            }

            //sprawdzenie początku i końca ścieżki (czy są w odpowiednich dzielnicach)
            if (path.Count > 0 && !streetsToDistrict1.Contains(path[0]))
            {
                message = $"Wynik OK, pierwszą ulicą zwróconej ścieżki ({path[0]}) nie można dojechać do pierwszej dzielnicy";
                resultCode = Result.BadResult;
                return;
            }
            if (path.Count > 0 && !streetsToDistrict2.Contains(path[path.Count - 1]))
            {
                message = $"Wynik OK, ostatnią ulicą zwróconej ścieżki ({path[path.Count - 1]}) nie można dojechać do drugiej dzielnicy";
                resultCode = Result.BadResult;
                return;
            }

            //sprawdzenie czy kolejne odcinki zawierają punkty przecięć
            for (int i = 0; i < path.Count - 1; i++)
            {
                Street s = streets[path[i]];
                if (Point.CrossProduct(s.p2 - s.p1, intersections[i] - s.p1) > EPS
                    || !(Math.Min(s.p1.x, s.p2.x) <= intersections[i].x + EPS && intersections[i].x <= Math.Max(s.p1.x, s.p2.x) + EPS && Math.Min(s.p1.y, s.p2.y) <= intersections[i].y + EPS && intersections[i].y <= Math.Max(s.p1.y, s.p2.y) + EPS)
                    )
                {
                    message = $"Wynik OK, {i}-ty punkt przecięcia nie leży na {i}-tym odcinku {s.p1} ++ {s.p2} != {intersections[i]}";
                    resultCode = Result.BadResult;
                    return;
                }
                s = streets[path[i + 1]];
                if (Point.CrossProduct(s.p2 - s.p1, intersections[i] - s.p1) > EPS
                    || !(Math.Min(s.p1.x, s.p2.x) <= intersections[i].x + EPS && intersections[i].x <= Math.Max(s.p1.x, s.p2.x) + EPS && Math.Min(s.p1.y, s.p2.y) <= intersections[i].y + EPS && intersections[i].y <= Math.Max(s.p1.y, s.p2.y) + EPS)
                    )
                {
                    message = $"Wynik OK, {i}-ty punkt przecięcia nie leży na {i + 1}-tym odcinku";
                    resultCode = Result.BadResult;
                    return;
                }
            }

            message = $"OK (czas:{PerformanceTime,6:#0.000} jednostek)";
            resultCode = Result.Success;
        }
    }


    public class Lab12TestModule : TestModule
    {
        List<Street[]> CreateCities()
        {
            Street[] c0 = new Street[] { //miasto z nieprawidłowymi ulicami (pokrywającymi się)
                new Street(new Point(-1,-1), new Point(1,1)),
                new Street(new Point(0,0), new Point(5,5)),
                new Street(new Point(1,1), new Point(3,3))
            };

            Street[] c1 = new Street[] { //wszystkie ulice współlilniowe, pierwsza i ostatnia oddzielone od pozostałych
                new Street(new Point(-11,-11), new Point(-100,-100)),
                new Street(new Point(0,0), new Point(1,1)),
                new Street(new Point(1,1), new Point(3,3)),
                new Street(new Point(0,0), new Point(-10,-10)),
                new Street(new Point(5,5), new Point(3,3)),
                new Street(new Point(6,6), new Point(5,5)),
                new Street(new Point(7,7), new Point(6.5,6.5))
            };

            Street[] c2 = new Street[] { //wszystkie ulice rozłączne, ułożone w sposób losowy
                new Street(new Point(0,0), new Point(0,0.5)),
                new Street(new Point(5,5), new Point(6,6)),
                new Street(new Point(-1,0), new Point(-1,-10)),
                new Street(new Point(4,5), new Point(5,6)),
                new Street(new Point(100,100), new Point(100,-10)),
                new Street(new Point(0.6,0.6), new Point(0.7,0.8))
            };

            Street[] c3 = new Street[] { //dwie długie równoległe ulice z "odnogami"
                new Street(new Point(0,0), new Point(60,0)),
                new Street(new Point(0,10), new Point(60,10)),
                new Street(new Point(0,5), new Point(0,0)),
                new Street(new Point(10,5), new Point(10,0)),
                new Street(new Point(20,6), new Point(20,-6)),
                new Street(new Point(30,9), new Point(30,-1)),
                new Street(new Point(40,10), new Point(40,11)),
                new Street(new Point(50,15), new Point(50,5)),
                new Street(new Point(60,5), new Point(60,10)),
            };

            Street[] c4 = new Street[] { //3 rozłączne kwadraty o wspólnym środku
                new Street(new Point(-1,-1), new Point(-1,1)),
                new Street(new Point(-1,1), new Point(1,1)),
                new Street(new Point(1,-1), new Point(1,1)),
                new Street(new Point(1,-1), new Point(-1,-1)),

                new Street(new Point(-2,-2), new Point(-2,2)),
                new Street(new Point(-2,2), new Point(2,2)),
                new Street(new Point(2,-2), new Point(2,2)),
                new Street(new Point(2,-2), new Point(-2,-2)),

                new Street(new Point(-3,-3), new Point(-3,3)),
                new Street(new Point(-3,3), new Point(3,3)),
                new Street(new Point(3,3), new Point(3,-3)),
                new Street(new Point(3,-3), new Point(-3,-3))
            };

            Street[] c5 = new Street[] { //tylko jedna ulica
                new Street(new Point(-5,-6), new Point(7,10))
            };

            Street[] c6 = new Street[] { //dwie rozłączne nieregularne gwiazdy
                new Street(new Point(0,0), new Point(5,5)),
                new Street(new Point(0,0), new Point(0,3)),
                new Street(new Point(0,0), new Point(2,5)),
                new Street(new Point(0,0), new Point(-1,5)),
                new Street(new Point(0,0), new Point(1,-3)),
                new Street(new Point(2,1), new Point(0,0)),

                new Street(new Point(10,10), new Point(12,12)),
                new Street(new Point(11,11), new Point(13,14)),
                new Street(new Point(11,11), new Point(10,9)),
                new Street(new Point(11,8), new Point(11,15))
            };

            Street[] c7 = new Street[] { //4 kwadraty, 3 z nich połączone
                new Street(new Point(0,0), new Point(1,0)),
                new Street(new Point(1,0), new Point(1,1)),
                new Street(new Point(1,1), new Point(0,1)),
                new Street(new Point(0,1), new Point(0,0)),

                new Street(new Point(5,5), new Point(6,5)),
                new Street(new Point(6,5), new Point(6,6)),
                new Street(new Point(6,6), new Point(5,6)),
                new Street(new Point(5,6), new Point(5,5)),

                new Street(new Point(0,5), new Point(1,5)),
                new Street(new Point(1,5), new Point(1,6)),
                new Street(new Point(1,6), new Point(0,6)),
                new Street(new Point(0,6), new Point(0,5)),

                new Street(new Point(5,0), new Point(6,0)),
                new Street(new Point(6,0), new Point(6,1)),
                new Street(new Point(6,1), new Point(5,1)),
                new Street(new Point(5,1), new Point(5,0)),

                //łączenia
                new Street(new Point(1,1), new Point(5,5)),
                new Street(new Point(1,0), new Point(0,5))
            };

            //duży test wydajnościowy, 2000 rozłącznych ulic
            Street[] c8 = new Street[2000];
            for (int i = 0; i < 2000; i++)
                c8[i] = new Street(new Point(i, 5), new Point(i, -5));

            //duży test wydajnościowy, 500 połączonych ulic + 500 połączonych ulic
            List<Street> c9 = new List<Street>();
            for (int i = 0; i < 1000; i++)
            {
                c9.Add(new Street(new Point(i, i), new Point(i + 1, i + 1)));
                c9.Add(new Street(new Point(-i, -5), new Point(-i - 1, -5)));
            }

            return new List<Street[]>() { c0, c1, c2, c3, c4, c5, c6, c7, c8, c9.ToArray() };
        }

        public override void PrepareTestSets()
        {
            List<Street[]> cities = CreateCities();

            //--- Część I: CheckIntersection---
            TestSets["CheckIntersectionLabTests"] = new TestSet(new City(), "CheckIntersection - testy lab", null, false);
            TestSets["CheckIntersectionLabTests"].TestCases.Add(new CheckIntersectionTestCase(1, null, "", new Street(new Point(-1, -1), new Point(1, 1)), new Street(new Point(-1, 1), new Point(1, -1)), 1));
            TestSets["CheckIntersectionLabTests"].TestCases.Add(new CheckIntersectionTestCase(1, null, "", new Street(new Point(0, 0), new Point(1, 2)), new Street(new Point(-5, -4), new Point(0, 0)), 1));
            TestSets["CheckIntersectionLabTests"].TestCases.Add(new CheckIntersectionTestCase(1, null, "", new Street(new Point(5, 5), new Point(7, 7)), new Street(new Point(4, 4), new Point(5, 5)), 1));
            TestSets["CheckIntersectionLabTests"].TestCases.Add(new CheckIntersectionTestCase(1, null, "", new Street(new Point(5, 6), new Point(1, 2)), new Street(new Point(1, 2), new Point(5, 6)), int.MaxValue));
            TestSets["CheckIntersectionLabTests"].TestCases.Add(new CheckIntersectionTestCase(1, null, "", new Street(new Point(0, -1), new Point(0, -0.5)), new Street(new Point(0, -2), new Point(0, 2)), int.MaxValue));
            TestSets["CheckIntersectionLabTests"].TestCases.Add(new CheckIntersectionTestCase(1, null, "", new Street(new Point(5, 6), new Point(0, 6)), new Street(new Point(1, 6), new Point(-2, 6)), int.MaxValue));
            TestSets["CheckIntersectionLabTests"].TestCases.Add(new CheckIntersectionTestCase(1, null, "", new Street(new Point(-1, -1), new Point(1, 1)), new Street(new Point(-2, -1), new Point(0, 1)), 0));
            TestSets["CheckIntersectionLabTests"].TestCases.Add(new CheckIntersectionTestCase(1, null, "", new Street(new Point(0, -5.5), new Point(0, -6)), new Street(new Point(0, -5.4), new Point(0, -1)), 0));
            TestSets["CheckIntersectionLabTests"].TestCases.Add(new CheckIntersectionTestCase(1, null, "", new Street(new Point(0, 0), new Point(1, 2)), new Street(new Point(5, 0), new Point(4, 2)), 0));
            //My tests
            TestSets["CheckIntersectionLabTests"].TestCases.Add(new CheckIntersectionTestCase(1, null, "", new Street(new Point(0, 0), new Point(0, 2)), new Street(new Point(0, 3), new Point(0, 5)), 0));
            TestSets["CheckIntersectionLabTests"].TestCases.Add(new CheckIntersectionTestCase(1, null, "", new Street(new Point(0, 0), new Point(0, 2)), new Street(new Point(0, 2), new Point(0, 3)), 1));
            TestSets["CheckIntersectionLabTests"].TestCases.Add(new CheckIntersectionTestCase(1, null, "", new Street(new Point(0, 0), new Point(0, 2)), new Street(new Point(0, 2), new Point(0, 0)), int.MaxValue));
            TestSets["CheckIntersectionLabTests"].TestCases.Add(new CheckIntersectionTestCase(1, null, "", new Street(new Point(0, 0), new Point(0, 2)), new Street(new Point(0, 1), new Point(0, 5)), int.MaxValue));
            TestSets["CheckIntersectionLabTests"].TestCases.Add(new CheckIntersectionTestCase(1, null, "", new Street(new Point(0, 0), new Point(0, 2)), new Street(new Point(0, 0), new Point(0, 1)), int.MaxValue));
            TestSets["CheckIntersectionLabTests"].TestCases.Add(new CheckIntersectionTestCase(1, null, "", new Street(new Point(0, 0), new Point(0, 3)), new Street(new Point(0, 0), new Point(2, 5)), 1));

            //--- Część II: CheckStreetsPairs ---
            TestSets["CheckStreetsPairsLabTests"] = new TestSet(new City(), "CheckStreetsPairs - testy lab", null, false);
            TestSets["CheckStreetsPairsLabTests"].TestCases.Add(new CheckStreetsPairsTestCase(20, new ArgumentException(), "", cities[0],
                new int[] { },
                new int[] { },
                new bool[] { }));
            TestSets["CheckStreetsPairsLabTests"].TestCases.Add(new CheckStreetsPairsTestCase(20, null, "", cities[1],
                new int[] { 0, 1, 3, 6, 3, 6, 4, 1 },
                new int[] { 1, 2, 2, 0, 3, 5, 5, 5 },
                new bool[] { false, true, true, false, true, false, true, true }));
            TestSets["CheckStreetsPairsLabTests"].TestCases.Add(new CheckStreetsPairsTestCase(20, null, "", cities[2],
                new int[] { 0, 1, 5, 5, 3, 1, 1, 2 },
                new int[] { 1, 2, 2, 0, 1, 1, 5, 3 },
                new bool[] { false, false, false, false, false, true, false, false }));
            TestSets["CheckStreetsPairsLabTests"].TestCases.Add(new CheckStreetsPairsTestCase(20, null, "", cities[3],
                new int[] { 1, 0, 4, 2, 1, 1, 7, 7, 6 },
                new int[] { 0, 2, 0, 5, 6, 8, 8, 2, 3 },
                new bool[] { false, true, true, true, true, true, true, false, false }));
            TestSets["CheckStreetsPairsLabTests"].TestCases.Add(new CheckStreetsPairsTestCase(20, null, "", cities[4],
                new int[] { 0, 4, 9, 2, 1, 6, 7 },
                new int[] { 3, 5, 11, 5, 8, 10, 8 },
                new bool[] { true, true, true, false, false, false, false }));
            TestSets["CheckStreetsPairsLabTests"].TestCases.Add(new CheckStreetsPairsTestCase(20, null, "", cities[5],
                new int[] { 0 },
                new int[] { 0 },
                new bool[] { true }));
            TestSets["CheckStreetsPairsLabTests"].TestCases.Add(new CheckStreetsPairsTestCase(20, null, "", cities[6],
                new int[] { 0, 3, 1, 6, 8, 1, 5 },
                new int[] { 2, 5, 5, 7, 6, 7, 6 },
                new bool[] { true, true, true, true, true, false, false }));
            TestSets["CheckStreetsPairsLabTests"].TestCases.Add(new CheckStreetsPairsTestCase(20, null, "", cities[7],
                new int[] { 0, 4, 9, 13, 0, 5, 1, 5 },
                new int[] { 2, 5, 10, 15, 11, 10, 13, 15 },
                new bool[] { true, true, true, true, true, true, false, false }));
            List<int> c8ListCheckStreets1 = new List<int>();
            List<int> c8ListCheckStreets2 = new List<int>();
            List<bool> c8ResultCheckStreetsPairs = new List<bool>();
            for (int i = 0; i < 2000; i++)
                for (int j = 0; j < 2000; j++)
                    if (i != j)
                    {
                        c8ListCheckStreets1.Add(i);
                        c8ListCheckStreets2.Add(j);
                        c8ResultCheckStreetsPairs.Add(false);
                    }
            TestSets["CheckStreetsPairsLabTests"].TestCases.Add(new CheckStreetsPairsTestCase(20, null, "", cities[8], c8ListCheckStreets1.ToArray(), c8ListCheckStreets2.ToArray(), c8ResultCheckStreetsPairs.ToArray()));

            List<int> c9ListCheckStreets1 = new List<int>();
            List<int> c9ListCheckStreets2 = new List<int>();
            List<bool> c9ResultCheckStreetsPairs = new List<bool>();
            for (int i = 0; i < 1000; i += 2)
                for (int j = 0; j < 1000; j += 2)
                    if (i != j)
                    {
                        c9ListCheckStreets1.Add(i);
                        c9ListCheckStreets2.Add(j);
                        c9ResultCheckStreetsPairs.Add(true);
                    }
            for (int i = 1; i < 1000; i += 2)
                for (int j = 1; j < 1000; j += 2)
                    if (i != j)
                    {
                        c9ListCheckStreets1.Add(i);
                        c9ListCheckStreets2.Add(j);
                        c9ResultCheckStreetsPairs.Add(true);
                    }
            for (int i = 0; i < 1000; i += 2)
                for (int j = 1; j < 1000; j += 2)
                {
                    c9ListCheckStreets1.Add(i);
                    c9ListCheckStreets2.Add(j);
                    c9ResultCheckStreetsPairs.Add(false);
                }
            TestSets["CheckStreetsPairsLabTests"].TestCases.Add(new CheckStreetsPairsTestCase(20, null, "", cities[9], c9ListCheckStreets1.ToArray(), c9ListCheckStreets2.ToArray(), c9ResultCheckStreetsPairs.ToArray()));

            //--- Część III: GetIntersectionPoint ---
            TestSets["GetIntersectionPointLabTests"] = new TestSet(new City(), "GetIntersectionPoint - testy lab", null, false);
            TestSets["GetIntersectionPointLabTests"].TestCases.Add(new GetIntersectionPointTestCase(1, null, "", new Street(new Point(0, 0), new Point(3, 3)), new Street(new Point(0, 3), new Point(3, 0)), new Point(1.5, 1.5)));
            TestSets["GetIntersectionPointLabTests"].TestCases.Add(new GetIntersectionPointTestCase(1, null, "", new Street(new Point(0, 4), new Point(4, 2)), new Street(new Point(-4, -6), new Point(4, 6)), new Point(2, 3)));
            TestSets["GetIntersectionPointLabTests"].TestCases.Add(new GetIntersectionPointTestCase(1, null, "", new Street(new Point(0, 0), new Point(2, 8)), new Street(new Point(-5, 1), new Point(5, 1)), new Point(0.25, 1)));
            TestSets["GetIntersectionPointLabTests"].TestCases.Add(new GetIntersectionPointTestCase(1, null, "", new Street(new Point(0, 2.5), new Point(2.5, 2.5)), new Street(new Point(-5, -5), new Point(5, 5)), new Point(2.5, 2.5)));
            TestSets["GetIntersectionPointLabTests"].TestCases.Add(new GetIntersectionPointTestCase(1, null, "", new Street(new Point(-2, 5), new Point(-2, -5)), new Street(new Point(-2, -1), new Point(2, -1)), new Point(-2, -1)));
            TestSets["GetIntersectionPointLabTests"].TestCases.Add(new GetIntersectionPointTestCase(1, null, "", new Street(new Point(1, 1), new Point(2, 2)), new Street(new Point(1.234, -100), new Point(1.234, 1000)), new Point(1.234, 1.234)));
            TestSets["GetIntersectionPointLabTests"].TestCases.Add(new GetIntersectionPointTestCase(1, null, "", new Street(new Point(0, 0), new Point(1, 1)), new Street(new Point(1, 1), new Point(3, 3)), new Point(1, 1)));
            TestSets["GetIntersectionPointLabTests"].TestCases.Add(new GetIntersectionPointTestCase(1, null, "", new Street(new Point(10, 10), new Point(3, 3)), new Street(new Point(1, 1), new Point(3, 3)), new Point(3, 3)));
            TestSets["GetIntersectionPointLabTests"].TestCases.Add(new GetIntersectionPointTestCase(1, new ArgumentException(), "", new Street(new Point(1, 5), new Point(2, 3)), new Street(new Point(1, 5), new Point(2, 3)), new Point(0, 0)));
            TestSets["GetIntersectionPointLabTests"].TestCases.Add(new GetIntersectionPointTestCase(1, new ArgumentException(), "", new Street(new Point(-1, -1), new Point(1, 1)), new Street(new Point(-2, -1), new Point(0, 1)), new Point(0, 0)));
            TestSets["GetIntersectionPointLabTests"].TestCases.Add(new GetIntersectionPointTestCase(1, new ArgumentException(), "", new Street(new Point(0, 0), new Point(1, 2)), new Street(new Point(4, 0), new Point(3, 2)), new Point(0, 0)));


            //--- Część IV: CheckDistricts ---
            TestSets["CheckDistrictsLabTests"] = new TestSet(new City(), "CheckDistricts - testy lab", null, false);
            TestSets["CheckDistrictsLabTests"].TestCases.Add(new CheckDistrictsTestCase(20, null, "", cities[1],
                new Point[] { new Point(-11, -11), new Point(-11, -10), new Point(-10, -10), new Point(-10, -11) },
                new Point[] { new Point(5, 5), new Point(5, 7), new Point(7, 7), new Point(7, 5) },
                true, 4, new List<int> { 0, 3 }, new List<int> { 4, 5, 6 }));
            TestSets["CheckDistrictsLabTests"].TestCases.Add(new CheckDistrictsTestCase(20, null, "", cities[1],
                new Point[] { new Point(-12, -12), new Point(-12, -50), new Point(-50, -12) },
                new Point[] { new Point(5, 5), new Point(5, 100), new Point(100, 100), new Point(100, 5) },
                false, 0, new List<int> { 0 }, new List<int> { 4, 5 }));
            TestSets["CheckDistrictsLabTests"].TestCases.Add(new CheckDistrictsTestCase(20, null, "", cities[3],
                new Point[] { new Point(0.5, -0.5), new Point(0.5, 1), new Point(1, 1) },
                new Point[] { new Point(49.5, -1), new Point(50, -1), new Point(50, 1), new Point(49.5, 1) },
                true, 1, new List<int> { 0 }, new List<int> { 0 }));
            TestSets["CheckDistrictsLabTests"].TestCases.Add(new CheckDistrictsTestCase(20, null, "", cities[4],
                new Point[] { new Point(0.5, 0), new Point(100, -0.5), new Point(100, 0.5) },
                new Point[] { new Point(-0.5, 0), new Point(-100, -0.5), new Point(-100, 0.5) },
                true, 3, new List<int> { 2, 6, 10 }, new List<int> { 0, 4, 8 }));
            TestSets["CheckDistrictsLabTests"].TestCases.Add(new CheckDistrictsTestCase(20, null, "", cities[4],
                new Point[] { new Point(-3, -3), new Point(-3, -2), new Point(-2, -2), new Point(-2, -3) },
                new Point[] { new Point(3, 3), new Point(3, 2), new Point(2, 2), new Point(2, 3) },
                true, 2, new List<int> { 4, 7, 8, 11 }, new List<int> { 5, 6, 9, 10 }));
            TestSets["CheckDistrictsLabTests"].TestCases.Add(new CheckDistrictsTestCase(20, null, "", cities[5],
                new Point[] { new Point(-5, 6), new Point(-12, -50), new Point(-4, -1) },
                new Point[] { new Point(5, 2), new Point(7, 10), new Point(4, 4) },
                true, 1, new List<int> { 0 }, new List<int> { 0 }));
            TestSets["CheckDistrictsLabTests"].TestCases.Add(new CheckDistrictsTestCase(20, null, "", cities[6],
                new Point[] { new Point(-5, -1), new Point(-12, -50), new Point(-2, -1) },
                new Point[] { new Point(100, 0), new Point(1, 1), new Point(0, 100), new Point(100, 100) },
                false, 0, new List<int>(), new List<int> { 0, 2, 5 }));
            TestSets["CheckDistrictsLabTests"].TestCases.Add(new CheckDistrictsTestCase(20, null, "", cities[8],
                new Point[] { new Point(0, 100), new Point(100, 100), new Point(100, 0) },
                new Point[] { new Point(101, 200), new Point(200, 200), new Point(200, 101) },
                false, 0, new List<int> { 95, 96, 97, 98, 99, 100 }, new List<int>()));
            TestSets["CheckDistrictsLabTests"].TestCases.Add(new CheckDistrictsTestCase(20, null, "", cities[9],
                new Point[] { new Point(1, 1), new Point(-1, -1), new Point(1, 2) },
                new Point[] { new Point(499, 499), new Point(500, 500), new Point(499, 500) },
                true, 498, new List<int> { 0, 2 }, new List<int> { 996, 998 }));
        }

        public override double ScoreResult(out string message)
        {
            message = null;
            return 1.0;
        }
    }

    public class Lab12Main
    {
        public static void Main()
        {
            var testModule = new Lab12TestModule();
            testModule.PrepareTestSets();
            foreach (var testSet in testModule.TestSets)
            {
                testSet.Value.PerformTests(verbose: true, checkTimeLimit: false);
            }
        }
    }
}