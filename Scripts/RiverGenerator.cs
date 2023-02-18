using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RiverGenerator : MonoBehaviour
{
    private static int[,] adjacentDirections = new int[,]{{-1,-1},{-1,0},{-1,1},{0,1},{1,1},{1,0},{1,-1},{0,-1}};
    private static bool edgeReached;
    public static Vector2[] GenerateSources(float[,] heightMap, int sourcesNum)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float[,] riverMap = new float[width, height];
        Vector2[] sources = new Vector2[sourcesNum];

        for (int s = 0; s < sourcesNum; s++) // Generates number of sources specified by the user
        {
            bool check = false;
            while (check == false) // Makes sure that the source generated is higher than a certain height value
            {
                Vector2 sourcePosition = GenerateSource(); // Generates the source
                if (heightMap[(int)sourcePosition.x, (int)sourcePosition.y] > 0.8)
                {
                    riverMap[(int)sourcePosition.x, (int)sourcePosition.y] = 1.0f;
                    sources[s].Set((int)sourcePosition.x, (int)sourcePosition.y);
                    check = true;
                }
            }
        }
        //float[,] modifiedHeightMap = GenerateRiverHeightMap(heightMap, riverMap);
        //MeshData meshData = MeshGenerator.GenerateMesh(modifiedHeightMap);
        //TerrainDrawer drawer = FindObjectOfType<TerrainDrawer>();
        //drawer.DrawTerrainWithRivers(modifiedHeightMap, meshData, riverMap);
        return sources;
    }
    public static float[,] GenerateRiverMap(float[,] heightMap, Vector2[] sources)
    {
        int sourcesNum = sources.Length;
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float[,] riverMap = new float[width, height];

        for (int s = 0; s < sourcesNum; s++) // Creates a river for each source generated
        {
            edgeReached = false;
            float currentRiverHeight = heightMap[(int)sources[s].x, (int)sources[s].y];
            riverMap[(int)sources[s].x, (int)sources[s].y] = 1.0f;
            Vector2 nextRiverPosition = new();
            Vector2 currentRiverPosition = new();
            currentRiverPosition.Set((int)sources[s].x, (int)sources[s].y);
            while (currentRiverHeight > 0.2f && edgeReached == false)
            {
                nextRiverPosition = FindNextPosition(currentRiverPosition, heightMap, riverMap);
                riverMap[(int)nextRiverPosition.x, (int)nextRiverPosition.y] = 1.0f;
                currentRiverHeight = heightMap[(int)nextRiverPosition.x, (int)nextRiverPosition.y];
                currentRiverPosition = nextRiverPosition;
            }
            Debug.Log("Source: " + s + " River finished at height: " + currentRiverHeight);
            Debug.Log("River finished at position X: " + currentRiverPosition.x + " Y: " + currentRiverPosition.y);
        }

        return riverMap;
    }
    public static float[,] GenerateRiverHeightMap(float[,] heightMap, float[,] riverMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float[,] modifiedHeightMap = heightMap;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (riverMap[x, y] == 1.0f)
                {
                    modifiedHeightMap[x, y] = modifiedHeightMap[x, y] - 0.001f;
                }
            }
        }
        return modifiedHeightMap;
    }

    private static Vector2 GenerateSource()
    {
        Vector2 sourcePosition = new();
        System.Random rnd = new();
        int randomX = rnd.Next(1, 5000);
        int randomY = rnd.Next(1, 5000);
        sourcePosition.Set(randomX, randomY);
        return sourcePosition;
    }
    private static Vector2 FindNextPosition(Vector2 currentRiverPosition, float[,] heightMap, float[,] riverMap)
    {
        edgeReached = false;
        Vector2 nextPos = new();

        bool lowerFound = false;
        List<Vector2> visited = new List<Vector2>();
        List<Vector2> toSearchFrom = new List<Vector2>();
        // Adds the current position as the first point to search from
        visited.Add(currentRiverPosition);
        toSearchFrom.Add(currentRiverPosition);
        int count = 0;
        //Searches until a lower point is found
        while (lowerFound == false && count < 100)
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
                        goto Reached; 
                    }
                }
            }
            // the search from array is given all the new values to search from
            toSearchFrom = newToSearchFrom;
            count++;
        }
        Reached:
        return nextPos;
    }
}