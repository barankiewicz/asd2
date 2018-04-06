﻿using ASD.Graphs;
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
            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    //Get the neighbours, if it tries to go out of bounds set the neighbor to 'N'
                    char curChar = maze[i, j];
                    char left = j >= 1 ? maze[i, j - 1] : 'N';
                    char right = j < x - 1 ? maze[i, j + 1] : 'N';
                    char up = i >= 1 ? maze[i - 1, j] : 'N';
                    char down = i < y - 1 ? maze[i + 1, j] : 'N';

                    //If it comes across S or E, mark their vNo's (vertices number)
                    if (curChar == 'S')
                        start = vNo;
                    if (curChar == 'E')
                        end = vNo;

                    //Go through each neighbour and add proper edges if applicable
                    switch(up)
                    {
                        case 'N':
                            break;
                        case 'X':
                            if(withDynamite)
                                g.AddEdge(vNo, vNo - x, t);
                            break;
                        default:
                            g.AddEdge(vNo, vNo - x);
                            break;
                    }
                    switch (down)
                    {
                        case 'N':
                            break;
                        case 'X':
                            if (withDynamite)
                                g.AddEdge(vNo, vNo + x, t);
                            break;
                        default:
                            g.AddEdge(vNo, vNo + x);
                            break;
                    }
                    switch (left)
                    {
                        case 'N':
                            break;
                        case 'X':
                            if (withDynamite)
                                g.AddEdge(vNo, vNo - 1, t);
                            break;
                        default:
                            g.AddEdge(vNo, vNo - 1);
                            break;
                    }
                    switch (right)
                    {
                        case 'N':
                            break;
                        case 'X':
                            if (withDynamite)
                                g.AddEdge(vNo, vNo + 1, t);
                            break;
                        default:
                            g.AddEdge(vNo, vNo + 1);
                            break;
                    }
                    vNo++;
                }
            }

            path = "";
            PathsInfo[] paths;
            if (!g.DijkstraShortestPaths(start, out paths))
                return -1;

            if (paths[end].Last == null)
                return -1;

            Edge[] edgePath = PathsInfo.ConstructPath(start, end, paths);
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < edgePath.Length; i++)
            {
                int curXFrom = edgePath[i].From / x;
                int curYFrom = edgePath[i].From - x * curXFrom;
                int curXTo = edgePath[i].To / x;
                int curYTo = edgePath[i].To - x * curXTo;

                if (curXFrom == curXTo)
                {
                    if (curYFrom < curYTo)
                        str.Append('E');
                    else
                        str.Append('W');
                }
                else
                {
                    if (curXFrom < curXTo)
                        str.Append('S');
                    else
                        str.Append('N');
                }
            }
            path = str.ToString();
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
            int x = maze.GetLength(1);
            int y = maze.GetLength(0);
            int layerOffset = x * y;
            int vNo = 0;
            int start = 0;
            int end = 0;

            Graph g = new AdjacencyListsGraph<AVLAdjacencyList>(true, (k + 1) * x * y);
            for (int p = 0; p <= k; p++)
            {
                for (int i = 0; i < y; i++)
                {
                    for (int j = 0; j < x; j++)
                    {
                        if (maze[i, j] == 'S' && p == 0)
                            start = vNo;
                        if (maze[i, j] == 'E')
                            end = vNo;

                        char up = i >= 1 ? maze[i - 1, j] : 'N';
                        char down = i < y - 1 ? maze[i + 1, j] : 'N';
                        char left = j >= 1 ? maze[i, j - 1] : 'N';
                        char right = j < x - 1 ? maze[i, j + 1] : 'N';

                        int nextLayerVert = vNo + layerOffset;
                        switch (up)
                        {
                            case 'X':
                                if (p < k)
                                    g.AddEdge(vNo, nextLayerVert - x, t);
                                break;
                            case 'N':
                                break;
                            default:
                                g.AddEdge(vNo, vNo - x);
                                break;
                        }
                        switch (down)
                        {
                            case 'X':
                                if(p < k)
                                    g.AddEdge(vNo, nextLayerVert + x, t);
                                break;
                            case 'N':
                                break;
                            default:
                                g.AddEdge(vNo, vNo + x);
                                break;
                        }
                        switch (left)
                        {
                            case 'X':
                                if (p < k)
                                    g.AddEdge(vNo, nextLayerVert - 1, t);
                                break;
                            case 'N':
                                break;
                            default:
                                g.AddEdge(vNo, vNo - 1);
                                break;
                        }
                        switch (right)
                        {
                            case 'X':
                                if (p < k)
                                    g.AddEdge(vNo, nextLayerVert + 1, t);
                                break;
                            case 'N':
                                break;
                            default:
                                g.AddEdge(vNo, vNo + 1);
                                break;
                        }
                        vNo++;
                    }
                }
            }

            path = "";
            PathsInfo[] paths;
            if (!g.DijkstraShortestPaths(start, out paths))
                return -1;

            int res = -1;
            int minEnd = -1;
            int minEndValue = int.MaxValue;
            while (end >= 0)
            {
                if (paths[end].Last != null)
                {
                    res = (int)paths[end].Dist;
                    if(res < minEndValue)
                    {
                        minEnd = end;
                        minEndValue = res;
                    }
                }
                end -= layerOffset;
            }
            if (minEnd < 0)
                return -1;

            //GET THE PATH
            Edge[] edgePath = PathsInfo.ConstructPath(start, minEnd, paths);
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < edgePath.Length; i++)
            {
                int layerFrom = edgePath[i].From / layerOffset;
                int layerTo = edgePath[i].To / layerOffset;

                int normalvNoFrom = edgePath[i].From - layerFrom * layerOffset;
                int normalvNoTo = edgePath[i].To - layerTo * layerOffset;

                int curXFrom = normalvNoFrom / x;
                int curYFrom = normalvNoFrom - x * curXFrom;
                int curXTo = normalvNoTo / x;
                int curYTo = normalvNoTo - x * curXTo;

                if (curXFrom == curXTo)
                {
                    if (curYFrom < curYTo)
                        str.Append('E');
                    else
                        str.Append('W');
                }
                else
                {
                    if (curXFrom < curXTo)
                        str.Append('S');
                    else
                        str.Append('N');
                }
            }
            path = str.ToString();
            return minEndValue;
        }
    }
}