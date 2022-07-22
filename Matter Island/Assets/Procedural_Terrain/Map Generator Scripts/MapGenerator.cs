using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading; 
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {
    
    public enum MapMode { NoiseMap, ColorMap, Mesh };
    public MapMode mode;

    public const int mapTileSize = 241;
    [Range(0, 6)]
    public int levelOfDetail;
    public float noiseScale;
    public int levels;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public int seed;
    public float heightGain;
    public AnimationCurve gainCurve;
    public bool autoUpdate;
    public Vector2 offset;
    public TerrainType[] regions;

    Queue<mapThreadInfo<MapData>> mapDtThreadInfoQ = new Queue<mapThreadInfo<MapData>>();

    /*          Generate and Draw Map          */
    /*-----------------------------------------*/
    public void DrawMap(){
        MapData mapData = GenarateMapData();
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if(mode == MapMode.NoiseMap){
            display.DrawTexture(Utils.textureFromValueMap(mapData.heightMap));
        }else if(mode == MapMode.ColorMap){
            display.DrawTexture(Utils.textureFromColorMap(mapData.colorMap,mapTileSize,mapTileSize));
        }else if (mode==MapMode.Mesh){
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap,heightGain, gainCurve,levelOfDetail),
                                Utils.textureFromColorMap(mapData.colorMap, mapTileSize, mapTileSize));
        }
    }

 MapData GenarateMapData(){
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
        return new MapData(noiseMap, colorMap);
    }

    /*               Threading               */
    /*_______________________________________*/

    public void RequestMapData(Action<MapData> callback) {
        ThreadStart my_thread = delegate {
            MapDataThread (callback);
        };
        new Thread(my_thread).Start();
    }

void MapDataThread(Action<MapData> callback)
    {
        MapData mapData = GenarateMapData();
        lock (mapDtThreadInfoQ){
            mapDtThreadInfoQ.Enqueue(new mapThreadInfo<MapData>(callback, mapData));
        }

        void Update(){
            if (mapDtThreadInfoQ.Count > 0) { 
                for (int i = 0; i < mapDtThreadInfoQ.Count; i++){
                    mapThreadInfo<MapData> threadInfo = mapDtThreadInfoQ.Dequeue();
                    threadInfo.callback(threadInfo.param);
                }
            }
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

    struct mapThreadInfo<T> {
        public readonly Action<T> callback;
        public readonly T param;

        public mapThreadInfo(Action<T> callback, T param) {
            this.callback = callback;
            this.param = param;
        }
    }
}
