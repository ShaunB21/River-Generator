using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SourceGenerator
{
    public static Vector2[] GenerateSources(float[,] heightMap, int sourcesNum, int seed)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float[,] riverMap = new float[width, height];
        Vector2[] sources = new Vector2[sourcesNum];
        List<Vector2> potentialSources = new List<Vector2>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (heightMap[x, y] > 0.8f)
                {
                    potentialSources.Add(new Vector2(x, y));
                }
            }
        }
        for (int s = 0; s < sourcesNum; s++) // Generates number of sources specified by the user
        {
            seed = seed + s;
            Vector2 sourcePosition = GenerateSource(potentialSources, seed); // Generates the source
            riverMap[(int)sourcePosition.x, (int)sourcePosition.y] = heightMap[(int)sourcePosition.x, (int)sourcePosition.y];
            sources[s].Set((int)sourcePosition.x, (int)sourcePosition.y);
        }
        return sources;
    }

    private static Vector2 GenerateSource(List<Vector2> potentialSources, int seed)
    {
        Vector2 sourcePosition = new();
        System.Random rnd = new(seed);
        // Gets a random vector from the list of potential sources
        int randomVector = rnd.Next(0, potentialSources.Count);
        sourcePosition.Set(potentialSources[randomVector].x, potentialSources[randomVector].y);
        return sourcePosition;
    }
}
