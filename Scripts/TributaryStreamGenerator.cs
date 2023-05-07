using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TributaryStreamGenerator
{
    private int[,] adjacentDirections = new int[,] { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, 1 }, { 1, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 } };
    private bool edgeReached;
    private Vector2[] streamSources;
    private float[,] heightMap;
    private int width;
    private int height;
    private float[,] riverMap;
    private int seed;
    public TributaryStreamGenerator(float[,] heightMap, float[,] riverMap, int seed)
    {
        this.heightMap = heightMap;
        width = heightMap.GetLength(0);
        height = heightMap.GetLength(1);
        this.riverMap = riverMap;
        this.seed = seed;
        this.streamSources = GenerateStreamSources(3);
    }

    public Vector2[] GenerateStreamSources(int sourcesNum)
    {
        Vector2[] sources = new Vector2[sourcesNum];
        List<Vector2> potentialSources = new List<Vector2>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (riverMap[x, y] > 0.0f)
                {
                    potentialSources.Add(new Vector2(x, y));
                }
            }
        }
        System.Random rnd = new(seed);

        // Checks to see if potential sources array is not empty
        if (potentialSources.Count > 0)
        {
            // Generates number of sources specified by the user
            for (int s = 0; s < sourcesNum; s++) 
            {
                seed = seed + s;

                Vector2 sourcePosition = new();
                int randomVector = rnd.Next(0, potentialSources.Count);
                sourcePosition.Set(potentialSources[randomVector].x, potentialSources[randomVector].y);
                sources[s].Set((int)sourcePosition.x, (int)sourcePosition.y);

            }
        }

        return sources;
    }
    public float[,] GenerateRiverMap()
    {
        int sourcesNum = streamSources.Length;
        for (int s = 0; s < sourcesNum; s++) // Creates a river for each source generated
        {
            edgeReached = false;
            float currentRiverHeight = heightMap[(int)streamSources[s].x, (int)streamSources[s].y];
            Vector2 currentRiverPosition = new();
            currentRiverPosition.Set((int)streamSources[s].x, (int)streamSources[s].y);
            while (currentRiverHeight < 0.8f && edgeReached == false)
            {
                Vector2 nextRiverPosition = FindNextPosition(currentRiverPosition);
                try
                {
                    if (nextRiverPosition == currentRiverPosition)
                    {
                        edgeReached = true;
                    }
                    else
                    {
                        riverMap[(int)nextRiverPosition.x, (int)nextRiverPosition.y] = heightMap[(int)nextRiverPosition.x, (int)nextRiverPosition.y];
                        currentRiverHeight = heightMap[(int)nextRiverPosition.x, (int)nextRiverPosition.y];
                        currentRiverPosition = nextRiverPosition;
                    }
                }
                catch (IndexOutOfRangeException e)
                {
                    edgeReached = true;
                }
            }
        }
        return riverMap;
    }

    // Public only for testing purposes
    public Vector2 FindNextPosition(Vector2 currentRiverPosition)
    {
        edgeReached = false;
        Vector2 nextPos = new();
        nextPos.Set(currentRiverPosition.x, currentRiverPosition.y);
        for (int j = 0; j < adjacentDirections.GetLength(0); j++)
        {
            // Searches adjacent point
            int cx = (int)currentRiverPosition.x + adjacentDirections[j, 0];
            int cy = (int)currentRiverPosition.y + adjacentDirections[j, 1];
            try
            {
                if (heightMap[cx, cy] > heightMap[(int)currentRiverPosition.x, (int)currentRiverPosition.y] && riverMap[cx, cy] != 1.0f)
                {
                    nextPos.Set(cx, cy);
                }
            }
            catch (IndexOutOfRangeException e)
            {
                edgeReached = true;
            }
        }
        return nextPos;
    }
}
