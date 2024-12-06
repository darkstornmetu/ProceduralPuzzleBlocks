using System.Collections.Generic;
using ProceduralBlocks.Data;
using ProceduralBlocks.Grid;
using UnityEngine;

public abstract class TriangleTraversal
{
    protected Grid<GridCellData> Grid { get; }
    protected int GridSize { get; }
    protected List<TriangleData> UnassignedTriangles { get; }

    protected TriangleTraversal(Grid<GridCellData> grid, int gridSize, List<TriangleData> unassignedTriangles)
    {
        Grid = grid;
        GridSize = gridSize;
        UnassignedTriangles = unassignedTriangles;
    }

    public abstract IEnumerable<TriangleData> GetUnassignedTrianglesInOrder(Vector2Int startingCorner);
}

public enum TraversalMethod
{
    ClockwiseSpiral,
    CounterClockwiseSpiral,
    Horizontal,
    Vertical
}

public enum Direction
{
    Up,
    Right,
    Down,
    Left
}

public enum StartingCorner
{
    BottomLeft,    // (0,0)
    BottomRight,   // (GridSize-1,0)
    TopLeft,       // (0,GridSize-1)
    TopRight       // (GridSize-1,GridSize-1)
}