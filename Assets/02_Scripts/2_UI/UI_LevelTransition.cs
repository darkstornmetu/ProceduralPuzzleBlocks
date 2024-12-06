using Sirenix.OdinInspector;
using ProceduralBlocks.Managers;
using UnityEngine;

namespace ProceduralBlocks.UI
{
    public class UI_LevelTransition : MonoBehaviour
    {
        [Header("Level End")]
        [SerializeField] private GameObject _winPanel;
        
        [Button]
        private void ShowWinUI()
        {
            _winPanel.SetActive(true);
        }
        
        private void OnEnable()
        {
            GameManager.onLevelSuccess += ShowWinUI;
        }

        private void OnDisable()
        {
            GameManager.onLevelSuccess -= ShowWinUI;
        }
    }
}