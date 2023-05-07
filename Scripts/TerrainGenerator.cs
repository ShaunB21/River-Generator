using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    private float[,] noiseMap;
    private float[,] noiseMapClone;
    private Vector2[] sourceArray;

    public int mapWidth = 5000;
    public int mapHeight = 5000;

    public int terrainSeed;
    [Range(1, 10)]
    public int sources;
    public int riverScale;
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
        noiseMapClone = noiseMap.Clone() as float[,];

        MeshGenerator mg = new MeshGenerator(noiseMap);
        MeshData meshData = mg.GenerateMesh();

        TerrainDrawer drawer = FindObjectOfType<TerrainDrawer>();
        drawer.DrawTerrain(noiseMap, meshData);
        Debug.Log("Terrain Generated");

    }
    public void GenerateSources()
    {
        SourceGenerator sg = new SourceGenerator(noiseMap, sources, riverSeed);
        sourceArray = sg.GenerateSources();
        Debug.Log("Sources Generated");
    }
    public void GenerateRivers()
    {
        RiverGenerator rg = new RiverGenerator(noiseMap, sourceArray, riverSeed, riverScale);

        float[,] riverMap = rg.GenerateRiverMap();
        float[,] riverMapClone = riverMap.Clone() as float[,];

        TributaryStreamGenerator tsg = new TributaryStreamGenerator(noiseMap, riverMapClone, riverSeed);
        riverMapClone = tsg.GenerateRiverMap();

        RiverCarver rc = new RiverCarver(noiseMapClone, riverMapClone);
        float[,] heightMap = rc.GenerateRiverHeightMap();

        MeshGenerator mg = new MeshGenerator(heightMap);
        MeshData meshData = mg.GenerateMesh();

        TerrainDrawer drawer = FindObjectOfType<TerrainDrawer>();
        drawer.DrawTerrainWithRivers(heightMap, meshData, riverMapClone);
        noiseMapClone = noiseMap.Clone() as float[,];
        riverMapClone = riverMap.Clone() as float[,];
        Debug.Log("Rivers Generated");
    }
}