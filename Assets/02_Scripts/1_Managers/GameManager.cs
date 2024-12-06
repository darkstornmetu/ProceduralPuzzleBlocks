using System;

namespace ProceduralBlocks.Managers
{
    public static class GameManager
    {
        public static event Action onLevelStart;
        public static event Action onLevelOver;

        public static event Action onLevelSuccess;
        public static event Action onLevelFail;

        public static bool IsDecided { get; set; }
        
        public static void GameStart()
        {
            onLevelStart?.Invoke();
            IsDecided = false;
        }

        public static void GameFail()
        {
            onLevelOver?.Invoke();
            onLevelFail?.Invoke();      
            GameManager.IsDecided = true;
        }

        public static void GameSuccess()
        {
            GameManager.IsDecided = true;
            onLevelOver?.Invoke();
            onLevelSuccess?.Invoke();
        }        
    }
}