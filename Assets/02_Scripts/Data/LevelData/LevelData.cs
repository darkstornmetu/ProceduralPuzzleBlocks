using System;
using System.Collections.Generic;
using ProceduralBlocks.Data.Process;

namespace ProceduralBlocks.Data
{
    [Serializable]
    public class LevelData : IParsable
    {
        public DifficultyLevel DifficultyLevel;

        public float CellSize;
        public int GridSize;
        public List<PieceData> Pieces;

        public LevelData(DifficultyLevel difficultyLevel, int gridSize, float cellSize, List<PieceData> pieces)
        {
            DifficultyLevel = difficultyLevel;
            GridSize = gridSize;
            Pieces = pieces;
            CellSize = cellSize;
        }
    }
}