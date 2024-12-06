using ProceduralBlocks.Data;
using UnityEngine;

public class TriangleMesh : MonoBehaviour
{
    [SerializeField] private PieceMaterialSet _pieceMaterialSet;
    [SerializeField] private Renderer _renderer;

    public void SetMeshColor(PieceColor color)
    {
        var pieceMat = _pieceMaterialSet.GetDataByEnum(color);
        _renderer.sharedMaterial = pieceMat;
    }
}