using UnityEngine;

[CreateAssetMenu(menuName = "ProceduralBlocks/LevelsContainer", fileName = "LevelsContainer", order = 0)]
public class LevelsContainer : ScriptableObject
{
    public TextAsset[] Levels => _levels;
        
    [SerializeField] private TextAsset[] _levels;
}