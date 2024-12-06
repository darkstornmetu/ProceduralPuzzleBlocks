using ProceduralBlocks.Grid;
using UnityEngine;

[Injectable]
public class GridArea : MonoBehaviour
{
    [SerializeField] private GridBorder _gridBorderPrefab;
    [SerializeField] private GridDot _gridDotPrefab;

    private IGridService<Vector3> _positionsGrid;
    
    [Inject]
    private void Construct(IGridService<Vector3> posGrid)
    {
        _positionsGrid = posGrid;
    }

    public void InitializeGridArea(int gridSize)
    {
        InstantiateGridBorders(gridSize);
        InstantiateGridDots(gridSize);
    }

    private void InstantiateGridDots(int gridSize)
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector2Int coords = new Vector2Int(x, y);
                Vector3 pos = _positionsGrid.GetItemAtCoordinates(coords);
                Instantiate(_gridDotPrefab, pos, Quaternion.identity, transform);
            }
        }
    }

    private void InstantiateGridBorders(int gridSize)
    {
        for (int i = 0; i < 4; i++)
        {
            Vector3 direction = GetDirectionFromIndex(i);
            Vector3 instantiatePos = transform.position + direction * (gridSize / 2f);
            Quaternion instantiateRot = Quaternion.Euler(new Vector3(0, 0, -90 * i));
            
            var gridBorder = Instantiate(_gridBorderPrefab, instantiatePos, instantiateRot, transform);
            gridBorder.SetSize(gridSize);
        }
        
        Vector3 GetDirectionFromIndex(int index)
        {
            return index switch
            {
                0 => Vector3.left,
                1 => Vector3.up,
                2 => Vector3.right,
                3 => Vector3.down,
                _ => Vector3.zero
            };
        }
    }
}
