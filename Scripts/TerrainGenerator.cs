using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    private float[,] noiseMap;
    private Vector2[] sourceArray;

    public int mapWidth = 5000;
    public int mapHeight = 5000;

    public int terrainSeed;
    [Range(1, 10)]
    public int sources;
    public int riverSeed;



    public void GenerateTerrain()
    {
        float noiseScale = 800;
        int octaves = 4;
        float persistence = 0.5f;
        float lacunarity = 2;
        Vector2 offset = new Vector2(0, 0);

        NoiseGenerator noise = new NoiseGenerator(mapWidth, mapHeight, terrainSeed, noiseScale, octaves, persistence, lacunarity, offset);
        noiseMap = noise.GenerateNoiseMap();

        MeshGenerator mg = new MeshGenerator(noiseMap);
        MeshData meshData = mg.GenerateMesh();

        TerrainDrawer drawer = FindObjectOfType<TerrainDrawer>();
        drawer.DrawTerrain(noiseMap, meshData);
        Debug.Log("Terrain Generated");

    }
    public void GenerateSources()
    {
        sourceArray = SourceGenerator.GenerateSources(noiseMap, sources, riverSeed);
        Debug.Log("Sources Generated");
    }
    public void GenerateRivers()
    {
        RiverGenerator rg = new RiverGenerator(noiseMap, sourceArray, riverSeed);

        float[,] riverMap = rg.GenerateRiverMap();

        TributaryStreamGenerator tsg = new TributaryStreamGenerator(noiseMap, riverMap, riverSeed);
        riverMap = tsg.GenerateRiverMap();

        RiverCarver rc = new RiverCarver(noiseMap, riverMap);
        float[,] heightMap = rc.GenerateRiverHeightMap();

        MeshGenerator mg = new MeshGenerator(heightMap);
        MeshData meshData = mg.GenerateMesh();

        TerrainDrawer drawer = FindObjectOfType<TerrainDrawer>();
        drawer.DrawTerrainWithRivers(heightMap, meshData, riverMap);
        Debug.Log("Rivers Generated");
    }
}