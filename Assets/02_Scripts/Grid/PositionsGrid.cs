using ProceduralBlocks.Data;
using UnityEngine;

namespace ProceduralBlocks.Grid
{
    public class PositionsGrid : IGridService<Vector3>
    {
        public Vector2Int GridSize { get; private set; }
    
        private Grid<Vector3> _positionsGrid;
        private Vector3 _gridOrigin;
        private Vector3 _gridOffset;

        private readonly float _cellSize;
        private Bounds _gridBounds;
    
        public PositionsGrid(LevelData levelData, Vector3 gridOffset)
        {
            _gridOffset = gridOffset;
            _cellSize = levelData.CellSize;
            InitializeGrid(levelData.GridSize, levelData.GridSize);
            _gridBounds = new Bounds(_gridOffset, Vector3.one * levelData.GridSize);
        }
    
        private void InitializeGrid(int width, int height)
        {
            _positionsGrid = new Grid<Vector3>(width, height);
        
            GridSize = new Vector2Int(width, height);
            _gridOrigin = new Vector3(width - 1, height - 1) * (_cellSize * -0.5f) + _gridOffset;
        
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2Int coordinates = new Vector2Int(x, y);

                    Vector3 offset = new Vector3(
                        x * _cellSize,
                        y * _cellSize);
                
                    var currentPos = _gridOrigin + offset;
                
                    _positionsGrid.SetItem(coordinates, currentPos);
                }   
            }
        }

        public bool IsPointInsideTheGrid(Vector3 point)
        {
            return _gridBounds.Contains(point);
        }
    
        public bool TryGetNearestGridPosition(Vector3 position, float maxThreshold, out Vector3 gridPos)
        {
            gridPos = default;
        
            int x = Mathf.RoundToInt((position.x - _gridOrigin.x) / _cellSize);
            int y = Mathf.RoundToInt((position.y - _gridOrigin.y) / _cellSize);
    
            x = Mathf.Clamp(x, 0, _positionsGrid.Width - 1);
            y = Mathf.Clamp(y, 0, _positionsGrid.Height - 1);

            gridPos = _positionsGrid.GetItem(new Vector2Int(x, y));

            return Vector3.Distance(position, gridPos) < maxThreshold;
        }
    
        public bool TryGetItemAtCoordinates(Vector2Int coords, out Vector3 position)
        {
            position = default;
            if (!_positionsGrid.TryGetItem(coords, out Vector3 pos)) return false;

            position = pos;
            return true;
        }

        public bool TryGetCoordinatesFromItem(Vector3 position, out Vector2Int coords)
        {
            coords = default;
            if (!_positionsGrid.TryGetCoordinate(position, out Vector2Int coordinates)) return false;

            coords = coordinates;
            return true;
        }

        public Vector3 GetItemAtCoordinates(Vector2Int coords)
        {
            return _positionsGrid.GetItem(coords);
        }
    }
}