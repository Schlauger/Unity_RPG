using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour {

    public const float maxViewDst = 300;
    public Transform viewer;

    public static Vector2 viewerPosition;
    int tileSize;
    int visibleTiles;

    void Start() {
        tileSize = MapGenerator.mapTileSize - 1;
        visibleTiles = Mathf.RoundToInt(maxViewDst / tileSize);
    }
}
