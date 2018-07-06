# asd2

This repo consists of all the laboratory tasks I had to write during the Algorithms and Data Structures course at my university. Each task was divided into 2 parts: the lab part and home part. The lab part was the part where I was shown the problem and had to solve it in 90 minutes. After that, during the home part, I had to refine my solution (improve time complexity, for example) in order to pass more rigorous unit tests.

## Installation
In order to run the code, you need to build it in VS. Remember to add a reference to `dll/Graph.dll` and `dll/TestSet.dll`. The former is a graph library that we were using during the course. The latter one helps to define unit tests for the algorithms.

## Tasks

Each task has a txt/pdf/html document inside that specifies in great detail what the task is about. Unfortunately, those are all in Polish and fairly lengthy, so I'll skip translating them for now. Below, however, is the list of the tasks with their brief descriptions in English:


### 2.  Dynamic programming 1 
In this task, I had to solve the problem of cutting wood to maximize profit. Given a n x m wood planck, and a list of desired shapes along with their values, I had to maximize the profit I could make by cutting the wood in a certain way. In the second part of the task, I also had to present what exact cuts I should make to get the desired profit.

### 3. Graphs 1
In this task, I had to write algorithms that:
- Compute a square of a given graph G
- Compute a line graph of a given graph G
- Compute a vertex coloring of a given graph G (using a greedy algorithm)

### 4. Dynamic programming 2
In this task, I had to use the dynamic programming technique to solve the following problem:
I was given a n x m array that acted as a coordinates system. In this system, some coordinates had a value assigned to them. I had to compute the maximal value that one could get starting his journey at point (0,0) and ending it at point (n - 1, m - 1) by only moving left or up. I also had to computer the precise path that one would take to achieve said maximal value.

In the second part of the task, I had to implement the solution that took into account not only going to the destination, but also coming back to point (0,0). Again, I had to compute both value and the path.

### 5. Graphs 2
In this task, I had to write algorithms that:
- Checks if a given graph G is a tree
- Finds fundamental cycles of a given graph G
- Adds 2 fundamental cycles

### 6. Finding shortest path in graphs
In this task, I was given a maze in a form of a 2D char array:
- 'S' - maze start
- 'E' - maze end
- 'X' - wall
- 'O' - normal road

I had to compute the path from start to end given the following restraints:

1. No restraints - just find the shortest path from S to E
2. The maze runner had an infinite amount of dynamite at hand. He could tear down any wall in his sight. Tearing down the wall cost some time (I had to compute the shortest path)
3. The maze runner only had 1 piece of dynamite
4. The maze runner had n pieces of dynamite

### 7. Backtracking 1
There are two workers who move weights from point A to B. There are `n` blocks of varying weight. I needed to divide the blocks between two workers to meet the following criteria:
1. I want to assign blocks of a total weight of `k` to each worker. Not all blocks from the `n` available must be used.
2. I want to assign all blocks so that the difference between workers' total weights is minimal (ideally 0)

### 8. Buildings along the road
There are `K` objects to be built along the road. The road itself has `N` localization to build (where `N` is a number of kms)
I had to write an algorithm that assigns the objects to localization so that the minimal distance between 2 objects is maximal.
The algorithm needed to be `O(NlogD)`, where `D` is the maximal distance between 2 objects.

### 9. Graphs 3
In this task, I had to write the following algorithms:
- Compute an induced matching of a given graph G
- Compute a maximal induced matching of a given graph G

I needed to use backtracking to do that.

### 10. Flows in graphs 1
In this task, I had to construct an algorithm that computes a circulation in graph G. In the second part of the task, I had to compute the circulation that not only has an upper, but also a lower bound.

### 11. Flows in graphs 2
There is a TV factory. It is trying to make a plan of production and sales. The production in this factory meets the following criteria:

- The factory can produce a certain amount of TVs each week. The amount can vary from week to week
- Producing one TV costs a certain amount. The cost can vary from week to week.
- The factory sells it's TVs to contrahents. Each contrahent has a certain amount of TVs he can buy for a certain price. Both prices and amounts can vary from week to week for each contrahent
- If needed, the factory can store some of it's TVs in a magazine. The magazine has a fixed capacity and price (it doesn't change from week to week).

I had to construct a production plan that:
1. Stage 1. Maximizes the *production* of the factory. Also, there's only *one* contrahent the factory can sell to
2. Stage 2. Maximizes the *profit* of the factory. Also, there's `n` contrahents.

### 12. Geometry 1.
There is a list of streets in the city (represented by segments in 2D cartesian space). I had to write following algorithms:
1. CheckIntersection - given 2 streets, check if they intersect in 0 places, 1 place or infinity of places
2. CheckStreetPairs - given the `streets` list of all streets in the city and 2 additional lists `streetsToCheck1` and `streetsToCheck2` will return a list that for each pair of streets will say if you can commute between them (possibly using other streets). I had to use Union-Find structure to implement this in `O(n)` time.
3. GetIntersectionPoint - given 2 streets, find a point at which they intersect
4. CheckDistricts - given two polygons in a form of a section list and a list of all streets in the city, determine whether there's a way to go from one district to another or not.

### 13. Geometry 2.
There is an uneven terrain represented as a polyline in 2D space. We pour water in this terrain. The task is to:
1. Compute the depth of water for each terrain's point
2. Compute the volume of water poured onto the terrain

### 14. String compression
In this task, we had to use LZ77 compression technique to compress and decompress the given string. The compressing algorithm had to work in better than linear time!
