using System.Collections.Generic;
using ProceduralBlocks.Data;
using UnityEngine;

public class PieceArea : MonoBehaviour
{
    [SerializeField] private Piece _piecePrefab;
    [SerializeField] private Triangle _trianglePrefab;

    [SerializeField] private Collider _areaCollider;
        
    public void InitializePieceArea(List<PieceData> pieces)
    {
        InstantiatePieces(pieces);
    }
    
    private void InstantiatePieces(List<PieceData> pieces) 
    {
        foreach (var pieceData in pieces)
        {
            Piece piece = Instantiate(_piecePrefab, transform);
            
            foreach (var triangleData in pieceData.Triangles)
            {
                var triangle = Instantiate(_trianglePrefab, piece.transform);
                
                triangle.SetTriangleColor(pieceData.PieceColor);

                triangle.transform.localPosition = 
                    new Vector3(triangleData.Coordinates.x, triangleData.Coordinates.y, 0);
                triangle.transform.localRotation = 
                    Quaternion.Euler(new Vector3(0, 0, triangleData.TriangleIndex * -90f));
                
                piece.AddTriangleToPiece(triangleData);
            }

            piece.transform.position = GetRandomPoint() + (piece.transform.position - GetCenterPoint(piece.transform));
            piece.InstantiatePeripheryDots();
        }
    }

    private Vector3 GetRandomPoint()
    {
        var boundsMin = _areaCollider.bounds.min;
        var boundsMax = _areaCollider.bounds.max;
        
        return new Vector3(
            Random.Range(boundsMin.x, boundsMax.x),
            Random.Range(boundsMin.y, boundsMax.y),
            Random.Range(boundsMin.z, boundsMax.z)
        );
    }

    private Vector3 GetCenterPoint(Transform t)
    {
        Vector3 center = Vector3.zero;

        for (int i = 0; i < t.childCount; i++)
        {
            center += t.GetChild(i).position;
        }

        center /= t.childCount;
        return center;
    }
}
