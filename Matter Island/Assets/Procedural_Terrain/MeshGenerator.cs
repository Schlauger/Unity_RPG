using System.Collections;
using UnityEngine;
/// <summary> DFS
///</summary> 
public static class MeshGenerator
{

    public static MeshData GenerateTerrainMesh(float[,] heightMap,float heightGain, AnimationCurve heightGainCurve, int levelOfDetail){
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float minX = (width - 1) / (-2f);
        float minZ = (height - 1) / (2f);
        

        int meshSimplificationIncrement = (levelOfDetail == 0)?1:levelOfDetail * 2;
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;
        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
        int vertexIdx = 0;

        for (int y = 0; y < height; y += meshSimplificationIncrement){
            for (int x = 0; x < width; x += meshSimplificationIncrement){
                //meshData.vertices[vertexIdx] = new Vector3(minX + x,heightMap[x,y],minZ-y);
                meshData.addVertex(vertexIdx, (float)minX + (float)x, (float)heightGainCurve.Evaluate(heightMap[x, y]) * (float)heightGain, (float)minZ - (float)y);
                //meshData.uvs[vertexIdx] = new Vector2( ((width-1)-x) / (float)width, y / (float)height);
                meshData.addUV(vertexIdx,((width-1)-x)/ (float)width, y/(float)height);


                if (x < width - 1 && y < height - 1){
                    meshData.addTriangle(vertexIdx, vertexIdx + verticesPerLine  + 1, vertexIdx + verticesPerLine );
                    meshData.addTriangle(vertexIdx + verticesPerLine + 1, vertexIdx, vertexIdx + 1);
                }
                vertexIdx++;
            }
        }
        return meshData;
    }
}
