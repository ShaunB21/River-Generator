/*
 * Noise generator code created by Sebastian Lague (2017)
 * https://github.com/SebLague/Procedural-Landmass-Generation
 */

using System.Collections;
using UnityEngine;
using System;

public class NoiseGenerator
{
    private int mapWidth;
    private int mapHeight;
    private int seed;
    private float scale;
    private int octaves;
    private float persistence;
    private float lacunarity;
    private Vector2 offset;
    public NoiseGenerator(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistence, float lacunarity, Vector2 offset)
    {
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        this.seed = seed;
        this.scale = scale;
        this.octaves = octaves;
        this.persistence = persistence;
        this.lacunarity = lacunarity;
        this.offset = offset;
    }

    public float[,] GenerateNoiseMap()
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++){
            for (int x = 0; x < mapWidth; x++){
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++){
                    float sampleX = (x-halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y-halfHeight) / scale * frequency + octaveOffsets[i].y;
                    // Random number between -1 and 1
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }
        // Normalise noise map in the range -1 to 1
        for (int y = 0; y < mapHeight; y++){
            for (int x = 0; x < mapWidth; x++){
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }
        return noiseMap;
    }
}

