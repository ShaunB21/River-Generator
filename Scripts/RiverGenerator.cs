using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RiverGenerator
{
    private int[,] adjacentDirections = new int[,]{{-1,-1},{-1,0},{-1,1},{0,1},{1,1},{1,0},{1,-1},{0,-1}};
    private bool edgeReached;

    private float[,] heightMap;
    private Vector2[] sources;
    private int width;
    private int height;
    private float[,] riverMap;
    private int seed;
    public RiverGenerator(float[,] heightMap, Vector2[] sources, int seed)
    {
        this.heightMap = heightMap;
        this.sources = sources;
        width = heightMap.GetLength(0);
        height = heightMap.GetLength(1);
        riverMap = new float[width, height];
        this.seed = seed;
    }
    public float[,] GenerateRiverMap()
    {
        int sourcesNum = sources.Length;
        for (int s = 0; s < sourcesNum; s++) // Creates a river for each source generated
        {
            edgeReached = false;
            float currentRiverHeight = heightMap[(int)sources[s].x, (int)sources[s].y];
            Vector2 currentRiverPosition = new();
            currentRiverPosition.Set((int)sources[s].x, (int)sources[s].y);
            while (currentRiverHeight > 0.2f && edgeReached == false)
            {
                Vector2 nextRiverPosition = FindNextPosition(currentRiverPosition, s);
                try
                {
                    // Bresenham's line algorithm
                    int x = (int)currentRiverPosition.x;
                    int y = (int)currentRiverPosition.y;

                    int w = (int)nextRiverPosition.x - (int)currentRiverPosition.x;
                    int h = (int)nextRiverPosition.y - (int)currentRiverPosition.y;

                    int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;

                    if (w < 0) { dx1 = -1; dx2 = -1; }
                    else if (w > 0) { dx1 = 1; dx2 = 1; }

                    if (h < 0) { dy1 = -1; }
                    else if (h > 0) { dy1 = 1; }

                    int longest = Math.Abs(w);
                    int shortest = Math.Abs(h);

                    if (longest < shortest)
                    {
                        longest = Math.Abs(h);
                        shortest = Math.Abs(w);
                        if (h < 0) {
                            dy2 = -1;
                        }
                        else if (h > 0) {
                            dy2 = 1;
                        }
                        dx2 = 0;
                    }
                    int numerator = longest >> 1;
                    System.Random rnd = new(seed);
                    int offset = 0;
                    for (int i = 0; i <= longest; i++)
                    {
                        offset = offset + rnd.Next(0, 2) * 2 - 1;
                        riverMap[x + offset, y] = heightMap[(int)currentRiverPosition.x, (int)currentRiverPosition.y];
                        numerator += shortest;
                        if (numerator > longest)
                        {
                            numerator -= longest;
                            x += dx1;
                            y += dy1;
                        }
                        else
                        {
                            x += dx2;
                            y += dy2;
                        }
                    }

                    riverMap[(int)nextRiverPosition.x, (int)nextRiverPosition.y] = heightMap[(int)nextRiverPosition.x, (int)nextRiverPosition.y];
                    currentRiverHeight = heightMap[(int)nextRiverPosition.x, (int)nextRiverPosition.y];
                    currentRiverPosition = nextRiverPosition;
                } catch (IndexOutOfRangeException e)
                {
                    edgeReached = true;
                }         
            }
        }

        return riverMap;
    }
  
    // Public only for testing purposes
    public Vector2 FindNextPosition(Vector2 currentRiverPosition, int source)
    {
        edgeReached = false;
        Vector2 nextPos = new();
        nextPos.Set(currentRiverPosition.x, currentRiverPosition.y);
        bool lowerFound = false;
        List<Vector2> visited = new List<Vector2>();
        List<Vector2> toSearchFrom = new List<Vector2>();
        // Adds the current position as the first point to search from
        visited.Add(currentRiverPosition);
        toSearchFrom.Add(currentRiverPosition);
        int count = 0;
        //Searches until a lower point is found
        while (lowerFound == false)
        {
            // Creates array of new points to search from
            List<Vector2> newToSearchFrom = new List<Vector2>();
            // For every vector to search from, all 8 adjacent points will be searched
            for (int i = 0; i < toSearchFrom.Count; i++)
            {
                Vector2 currentPos = toSearchFrom[i];
                // Searches all 8 adjacent points

                for (int j = 0; j < adjacentDirections.GetLength(0); j++)
                {
                    bool visitedCheck = false;
                    // Searches adjacent point
                    int cx = (int)currentPos.x + adjacentDirections[j, 0];
                    int cy = (int)currentPos.y + adjacentDirections[j, 1];
                    // Checks to see if the point has already been visited
                    for (int vi = 0; vi < visited.Count; vi++)
                    {
                        if (visited[vi].x == cx && visited[vi].y == cy)
                        {
                            // If it is in the visited array, then visited check will be true
                            visitedCheck = true;
                        }
                    }
                    try
                    {
                        // If visited check is false then it will check to see if the adjacent point is lower than the original point
                        if (visitedCheck == false)
                        {
                            // If it is lower, then it will set the next position as that point and lower found will be true
                            if (heightMap[cx, cy] < heightMap[(int)currentRiverPosition.x, (int)currentRiverPosition.y] && riverMap[cx, cy] != 1.0f)
                            {
                                nextPos.Set(cx, cy);
                                lowerFound = true;
                                goto End;
                            }
                            Vector2 visitedPos = new();
                            visitedPos.Set(cx, cy);
                            // Adds adjacent point to the list of visited points
                            visited.Add(visitedPos);
                            // Adds adajcent point to the list of points to search from next time
                            // as all adjacent points of the adjacent points must be searched until 
                            // a lower point is found
                            newToSearchFrom.Add(visitedPos);
                        }
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        edgeReached = true;
                        goto End;
                    }
                }
            }
            // the search from array is given all the new values to search from
            toSearchFrom = newToSearchFrom;
            count++;
            // If array has been searching for too long, switch to other method that is faster
            if (count > 100)
            {
                // Goes in a straight line until a lower point is found
                while (lowerFound == false)
                {
                    int xIncrement;
                    int yIncrement;
                    if (sources[source].x - currentRiverPosition.x > 0)
                    {
                        xIncrement = -1;
                    }
                    else
                    {
                        xIncrement = 1;
                    }

                    if (sources[source].y - currentRiverPosition.y > 0)
                    {
                        yIncrement = -1;
                    }
                    else
                    {
                        yIncrement = 1;
                    }
                    nextPos.x = nextPos.x + xIncrement;
                    nextPos.y = nextPos.y + yIncrement;
                    try
                    {
                        if (heightMap[(int)nextPos.x, (int)nextPos.y] < heightMap[(int)currentRiverPosition.x, (int)currentRiverPosition.y])
                        {
                            lowerFound = true;
                        }
                    }
                    catch (Exception e)
                    {
                        edgeReached = true;
                        break;
                    }
                }
                goto End;
            }
        }
        End:
        return nextPos;
    }
}
// Terrain 343 - River 343 (best erosion)
// Terrain 343 - River 3333
// Terrain 343 - River 7223
// Terrain 343 - River 3443 (erosion) (LONG GENERATION)
// Terrain 343 - River 344343 (erosion)
// Terrain 6565 - River 844 (four sources)
// Terrain 444 - River 88 (big erosion)
// Terrain 79343 - River 9 (BETTER EROSION TEST) Advencheress
// Terrain 343 - River 893545
