using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using hger;

public class CPathFinder
{
    public CPathFinder()
    {
        Debug.LogError("CGridMap lost");
        return;
    }
    public CPathFinder(CGridMap _gridMap, bool _allowDiagonal, bool _allowThrow)
    {
        gridMap = _gridMap;
        width = _gridMap.width;
        height = _gridMap.height;
        cellSize = _gridMap.cellSize;
        originPos = _gridMap.originPos;
        allowDiagonal = _allowDiagonal;
        allowThrow = _allowThrow;
    }

    CGridMap gridMap;
    int width, height;
    float cellSize;
    Vector2 originPos;

    CNode startNode, targetNode, currentNode; // 시작, 목표, 현재
    CNode[,] nodeArray; // 전체 노드 배열
    List<CNode> openList, closeList; // 열린 리스트, 닫힌 리스트

    public List<CNode> finalNodeList;
    
    bool allowDiagonal, allowThrow;

    public void Initialize(Vector2 _startPos, Vector2 _targetPos)
    {
        int startX, startY, targetX, targetY;
        gridMap.GetXY(_startPos, out startX, out startY);
        gridMap.GetXY(_targetPos, out targetX, out targetY);

        startNode = nodeArray[startX, startY];
        targetNode = nodeArray[targetX, targetY];
    }

    public void PathFinding(Vector2 _startPos, Vector2 _targetPos)
    {
        if (nodeArray == null)
            nodeArray = new CNode[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                bool isWall = false;

                foreach (Collider2D col in Physics2D.OverlapCircleAll(gridMap.GetWorldPosition(i, j) + new Vector2(0.5f, 0.5f), 0.4f * cellSize))
                {
                    // 현재 노드 제외
                    if (col.OverlapPoint(_startPos)) continue;

                    // 벽 & 유닛 콜리더 Block 처리
                    if (col.gameObject.layer == LayerMask.NameToLayer("Wall") ||
                        col.gameObject.layer == LayerMask.NameToLayer("Unit"))
                        isWall = true;
                    else
                        isWall = false;
                }

                nodeArray[i, j] = new CNode(isWall, i, j);
            }
        }
        //SetStartGrid(startPos);
        //SetTargetGrid(targetPos);

        int startX, startY, targetX, targetY;
        gridMap.GetXY(_startPos, out startX, out startY);
        gridMap.GetXY(_targetPos, out targetX, out targetY);

        // 타겟의 영역 이탈 판별
        if (width <= startX ||
            height <= startY ||
            0 > startX ||
            0 > startY ||
            width <= targetX ||
            height <= targetY ||
            0 > targetX ||
            0 > targetY)
        {
            //Debug.LogWarning("Target Pos Get Out of Range");
            //finalNodeList = new List<CNode>(); // 최근 탐색 경로 초기화
            return;
        }

        startNode = nodeArray[startX, startY];
        targetNode = nodeArray[targetX, targetY];

        if (startNode.isWall || targetNode.isWall)
        {
            //Debug.LogWarning("Start or Target node is not correct");
            return;
        }

        if (startNode == null)
            startNode = nodeArray[0, 0];
        if (targetNode == null)
            targetNode = nodeArray[width - 1, height - 1];

        openList = new List<CNode>() { startNode };
        closeList = new List<CNode>();
        finalNodeList = new List<CNode>();

        while (openList.Count > 0)
        {
            currentNode = openList[0];

            for(int i = 0; i < openList.Count; i++)
            {
                // fCost(total cost) 같거나 작고, hCost(직선거리) 더 작을 때
                if (openList[i].fCost <= currentNode.fCost && openList[i].hCost < currentNode.hCost)
                    currentNode = openList[i];
            }

            openList.Remove(currentNode);
            closeList.Add(currentNode);

            // Last Loop
            if(currentNode == targetNode)
            {
                CNode currentTargetNode = targetNode;
                
                // 맺어진 부모 관계(연결 관계)를 거슬러 올라가며 finalNodeList에 추가
                while(currentTargetNode != startNode)
                {
                    finalNodeList.Add(currentTargetNode);
                    currentTargetNode = currentTargetNode.parent;
                }

                finalNodeList.Add(startNode);
                finalNodeList.Reverse(); // 뒤집어서 순서 정렬

                for (int i = 0; i < finalNodeList.Count - 1; i++)
                {
                    Vector2 start = gridMap.GetWorldPosition(finalNodeList[i].x, finalNodeList[i].y);
                    //Debug.Log(start);
                    Vector2 end = gridMap.GetWorldPosition(finalNodeList[i + 1].x, finalNodeList[i + 1].y);
                    //Debug.Log(end);
                    Debug.DrawLine(start, end, Color.white, 1f);
                }

                return;
            }


            // ↗↖↙↘
            if (allowDiagonal)
            {
                AddOpenList(currentNode.x + 1, currentNode.y + 1);
                AddOpenList(currentNode.x - 1, currentNode.y + 1);
                AddOpenList(currentNode.x - 1, currentNode.y - 1);
                AddOpenList(currentNode.x + 1, currentNode.y - 1);
            }

            // ↑ → ↓ ←
            AddOpenList(currentNode.x, currentNode.y + 1);
            AddOpenList(currentNode.x + 1, currentNode.y);
            AddOpenList(currentNode.x, currentNode.y - 1);
            AddOpenList(currentNode.x - 1, currentNode.y);
        }



    }

    void AddOpenList(int x, int y)
    {
        int straightCost = 10;
        int diagonalCost = 14;

        // 1. 그리드 맵 안에 존재하는가?
        // 2. 벽인가?
        // 3. closeList에 포함 되어 있는가?
        if (x <= width - 1 && 
            y <= height - 1 && 
            x >= 0 && 
            y >= 0 && 
            !nodeArray[x, y].isWall&& 
            !closeList.Contains(nodeArray[x, y]))
        {
            // 대각 이동
            // 수직 & 수평의 벽 동시 존재할 시 통과 불가
            if (allowDiagonal)
                if (nodeArray[currentNode.x, y].isWall && 
                    nodeArray[x, currentNode.y].isWall)
                    return;

            // 코너 이동
            // 벽 인식시 대각 이동 불가
            if (!allowThrow)
                if (nodeArray[currentNode.x, y].isWall ||
                    nodeArray[x, currentNode.y].isWall)
                    return;

            CNode neighborNode = nodeArray[x, y];

            int moveCost = currentNode.gCost;

            // 수직 혹은 수평선 dir
            if (currentNode.x == x ||
                currentNode.y == y) 
            {
                moveCost += straightCost;
            }
            // 대각선 dir
            else
            {
                moveCost += diagonalCost;
            }

            // 1. 예상 이동 비용이 이웃 노드의 비용보다 적게 드는가?
            // 2. 혹은 열린 리스트에 이웃 노드가 포함되어 있지 않는가?
            if (moveCost < neighborNode.gCost ||
                !openList.Contains(neighborNode))
            {
                neighborNode.gCost = moveCost;
                neighborNode.hCost = (
                    Mathf.Abs(neighborNode.x - targetNode.x) 
                    + Mathf.Abs(neighborNode.y - targetNode.y)
                    ) * straightCost;
                neighborNode.parent = currentNode;

                openList.Add(neighborNode);
            }

        }
    }

}
