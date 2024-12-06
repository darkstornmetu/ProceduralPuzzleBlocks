using System.Collections.Generic;
using System.Linq;
using ProceduralBlocks.Data;
using ProceduralBlocks.Selection;
using UnityEngine;

[SelectionBase]
public class Piece : MonoBehaviour, ISelectable
{
    [SerializeField] private GridDot _dotPrefab;

    public List<TriangleData> TrianglesData { get; } = new();

    private readonly List<GridDot> _peripheryDots = new();
    
    public void AddTriangleToPiece(TriangleData triangleData)
    {
        TrianglesData.Add(triangleData);
    }

    public void InstantiatePeripheryDots()
    {
        List<Vector2Int> peripheryCells = new ();

        var groupedTriangles = TrianglesData.GroupBy(t => t.Coordinates);
        foreach (var triangleSet in groupedTriangles)
        {
            if (triangleSet.Count() < 4)
                peripheryCells.Add(triangleSet.Key);
        }

        foreach (var cellPoint in peripheryCells)
        {
            var dot = Instantiate(_dotPrefab, transform);
            dot.transform.localPosition = new Vector3(cellPoint.x, cellPoint.y);
            dot.SetActive(false);
            _peripheryDots.Add(dot);
        }
    }
    
    public bool CanSelect()
    {
        return true;
    }

    public void Select()
    {
        _peripheryDots.ForEach(d => d.SetActive(true));
    }

    public void Deselect()
    {
        _peripheryDots.ForEach(d => d.SetActive(false));
    }
}