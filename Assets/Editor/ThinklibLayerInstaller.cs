using UnityEditor;
using UnityEngine;

public class ThinklibLayerInstaller : MonoBehaviour
{
    [MenuItem("Thinklib/Install Required Layers")]
    public static void InstallLayers()
    {
        string[] requiredLayers = new string[]
        {
            "Default", "TransparentFX", "Ignore Raycast", "Water", "UI",
            "PlayerInvulnerable", "Enemy", "Player", "NPC"
        };

        foreach (var layer in requiredLayers)
        {
            AddLayerIfMissing(layer);
        }

        EditorUtility.DisplayDialog("Thinklib Setup",
            "The required layers have been added if they were missing.\n\n" +
            "⚠️ Now open Edit > Project Settings > Physics 2D and disable the collision between:\n" +
            "PlayerInvulnerable ↔ Enemy",
            "Ok, got it");
    }

    private static void AddLayerIfMissing(string layerName)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layersProp = tagManager.FindProperty("layers");

        bool found = false;
        for (int i = 0; i < layersProp.arraySize; i++)
        {
            SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);
            if (sp != null && sp.stringValue == layerName)
            {
                found = true;
                break;
            }
        }

        if (!found)
        {
            for (int i = 8; i < layersProp.arraySize; i++) // user layers start at index 8
            {
                SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);
                if (string.IsNullOrEmpty(sp.stringValue))
                {
                    sp.stringValue = layerName;
                    Debug.Log($"✅ Layer '{layerName}' created at slot {i}");
                    tagManager.ApplyModifiedProperties();
                    break;
                }
            }
        }
    }
}
