using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum MapMode{NoiseMap, ColorMap};
    public MapMode mode;

    public TerrainType[] regions; 
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public bool autoUpdate;
    [Range(0,1)]
    public int levels;
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
    public void GenarateMap(){
        float[,] noiseMap = Noise.GenarateNoiseMap(mapWidth,mapHeight,seed,noiseScale,levels,persistance,lacunarity,offset);
        Color[] colorMap = new Color[mapWidth * mapHeight];
        float current;
        for (int y = 0; y < mapHeight; y++){
            for (int x = 0; x < mapWidth; x++){
                current=noiseMap[x,y];
                for (int i = 0; i < regions.Length; i++){
                    if(current <= regions[i].height){
                        colorMap[y*mapWidth + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if(mode == MapMode.NoiseMap){
            display.DrawTexture(Utils.textureFromValueMap(noiseMap));
        }else if(mode == MapMode.ColorMap){
            display.DrawTexture(Utils.textureFromColorMap(colorMap,mapWidth,mapHeight)); 
        }
    }


void OnValidate(){
    if(mapWidth<1){
        mapWidth=1;
    }
    if(mapHeight<1){
        mapHeight=1;        
    }
    if(lacunarity<1){
        lacunarity=1;
    }
    if(levels<0){
        levels=0;
    }
}
}
