using System.Collections.Generic;
using ProceduralBlocks.Data;
using ProceduralBlocks.Grid;

public class ClockwiseSpiralTraversal : SpiralTraversal
{
    public ClockwiseSpiralTraversal(Grid<GridCellData> grid, int gridSize, List<TriangleData> unassignedTriangles) 
        : base(grid, gridSize, unassignedTriangles)
    {
    }

    protected override Dictionary<StartingCorner, List<Direction>> DirectionSequence()
    {
        return new Dictionary<StartingCorner, List<Direction>>
        {
            { StartingCorner.BottomLeft, new List<Direction> { Direction.Up, Direction.Right, Direction.Down, Direction.Left } },
            { StartingCorner.BottomRight, new List<Direction> { Direction.Left, Direction.Up, Direction.Right, Direction.Down } },
            { StartingCorner.TopLeft, new List<Direction> { Direction.Right, Direction.Down, Direction.Left, Direction.Up } },
            { StartingCorner.TopRight, new List<Direction> { Direction.Down, Direction.Left, Direction.Up, Direction.Right } },
        };
    }
}