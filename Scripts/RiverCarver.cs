using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RiverCarver
{
    private float[,] heightMap;
    private int width;
    private int height;
    private float[,] riverMap;
    public RiverCarver(float[,] heightMap, float[,] riverMap)
    {
        this.heightMap = heightMap;
        width = heightMap.GetLength(0);
        height = heightMap.GetLength(1);
        this.riverMap = riverMap;
    }

    public float[,] GenerateRiverHeightMap()
    {
        float[,] modifiedHeightMap = heightMap;
        float[,] erosionMap = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (riverMap[x, y] > 0.0f)
                {
                    float heightIncrease = 0.001f;
                    int i = 1;
                    bool done = false;
                    // Check if the river has gone through a hill by seeing the height difference
                    if (heightMap[x, y] - riverMap[x, y] > 0.01)
                    {
                        while (done == false)
                        {
                            try
                            {
                                if (riverMap[x, y] + heightIncrease > modifiedHeightMap[x + i, y])
                                {
                                    done = true;
                                } 
                                else
                                {
                                    modifiedHeightMap[x + i, y] = riverMap[x, y] + heightIncrease;
                                    erosionMap[x + i, y] = riverMap[x, y] + heightIncrease;
                                }
                            }
                            catch (IndexOutOfRangeException e) { break; }
                            heightIncrease += 0.001f;
                            i++;
                        }
                        done = false;
                        heightIncrease = 0.001f;
                        i = 1;
                        while (done == false)
                        {
                            try
                            {
                                if (riverMap[x, y] + heightIncrease > modifiedHeightMap[x - i, y])
                                {
                                    done = true;
                                }
                                else
                                {
                                    modifiedHeightMap[x - i, y] = riverMap[x, y] + heightIncrease;
                                    erosionMap[x - i, y] = riverMap[x, y] + heightIncrease;
                                }
                            }
                            catch (IndexOutOfRangeException e) { break; }
                            heightIncrease += 0.001f;
                            i++;
                        }
                    }
                    // Carve river bed
                    modifiedHeightMap[x, y] = modifiedHeightMap[x, y] - 0.001f;
                }
            }
        }

        // Smoothen erosion edges
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (erosionMap[x, y] > 0.0f)
                {
                    float heightIncrease = 0.001f;
                    bool done = false;
                    int i = 1;
                    try
                    {
                        if (heightMap[x, y + 1] - erosionMap[x, y] > 0.005)
                        {
                            while (done == false)
                            {
                                try
                                {
                                    if (erosionMap[x, y] + heightIncrease > modifiedHeightMap[x, y + i])
                                    {
                                        done = true;
                                    }
                                    else
                                    {
                                        modifiedHeightMap[x, y + i] = erosionMap[x, y] + heightIncrease;
                                    }
                                }
                                catch (IndexOutOfRangeException e) { break; }
                                heightIncrease += 0.001f;
                                i++;
                            }
                        }

                        if (heightMap[x, y - 1] - erosionMap[x, y] > 0.005)
                        {
                            while (done == false)
                            {
                                try
                                {
                                    if (erosionMap[x, y] + heightIncrease > modifiedHeightMap[x, y - i])
                                    {
                                        done = true;
                                    }
                                    else
                                    {
                                        modifiedHeightMap[x, y - i] = erosionMap[x, y] + heightIncrease;
                                    }
                                }
                                catch (IndexOutOfRangeException e) { break; }
                                heightIncrease += 0.001f;
                                i++;
                            }
                        }
                    }
                    catch (IndexOutOfRangeException e) { break; }             
                }
            }
        }

        return modifiedHeightMap;
    }
}
