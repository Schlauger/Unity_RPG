    using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class EndlessTerrain : MonoBehaviour
{
    const float scale = 5f; // ep.10
    const float viewerThresholdfromTile = 25f;
    const float sqrtviewerThresholdfromTile = viewerThresholdfromTile * viewerThresholdfromTile;
    public LODInfo[] detailLevels;
    public static float maxViewDst;
    public Transform viewer; 
    public Material mapMaterial;
    public static Vector2 viewerPosition;
    Vector2 viewerPrePosition; // ep 9
    static MapGenerator mapGenerator;
    int tileSize;
    int visibleTiles;

    Dictionary<Vector2, TerrainTile> terrainTilesDict = new Dictionary<Vector2, TerrainTile>();
    static List<TerrainTile> existedTiles = new List<TerrainTile>(); // ep.10 
    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        tileSize = MapGenerator.mapTileSize - 1;
        visibleTiles = Mathf.RoundToInt(maxViewDst / tileSize);
        updateVisibleTiles();
    }

    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / scale;
        if ((viewerPrePosition-viewerPosition).sqrMagnitude > sqrtviewerThresholdfromTile) { // ep 9
            viewerPrePosition = viewerPosition;
            updateVisibleTiles();
        } 
        
    }

    void updateVisibleTiles()
    {
        for (int i = 0; i < existedTiles.Count; i++)
        {
            existedTiles[i].setVisible(false);
        }
        existedTiles.Clear();

        int currentTileX = Mathf.RoundToInt(viewerPosition.x / tileSize);
        int currentTileY = Mathf.RoundToInt(viewerPosition.y / tileSize);

        Vector2 viewedChunkCoord;
        for (int yOffset = -visibleTiles; yOffset <= visibleTiles; yOffset++)
        {
            for (int xOffset = -visibleTiles; xOffset <= visibleTiles; xOffset++)
            {

                //Debug.Log("TileX:"+xOffset+", tileY:"+yOffset);
                //timer(20);
                viewedChunkCoord = new Vector2((float)currentTileX + (float)xOffset, (float)currentTileY + (float)yOffset);
                //timer(15);
                if (terrainTilesDict.ContainsKey(viewedChunkCoord))
                {
                    //timer(50);
                    //Debug.Log(terrainTilesDict.ContainsKey(viewedChunkCoord)+"->"+"TileX:"+viewedChunkCoord.x+", tileY:"+viewedChunkCoord.y);
                    terrainTilesDict[viewedChunkCoord].UpdateTile();
                    /* ep 10
                    if (terrainTilesDict[viewedChunkCoord].isVisible()){
                        existedTiles.Add(terrainTilesDict[viewedChunkCoord]);
                    }
                    */
                }
                else {
                    terrainTilesDict.Add(viewedChunkCoord, new TerrainTile(viewedChunkCoord, tileSize, detailLevels, transform, mapMaterial));
                }
            }
        }
    }

/*                    Terrain Tile                    */
/*----------------------------------------------------*/
    public class TerrainTile
    {
        GameObject obj;
        Vector2 position;
        Bounds bounds;

        MeshFilter meshFilter;
        MeshRenderer meshRenderer;
        MeshCollider meshCollider; // ep.13
        LODInfo[] lods; // ep 9
        LODMesh[] lodMeshes;
        MapData mapData;
        bool mapDataReceived;
        int preLODidx = -1;

        public TerrainTile(Vector2 pos, int size,LODInfo[] lods, Transform parent, Material material)
        {
            this.lods = lods; // ep 9
            position = pos * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 position3D = new Vector3(position.x, 0, position.y);

            obj = new GameObject("Terrain Tile [" + position.x + ", " + position.y + "]");
            meshRenderer = obj.AddComponent<MeshRenderer>();
            meshFilter = obj.AddComponent<MeshFilter>();
            meshCollider = obj.AddComponent<MeshCollider>();
            meshRenderer.material = material;
            obj.transform.position = position3D * scale; // ep.10
            obj.transform.parent = parent;
            obj.transform.localScale = Vector3.one * scale;
            setVisible(false);

            lodMeshes = new LODMesh[lods.Length]; //ep 9
            for (int i = 0; i < lods.Length; i++){
                lodMeshes[i] = new LODMesh(lods[i].lod, UpdateTile);

            }
            mapGenerator.RequestMapData(position, OnMapDtReceived);
        }

        void OnMapDtReceived(MapData mapDt){
            this.mapData = mapDt; //EP 9
            mapDataReceived = true;
            Texture2D texture = Utils.textureFromColorMap(mapData.colorMap, MapGenerator.mapTileSize, MapGenerator.mapTileSize);
            meshRenderer.material.mainTexture = texture;
            UpdateTile();
            //mapGenerator.RequestMeshData(mapDt, OnMeshDtReceived);
        }

        // void OnMeshDtReceived(MeshData meshData){
        //     meshFilter.mesh = meshData.CreateMesh();
        // }

        public void UpdateTile() {
            if(mapDataReceived){
                
            float dstFromviewer = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = dstFromviewer <= maxViewDst;
            
            if(visible){
                    int lodIdx = 0;
                    for (int i = 0; i < lods.Length; i++) {
                        if (dstFromviewer>lods[i].visibleDstThreshold) {
                            lodIdx = i + 1;
                        }else{
                            break;
                        }
                    }
                    if (lodIdx != preLODidx) {
                        LODMesh lodMesh = lodMeshes[lodIdx];
                        if(lodMesh.hasMesh){
                            preLODidx = lodIdx;
                            meshFilter.mesh = lodMesh.mesh;
                            meshCollider.sharedMesh = lodMesh.mesh; 
                        }else if (true) {
                            lodMesh.RequestMesh(mapData);
                        }
                    }
                    existedTiles.Add(this); 
                }
            setVisible(visible);
            }
        }

        public void setVisible(bool visible)
        {
            obj.SetActive(visible);
        }
        public bool isVisible()
        {
            return obj.activeSelf;
        }
    }

    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int lod;
        System.Action updateCallback;

        public LODMesh(int lod, System.Action updateCallback){
            this.lod = lod;
            this.updateCallback = updateCallback;
        }

        void OnMeshDataReceived(MeshData meshData){
            mesh = meshData.CreateMesh();
            this.hasMesh = true;
            updateCallback(); // EP 9
        }

        public void RequestMesh(MapData mapData){
            hasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);
        }

    }
    [System.Serializable]
    public struct LODInfo {
        public int lod;
        public float visibleDstThreshold;
    }

    public void timer(int sec){
        StartCoroutine(boltClock(sec));
    }
    IEnumerator boltClock(int sec){
        yield return new WaitForSeconds(sec);
    }
}