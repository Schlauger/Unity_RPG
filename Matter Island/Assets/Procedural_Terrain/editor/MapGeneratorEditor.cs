using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(MapGenerator))]
public class MapGeneratorEditor : Editor {
    // Start is called before the first frame update
    public override void OnInspectorGUI(){
        MapGenerator mapGen = (MapGenerator)target;

        if(DrawDefaultInspector()){
            if(mapGen.autoUpdate){
                mapGen.GenarateMap();
            }
        }

        if(GUILayout.Button("Generate")){
            mapGen.GenarateMap();
        }
    }
}
