using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData {
    public Vector3[] vertices;
    private int[] triangles;
    public Vector2[] uvs;

    private int triangleIdx;

    public MeshData(int meshWidth, int meshHeight){
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth-1)*(meshHeight-1)*6];
        triangleIdx = 0;

    }
    public void addVertex(int idx, float x, float y, float z){
        this.vertices[idx]=new Vector3(x,y,z);
    }
    public void addUV(int idx, float x, float y){
        this.uvs[idx] = new Vector2(x, y);
    }

    public void addTriangle(int p1, int p2, int p3){
        triangles[triangleIdx] = p1;
        triangles[triangleIdx + 1] = p2;
        triangles[triangleIdx + 2] = p3;
        triangleIdx +=3;
    }

    public  Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = this.vertices;
        mesh.triangles = this.triangles;
        mesh.uv = this.uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
