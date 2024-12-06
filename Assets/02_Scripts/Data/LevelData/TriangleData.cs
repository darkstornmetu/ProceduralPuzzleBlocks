using System;
using UnityEngine;

namespace ProceduralBlocks.Data
{
    [Serializable]
    public class TriangleData
    {
        [NonSerialized]
        public int PieceID;
        
    
        public Vector2Int Coordinates; 
        public int TriangleIndex;
        public bool IsAssigned;
    
        public TriangleData(Vector2Int coordinates, int triangleIndex, bool isAssigned)
        {
            Coordinates = coordinates;
            TriangleIndex = triangleIndex;
            IsAssigned = isAssigned;
        }
    }
}