using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData {
    Vector3[] vertices; // ep.12 changed
    int[] triangles;
    Vector2[] uvs;

    Vector3[] borderVertices;  // ep.12
    int[] borderTriangles;
    int borderTriangleIdx;

    private int triangleIdx;

    public MeshData(int verticesPerLine){ // ep.12 changed
        vertices = new Vector3[verticesPerLine * verticesPerLine];
        uvs = new Vector2[verticesPerLine * verticesPerLine];
        triangles = new int[(verticesPerLine-1)*(verticesPerLine-1)*6];
        triangleIdx = 0;

        borderVertices = new Vector3[verticesPerLine * 4 + 4];
        borderTriangles = new int [24 * verticesPerLine]; // 6 * 4
    }

     public void addVertex(Vector3 vertexPos, Vector2 uv, int vertexIdx){ // erp.12
        if (vertexIdx < 0) {
            borderVertices[-vertexIdx - 1] = vertexPos;
        }else{
            vertices[vertexIdx] = vertexPos;
            uvs[vertexIdx] = uv;
        }
    }

    public void addVertex(int idx, float x, float y, float z){
        this.vertices[idx]=new Vector3(x,y,z);
    }
    public void addUV(int idx, float x, float y){
        this.uvs[idx] = new Vector2(x, y);
    }

    public void addTriangle(int p1, int p2, int p3){ // ep.12
        if (p1<0 || p2<0 || p3<0){
            borderTriangles[borderTriangleIdx] = p1;
            borderTriangles[borderTriangleIdx + 1] = p2;
            borderTriangles[borderTriangleIdx + 2] = p3;
            borderTriangleIdx += 3;
        }else{
            triangles[triangleIdx] = p1;
            triangles[triangleIdx + 1] = p2;
            triangles[triangleIdx + 2] = p3;
            triangleIdx += 3;
        }
    }

    Vector3[] CalculateNormals() { //ep.12 
        Vector3[] vertexNormals = new Vector3[vertices.Length];
        int triangleCount = triangles.Length / 3;
        //int normTringIdx, vertxIdxA, vertxIdxB, vertxIdxC;
        //Vector3 triangleNormal;

        for (int i = 0; i < triangleCount; i++){
            int normalTriangIdx = i * 3;
            int vertexIdxA = triangles[normalTriangIdx];
            int vertexIdxB = triangles[normalTriangIdx + 1];
            int vertexIdxC = triangles[normalTriangIdx + 2];
            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIdxA, vertexIdxB, vertexIdxC);
            vertexNormals[vertexIdxA] += triangleNormal;
            //vertexNormals[vertexIdxA].Normalize();
            vertexNormals[vertexIdxB] += triangleNormal;
            //vertexNormals[vertexIdxB].Normalize();
            vertexNormals[vertexIdxC] += triangleNormal;
            //vertexNormals[vertexIdxC].Normalize();
        }

        int borderTriangleCount = borderTriangles.Length / 3;
        for (int i = 0; i < borderTriangleCount; i++){
            int normalTriangleIdx = i * 3;
            int vertxIdxA = borderTriangles[normalTriangleIdx];
            int vertxIdxB = borderTriangles[normalTriangleIdx + 1];
            int vertxIdxC = borderTriangles[normalTriangleIdx + 2];
            Vector3 triangleNormal = SurfaceNormalFromIndices(vertxIdxA, vertxIdxB, vertxIdxC);
            if (vertxIdxA >= 0){
                vertexNormals[vertxIdxA] += triangleNormal;
                //vertexNormals[vertxIdxA].Normalize();
            }
            if (vertxIdxB >= 0){
                vertexNormals[vertxIdxB] += triangleNormal;
                //vertexNormals[vertxIdxB].Normalize();
            }
            if (vertxIdxC >= 0){
                vertexNormals[vertxIdxC] += triangleNormal;
                //vertexNormals[vertxIdxC].Normalize();
            }
            }

        for (int i = 0; i < vertexNormals.Length; i++) {
            vertexNormals[i].Normalize();
        }
        return vertexNormals;
    }

    Vector3 SurfaceNormalFromIndices(int idxA,int idxB, int idxC) {
        Vector3 pointA = (idxA < 0)?borderVertices[-idxA-1]:vertices[idxA];
        Vector3 pointB = (idxB < 0)?borderVertices[-idxB-1]:vertices[idxB];
        Vector3 pointC = (idxC < 0)?borderVertices[-idxC-1]:vertices[idxC];

        Vector3 ab = pointB - pointA;
        Vector3 ac = pointC - pointA;
        return Vector3.Cross(ab, ac).normalized;
    }

    public  Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = this.vertices;
        mesh.triangles = this.triangles;
        mesh.uv = this.uvs;
        //mesh.RecalculateNormals();
        mesh.normals = CalculateNormals();
        return mesh;
    }
}
