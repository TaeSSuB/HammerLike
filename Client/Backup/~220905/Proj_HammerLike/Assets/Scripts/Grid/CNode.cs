using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CNode
{
    public CNode(bool _isWall, int _x, int _y)
    {
        isWall = _isWall;
        x = _x;
        y = _y;
    }

    public bool isWall;
    public int x, y, gCost, hCost;

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public CNode parent;
}
