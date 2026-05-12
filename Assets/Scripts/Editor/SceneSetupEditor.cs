using UnityEditor;
using UnityEngine;
using Watermelon.Gameplay;

namespace Watermelon.Editor
{
    public static class SceneSetupEditor
    {
        [MenuItem("Watermelon/Setup/Add Wall Visuals")]
        public static void AddWallVisuals()
        {
            string[] wallNames = { "WallLeft", "WallRight", "WallBottom" };
            int count = 0;

            foreach (var wallName in wallNames)
            {
                var go = GameObject.Find(wallName);
                if (go == null)
                {
                    Debug.LogWarning($"[SceneSetup] '{wallName}' not found in scene.");
                    continue;
                }

                if (go.GetComponent<SpriteRenderer>() == null)
                    go.AddComponent<SpriteRenderer>();

                if (go.GetComponent<WallVisual>() == null)
                    go.AddComponent<WallVisual>();

                EditorUtility.SetDirty(go);
                count++;
            }

            if (count > 0)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene());
                Debug.Log($"[SceneSetup] WallVisual added to {count} wall(s). Save the scene.");
            }
        }
    }
}
