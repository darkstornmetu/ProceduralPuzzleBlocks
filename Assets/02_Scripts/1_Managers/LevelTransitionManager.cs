using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProceduralBlocks.LevelTransition
{
    public class LevelTransitionManager : ILevelTransitionService
    {
        private const string _LEVEL_KEY = "lm_levelKey";

        private int _currentLevelIndex;
        private readonly int _totalLevelCount;

        public LevelTransitionManager(int totalLevelCount)
        {
            _totalLevelCount = totalLevelCount;
            _currentLevelIndex = GetCurrentLevelIndexFromDisk();
        }
        
        public int GetCurrentLevelIndexFromDisk()
        {
            return PlayerPrefs.GetInt(_LEVEL_KEY);
        }

        public void LoadCurrentLevel()
        {
            SceneTransition();
        }

        public void LoadNextLevel()
        {
            if (_currentLevelIndex < _totalLevelCount - 1)
                _currentLevelIndex++;
            else
                _currentLevelIndex = 0;

            SaveLevelToDisk();

            SceneTransition();
        }
        
        private void SaveLevelToDisk()
        {
            PlayerPrefs.SetInt(_LEVEL_KEY, _currentLevelIndex);
        }

        private void SceneTransition()
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
    }
}