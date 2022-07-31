using System.Collections;
using UnityEngine;
/// <summary> DFS
///</summary> 
public static class MeshGenerator
{

    public static MeshData GenerateTerrainMesh(float[,] heightMap,float heightGain, AnimationCurve _heightGainCurve, int levelOfDetail){
        AnimationCurve heightGainCurve = new AnimationCurve(_heightGainCurve.keys);
        int meshSimplificationIncrement = (levelOfDetail == 0)?1:levelOfDetail * 2;
        int borderSize = heightMap.GetLength(0); // ep.12 changed
        //int height = heightMap.GetLength(1);
        int meshSize = borderSize - 2*meshSimplificationIncrement;
        int meshSizeUnsimplified = borderSize - 2;
        float minX = (meshSizeUnsimplified - 1) / (-2f);
        float minZ = (meshSizeUnsimplified - 1) / (2f);

        
        int verticesPerLine = (meshSize - 1) / meshSimplificationIncrement + 1;
        MeshData meshData = new MeshData(verticesPerLine);
        // int vertexIdx = 0;
        int[,] vertexIndicesMap = new int[borderSize, borderSize]; // ep.12
        int meshVertexIdx = 0;
        int borderVertexIdx = -1;
        bool isBorderVertex;
        for (int y = 0; y < borderSize; y += meshSimplificationIncrement){
            for (int x = 0; x < borderSize; x += meshSimplificationIncrement){
                isBorderVertex = (y == 0) || (y == borderSize - 1) || (x == 0) || (x == borderSize - 1);

                if (isBorderVertex) {
                    vertexIndicesMap[x, y] = borderVertexIdx;
                    borderVertexIdx--;
                }else{
                    vertexIndicesMap[x, y] = meshVertexIdx;
                    meshVertexIdx++;
                }
            }
        }

        int vertexIdx;      //ep.12
        Vector3 vertexPos;
        Vector2 percent;
        float height;
        int a, b, c, d;
        for (int y = 0; y < borderSize; y += meshSimplificationIncrement){
            for (int x = 0; x < borderSize; x += meshSimplificationIncrement){
                vertexIdx = vertexIndicesMap[x, y];//ep.12
                //meshData.uvs[vertexIdx] = new Vector2( ((width-1)-x) / (float)width, y / (float)height);
                //meshData.addUV(vertexIdx,(x)/ (float)borderSize, y/(float)borderSize)
                percent = new Vector2((x-meshSimplificationIncrement) / (float)meshSize, (y-meshSimplificationIncrement) / (float)meshSize);
                height = (float)heightGainCurve.Evaluate(heightMap[x, y]) * (float)heightGain;
                //meshData.vertices[vertexIdx] = new Vector3(minX + x,heightMap[x,y],minZ-y);
                //meshData.addVertex(vertexIdx, (float)minX + (float)x, (float)heightGainCurve.Evaluate(heightMap[x, y]) * (float)heightGain, (float)minZ - (float)y);
                vertexPos=new Vector3((float)minX + (float)percent.x*meshSizeUnsimplified, height, (float)minZ - (float)percent.y*meshSizeUnsimplified);

                meshData.addVertex(vertexPos, percent, vertexIdx);

                if (x < borderSize - 1 && y < borderSize - 1){
                    a = vertexIndicesMap[x, y];
                    b = vertexIndicesMap[x + meshSimplificationIncrement, y];
                    c = vertexIndicesMap[x, y + meshSimplificationIncrement];
                    d = vertexIndicesMap[x + meshSimplificationIncrement, y + meshSimplificationIncrement];
                    //meshData.addTriangle(vertexIdx, vertexIdx + verticesPerLine  + 1, vertexIdx + verticesPerLine );
                    meshData.addTriangle(a, b, c);
                    meshData.addTriangle(d, a, b);
                }
                vertexIdx++;
            }
        }
        return meshData;
    }
}
