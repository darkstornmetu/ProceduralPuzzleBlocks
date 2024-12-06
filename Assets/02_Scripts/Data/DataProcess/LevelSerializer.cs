using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace ProceduralBlocks.Data.Process
{
    public class LevelSerializer : IParser<LevelData>
    {
        private static readonly string sr_levelsDirectory = Application.dataPath + "/04_GamePrefabs/Levels";

        public static void SaveLevel(LevelData levelData)
        {
            string json = JsonUtility.ToJson(levelData, false);
            string guid = GUID.Generate().ToString();
            File.WriteAllText($"{sr_levelsDirectory}/Level_{levelData.DifficultyLevel}_{guid}.json", json);
#if UNITY_EDITOR        
            Debug.Log($"Level_{levelData.DifficultyLevel}_{guid} generated");
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            AssetDatabase.Refresh();
#endif
        }
   
        public LevelData ParseData(string file)
        {
            return JsonUtility.FromJson<LevelData>(file);
        }
    }
}
