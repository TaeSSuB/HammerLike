using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NuelLib.Mathmetics;
using NuelLib.AI;

public class NuelTester : MonoBehaviour
{
    AStar3D aStar3D = new AStar3D();

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
                current3D?.Clear();  
                
                current2D = AStarExample2D();
            }
            else if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                current2D.Clear();
                current3D?.Clear();

                current3D = AStarExample3D();
            }
        }

        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            
            BMPNoiseGenerator.GenerateNoiseBMP("Assets/", "BMPNoiseResult");
        }

        if(Input.GetKeyDown(KeyCode.Alpha5))
        {
            RandomRangeDistribution.MeasureRandomRange();
        }

        if(Input.GetKeyDown(KeyCode.Alpha6))
        {
            int n = 100;
            int kSSR = 1;
            int kSR = 5;
            int kR = 20;
            double pSSR = 0.01; // 1% = 100회 당 1번
            double pSR = 0.05; // 5% = 20회 당 1번
            double pR = 0.2; // 20% = 5회 당 1번

            double probabilitySSR = NuelMath.CalculateProbability(n, kSSR, pSSR);
            double probabilitySR = NuelMath.CalculateProbability(n, kSR, pSR);
            double probabilityR = NuelMath.CalculateProbability(n, kR, pR);

            Debug.Log($"Probability of getting exactly {kSSR} SSR in {n} tries: {probabilitySSR * 100:F4}%");
            Debug.Log($"Probability of getting exactly {kSR} SR in {n} tries: {probabilitySR * 100:F4}%");
            Debug.Log($"Probability of getting exactly {kR} R in {n} tries: {probabilityR * 100:F4}%");
        }
    }

    List<AStar2D.Node2D> AStarExample2D()
    {
        return AStar2D.AStar2DExample(true);
    }

    List<AStar3D.Node3D> AStarExample3D()
    {
        int randomWidth = Random.Range(3, 10);
        int randomHeight = Random.Range(3, 10);
        int randomDepth = Random.Range(3, 10);
        float wallProbability = Random.Range(0.1f, 0.8f);
        return aStar3D.AStar3DExample(AStar3D.MovementType.DiagonalWithWalls, randomWidth, randomHeight, randomDepth, wallProbability);
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
                        Gizmos.color = new Color(0.1f, 0.1f, 0.1f);
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

        if(current3D != null && current3D.Count > 0)
        {
            //var example3DMap = aStar3D.ExampleMap(10, 10, 10);
            var example3DMap = aStar3D.currentGrid;

            for (int z = 0; z < example3DMap.GetLength(2); z++)
            {
                for (int y = 0; y < example3DMap.GetLength(1); y++)
                {
                    for (int x = 0; x < example3DMap.GetLength(0); x++)
                    {
                        if (example3DMap[x, y, z] == 1)
                        {
                            Gizmos.color = new Color(0.1f, 0.1f, 0.1f);
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
            var startPos = new Vector3(aStar3D.start.X, aStar3D.start.Y, aStar3D.start.Z);
            Gizmos.DrawCube(startPos, Vector3.one * 0.5f);

            Gizmos.color = Color.blue;
            var endPos = new Vector3(aStar3D.goal.X, aStar3D.goal.Y, aStar3D.goal.Z);
            Gizmos.DrawCube(endPos, Vector3.one * 0.5f);
        }

    }
}
