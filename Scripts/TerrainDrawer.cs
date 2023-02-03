using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDrawer : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void DrawTerrain(float[,] noiseMap, MeshData meshData)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        Console.WriteLine(meshData.vertices);
        Texture2D texture = new Texture2D(width, height);
        Color[] colourMap = CreateColourMap(noiseMap);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
    public void DrawTerrainWithRivers(float[,] noiseMap, MeshData meshData, float[,] riverMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        Console.WriteLine(meshData.vertices);
        Texture2D texture = new Texture2D(width, height);
        Color[] colourMap = CreateColourMap(noiseMap);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (riverMap[x, y] == 1.0f) // river
                {
                    Color newColor = new Color(24f / 255f, 22f / 255f, 172f / 255f, 0.8f);
                    colourMap[y * width + x] = newColor;
                }
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
    private Color[] CreateColourMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float currentHeight = noiseMap[x, y];
                if (currentHeight < 0.2f) // water
                {
                    Color newColor = new Color(24f / 255f, 22f / 255f, 172f / 255f, 0.8f);
                    colourMap[y * width + x] = newColor;
                }
                else if (currentHeight >= 0.2f && currentHeight < 0.22f) // sand
                {
                    Color newColor = new Color(244f / 255f, 239f / 255f, 144f / 255f, 0.8f);
                    colourMap[y * width + x] = newColor;
                }
                else if (currentHeight >= 0.22f && currentHeight < 0.45f) // grass
                {
                    Color newColor = new Color(31f / 255f, 142f / 255f, 41f / 255f, 0.8f);
                    colourMap[y * width + x] = newColor;
                }
                else if (currentHeight >= 0.45f && currentHeight < 0.7f) // grass
                {
                    Color newColor = new Color(14f / 255f, 130f / 255f, 45f / 255f, 0.8f);
                    colourMap[y * width + x] = newColor;
                }
                else if (currentHeight >= 0.7f && currentHeight < 0.9f) // rock
                {
                    Color newColor = new Color(107f / 255f, 108f / 255f, 108f / 255f, 0.8f);
                    colourMap[y * width + x] = newColor;
                }
                else if (currentHeight >= 0.9f) // snow
                {
                    Color newColor = new Color(249f / 255f, 249f / 255f, 249f / 255f, 0.8f);
                    colourMap[y * width + x] = newColor;
                }
            }
        }
        return colourMap;
    }
}
