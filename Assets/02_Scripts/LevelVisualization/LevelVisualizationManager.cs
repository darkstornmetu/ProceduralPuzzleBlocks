using ProceduralBlocks.Data;
using ProceduralBlocks.Managers;
using UnityEngine;

[Injectable]
public class LevelVisualizationManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridArea _gridArea;
    [SerializeField] private PieceArea _pieceArea;
    
    private LevelData _currentLevelData;

    [Inject]
    private void Construct(LevelData data)
    {
        _currentLevelData = data;
    }
    
    private void Start()
    {
        _gridArea.InitializeGridArea(_currentLevelData.GridSize);
        _pieceArea.InitializePieceArea(_currentLevelData.Pieces);
        GameManager.GameStart();
    }
}