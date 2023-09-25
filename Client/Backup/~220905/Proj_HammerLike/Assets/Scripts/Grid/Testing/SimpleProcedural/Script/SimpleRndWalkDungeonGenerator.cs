using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleRndWalkDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField]
    protected SimpleRandomWalkSO randomWalkParameters;

    protected override void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk(randomWalkParameters, startPosition);
        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(floorPositions);

        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    protected HashSet<Vector2Int> RunRandomWalk(SimpleRandomWalkSO parameter, Vector2Int position)
    {
        //throw new NotImplementedException();

        var currentPosition = position;

        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

        for (int  i = 0;  i < parameter.iterations; ++i)
        {
            var path = ProceduraGenerationAlgorithms.SimpleRandomWalk(currentPosition, parameter.walkLength);

            floorPositions.UnionWith(path);

            if (parameter.startRandomlyEachIteration)
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
        }
        return floorPositions;
    }
}
