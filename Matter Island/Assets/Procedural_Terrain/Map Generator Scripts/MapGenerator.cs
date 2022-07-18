using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum MapMode{NoiseMap, ColorMap, Mesh};
    public MapMode mode;

    public const int mapTileSize = 241;
	[Range(0,6)]
	public int levelOfDetail;
    public float noiseScale;
    public int levels;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;
    public int seed;
    public TerrainType[] regions;
    public float heightGain;
    public bool autoUpdate;
    public AnimationCurve gainCurve;
    public Vector2 offset;
    
    public void GenarateMap(){
        float[,] noiseMap = Noise.GenarateNoiseMap(mapTileSize,mapTileSize,seed,noiseScale,levels,persistance,lacunarity,offset);
        Color[] colorMap = new Color[mapTileSize * mapTileSize];
        float current;
        for (int y = 0; y < mapTileSize; y++){
            for (int x = 0; x < mapTileSize; x++){
                current=noiseMap[x,y];

                for (int i = 0; i < regions.Length; i++){
                    if(current <= regions[i].height){
                        colorMap[y*mapTileSize + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if(mode == MapMode.NoiseMap){
            display.DrawTexture(Utils.textureFromValueMap(noiseMap));
        }else if(mode == MapMode.ColorMap){
            display.DrawTexture(Utils.textureFromColorMap(colorMap,mapTileSize,mapTileSize));
        }else if (mode==MapMode.Mesh){
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap,heightGain, gainCurve,levelOfDetail), Utils.textureFromColorMap(colorMap, mapTileSize, mapTileSize));
        }
    }


void OnValidate(){
    if(lacunarity<1){
        lacunarity=1;
    }
    if(levels<0){
        levels=0;
    }
}
}
