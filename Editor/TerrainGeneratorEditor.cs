using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TerrainGenerator mapGen = (TerrainGenerator)target;
        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.GenerateTerrain();
            }
        }

        if (GUILayout.Button ("Generate Terrain"))
        {
            mapGen.GenerateTerrain();
        }
        if (GUILayout.Button("Generate Sources"))
        {
            mapGen.GenerateSources();
        }
        if (GUILayout.Button ("Generate River"))
        {
            mapGen.GenerateRivers();
        }
    }
}
