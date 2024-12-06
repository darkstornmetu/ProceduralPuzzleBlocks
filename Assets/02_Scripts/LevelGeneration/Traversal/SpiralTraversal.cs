using System.Collections.Generic;
using ProceduralBlocks.Data;
using ProceduralBlocks.Grid;
using UnityEngine;

public abstract class SpiralTraversal : TriangleTraversal
{
    protected SpiralTraversal(Grid<GridCellData> grid, int gridSize, List<TriangleData> unassignedTriangles) 
        : base(grid, gridSize, unassignedTriangles)
    {
    }
    
    public override IEnumerable<TriangleData> GetUnassignedTrianglesInOrder(Vector2Int startingCorner)
    {
        foreach (var coords in GetSpiralCoordinates(DirectionSequence()[GetStartingCorner(startingCorner)]))
        {
            GridCellData cell = Grid.GetItem(coords);
            foreach (var triangle in cell.Triangles)
            {
                if (UnassignedTriangles.Contains(triangle))
                    yield return triangle;
            }
        }
    }
    
    protected abstract Dictionary<StartingCorner, List<Direction>> DirectionSequence();
    
    private StartingCorner GetStartingCorner(Vector2Int startingCorner)
    {
        if (startingCorner == new Vector2Int(0, 0))
            return StartingCorner.BottomLeft;
        if (startingCorner == new Vector2Int(GridSize - 1, 0))
            return StartingCorner.BottomRight;
        if (startingCorner == new Vector2Int(0, GridSize - 1))
            return StartingCorner.TopLeft;
        if (startingCorner == new Vector2Int(GridSize - 1, GridSize - 1))
            return StartingCorner.TopRight;

        return StartingCorner.BottomLeft;
    }
    
    private IEnumerable<Vector2Int> GetSpiralCoordinates(List<Direction> directionSequence)
    {
        int left = 0;
        int right = GridSize - 1;
        int bottom = 0;
        int top = GridSize - 1;

        int directionIndex = 0;

        while (left <= right && bottom <= top)
        {
            Direction currentDirection = directionSequence[directionIndex % directionSequence.Count];

            switch (currentDirection)
            {
                case Direction.Up:
                    for (int y = bottom; y <= top; y++)
                        yield return new Vector2Int(left, y);
                    break;

                case Direction.Right:
                    for (int x = left; x <= right; x++)
                        yield return new Vector2Int(x, top);
                    break;

                case Direction.Down:
                    for (int y = top; y >= bottom; y--)
                        yield return new Vector2Int(right, y);
                    break;

                case Direction.Left:
                    for (int x = right; x >= left; x--)
                        yield return new Vector2Int(x, bottom);
                    break;
            }

            // Update boundaries based on direction
            switch (currentDirection)
            {
                case Direction.Up:
                    left++;
                    break;
                case Direction.Right:
                    top--;
                    break;
                case Direction.Down:
                    right--;
                    break;
                case Direction.Left:
                    bottom++;
                    break;
            }

            directionIndex++;
        }
    }
}