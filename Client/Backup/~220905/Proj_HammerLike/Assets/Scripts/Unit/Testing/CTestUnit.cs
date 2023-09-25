using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using hger;

public class CTestUnit : MonoBehaviour
{
    public GridManager gridManager;

    public GameObject player;
    public GameObject monster;
    public float delay = 0.5f;
    public float speed = 1f;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gridManager.finder.PathFinding(monster.transform.position, player.transform.position);

            var nodeList = gridManager.finder.finalNodeList;

            for (int i = 0; i < nodeList.Count - 1; i++)
            {
                Vector2Int targetVec = new Vector2Int(nodeList[i].x, nodeList[i].y);
                Vector2Int parentVec = new Vector2Int(nodeList[i + 1].x, nodeList[i + 1].y);
                Debug.DrawLine(
                    gridManager.gridMap.GetWorldPosition(targetVec.x, targetVec.y) - gridManager.gridOffset * gridManager.cellSize,
                    gridManager.gridMap.GetWorldPosition(parentVec.x, parentVec.y) - gridManager.gridOffset * gridManager.cellSize,
                    Color.red,
                    100f
                    );
            }

            StartCoroutine(CoMonsterMove(gridManager.finder, monster, delay));
        }

    }

    IEnumerator CoMonsterMove(CPathFinder finder, GameObject obj, float delay)
    {
        var nodeList = finder.finalNodeList;

        for (int i = 0; i < nodeList.Count; i++)
        {
            var currentNode = nodeList[i];

            Vector3 targetPos = gridManager.gridMap.GetWorldPosition(currentNode.x, currentNode.y) - gridManager.gridOffset * gridManager.cellSize;

            Vector3 objPos = obj.transform.position;

            var dir = (targetPos - objPos).normalized;

            float timer = 0f;

            while (true)
            {
                timer += Time.deltaTime;

                obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetPos, Time.deltaTime * speed);

                if (obj.transform.position == targetPos)
                    break;

                if (timer > 10f)
                    break;

                //yield return new WaitForSeconds(Time.deltaTime);
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForSeconds(delay);
        }

        yield return null;
    }
}
