using UnityEngine;

namespace ProceduralBlocks.Data
{
    public class GridCellData
    {
        public Vector2Int Coordinates;
        public readonly TriangleData[] Triangles;

        public GridCellData(Vector2Int coordinates)
        {
            Coordinates = coordinates;
            Triangles = new TriangleData[4];

            for (int i = 0; i < Triangles.Length; i++) 
                Triangles[i] = new TriangleData(coordinates, i, false);
        }
    }
}