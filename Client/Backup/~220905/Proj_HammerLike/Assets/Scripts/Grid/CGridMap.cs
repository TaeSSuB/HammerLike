using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGridMap
{
    public CGridMap(int _width, int _height, float _cellSize, Vector2 _originPos)
    {
        width = _width;
        height = _height;
        cellSize = _cellSize;
        originPos = _originPos - new Vector2(width / 2, height / 2);
    }

    public int width, height;
    public float cellSize;
    public Vector2 originPos;

    public void drawGridAll()
    {
        for(int _x = 0; _x < width; _x++)
        {
            for (int _y = 0; _y < height; _y++)
            {
                drawGrid(_x, _y);
            }
        }

        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
    }

    public void drawGrid(int x, int y)
    {
        Vector2 centerPos = GetWorldPosition(x, y);

        Debug.DrawLine(centerPos, GetWorldPosition(x, y + 1), Color.white, 100f);
        Debug.DrawLine(centerPos, GetWorldPosition(x + 1, y ), Color.white, 100f);
    }

    public Vector2 GetWorldPosition(int x, int y)
    {
        return (new Vector2(x, y) * cellSize) + originPos;
    }

    // 월드 좌표와 그리드 사이즈를 통해 타겟 그리드 좌표 추출
    public void GetXY(Vector2 worldPos, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPos - originPos).x / cellSize);
        y = Mathf.FloorToInt((worldPos - originPos).y / cellSize);
    }
}
