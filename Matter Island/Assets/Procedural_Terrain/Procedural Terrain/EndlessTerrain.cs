using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class EndlessTerrain : MonoBehaviour {

    public const float maxViewDst = 200;
    public Transform viewer;
    public Material mapMaterial;

    public static Vector2 viewerPosition;
    static MapGenerator mapGenerator;
    int tileSize;
    int visibleTiles;

    Dictionary<Vector2, TerrainTile> terrainTilesDict = new Dictionary<Vector2, TerrainTile>();
    List<TerrainTile> existedTiles = new List<TerrainTile>();

    void Start(){
        mapGenerator = FindObjectOfType<MapGenerator>();
        tileSize = MapGenerator.mapTileSize - 1;
        visibleTiles = Mathf.RoundToInt(maxViewDst / tileSize);
    }

    void Update(){
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        updateVisibleTiles();
    }

    void updateVisibleTiles(){  
        for (int i = 0; i < existedTiles.Count; i++){
            existedTiles[i].setVisible(false);
        }
        existedTiles.Clear();

        int currentTileX = Mathf.RoundToInt(viewerPosition.x / tileSize);
        int currentTileY = Mathf.RoundToInt(viewerPosition.y / tileSize);
        

        for (int yOffset = -visibleTiles; yOffset <= visibleTiles; yOffset++){
            for (int xOffset = -visibleTiles; xOffset <= visibleTiles; xOffset++){
                
                //Debug.Log("TileX:"+xOffset+", tileY:"+yOffset);
                //timer(20);
                Vector2 viewedChunkCoord = new Vector2((float)currentTileX + (float)xOffset, (float)currentTileY + (float)yOffset);
                //timer(15);
                if (terrainTilesDict.ContainsKey(viewedChunkCoord)){
                    //timer(50);
                    //Debug.Log(terrainTilesDict.ContainsKey(viewedChunkCoord)+"->"+"TileX:"+viewedChunkCoord.x+", tileY:"+viewedChunkCoord.y);
                    terrainTilesDict[viewedChunkCoord].UpdateTile();
                    if (terrainTilesDict[viewedChunkCoord].isVisible()){
                        existedTiles.Add(terrainTilesDict[viewedChunkCoord]);
                    }
                }else{
                    terrainTilesDict.Add(viewedChunkCoord, new TerrainTile(viewedChunkCoord, tileSize,transform, mapMaterial));
                }
                
                
            }
        }

    }


    public class TerrainTile {
        GameObject obj;
        Vector2 position;
        Bounds bounds;

        MeshFilter meshFilter;
        MeshRenderer meshRenderer;

        public TerrainTile(Vector2 pos, int size, Transform parent, Material material){
            position = pos * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 position3D = new Vector3(position.x, 0, position.y);

            obj = new GameObject("Terrain Tile");
            meshRenderer = obj.AddComponent<MeshRenderer>();
            meshFilter = obj.AddComponent<MeshFilter>();
            meshRenderer.material = material;

            obj.transform.position = position3D;
            obj.transform.parent = parent;
            setVisible(false);

            mapGenerator.RequestMapData(OnMapDtReceived);
        } 

        void OnMapDtReceived(MapData mapDt) {
            mapGenerator.RequestMeshData(mapDt, OnMapDtReceived);
        }

        void OnMapDtReceived(MeshData meshData){
            meshFilter.mesh = meshData.CreateMesh();
        }

        public void UpdateTile() {
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

     public void timer(int sec){
        StartCoroutine(boltClock(sec));
    }
    IEnumerator boltClock(int sec){
        yield return new WaitForSeconds(sec);
    }
}