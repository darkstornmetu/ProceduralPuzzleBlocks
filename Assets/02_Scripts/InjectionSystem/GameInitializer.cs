using System;
using System.Collections.Generic;
using DependencyInjectionSystem;
using ProceduralBlocks.Data;
using ProceduralBlocks.Data.Process;
using ProceduralBlocks.Grid;
using ProceduralBlocks.LevelTransition;
using ProceduralBlocks.Managers;
using ProceduralBlocks.Selection;
using ProceduralBlocks.UserInput;
using Sirenix.OdinInspector;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameInitializer : MonoBehaviour
{
    [Header("Inject Scene References")] 
    [SerializeField] private Camera _mainCam;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private Transform _gridArea;

    [Header("Level References")]
    [SerializeField] private bool _overrideIndex;
    [SerializeField, ShowIf("@_overrideIndex")] private int _levelIndex;
    [SerializeField, InlineEditor] private LevelsContainer _levelsContainer;
    
    private void Awake()
    {
        LevelTransitionManager levelManager = new LevelTransitionManager(_levelsContainer.Levels.Length);

        int index = _overrideIndex ? _levelIndex : levelManager.GetCurrentLevelIndexFromDisk();
        var currentLevel = _levelsContainer.Levels[index];
        
        IParser<LevelData> levelParser = new LevelSerializer();
        LevelData levelData = levelParser.ParseData(currentLevel.text);
        
        PositionsGrid positionsGrid = new PositionsGrid(levelData, _gridArea.position);
        PieceOverlapChecker pieceOverlapChecker = new PieceOverlapChecker(positionsGrid);
        
        ServiceContainer.Register(levelData);
        
        ServiceContainer.Register(_mainCam);
        ServiceContainer.Register(pieceOverlapChecker);
        ServiceContainer.Register(positionsGrid);
        
        ServiceContainer.Register<IGridService<Vector3>>(positionsGrid);
        ServiceContainer.Register<ILevelTransitionService>(levelManager);
        ServiceContainer.Register<ISelectionService>(new SelectionManager(_mainCam));
        ServiceContainer.Register<IInputService>(_inputManager);
        
        InjectDependenciesInScene();
    }

    private void InjectDependenciesInScene()
    {
        var injectables = InjectableMonoBehaviours();
        
        foreach (var injectable in injectables) 
            ServiceContainer.InjectDependencies(injectable);
    }

    private IEnumerable<MonoBehaviour> InjectableMonoBehaviours()
    {
        var allMonoBehaviours = 
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        foreach (var monoBehaviour in allMonoBehaviours)
        {
            var type = monoBehaviour.GetType();
            if (Attribute.IsDefined(type, typeof(InjectableAttribute)))
            {
                yield return monoBehaviour;
            }
        }
    }
}