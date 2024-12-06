using System;
using System.Collections.Generic;
using ProceduralBlocks.Grid;
using ProceduralBlocks.Managers;
using UnityEngine;

public class PieceOverlapChecker
{
    private readonly PositionsGrid _positionsGrid;

    private readonly HashSet<TriangleIdentifier> _occupiedTriangles = new();
    private readonly Dictionary<Piece, List<TriangleIdentifier>> _pieceMap = new();
    
    public PieceOverlapChecker(PositionsGrid positionsGrid)
    {
        _positionsGrid = positionsGrid;
    }
    
    public bool CheckOverlap(Piece piece)
    {
        Transform pieceTransform = piece.transform;

        bool isOnGrid = _positionsGrid.IsPointInsideTheGrid(pieceTransform.position);

        if (isOnGrid)
        {
            if (!_positionsGrid.TryGetNearestGridPosition(pieceTransform.position, 0.4f,
                    out var gridPos)) return true;
            if (!CheckGridBounds(piece, gridPos, out var pieceTriangles)) return true;

            if (!_pieceMap.ContainsKey(piece))
            {
                if (CheckOverlapWithOtherPieces(pieceTriangles)) return true;
            
                _pieceMap.Add(piece, pieceTriangles);
                AssignTrianglesToOccupationList(pieceTriangles);
            }
            else
            {
                var previousTriangles = _pieceMap[piece];
            
                RemoveTrianglesFromOccupationList(previousTriangles);
                if (CheckOverlapWithOtherPieces(pieceTriangles))
                {
                    AssignTrianglesToOccupationList(previousTriangles);
                    return true;
                }
            
                AssignTrianglesToOccupationList(pieceTriangles);
                _pieceMap[piece] = pieceTriangles;
            }
        
            pieceTransform.position = gridPos;
        }
        else
        {
            if (_pieceMap.TryGetValue(piece, out var previousTriangles))
            {
                RemoveTrianglesFromOccupationList(previousTriangles);
                _pieceMap.Remove(piece);
            }
        }
        
        if (_occupiedTriangles.Count >= _positionsGrid.GridSize.x * _positionsGrid.GridSize.y * 4)
            GameManager.GameSuccess();
        
        return false;
    }

    private void RemoveTrianglesFromOccupationList(List<TriangleIdentifier> previousTriangles)
    {
        foreach (var triangle in previousTriangles)
        {
            _occupiedTriangles.Remove(triangle);
        }
    }

    private void AssignTrianglesToOccupationList(List<TriangleIdentifier> triangles)
    {
        _occupiedTriangles.UnionWith(triangles);
    }

    private bool CheckGridBounds(Piece piece, Vector3 gridPos, out List<TriangleIdentifier> pieceTriangles)
    {
       pieceTriangles = new List<TriangleIdentifier>();

        foreach (var triangle in piece.TrianglesData)
        {
            Vector3 globalPosition = gridPos + new Vector3(triangle.Coordinates.x, triangle.Coordinates.y, 0);
            
            if (!_positionsGrid.TryGetCoordinatesFromItem(globalPosition, out var gridCoordinates)) 
                return false;
            
            TriangleIdentifier identifier = new TriangleIdentifier(gridCoordinates, triangle.TriangleIndex);
            pieceTriangles.Add(identifier);
        }
        
        return true;
    }

    private bool CheckOverlapWithOtherPieces(List<TriangleIdentifier> newTriangles)
    {
        foreach (var triangleIdentifier in newTriangles)
        {
            if (_occupiedTriangles.Contains(triangleIdentifier))
                return true; 
        }
        
        return false;
    }
}

public readonly struct TriangleIdentifier : IEquatable<TriangleIdentifier>
{
    private readonly Vector2Int _coordinates;
    private readonly int _triangleIndex;      

    public TriangleIdentifier(Vector2Int coordinates, int triangleIndex)
    {
        _coordinates = coordinates;
        _triangleIndex = triangleIndex;
    }
    
    public override bool Equals(object obj)
    {
        return obj is TriangleIdentifier other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_coordinates, _triangleIndex);
    }

    public bool Equals(TriangleIdentifier other)
    {
        return _coordinates.Equals(other._coordinates) && _triangleIndex == other._triangleIndex;
    }
}