using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NuelLib.AI
{
    /// <summary>
    /// A* ��ã�� �˰���
    /// </summary>
    public class AStar2D
    {
        /// <summary>
        /// ���
        /// AStar �˰����� ����Ͽ� ���������� ��ǥ�������� ��θ� ã�´�.
        /// </summary>
        public class Node2D : IComparable<Node2D>
        {
            /// <summary>
            /// ���� ����� ��ǥ - X
            /// </summary>
            public int X { get; set; }

            /// <summary>
            /// ���� ����� ��ǥ - Y
            /// </summary>
            public int Y { get; set; }

            /// <summary>
            /// ���������� ���� �������� ���
            /// </summary>
            public double G { get; set; }

            /// <summary>
            /// ���� ��忡�� ��ǥ �������� �޸���ƽ ���
            /// </summary>
            public double H { get; set; }

            /// <summary>
            /// �� ���
            /// </summary>
            public double F => G + H;

            /// <summary>
            /// �θ� ���
            /// </summary>
            public Node2D Parent { get; set; }

            /// <summary>
            /// ��� ����
            /// </summary>
            public Color color = Color.red;

            /// <summary>
            /// ��� ������
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            public Node2D(int x, int y)
            {
                X = x;
                Y = y;
                G = 0;
                H = 0;
                Parent = null;
            }

            /// <summary>
            /// �� �Լ� for PriorityQueue
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public int CompareTo(Node2D other)
            {
                int compare = F.CompareTo(other.F);

                if (compare == 0)
                {
                    compare = H.CompareTo(other.H);
                }
                return compare;
            }

            /// <summary>
            /// ��� �� �Լ� for PriorityQueue
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                if (obj is Node2D other)
                {
                    return X == other.X && Y == other.Y;
                }
                return false;
            }

            /// <summary>
            /// Get HashCode for PriorityQueue
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return (X, Y).GetHashCode();
            }
        }

        public bool allowDiagonalMovement = true; // �밢�� �̵��� ����ϴ��� ����

        /// <summary>
        /// �޸���ƽ �Լ�
        /// </summary>
        /// <param name="start"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        public static double Heuristic(Node2D start, Node2D goal)
        {
            // Using Manhattan distance as heuristic
            return Math.Abs(start.X - goal.X) + Math.Abs(start.Y - goal.Y);
        }

        /// <summary>
        /// �̿� ��� ã��
        /// </summary>
        /// <param name="node"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static List<Node2D> GetNeighbors(Node2D node, int[,] grid, bool allowDiagonalMovement)
        {
            var neighbors = new List<Node2D>();

            // �⺻ ���� (�����¿�)
            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            // �밢�� ����
            if (allowDiagonalMovement)
            {
                int[] dxDiagonal = { -1, -1, 1, 1 };
                int[] dyDiagonal = { -1, 1, -1, 1 };

                for (int i = 0; i < dxDiagonal.Length; i++)
                {
                    int newX = node.X + dxDiagonal[i];
                    int newY = node.Y + dyDiagonal[i];

                    if (newX >= 0 && newX < grid.GetLength(0) && newY >= 0 && newY < grid.GetLength(1) && grid[newX, newY] == 0)
                    {
                        neighbors.Add(new Node2D(newX, newY));
                    }
                }
            }

            // �����¿� ����
            for (int i = 0; i < dx.Length; i++)
            {
                int newX = node.X + dx[i];
                int newY = node.Y + dy[i];

                if (newX >= 0 && newX < grid.GetLength(0) && newY >= 0 && newY < grid.GetLength(1) && grid[newX, newY] == 0)
                {
                    neighbors.Add(new Node2D(newX, newY));
                }
            }

            return neighbors;
        }

        /// <summary>
        /// �� ã�� �˰���
        /// </summary>
        /// <param name="start"></param>
        /// <param name="goal"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static List<Node2D> AStarSearch(Node2D start, Node2D goal, int[,] grid, bool allowDiagonalMovement)
        {
            var openSet = new PriorityQueue<Node2D>();
            var closedSet = new HashSet<Node2D>();
            var nodeMap = new Dictionary<(int, int), Node2D>();

            start.H = Heuristic(start, goal);
            openSet.Enqueue(start);
            nodeMap[(start.X, start.Y)] = start;

            while (openSet.Count > 0)
            {
                var current = openSet.Dequeue();

                if (current.X == goal.X && current.Y == goal.Y)
                {
                    return ReconstructPath(current);
                }

                closedSet.Add(current);

                foreach (var neighbor in GetNeighbors(current, grid, allowDiagonalMovement))
                {
                    if (closedSet.Contains(neighbor))
                        continue;

                    var existingNeighbor = nodeMap.ContainsKey((neighbor.X, neighbor.Y)) ? nodeMap[(neighbor.X, neighbor.Y)] : null;

                    double tentativeG = current.G + 1;

                    if (existingNeighbor == null || tentativeG < existingNeighbor.G)
                    {
                        neighbor.Parent = current;
                        neighbor.G = tentativeG;
                        neighbor.H = Heuristic(neighbor, goal);

                        if (existingNeighbor == null)
                        {
                            openSet.Enqueue(neighbor);
                            nodeMap[(neighbor.X, neighbor.Y)] = neighbor;
                        }
                        else
                        {
                            openSet.Remove(existingNeighbor);
                            openSet.Enqueue(neighbor);
                            nodeMap[(neighbor.X, neighbor.Y)] = neighbor;
                        }
                    }
                }
            }

            return null; // No path found
        }

        /// <summary>
        /// ��� �籸��
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static List<Node2D> ReconstructPath(Node2D node)
        {
            var path = new List<Node2D>();
            while (node != null)
            {
                path.Add(node);
                node = node.Parent;
            }
            path.Reverse();
            return path;
        }

        /// <summary>
        /// A* 2D ����
        /// </summary>
        public static List<Node2D> AStar2DExample(bool inAllowDiagonalMovement)
        {
            // Grid ���� ����
            // 0: �̵� ������ ����
            // 1: ��
            int[,] grid = ExampleMap();

            var start = StartNode();
            var goal = GoalNode();
            var path = AStarSearch(start, goal, grid, inAllowDiagonalMovement);

            if (path != null && path.Count > 0)
            {
                Debug.Log("Path found:");
                foreach (var node in path)
                {
                    Debug.Log($"({node.X}, {node.Y})");
                }
            }
            else
            {
                Debug.Log("No path found.");
            }

            return path;
        }

        public static int[,] ExampleMap()
        {
            int[,] grid =
            {
            { 0, 1, 0, 0, 0 },
            { 0, 1, 0, 1, 0 },
            { 0, 0, 0, 1, 0 },
            { 0, 1, 0, 0, 0 },
            { 0, 0, 0, 1, 0 }
        };

            return grid;
        }

        public static Node2D StartNode()
        {
            return new Node2D(0, 0);
        }

        public static Node2D GoalNode()
        {
            return new Node2D(4, 4);
        }
    }


    /// <summary>
    /// A* ��ã�� �˰��� 3D
    /// </summary>

    public class AStar3D : MonoBehaviour
    {
        public enum MovementType
        {
            Vertical,
            Diagonal,
            DiagonalWithWalls
        }

        public class Node3D : IComparable<Node3D>
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }
            public double G { get; set; }
            public double H { get; set; }
            public double F => G + H;
            public Node3D Parent { get; set; }

            public Color color = Color.red;

            public Node3D(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
                G = 0;
                H = 0;
                Parent = null;
            }

            public int CompareTo(Node3D other)
            {
                int compare = F.CompareTo(other.F);
                if (compare == 0)
                {
                    compare = H.CompareTo(other.H);
                }
                return compare;
            }

            public override bool Equals(object obj)
            {
                if (obj is Node3D other)
                {
                    return X == other.X && Y == other.Y && Z == other.Z;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return (X, Y, Z).GetHashCode();
            }
        }

        public MovementType movementType = MovementType.Vertical; // �̵� ���� ����
        public int[,,] currentGrid;
        public Node3D start;
        public Node3D goal;

        public static double Heuristic(Node3D start, Node3D goal)
        {
            // Using Manhattan distance as heuristic
            return Math.Abs(start.X - goal.X) + Math.Abs(start.Y - goal.Y) + Math.Abs(start.Z - goal.Z);
        }

        public static List<Node3D> GetNeighbors(Node3D node, int[,,] grid, MovementType movementType)
        {
            var neighbors = new List<Node3D>();

            // �⺻ ���� (�����¿� �� ����)
            int[] dx = { -1, 1, 0, 0, 0, 0 };
            int[] dy = { 0, 0, -1, 1, 0, 0 };
            int[] dz = { 0, 0, 0, 0, -1, 1 };

            for (int i = 0; i < dx.Length; i++)
            {
                int newX = node.X + dx[i];
                int newY = node.Y + dy[i];
                int newZ = node.Z + dz[i];

                if (newX >= 0 && newX < grid.GetLength(0) && newY >= 0 && newY < grid.GetLength(1) && newZ >= 0 && newZ < grid.GetLength(2) && grid[newX, newY, newZ] == 0)
                {
                    neighbors.Add(new Node3D(newX, newY, newZ));
                }
            }

            // �밢�� ����
            if (movementType == MovementType.Diagonal || movementType == MovementType.DiagonalWithWalls)
            {
                int[] dxDiagonal = { -1, -1, -1, 1, 1, 1, -1, 1, -1, -1, 1, 1 };
                int[] dyDiagonal = { -1, -1, 1, -1, 1, 1, 0, 0, 0, 0, 0, 0 };
                int[] dzDiagonal = { 0, 1, 0, 0, 0, 0, -1, -1, 1, 1, -1, 1 };

                for (int i = 0; i < dxDiagonal.Length; i++)
                {
                    int newX = node.X + dxDiagonal[i];
                    int newY = node.Y + dyDiagonal[i];
                    int newZ = node.Z + dzDiagonal[i];

                    if (newX >= 0 && newX < grid.GetLength(0) && newY >= 0 && newY < grid.GetLength(1) && newZ >= 0 && newZ < grid.GetLength(2) && grid[newX, newY, newZ] == 0)
                    {
                        if (movementType == MovementType.DiagonalWithWalls)
                        {
                            // �� ����� �밢 �̵�
                            bool isPathBlocked = false;

                            // �� �� ���⿡ ���� �ִ��� Ȯ��
                            if (dxDiagonal[i] != 0 && dyDiagonal[i] != 0)
                            {
                                if (grid[newX, node.Y, node.Z] == 1 || grid[node.X, newY, node.Z] == 1)
                                {
                                    isPathBlocked = true;
                                }
                            }
                            if (dxDiagonal[i] != 0 && dzDiagonal[i] != 0)
                            {
                                if (grid[newX, node.Y, node.Z] == 1 || grid[node.X, node.Y, newZ] == 1)
                                {
                                    isPathBlocked = true;
                                }
                            }
                            if (dyDiagonal[i] != 0 && dzDiagonal[i] != 0)
                            {
                                if (grid[node.X, newY, node.Z] == 1 || grid[node.X, node.Y, newZ] == 1)
                                {
                                    isPathBlocked = true;
                                }
                            }

                            if (!isPathBlocked)
                            {
                                neighbors.Add(new Node3D(newX, newY, newZ));
                            }
                        }
                        else
                        {
                            neighbors.Add(new Node3D(newX, newY, newZ));
                        }
                    }
                }
            }

            return neighbors;
        }

        public static List<Node3D> AStarSearch(Node3D start, Node3D goal, int[,,] grid, MovementType movementType)
        {
            var openSet = new PriorityQueue<Node3D>();
            var closedSet = new HashSet<Node3D>();
            var nodeMap = new Dictionary<(int, int, int), Node3D>();

            start.H = Heuristic(start, goal);
            openSet.Enqueue(start);
            nodeMap[(start.X, start.Y, start.Z)] = start;

            while (openSet.Count > 0)
            {
                var current = openSet.Dequeue();

                if (current.X == goal.X && current.Y == goal.Y && current.Z == goal.Z)
                {
                    return ReconstructPath(current);
                }

                closedSet.Add(current);

                foreach (var neighbor in GetNeighbors(current, grid, movementType))
                {
                    if (closedSet.Contains(neighbor))
                        continue;

                    var existingNeighbor = nodeMap.ContainsKey((neighbor.X, neighbor.Y, neighbor.Z)) ? nodeMap[(neighbor.X, neighbor.Y, neighbor.Z)] : null;

                    double tentativeG = current.G + 1;

                    if (existingNeighbor == null || tentativeG < existingNeighbor.G)
                    {
                        neighbor.Parent = current;
                        neighbor.G = tentativeG;
                        neighbor.H = Heuristic(neighbor, goal);

                        if (existingNeighbor == null)
                        {
                            openSet.Enqueue(neighbor);
                            nodeMap[(neighbor.X, neighbor.Y, neighbor.Z)] = neighbor;
                        }
                        else
                        {
                            openSet.Remove(existingNeighbor);
                            openSet.Enqueue(neighbor);
                            nodeMap[(neighbor.X, neighbor.Y, neighbor.Z)] = neighbor;
                        }
                    }
                }
            }

            return null; // No path found
        }

        private static List<Node3D> ReconstructPath(Node3D node)
        {
            var path = new List<Node3D>();
            while (node != null)
            {
                path.Add(node);
                node = node.Parent;
            }
            path.Reverse();
            return path;
        }

        public List<Node3D> AStar3DExample(MovementType inMovementType, int inWidth = 5, int inHeight = 3, int inDepth = 5, float inWallProbability = 0.3f)
        {
            // ���� �� ���� ����
            int width = inWidth;
            int height = inHeight;
            int depth = inDepth;
            float wallProbability = inWallProbability; // ���� ������ Ȯ��

            currentGrid = ExampleMap(width, height, depth, wallProbability);

            start = new Node3D(0, 0, 0);
            goal = new Node3D(width - 1, height - 1, depth - 1);
            var path = AStarSearch(start, goal, currentGrid, inMovementType);

            if (path != null && path.Count > 0)
            {
                Debug.Log("Path found:");
                foreach (var node in path)
                {
                    Debug.Log($"({node.X}, {node.Y}, {node.Z})");
                }
            }
            else
            {
                Debug.Log("No path found.");
            }

            return path;
        }

        public int[,,] ExampleMap(int width, int height, int depth, float wallProbability = 0.3f)
        {
            int[,,] grid = new int[width, height, depth];

            // ���� �������� ����
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        if (UnityEngine.Random.value < wallProbability)
                        {
                            grid[x, y, z] = 1; // ��
                        }
                        else
                        {
                            grid[x, y, z] = 0; // �̵� ������ ����
                        }
                    }
                }
            }

            // ���۰� ��ǥ ������ �׻� �̵� ������ �������� ����
            grid[0, 0, 0] = 0;
            grid[width - 1, height - 1, depth - 1] = 0;

            return grid;
        }

        public Node3D StartNode()
        {
            return start;
        }

        public Node3D GoalNode()
        {
            return goal;
        }
    }
}