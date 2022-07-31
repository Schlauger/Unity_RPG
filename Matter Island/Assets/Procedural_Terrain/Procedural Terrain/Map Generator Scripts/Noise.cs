using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise 
{
    public enum NormalizeMode { Local, Global };

    public static float[,] GenarateNoiseMap(int mapWidth,int mapHeight,int seed, float scale,int levels,float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode){
        float[,] noiseMap = new float[mapWidth,mapHeight];

        float pointX;
        float pointY;
        float perlinValue;

        
        float noiseHeight = 0;

        System.Random rng = new System.Random(seed);
        Vector2[] levelOffsets = new Vector2[levels];

        float amplitude = 1;
        float frequency = 1;
        float maxPossibleHeight = 0; // ep.10

        for (int i = 0; i < levels; i++){
            float offsetX = rng.Next(-100000, 100000) + offset.x;
            float offsetY = rng.Next(-100000, 100000) - offset.y; // ep.10 changed
            levelOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude; // ep.10
            amplitude *= persistance;
        }

        if(scale<=0){
            scale=0.0001f;
        }

        float maxLocalNoiseHeight = float.MinValue; // ep.10 changed
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapWidth; y++){
            for (int x = 0; x < mapWidth; x++){
                amplitude = 1;
                frequency = 1;
                noiseHeight = 0;

                for (int i = 0; i < levels; i++){
                    //pointX = (x - halfWidth) / scale * frequency + levelOffsets[i].x;
                    pointX = (x - halfWidth + levelOffsets[i].x) / scale * frequency; // ep.10 changed
                    pointY = (y - halfHeight + levelOffsets[i].y) / scale * frequency;

                    perlinValue = Mathf.PerlinNoise(pointX, pointY) * 2 - 1;
                    noiseMap[x,y]= perlinValue;
                    noiseHeight += perlinValue * amplitude;
                    
                    amplitude *= persistance;
                    frequency *=lacunarity;
                }
                if(noiseHeight > maxLocalNoiseHeight){
                    maxLocalNoiseHeight = noiseHeight;
                }else if(noiseHeight < minLocalNoiseHeight) {
                    minLocalNoiseHeight = noiseHeight;
                    
                }
                noiseMap[x,y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++){
            for (int x = 0; x < mapWidth; x++){
                if (normalizeMode==NormalizeMode.Local) {  // ep.10
                    noiseMap[x,y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }else{
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight);
                    noiseMap[x, y] =Mathf.Clamp(normalizedHeight,0,int.MaxValue);
                }
            }
        }
        return noiseMap;
    }

}
