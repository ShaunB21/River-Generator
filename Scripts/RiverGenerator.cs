using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RiverGenerator : MonoBehaviour
{
    private static int[,] adjacentDirections = new int[,]{{-1,-1},{-1,0},{-1,1},{0,1},{1,1},{1,0},{1,-1},{0,-1}};
    private static bool edgeReached;
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
                if (heightMap[x,y] > 0.8f)
                {
                    potentialSources.Add(new Vector2(x,y));
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
            Vector2 currentRiverPosition = new();
            currentRiverPosition.Set((int)sources[s].x, (int)sources[s].y);
            while (currentRiverHeight > 0.2f && edgeReached == false)
            {
                Boolean lowerFound = false;
                Vector2 nextRiverPosition = FindNextPosition(currentRiverPosition, heightMap, riverMap);
                Vector2 currentPos = currentRiverPosition;

                if (nextRiverPosition == currentRiverPosition)
                {
                    Vector2 nextPos = nextRiverPosition;
                    int count = 0;
                    while (lowerFound == false)
                    {
                        int xIncrement;
                        int yIncrement;
                        if (sources[s].x - currentRiverPosition.x > 0)
                        {
                            xIncrement = -1;
                        } else
                        {
                            xIncrement = 1;
                        }

                        if (sources[s].y - currentRiverPosition.y > 0)
                        {
                            yIncrement = -1;
                        } else
                        {
                            yIncrement = 1;
                        }
                        nextPos.x = nextPos.x + xIncrement;
                        nextPos.y = nextPos.y + yIncrement;
                        try
                        {
                            if (heightMap[(int)nextPos.x, (int)nextPos.y] < heightMap[(int)currentRiverPosition.x, (int)currentRiverPosition.y])
                            {
                                nextRiverPosition = nextPos;
                                lowerFound = true;
                            }
                        } catch (Exception e)
                        {
                            edgeReached = true;
                            break;
                        }
                        riverMap[(int)nextPos.x, (int)nextPos.y] = heightMap[(int)currentRiverPosition.x, (int)currentRiverPosition.y];
                        count++;
                    }
                } else
                {
                    Boolean reached = false;
                    // Position to reach has bigger x and y values
                    if (nextRiverPosition.x > currentRiverPosition.x && nextRiverPosition.y > currentRiverPosition.y) 
                    {
                        while (reached == false)
                        {
                            if (currentPos.x != nextRiverPosition.x)
                            {
                                currentPos.x++;
                            } 
                            if (currentPos.y != nextRiverPosition.y)
                            {
                                currentPos.y++;
                            }
                            if (currentPos == nextRiverPosition)
                            {
                                reached = true;
                            } else
                            {
                                riverMap[(int)currentPos.x, (int)currentPos.y] = heightMap[(int)currentRiverPosition.x, (int)currentRiverPosition.y];
                            }
                        }
                    }
                    // Position to reach has smaller x and y values
                    else if (nextRiverPosition.x < currentRiverPosition.x && nextRiverPosition.y < currentRiverPosition.y)
                    {
                        while (reached == false)
                        {
                            if (currentPos.x != nextRiverPosition.x)
                            {
                                currentPos.x--;
                            }
                            if (currentPos.y != nextRiverPosition.y)
                            {
                                currentPos.y--;
                            }

                            if (currentPos == nextRiverPosition)
                            {
                                reached = true;
                            }
                            else
                            {
                                riverMap[(int)currentPos.x, (int)currentPos.y] = heightMap[(int)currentRiverPosition.x, (int)currentRiverPosition.y];
                            }
                        }
                    }
                    // Position to reach has bigger x value and smaller y value
                    else if (nextRiverPosition.x > currentRiverPosition.x && nextRiverPosition.y < currentRiverPosition.y)
                    {
                        while (reached == false)
                        {
                            if (currentPos.x != nextRiverPosition.x)
                            {
                                currentPos.x++;
                            }
                            if (currentPos.y != nextRiverPosition.y)
                            {
                                currentPos.y--;
                            }

                            if (currentPos == nextRiverPosition)
                            {
                                reached = true;
                            }
                            else
                            {
                                riverMap[(int)currentPos.x, (int)currentPos.y] = heightMap[(int)currentRiverPosition.x, (int)currentRiverPosition.y];
                            }
                        }
                    }
                    // Position to reach has smaller x value and bigger y value
                    else if (nextRiverPosition.x < currentRiverPosition.x && nextRiverPosition.y > currentRiverPosition.y)
                    {
                        while (reached == false)
                        {
                            if (currentPos.x != nextRiverPosition.x)
                            {
                                currentPos.x--;
                            }
                            if (currentPos.y != nextRiverPosition.y)
                            {
                                currentPos.y++;
                            }

                            if (currentPos == nextRiverPosition)
                            {
                                reached = true;
                            }
                            else
                            {
                                riverMap[(int)currentPos.x, (int)currentPos.y] = heightMap[(int)currentRiverPosition.x, (int)currentRiverPosition.y];
                            }
                        }
                    }
                }

                riverMap[(int)nextRiverPosition.x, (int)nextRiverPosition.y] = heightMap[(int)nextRiverPosition.x, (int)nextRiverPosition.y];
                currentRiverHeight = heightMap[(int)nextRiverPosition.x, (int)nextRiverPosition.y];
                currentRiverPosition = nextRiverPosition;
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
                if (riverMap[x, y] > 0.0f)
                {
                    float count = 0.0f;
                    int i = 0;
                    Boolean done = false;
                    if (heightMap[x + 0, y] - riverMap[x, y] > 0.01 || heightMap[x - 0, y] - riverMap[x, y] > 0.01)
                    {
                        while (done == false)
                        {
                            if (heightMap[x + i, y] - riverMap[x, y] > 0.01)
                            {
                                Debug.Log("Step height: " + heightMap[x + i, y] + "Original height: " + riverMap[x, y]);
                                Debug.Log(i);
                                if (riverMap[x, y] + count > modifiedHeightMap[x + i, y])
                                {
                                    done = true;
                                    break;
                                }
                                else
                                {
                                    modifiedHeightMap[x + i, y] = riverMap[x, y] + count;
                                }
                            }
                            count += 0.005f;
                            i++;
                        }
                        count = 0.0f;
                        i = 0;
                        done = false;
                        while (done == false)
                        {
                            if (heightMap[x - i, y] - riverMap[x, y] > 0.01)
                            {
                                if (riverMap[x, y] + count > modifiedHeightMap[x - i, y])
                                {
                                    break;
                                }
                                else
                                {
                                    modifiedHeightMap[x - i, y] = riverMap[x, y] + count;
                                }
                            }
                            count += 0.005f;
                            i++;
                        }
                    }

                    modifiedHeightMap[x, y] = riverMap[x, y] - 0.005f;
                }
            }
        }
        return modifiedHeightMap;
    }


    private static Vector2 GenerateSource(List<Vector2> potentialSources, int seed)
    {
        Vector2 sourcePosition = new();
        System.Random rnd = new(seed);
        int randomVector = rnd.Next(0, potentialSources.Count);
        sourcePosition.Set(potentialSources[randomVector].x, potentialSources[randomVector].y);
        return sourcePosition;
    }
    private static Vector2 FindNextPosition(Vector2 currentRiverPosition, float[,] heightMap, float[,] riverMap)
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
            if (count > 100)
            {
                goto End;
            }
        }
        End:
        return nextPos;
    }
}
// Terrain 343 - River 343 (erosion)
// Terrain 343 - River 3333
// Terrain 343 - River 7223
// Terrain 343 - River 3443 (erosion) (LONG GENERATION)
// Terrain 343 - River 344343 (erosion)