using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShaderSettings))]
public class ShaderInpector : Editor{
    public override void OnInspectorGUI(){
        base.OnInspectorGUI();

        if(GUILayout.Button("Create")){
            Debug.Log("Button pressed");
        }
    }

}
