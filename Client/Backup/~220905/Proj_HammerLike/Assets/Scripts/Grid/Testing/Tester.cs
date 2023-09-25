using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using hger;

public class Tester : MonoBehaviour
{
    public CPathFinder finder;
    public GameObject player;
    public GameObject monster;
    public float delay = 0.5f;

    CGridMap gridMap;
    public Vector2Int gridSize = new Vector2Int(3, 3);
    public Vector2 gridOffset = Vector2.zero;
    public float cellSize = 1f;

    public bool allowDiagonal;
    public bool allowThrow;

    // Start is called before the first frame update
    void Start()
    {
        gridMap = new CGridMap(gridSize.x, gridSize.y, cellSize, gridOffset);
        finder = new CPathFinder(gridMap, allowDiagonal, allowThrow);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            finder.PathFinding(monster.transform.position, player.transform.position);

            var nodeList = finder.finalNodeList;

            for (int i = 0; i < nodeList.Count - 1; i++)
            {
                Vector2Int targetVec = new Vector2Int(nodeList[i].x, nodeList[i].y);
                Vector2Int parentVec = new Vector2Int(nodeList[i + 1].x, nodeList[i + 1].y);
                Debug.DrawLine(
                    gridMap.GetWorldPosition(targetVec.x, targetVec.y) - gridOffset * cellSize,
                    gridMap.GetWorldPosition(parentVec.x, parentVec.y) - gridOffset * cellSize,
                    Color.red,
                    100f
                    );
            }

            StartCoroutine(CoMonsterMove(finder, monster, delay));
        }

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

    IEnumerator CoMonsterMove(CPathFinder finder, GameObject obj ,float delay)
    {
        var nodeList = finder.finalNodeList;

        while (true)
        {
            for (int i = 0; i < nodeList.Count; i++)
            {
                Vector2Int targetVec = new Vector2Int(nodeList[i].x, nodeList[i].y);
                obj.transform.position = gridMap.GetWorldPosition(targetVec.x, targetVec.y) - gridOffset * cellSize;
                Debug.Log("Mob Move to " + targetVec);
                yield return new WaitForSeconds(delay);
            }

            Debug.Log("Mob Move Finish");
            break;
            //yield return new WaitForSeconds(delay);
        }

        yield return null;
    }
}
