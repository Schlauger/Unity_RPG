using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise 
{
    public static float[,] GenarateNoiseMap(int mapWidth,int mapHeight,int seed, float scale,int levels,float persistance, float lacunarity, Vector2 offset){
        float[,] noiseMap = new float[mapWidth,mapHeight];

        float pointX;
        float pointY;
        float perlinValue;

        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        System.Random rng = new System.Random(seed);
        Vector2[] levelOffsets = new Vector2[levels];

        for (int i = 0; i < levels; i++){
            float offsetX = rng.Next(-100000, 100000) + offset.x;
            float offsetY = rng.Next(-100000, 100000) + offset.y;
            levelOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if(scale<=0){
            scale=0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapWidth; y++){
            for (int x = 0; x < mapWidth; x++){
                amplitude = 1;
                frequency = 1;
                noiseHeight = 0;

                for (int i = 0; i < levels; i++){
                    pointX = (x - halfWidth) / scale * frequency + levelOffsets[i].x;
                    pointY = (y - halfHeight) / scale * frequency + levelOffsets[i].y;
                    
                    perlinValue = Mathf.PerlinNoise(pointX, pointY) * 2 - 1;
                    noiseMap[x,y]= perlinValue;
                    noiseHeight += perlinValue * amplitude;
                    
                    amplitude *= persistance;
                    frequency *=lacunarity;
                }
                if(noiseHeight > maxNoiseHeight){
                    maxNoiseHeight = noiseHeight;
                }else if(noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;
                    
                }
                noiseMap[x,y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++){
            for (int x = 0; x < mapWidth; x++){
                noiseMap[x,y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
            
        }
        return noiseMap;
    }

}
