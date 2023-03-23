using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RiverGenerator : MonoBehaviour
{

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
            bool edgeReached = false;
            int count = 0;

            float currentRiverHeight = heightMap[(int)sources[s].x, (int)sources[s].y];
            riverMap[(int)sources[s].x, (int)sources[s].y] = 1.0f;
            float currentLowest = currentRiverHeight;
            Vector2 currentLowestPosition = new();
            Vector2 currentRiverPosition = new();
            currentRiverPosition.Set((int)sources[s].x, (int)sources[s].y);
            while (currentRiverHeight > 0.2f && edgeReached == false)
            {
                if (count > 3000)
                {
                    edgeReached = true;
                }
                try
                {

                    float lastLowest = currentLowest;
                    currentLowestPosition = FindSurroundingLowest(currentLowest, currentRiverPosition, heightMap, currentLowestPosition, riverMap);
                    currentLowest = heightMap[(int)currentLowestPosition.x, (int)currentLowestPosition.y];

                    if (currentLowest == lastLowest)
                    {

                        currentLowest += 0.1f;
                        currentLowestPosition = FindSurroundingLowest(currentLowest, currentRiverPosition, heightMap, currentLowestPosition, riverMap);
                        currentLowest = heightMap[(int)currentLowestPosition.x, (int)currentLowestPosition.y];
                    }

                }
                catch (IndexOutOfRangeException e)
                {
                    edgeReached = true;
                }

                riverMap[(int)currentLowestPosition.x, (int)currentLowestPosition.y] = 1.0f;
                currentRiverHeight = currentLowest;
                currentRiverPosition = currentLowestPosition;

                //count++;
            }
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
    private static Vector2 FindSurroundingLowest(float currentLowest, Vector2 currentRiverPosition, float[,] heightMap, Vector2 currentLowestPosition, float[,] riverMap)
    {
        Vector2 currentLowestPos = currentLowestPosition;
        bool surroundedByWater = false;

        if (heightMap[(int)currentRiverPosition.x - 1, (int)currentRiverPosition.y - 1] < currentLowest 
            && riverMap[(int)currentRiverPosition.x - 1, (int)currentRiverPosition.y - 1] != 1.0f)
        {
            currentLowestPos.Set(currentRiverPosition.x - 1, currentRiverPosition.y - 1);
        }
        if (heightMap[(int)currentRiverPosition.x - 1, (int)currentRiverPosition.y] < currentLowest 
            && riverMap[(int)currentRiverPosition.x - 1, (int)currentRiverPosition.y] != 1.0f)
        {
            currentLowestPos.Set(currentRiverPosition.x - 1, currentRiverPosition.y);
        }
        if (heightMap[(int)currentRiverPosition.x - 1, (int)currentRiverPosition.y + 1] < currentLowest 
            && riverMap[(int)currentRiverPosition.x - 1, (int)currentRiverPosition.y + 1] != 1.0f)
        {
            currentLowestPos.Set(currentRiverPosition.x - 1, currentRiverPosition.y + 1);
        }
        if (heightMap[(int)currentRiverPosition.x, (int)currentRiverPosition.y + 1] < currentLowest 
            && riverMap[(int)currentRiverPosition.x, (int)currentRiverPosition.y + 1] != 1.0f)
        {
            currentLowestPos.Set(currentRiverPosition.x, currentRiverPosition.y + 1);
        }
        if (heightMap[(int)currentRiverPosition.x + 1, (int)currentRiverPosition.y + 1] < currentLowest 
            && riverMap[(int)currentRiverPosition.x + 1, (int)currentRiverPosition.y + 1] != 1.0f) 
        {
            currentLowestPos.Set(currentRiverPosition.x + 1, currentRiverPosition.y + 1);
        }
        if (heightMap[(int)currentRiverPosition.x + 1, (int)currentRiverPosition.y] < currentLowest 
            && riverMap[(int)currentRiverPosition.x + 1, (int)currentRiverPosition.y] != 1.0f)
        {
            currentLowestPos.Set(currentRiverPosition.x + 1, currentRiverPosition.y);
        }
        if (heightMap[(int)currentRiverPosition.x + 1, (int)currentRiverPosition.y - 1] < currentLowest 
            && riverMap[(int)currentRiverPosition.x + 1, (int)currentRiverPosition.y - 1] != 1.0f)
        {
            currentLowestPos.Set(currentRiverPosition.x + 1, currentRiverPosition.y - 1);
        }
        if (heightMap[(int)currentRiverPosition.x, (int)currentRiverPosition.y - 1] < currentLowest 
            && riverMap[(int)currentRiverPosition.x, (int)currentRiverPosition.y - 1] != 1.0f)
        {
            currentLowestPos.Set(currentRiverPosition.x, currentRiverPosition.y - 1);
        }
        if (riverMap[(int)currentRiverPosition.x - 1, (int)currentRiverPosition.y - 1] == 1.0f
            && riverMap[(int)currentRiverPosition.x - 1, (int)currentRiverPosition.y] == 1.0f
            && riverMap[(int)currentRiverPosition.x - 1, (int)currentRiverPosition.y + 1] == 1.0f
            && riverMap[(int)currentRiverPosition.x, (int)currentRiverPosition.y + 1] == 1.0f
            && riverMap[(int)currentRiverPosition.x + 1, (int)currentRiverPosition.y + 1] == 1.0f
            && riverMap[(int)currentRiverPosition.x + 1, (int)currentRiverPosition.y] == 1.0f
            && riverMap[(int)currentRiverPosition.x + 1, (int)currentRiverPosition.y - 1] == 1.0f
            && riverMap[(int)currentRiverPosition.x, (int)currentRiverPosition.y - 1] == 1.0f)
        {
            surroundedByWater = true;
        }

        if (surroundedByWater == true)
        {
            System.Random rnd = new();
            int randomX = rnd.Next(-1, 1);
            int randomY = rnd.Next(-1, 1);
            currentLowestPos.Set((int)currentRiverPosition.x + randomX, (int)currentRiverPosition.y + randomY);
        }
        return currentLowestPos;
    }
}