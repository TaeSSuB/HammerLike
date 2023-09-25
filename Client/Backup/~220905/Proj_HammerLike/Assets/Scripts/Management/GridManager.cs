using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using hger;

public class GridManager : MonoBehaviour
{
    public CPathFinder finder;
    public CGridMap gridMap;
    public Vector2Int gridSize = new Vector2Int(3, 3);
    public Vector2 gridOffset = Vector2.zero;
    public float cellSize = 1f;

    public bool allowDiagonal;
    public bool allowThrow;

    // Start is called before the first frame update
    void Awake()
    {
        gridMap = new CGridMap(gridSize.x, gridSize.y, cellSize, gridOffset);
        finder = new CPathFinder(gridMap, allowDiagonal, allowThrow);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            gridMap.drawGridAll();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(Utils.GetMouseWorldPosition());

            int xx, yy;
            gridMap.GetXY(Utils.GetMouseWorldPosition(), out xx, out yy);

            Debug.Log(new Vector2(xx, yy));
        }
    }
}
