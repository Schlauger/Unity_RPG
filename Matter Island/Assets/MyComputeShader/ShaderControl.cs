using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BedgerObj {
    public Vector3 position;
    public Color color;
}


public class ShaderControl : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture renderTexture;

    public Mesh mesh;
    public Material material;
    public int count = 50;
    public int repetitions = 1;
    private List<GameObject> objects;
    private BedgerObj[] data;

    public void CreateObjects() {
        objects = new List<GameObject>();
        data = new BedgerObj[count * count];

        for (int x = 0; x < count; x++){
            for (int y = 0; y < count; y++){
                CreateObject(x,y);
            }   
        }
    }

    public void OnRandomizeGPU(){
        int colorSize = sizeof(float) * 4;
        int vecrtor3Size = sizeof(float) * 3;
        int totalSize = colorSize + vecrtor3Size;
        ComputeBuffer objectsBuffer = new ComputeBuffer(data.Length, totalSize);
        objectsBuffer.SetData(data);

        computeShader.SetBuffer(0, "objects", objectsBuffer); //// may change
        computeShader.SetFloat("resolution", data.Length);
        computeShader.Dispatch(0, data.Length / 10, 1, 1);

        objectsBuffer.GetData(data);
        for (int i = 0; i < objects.Count; i++){
            GameObject obj = objects[i];
            BedgerObj cube = data[i];
            obj.transform.position = cube.position;
            obj.GetComponent<MeshRenderer>().material.SetColor("_Color", cube.color);

            objectsBuffer.Dispose();
        }
    }

    private void OnGUI(){
        if (objects == null) {
            if(GUI.Button(new Rect(0,0,100,50),"Create")){
                CreateObjects();
            }
        }else{
            if (GUI.Button(new Rect(0,0,100,50),"Random CPU")){
                OnRandomizeCPU();
            }
            if (GUI.Button(new Rect(100,0,100,50),"Random GPU")){
                
            }
        }
    }

    private void OnRandomizeCPU() {
        for (int i = 0; i < repetitions; i++){
            for (int j = 0; j < objects.Count; j++){
                GameObject obj = objects[j];
                obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, Random.Range(-0.1f, 0.1f));
                obj.GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());

            }
            
        }
    }

    private void CreateObject(int x, int y){
        GameObject cube = new GameObject("Cube " + x * count + y, typeof(MeshFilter), typeof(MeshRenderer));
        cube.GetComponent<MeshFilter>().mesh = mesh;
        cube.GetComponent<MeshRenderer>().material = new Material(material);
        cube.transform.position = new Vector3(x, y, Random.Range(-0.1f, 0.1f));

        Color color = Random.ColorHSV();
        cube.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
        objects.Add(cube);
        BedgerObj cubeData = new BedgerObj();
        cubeData.position = cube.transform.position;
        cubeData.color = color;
        data[x * count + y] = cubeData;
    }

    void Start(){
        // renderTexture = new RenderTexture(256, 256, 24);
        // renderTexture.enableRandomWrite = true;
        // renderTexture.Create();
        // computeShader.SetTexture(0, "Result", renderTexture);
        // computeShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);

    }

    // private void OnRenderImage(RenderTexture source, RenderTexture destination){
    //     if(renderTexture==null){
    //         renderTexture = new RenderTexture(256,256,24);
    //         renderTexture.enableRandomWrite = true;
    //         renderTexture.Create();
    //     }
    //     computeShader.SetTexture(0, "Result", renderTexture);
    //     computeShader.SetFloat("resolution", renderTexture.width);
    //     computeShader.Dispatch(0,renderTexture.width/10, 1, 1);

    //     Graphics.Blit(renderTexture,destination);
    //     //Render(destination);
    // }

    private void Render(RenderTexture destination) {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
