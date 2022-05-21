using System.Collections.Generic;
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
        private static void FindAndRemoveMissingInSelected()
        {
            // EditorUtility.CollectDeepHierarchy does not include inactive children
            var deeperSelection = Selection.gameObjects.SelectMany(go => go.GetComponentsInChildren<Transform>(true))
                .Select(t => t.gameObject);
            var prefabs = new HashSet<Object>();
            int compCount = 0;
            int goCount = 0;
            foreach (var go in deeperSelection)
            {
                int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                if (count > 0)
                {
                    if (PrefabUtility.IsPartOfAnyPrefab(go))
                    {
                        RecursivePrefabSource(go, prefabs, ref compCount, ref goCount);
                        count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                        // if count == 0 the missing scripts has been removed from prefabs
                        if (count == 0)
                            continue;
                        // if not the missing scripts must be prefab overrides on this instance
                    }

                    Undo.RegisterCompleteObjectUndo(go, "Remove missing scripts");
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                    compCount += count;
                    goCount++;
                }
            }

            Debug.Log($"Found and removed {compCount} missing scripts from {goCount} GameObjects");
        }

        // Prefabs can both be nested or variants, so best way to clean all is to go through them all
        // rather than jumping straight to the original prefab source.
        private static void RecursivePrefabSource(GameObject instance, HashSet<Object> prefabs, ref int compCount,
            ref int goCount)
        {
            var source = PrefabUtility.GetCorrespondingObjectFromSource(instance);
            // Only visit if source is valid, and hasn't been visited before
            if (source == null || !prefabs.Add(source))
                return;

            // go deep before removing, to differantiate local overrides from missing in source
            RecursivePrefabSource(source, prefabs, ref compCount, ref goCount);

            int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(source);
            if (count > 0)
            {
                Undo.RegisterCompleteObjectUndo(source, "Remove missing scripts");
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(source);
                compCount += count;
                goCount++;
            }
        }
    }
}