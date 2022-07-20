using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour {
    /*---------------------------------------------------------------------------------------------*/
    /*____________________________________ Timers ____________________________________*/
    public static void timer(){
        //StartCoroutine(Utils.boltClock());
    }
    static IEnumerator boltClock(){
        yield return new WaitForSeconds(10);
    }
    /*---------------------------------------------------------------------------------------------*/
    /*____________________________________ Texture Generators ____________________________________*/
    public static Texture2D textureFromColorMap(Color[] colorMap, int width, int height){
        Texture2D texture = new Texture2D(width,height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

        public static Texture2D textureFromValueMap(float[,] valueMap){
        int width = valueMap.GetLength(0);
        int height = valueMap.GetLength(1);
        //Texture2D texture = new Texture2D(width,height);
        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++){
                colorMap[y*width + x] = Color.Lerp(Color.black, Color.white, valueMap[x, y]);
            }
        }
        //texture.SetPixels(colorMap);
        //texture.Apply();
        return textureFromColorMap(colorMap,width,height);
    }
}

[System.Serializable]
public struct TerrainType{
    public string type;
    public float height;
    public Color colour;

}