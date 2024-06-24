using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NuelLib.AI
{
    /// <summary>
    /// A* 길찾기 알고리즘
    /// </summary>
    public class AStar2D
    {
        /// <summary>
        /// 노드
        /// AStar 알고리즘을 사용하여 시작점에서 목표점까지의 경로를 찾는다.
        /// </summary>
        public class Node2D : IComparable<Node2D>
        {
            /// <summary>
            /// 현재 노드의 좌표 - X
            /// </summary>
            public int X { get; set; }

            /// <summary>
            /// 현재 노드의 좌표 - Y
            /// </summary>
            public int Y { get; set; }

            /// <summary>
            /// 시작점에서 현재 노드까지의 비용
            /// </summary>
            public double G { get; set; }

            /// <summary>
            /// 현재 노드에서 목표 노드까지의 휴리스틱 비용
            /// </summary>
            public double H { get; set; }

            /// <summary>
            /// 총 비용
            /// </summary>
            public double F => G + H;

            /// <summary>
            /// 부모 노드
            /// </summary>
            public Node2D Parent { get; set; }

            /// <summary>
            /// 노드 색상
            /// </summary>
            public Color color = Color.red;

            /// <summary>
            /// 노드 생성자
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
            /// 비교 함수 for PriorityQueue
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
            /// 노드 비교 함수 for PriorityQueue
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

        public bool allowDiagonalMovement = true; // 대각선 이동을 허용하는지 여부

        /// <summary>
        /// 휴리스틱 함수
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
        /// 이웃 노드 찾기
        /// </summary>
        /// <param name="node"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static List<Node2D> GetNeighbors(Node2D node, int[,] grid, bool allowDiagonalMovement)
        {
            var neighbors = new List<Node2D>();

            // 기본 방향 (상하좌우)
            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            // 대각선 방향
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

            // 상하좌우 방향
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
        /// 길 찾기 알고리즘
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
        /// 경로 재구성
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
        /// A* 2D 예제
        /// </summary>
        public static List<Node2D> AStar2DExample(bool inAllowDiagonalMovement)
        {
            // Grid 구성 예제
            // 0: 이동 가능한 공간
            // 1: 벽
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
    /// A* 길찾기 알고리즘 3D
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

        public MovementType movementType = MovementType.Vertical; // 이동 유형 설정
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

            // 기본 방향 (상하좌우 및 수직)
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

            // 대각선 방향
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
                            // 벽 고려한 대각 이동
                            bool isPathBlocked = false;

                            // 각 축 방향에 벽이 있는지 확인
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
            // 랜덤 맵 생성 예제
            int width = inWidth;
            int height = inHeight;
            int depth = inDepth;
            float wallProbability = inWallProbability; // 벽이 생성될 확률

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

            // 맵을 랜덤으로 생성
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        if (UnityEngine.Random.value < wallProbability)
                        {
                            grid[x, y, z] = 1; // 벽
                        }
                        else
                        {
                            grid[x, y, z] = 0; // 이동 가능한 공간
                        }
                    }
                }
            }

            // 시작과 목표 지점은 항상 이동 가능한 공간으로 설정
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