using UnityEngine;

public class Triangle : MonoBehaviour
{
    [SerializeField] private TriangleMesh _triangleMesh;
    
    public void SetTriangleColor(PieceColor color)
    {
        _triangleMesh.SetMeshColor(color);
    }
}