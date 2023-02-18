using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    private float[,] noiseMap;
    private MeshData meshData;
    private Vector2[] sourceArray;

    private int mapWidth = 5000;
    private int mapHeight = 5000;
    private float noiseScale = 800;


    private int octaves = 4;
    private float persistence = 0.5f;
    private float lacunarity = 2;

    public int seed;
    [Range(1, 10)]
    public int sources;
    public Vector2 offset;
    public bool autoUpdate;

    public void GenerateTerrain()
    {
        noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistence, lacunarity, offset);
        meshData = MeshGenerator.GenerateMesh(noiseMap);
        TerrainDrawer drawer = FindObjectOfType<TerrainDrawer>();
        drawer.DrawTerrain(noiseMap, meshData);
        Debug.Log("Terrain Generated");

    }
    public void GenerateSources()
    {
        sourceArray = RiverGenerator.GenerateSources(noiseMap, sources);
    }
    public void GenerateRivers()
    {
        float[,] riverMap = RiverGenerator.GenerateRiverMap(noiseMap, sourceArray);
        float[,] heightMap = RiverGenerator.GenerateRiverHeightMap(noiseMap, riverMap);
        meshData = MeshGenerator.GenerateMesh(heightMap);
        TerrainDrawer drawer = FindObjectOfType<TerrainDrawer>();
        drawer.DrawTerrainWithRivers(heightMap, meshData, riverMap);
    }
}
