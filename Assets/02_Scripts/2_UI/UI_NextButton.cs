using ProceduralBlocks.LevelTransition;
using UnityEngine;
using UnityEngine.UI;

namespace ProceduralBlocks.UI
{
    [Injectable]
    public class UI_NextButton : MonoBehaviour
    {
        [SerializeField] private Button _nextButton;

        private ILevelTransitionService _levelTransitionService;

        [Inject]
        private void Construct(ILevelTransitionService levelTransitionService)
        {
            _levelTransitionService = levelTransitionService;
        }
        
        private void NextLevel()
        {
            _nextButton.interactable = false;
            _levelTransitionService.LoadNextLevel();
        }
    
        private void OnEnable()
        {
            _nextButton.onClick.AddListener(NextLevel); 
        }

        private void OnDisable()
        {
            _nextButton.onClick.RemoveListener(NextLevel);
        }
    }
}