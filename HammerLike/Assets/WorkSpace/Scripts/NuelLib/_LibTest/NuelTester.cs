using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NuelLib;
using NuelLib.AI;
using UnityEditor.Experimental.GraphView;

public class NuelTester : MonoBehaviour
{
    List<AStar2D.Node2D> current2D = new List<AStar2D.Node2D>();
    List<AStar3D.Node3D> current3D = new List<AStar3D.Node3D>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.A))
        {
            if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                current2D.Clear();
                current3D.Clear();  
                
                current2D = AStarExample2D();
            }
            else if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                current2D.Clear();
                current3D.Clear();

                current3D = AStarExample3D();
            }
        }
    }

    List<AStar2D.Node2D> AStarExample2D()
    {
        return AStar2D.AStar2DExample();
    }

    List<AStar3D.Node3D> AStarExample3D()
    {
        return AStar3D.AStar3DExample();
    }

    private void OnDrawGizmos()
    {
        if(current2D.Count > 0)
        {
            var example2DMap = AStar2D.ExampleMap();

            for (int y = 0; y < example2DMap.GetLength(1); y++)
            {
                for (int x = 0; x < example2DMap.GetLength(0); x++)
                {
                    if (example2DMap[x, y] == 1)
                    {
                        Gizmos.color = Color.black;
                        Gizmos.DrawCube(new Vector2(x, y), Vector3.one * 0.8f);
                    }
                }
            }

            foreach (var node in current2D)
            {
                Gizmos.color = node.color;
                var nodePos = new Vector2(node.X, node.Y);
                Gizmos.DrawCube(nodePos, Vector3.one * 0.25f);

                if(node.Parent != null)
                {
                    Gizmos.color = Color.red;
                    var parentPos = new Vector2(node.Parent.X, node.Parent.Y);
                    Gizmos.DrawLine(nodePos, parentPos);
                }
            }

            Gizmos.color = Color.green;
            var startPos = new Vector2(AStar2D.StartNode().X, AStar2D.StartNode().Y);
            Gizmos.DrawCube(startPos, Vector3.one * 0.5f);

            Gizmos.color = Color.blue;
            var endPos = new Vector2(AStar2D.GoalNode().X, AStar2D.GoalNode().Y);
            Gizmos.DrawCube(endPos, Vector3.one * 0.5f);
        }

        if(current3D.Count > 0)
        {
            var example3DMap = AStar3D.ExampleMap();

            for (int z = 0; z < example3DMap.GetLength(2); z++)
            {
                for (int y = 0; y < example3DMap.GetLength(1); y++)
                {
                    for (int x = 0; x < example3DMap.GetLength(0); x++)
                    {
                        if (example3DMap[x, y, z] == 1)
                        {
                            Gizmos.color = Color.black;
                            Gizmos.DrawCube(new Vector3(x, y, z), Vector3.one * 0.8f);
                        }
                    }
                }
            }

            foreach (var node in current3D)
            {
                Gizmos.color = node.color;
                var nodePos = new Vector3(node.X, node.Y, node.Z);
                Gizmos.DrawCube(nodePos, Vector3.one * 0.25f);

                if(node.Parent != null)
                {
                    Gizmos.color = Color.red;
                    var parentPos = new Vector3(node.Parent.X, node.Parent.Y, node.Parent.Z);
                    Gizmos.DrawLine(nodePos, parentPos);
                }
            }

            Gizmos.color = Color.green;
            var startPos = new Vector3(AStar3D.StartNode().X, AStar3D.StartNode().Y, AStar3D.StartNode().Z);
            Gizmos.DrawCube(startPos, Vector3.one * 0.5f);

            Gizmos.color = Color.blue;
            var endPos = new Vector3(AStar3D.GoalNode().X, AStar3D.GoalNode().Y, AStar3D.GoalNode().Z);
            Gizmos.DrawCube(endPos, Vector3.one * 0.5f);
        }

    }
}
