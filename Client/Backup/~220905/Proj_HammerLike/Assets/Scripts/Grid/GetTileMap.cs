using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GetTileMap : MonoBehaviour
{
    public Tilemap baseTileMap;
    public Tilemap decoTileMap;
    public Tilemap wallTileMap;
    public Tile tile;
    // Start is called before the first frame update
    void Awake()
    {
        baseTileMap = GetComponent<Tilemap>();
    }

    void Start()
    {
        BoundsInt bounds = baseTileMap.cellBounds;
        TileBase[] allTiles = baseTileMap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    Debug.Log("x:" + x + " y:" + y + " tile:" + tile.name);
                }
                else
                {
                    Debug.Log("x:" + x + " y:" + y + " tile: (null)");
                }
            }
        }


    }
}
