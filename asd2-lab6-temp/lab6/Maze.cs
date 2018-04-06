using ASD.Graphs;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace ASD
{
    public class Maze : MarshalByRefObject
    {

        /// <summary>
        /// Wersje zadania I oraz II
        /// Zwraca najkrótszy możliwy czas przejścia przez labirynt bez dynamitów lub z dowolną ich liczbą
        /// </summary>
        /// <param name="maze">labirynt</param>
        /// <param name="withDynamite">informacja, czy dostępne są dynamity 
        /// Wersja I zadania -> withDynamites = false, Wersja II zadania -> withDynamites = true</param>
        /// <param name="path">zwracana ścieżka</param>
        /// <param name="t">czas zburzenia ściany (dotyczy tylko wersji II)</param> 
        public int FindShortestPath(char[,] maze, bool withDynamite, out string path, int t = 0)
        {
            int x = maze.GetLength(1);
            int y = maze.GetLength(0);
            int vNo = 0;
            int start = 0;
            int end = 0;

            Graph g = new AdjacencyListsGraph<AVLAdjacencyList>(true, x * y);
            for(int i = 0; i < y; i++)
            {
                for (int j = 0; j<x; j++)
                {
                    if (maze[i, j] == 'S')
                    {
                        start = vNo;
                        if (withDynamite) //Work on it if you have dynamite
                        {
                            bool leftStone = j >= 1 ? maze[i, j - 1] == 'X' : false;
                            bool rightStone = j < x - 1 ? maze[i, j + 1] == 'X' : false;
                            bool upStone = i >= 1 ? maze[i - 1, j] == 'X' : false;
                            bool downStone = i < y - 1 ? maze[i + 1, j] == 'X' : false;

                            if (leftStone)
                            {
                                maze[i, j - 1] = 'O';
                                g.AddEdge(vNo, vNo - 1, t);
                            }

                            if (rightStone)
                            {
                                maze[i, j + 1] = 'O';
                                g.AddEdge(vNo, vNo + 1, t);
                            }

                            if (upStone)
                            {
                                maze[i - 1, j] = 'O';
                                g.AddEdge(vNo, vNo - x, t);
                            }

                            if (downStone)
                            {
                                maze[i + 1, j] = 'O';
                                g.AddEdge(vNo, vNo + x, t);
                            }
                        }
                    }
                        

                    if (maze[i, j] == 'E')
                    {
                        end = vNo;
                        if (withDynamite) //Work on it if you have dynamite
                        {
                            bool leftStone = j >= 1 ? maze[i, j - 1] == 'X' : false;
                            bool rightStone = j < x - 1 ? maze[i, j + 1] == 'X' : false;
                            bool upStone = i >= 1 ? maze[i - 1, j] == 'X' : false;
                            bool downStone = i < y - 1 ? maze[i + 1, j] == 'X' : false;

                            if (leftStone)
                            {
                                maze[i, j - 1] = 'O';
                                g.AddEdge(vNo, vNo - 1, t);
                            }

                            if (rightStone)
                            {
                                maze[i, j + 1] = 'O';
                                g.AddEdge(vNo, vNo + 1, t);
                            }

                            if (upStone)
                            {
                                maze[i - 1, j] = 'O';
                                g.AddEdge(vNo, vNo - x, t);
                            }

                            if (downStone)
                            {
                                maze[i + 1, j] = 'O';
                                g.AddEdge(vNo, vNo + x, t);
                            }
                        }
                    }
                        

                    if (maze[i, j] == 'X') //Dont work on it if its X
                    {
                        if (withDynamite) //Work on it if you have dynamite
                        {
                            bool leftStone = j >= 1 ? maze[i, j - 1] == 'X' : false;
                            bool rightStone = j < x - 1 ? maze[i, j + 1] == 'X' : false;
                            bool upStone = i >= 1 ? maze[i - 1, j] == 'X' : false;
                            bool downStone = i < y - 1 ? maze[i + 1, j] == 'X' : false;

                            if (leftStone)
                            {
                                maze[i, j - 1] = 'O';
                                g.AddEdge(vNo, vNo - 1, t);
                            }

                            if (rightStone)
                            {
                                maze[i, j + 1] = 'O';
                                g.AddEdge(vNo, vNo + 1, t);
                            }

                            if (upStone)
                            {
                                maze[i - 1, j] = 'O';
                                g.AddEdge(vNo, vNo - x, t);
                            }

                            if (downStone)
                            {
                                maze[i + 1, j] = 'O';
                                g.AddEdge(vNo, vNo + x, t);
                            }
                        }
                        else
                        {
                            vNo++; //Increment vertex counter
                            continue;
                        }
                    }

                    //////////////Its an 'O' part///////////////////////

                    //The normal part
                    bool left = j >= 1 ? maze[i, j - 1] != 'X' : false;
                    bool right = j < x - 1 ? maze[i, j + 1] != 'X' : false;
                    bool up = i >= 1 ? maze[i - 1, j] != 'X' : false;
                    bool down = i < y - 1 ? maze[i + 1, j] != 'X' : false;

                    if (left)
                    {
                        g.AddEdge(vNo, vNo - 1);
                    }

                    if (right)
                    {
                        g.AddEdge(vNo, vNo + 1);
                    }

                    if (up)
                    {
                        g.AddEdge(vNo, vNo - x);
                    }

                    if (down)
                    {
                        g.AddEdge(vNo, vNo + x);
                    }

                    //The dynamite part
                    if (withDynamite)
                    {
                        bool leftStone = j >= 1 ? maze[i, j - 1] == 'X' : false;
                        bool rightStone = j < x - 1 ? maze[i, j + 1] == 'X' : false;
                        bool upStone = i >= 1 ? maze[i - 1, j] == 'X' : false;
                        bool downStone = i < y - 1 ? maze[i + 1, j] == 'X' : false;

                        if (leftStone)
                        {
                            maze[i, j - 1] = 'O';
                            g.AddEdge(vNo, vNo - 1, t);
                        }

                        if (rightStone)
                        {
                            maze[i, j + 1] = 'O';
                            g.AddEdge(vNo, vNo + 1, t);
                        }

                        if (upStone)
                        {
                            maze[i - 1, j] = 'O';
                            g.AddEdge(vNo, vNo - x, t);
                        }

                        if (downStone)
                        {
                            maze[i + 1, j] = 'O';
                            g.AddEdge(vNo, vNo + x, t);
                        }
                    }

                    vNo++; //increment vCount
                }
            }

            path = null; // tej linii na laboratorium nie zmieniamy!
            PathsInfo[] paths;
            if (!g.DijkstraShortestPaths(start, out paths))
                return -1;

            if (paths[end].Last == null)
                return -1;

            return (int)paths[end].Dist;
        }

        /// <summary>
        /// Wersja III i IV zadania
        /// Zwraca najkrótszy możliwy czas przejścia przez labirynt z użyciem co najwyżej k lasek dynamitu
        /// </summary>
        /// <param name="maze">labirynt</param>
        /// <param name="k">liczba dostępnych lasek dynamitu, dla wersji III k=1</param>
        /// <param name="path">zwracana ścieżka</param>
        /// <param name="t">czas zburzenia ściany</param>
        public int FindShortestPathWithKDynamites(char[,] maze, int k, out string path, int t)
        {
            path = null; // tej linii na laboratorium nie zmieniamy!
            return 0;
        }
        
    }
}