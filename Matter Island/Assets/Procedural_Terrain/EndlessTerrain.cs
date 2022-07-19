using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{

    public const float maxViewDst = 450;
    public Transform viewer;

    public static Vector2 viewerPosition;
    int tileSize;
    int chunksvisibleTiles;

    Dictionary<Vector2, TerrainTile> terrainTilesDict = new Dictionary<Vector2, TerrainTile>();
    List<TerrainTile> existedTiles = new List<TerrainTile>();

    void Start(){
        tileSize = MapGenerator.mapTileSize - 1;
        chunksvisibleTiles = Mathf.RoundToInt(maxViewDst / tileSize);
    }

    void Update(){
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        updateVisibleTiles();
    }

    void updateVisibleTiles(){  
        for (int i = 0; i < existedTiles.Count; i++){
            existedTiles[i].setVisible(false);
        }
        terrainTilesDict.Clear();

        int currentTileX = Mathf.RoundToInt(viewerPosition.x / tileSize);
        int currentTileY = Mathf.RoundToInt(viewerPosition.y / tileSize);
        Vector2 viewedChunkCoord;

        for (int yOffset = -chunksvisibleTiles; yOffset <= chunksvisibleTiles; yOffset++){
            for (int xOffset = -chunksvisibleTiles; xOffset <= chunksvisibleTiles; xOffset++){
                viewedChunkCoord= new Vector2(currentTileX + xOffset, currentTileY + yOffset);
                if (terrainTilesDict.ContainsKey(viewedChunkCoord)){
                    terrainTilesDict[viewedChunkCoord].UpdateTile();
                    if (terrainTilesDict[viewedChunkCoord].isVisible()){
                        existedTiles.Add(terrainTilesDict[viewedChunkCoord]);
                    }
                }else{
                    terrainTilesDict.Add(viewedChunkCoord, new TerrainTile(viewedChunkCoord, tileSize,transform));
                }
            }
        }

    }


    public class TerrainTile
    {
        GameObject obj;
        Vector2 position;
        Bounds bounds;

        public TerrainTile(Vector2 pos, int size, Transform parent){
            position = pos * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 position3D = new Vector3(position.x, 0, position.y);

            obj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            obj.transform.position = position3D;
            obj.transform.localScale = (Vector3.one * size) / 10f;
            obj.transform.parent = parent;
            setVisible(false);
        }

        public void UpdateTile()
        {
            float dstFromviewer = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = dstFromviewer <= maxViewDst;
            setVisible(visible);
        }

        public void setVisible(bool visible){
            obj.SetActive(visible);
        }
        public bool isVisible() {
            return obj.activeSelf;
        }
    }
}