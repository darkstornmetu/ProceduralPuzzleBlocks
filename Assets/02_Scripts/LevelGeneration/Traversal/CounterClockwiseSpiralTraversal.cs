using System.Collections.Generic;
using ProceduralBlocks.Data;
using ProceduralBlocks.Grid;

public class CounterClockwiseSpiralTraversal : SpiralTraversal
{
    public CounterClockwiseSpiralTraversal(Grid<GridCellData> grid, int gridSize, List<TriangleData> unassignedTriangles) 
        : base(grid, gridSize, unassignedTriangles)
    {
    }
    
    protected override Dictionary<StartingCorner, List<Direction>> DirectionSequence()
    {
        return new Dictionary<StartingCorner, List<Direction>>
        {
            { StartingCorner.BottomLeft, new List<Direction> { Direction.Right, Direction.Up, Direction.Left, Direction.Down } },
            { StartingCorner.BottomRight, new List<Direction> { Direction.Up, Direction.Left, Direction.Down, Direction.Right } },
            { StartingCorner.TopLeft, new List<Direction> { Direction.Down, Direction.Right, Direction.Up, Direction.Left } },
            { StartingCorner.TopRight, new List<Direction> { Direction.Left, Direction.Down, Direction.Right, Direction.Up } },
        };
    }
}