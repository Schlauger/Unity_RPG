using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Tooltip("Setting for the Compute Shader")]
[CreateAssetMenu(fileName ="ShaderSettings", menuName ="Schlauger/ShaderSettings")]
public class ShaderSettings : ScriptableObject{
    [Tooltip("Mesh to apply the shader")]
    public Mesh srcMesh;
    [Tooltip("subMesh Index of the mesh")]
    public int sourceSubMeshIdx;
    public Vector3 scale;
    public Vector3 rotation;
    public float height;
}
