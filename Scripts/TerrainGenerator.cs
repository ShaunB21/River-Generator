using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    private float[,] noiseMap;
    private Vector2[] sourceArray;

    private int mapWidth = 5000;
    private int mapHeight = 5000;
    private float noiseScale = 800;


    private int octaves = 4;
    private float persistence = 0.5f;
    private float lacunarity = 2;
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
        RiverGenerator rg = new RiverGenerator(noiseMap, sourceArray);

        float[,] riverMap = rg.GenerateRiverMap();
        float[,] heightMap = rg.GenerateRiverHeightMap();

        MeshData meshData = MeshGenerator.GenerateMesh(heightMap);
        TerrainDrawer drawer = FindObjectOfType<TerrainDrawer>();
        drawer.DrawTerrainWithRivers(heightMap, meshData, riverMap);
        Debug.Log("Rivers Generated");
    }
}
