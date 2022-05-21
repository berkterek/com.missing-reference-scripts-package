using System.Linq;
using UnityEngine;
using UnityEditor;

namespace TerekGaming.EditorHelper
{
    public static class FindMissingScript
    {
        [MenuItem("Tools/Terek Gaming/Missing Scripts/Find Missing Scripts in Project")]
        static void FindMissingScriptsInProjectMenuItem()
        {
            bool isMissingScriptExist = false;
            string[] prefabPaths = AssetDatabase.GetAllAssetPaths()
                .Where(x => x.EndsWith(".prefab", System.StringComparison.OrdinalIgnoreCase)).ToArray();

            foreach (string path in prefabPaths)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                foreach (Component component in prefab.GetComponentsInChildren<Component>())
                {
                    if (component == null)
                    {
                        isMissingScriptExist = true;
                        Debug.Log($"Prefab found with missing script <color=red>{path}</color>", prefab);
                        break;
                    }
                }
            }

            if (!isMissingScriptExist)
            {
                Debug.Log("<color=green>There is not missing script in project</color>");
            }
        }

        [MenuItem("Tools/Terek Gaming/Missing Scripts/Find Missing Scripts in Current Scene")]
        static void FindMissingScriptsInCurrentSceneMenuItem()
        {
            bool isMissingScriptExist = false;
            GameObject[] gameObjects = GameObject.FindObjectsOfType<GameObject>(true);

            foreach (GameObject gameObject in gameObjects)
            {
                foreach (Component component in gameObject.GetComponentsInChildren<Component>())
                {
                    if (component == null)
                    {
                        isMissingScriptExist = true;
                        Debug.Log($"Prefab found with missing script <color=red>{gameObject.name}</color>", gameObject);
                        break;
                    }
                }    
            }
            
            if (!isMissingScriptExist)
            {
                Debug.Log("<color=green>There is not missing script in current scene</color>");
            }
        }
        
        [MenuItem("Tools/Terek Gaming/Missing Scripts/Find and Auto Remove Missing Scripts in Current Scene")]
        static void FindAndAutoRemoveMissingScriptsInCurrentSceneMenuItem()
        {
            bool isMissingScriptExist = false;
            GameObject[] gameObjects = GameObject.FindObjectsOfType<GameObject>(true);

            foreach (GameObject gameObject in gameObjects)
            {
                foreach (Component component in gameObject.GetComponentsInChildren<Component>())
                {
                    if (component == null)
                    {
                        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
                        isMissingScriptExist = true;
                    }
                }    
            }
            
            if (!isMissingScriptExist)
            {
                Debug.Log("<color=green>There is not missing script in current scene</color>");
            }
        }
        
        [MenuItem("Tools/Terek Gaming/Missing Scripts/Find And Auto Remove Missing Scripts in Project")]
        static void FindAndAutoRemoveMissingScriptsInProjectMenuItem()
        {
            bool isMissingScriptExist = false;
            string[] prefabPaths = AssetDatabase.GetAllAssetPaths()
                .Where(x => x.EndsWith(".prefab", System.StringComparison.OrdinalIgnoreCase)).ToArray();

            foreach (string path in prefabPaths)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                foreach (Component component in prefab.GetComponentsInChildren<Component>())
                {
                    if (component == null)
                    {
                        isMissingScriptExist = true;
                        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(prefab);
                    }
                }
            }

            if (!isMissingScriptExist)
            {
                Debug.Log("<color=green>There is not missing script in project</color>");
            }
        }
    }
}