using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralBlocks.Data
{
    [Serializable]
    public class PieceData
    {
        public int PieceID;
        public PieceColor PieceColor;
        public List<TriangleData> Triangles;

        [NonSerialized]
        public Vector2Int Offset;
        
        public PieceData(int pieceID, PieceColor pieceColor)
        {
            PieceID = pieceID;
            PieceColor = pieceColor;
        }
    }
}