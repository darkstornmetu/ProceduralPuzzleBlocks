using ProceduralBlocks.Managers;
using UnityEngine;

namespace ProceduralBlocks.Selection
{
    public static class SelectionLock
    {
        public static bool CanSelect { get; set; } = true;

        private static void LockSelection()
        {
            CanSelect = false;
        }
        
        private static void ResetSelection()
        {
            CanSelect = true;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            GameManager.onLevelStart += ResetSelection;
            GameManager.onLevelOver += LockSelection;
        }
    }
}