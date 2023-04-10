using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    private float[,] noiseMap;
    private Vector2[] sourceArray;

    public int mapWidth = 5000;
    public int mapHeight = 5000;
    public float noiseScale = 800;


    public int octaves = 4;
    public float persistence = 0.5f;
    public float lacunarity = 2;
    private Vector2 offset = new Vector2(0, 0);

    public int terrainSeed;
    [Range(1, 10)]
    public int sources;
    public int sourcesSeed;



    public void GenerateTerrain()
    {
        noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, terrainSeed, noiseScale, octaves, persistence, lacunarity, offset);
        MeshData meshData = MeshGenerator.GenerateMesh(noiseMap);
        TerrainDrawer drawer = FindObjectOfType<TerrainDrawer>();
        drawer.DrawTerrain(noiseMap, meshData);
        Debug.Log("Terrain Generated");

    }
    public void GenerateSources()
    {
        sourceArray = SourceGenerator.GenerateSources(noiseMap, sources, sourcesSeed);
        Debug.Log("Sources Generated");
    }
    public void GenerateRivers()
    {
        RiverGenerator rg = new RiverGenerator(noiseMap, sourceArray, sourcesSeed);

        float[,] riverMap = rg.GenerateRiverMap();
        float[,] heightMap = rg.GenerateRiverHeightMap();

        MeshData meshData = MeshGenerator.GenerateMesh(heightMap);
        TerrainDrawer drawer = FindObjectOfType<TerrainDrawer>();
        drawer.DrawTerrainWithRivers(heightMap, meshData, riverMap);
        Debug.Log("Rivers Generated");
    }
}
