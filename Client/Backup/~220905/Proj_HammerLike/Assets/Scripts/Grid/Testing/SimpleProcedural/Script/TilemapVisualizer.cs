using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    Tilemap floorTilemap, wallTilemap;
    [SerializeField]
    TileBase floorTile, wallTop;
    [SerializeField]
    GameObject roomsObj;
    [SerializeField]
    CRoom gridRoom;

    public void SetGridRoom(Vector2Int size, Vector2 centerPos)
    {
        gridRoom.gridSize.x = size.x;
        gridRoom.gridSize.y = size.y;

        if (size.x % 2 == 0)
            gridRoom.gridOffset.x = 0f;
        else
            gridRoom.gridOffset.x = -0.5f;

        if (size.y % 2 == 0)
            gridRoom.gridOffset.y = 0f;
        else
            gridRoom.gridOffset.y = -0.5f;

        gridRoom.gameObject.transform.position = centerPos;
        var instance = Instantiate(gridRoom.gameObject, roomsObj.transform);
    }

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    internal void PaintSingleBasicWall(Vector2Int position)
    {
        PaintSingleTile(wallTilemap, wallTop, position);
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        foreach(var roomObj in roomsObj.GetComponentsInChildren<CRoom>())
        {
            DestroyImmediate(roomObj.gameObject);
        }
    }
}
