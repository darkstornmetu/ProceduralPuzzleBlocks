using UnityEngine;

namespace ProceduralBlocks.Grid
{
    public interface IGridService<T>
    {
        public Vector2Int GridSize { get; } 
        public bool TryGetItemAtCoordinates(Vector2Int coords, out T item);
        public bool TryGetCoordinatesFromItem(T item, out Vector2Int coords);
        public T GetItemAtCoordinates(Vector2Int coords);
    }
}