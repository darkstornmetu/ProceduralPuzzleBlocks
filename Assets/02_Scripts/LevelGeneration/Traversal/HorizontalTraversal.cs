using System.Collections.Generic;
using ProceduralBlocks.Data;
using ProceduralBlocks.Grid;
using UnityEngine;

public class HorizontalTraversal : TriangleTraversal
{
    public HorizontalTraversal(Grid<GridCellData> grid, int gridSize, List<TriangleData> unassignedTriangles) : 
        base(grid, gridSize, unassignedTriangles)
    {
    }
    
    public override IEnumerable<TriangleData> GetUnassignedTrianglesInOrder(Vector2Int startingCorner)
    {
        int xStart = startingCorner.x;
        int yStart = startingCorner.y;
        int xEnd = xStart == 0 ? GridSize : -1;
        int yEnd = yStart == 0 ? GridSize : -1;
        int xStep = xStart == 0 ? 1 : -1;
        int yStep = yStart == 0 ? 1 : -1;

        for (int y = yStart; y != yEnd; y += yStep)
        {
            for (int x = xStart; x != xEnd; x += xStep)
            {
                Vector2Int coordinates = new Vector2Int(x, y);
                GridCellData cell = Grid.GetItem(coordinates);
                    
                foreach (var triangle in cell.Triangles)
                {
                    if (UnassignedTriangles.Contains(triangle))
                        yield return triangle;
                }
            }
        }
    }
}