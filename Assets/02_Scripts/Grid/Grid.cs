using System;
using System.Linq;
using UnityEngine;

namespace ProceduralBlocks.Grid
{
    public class Grid<T>
    {
        public int Width => _width;
        public int Height => _height;
    
        private readonly int _width;
        private readonly int _height;
    
        private readonly T[] _grid;

        public Grid(int width, int height)
        {
            _width = width;
            _height = height;

            _grid = new T[_width * _height];
        }
    
        public void SetItem(Vector2Int coordinates, T item)
        {
            if (!IsValidCoordinates(coordinates)) return;
        
            _grid[GetIndex(coordinates)] = item;
        }

        public bool TryGetItem(Vector2Int coords, out T item)
        {
            item = default;

            if (!IsValidCoordinates(coords))
                return false;
        
            item = GetItem(coords);
            return true;
        }
    
        public T GetItem(Vector2Int coords) => _grid[GetIndex(coords)];

        public bool TryGetCoordinate(T item, out Vector2Int coordinates)
        {
            coordinates = default;
        
            if (!_grid.Contains(item)) return false;

            coordinates = GetCoordinate(item);
            return true;
        }
    
        private Vector2Int GetCoordinate(T item)
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Vector2Int coords = new Vector2Int(x, y);
                
                    if (_grid[GetIndex(coords)].Equals(item))
                        return coords;
                }   
            }

            throw new ArgumentNullException();
        }
    
        private bool IsValidCoordinates(Vector2Int coordinates)
        {
            return coordinates.x >= 0 && coordinates.x < _width && coordinates.y >= 0 && coordinates.y < _height;
        }
    
        private int GetIndex(Vector2Int coordinates)
        {
            return coordinates.y * _width + coordinates.x;
        }
    }
}