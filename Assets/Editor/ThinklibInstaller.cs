using UnityEditor;
using UnityEngine;

public class ThinklibInstaller : MonoBehaviour
{
    [MenuItem("Thinklib/Instalar Dependências")]
    public static void Instalar()
    {
        AddLayerIfMissing("PlayerInvulnerable");
        AddLayerIfMissing("Enemy");

        EditorUtility.DisplayDialog("Thinklib Setup",
            "As Layers 'PlayerInvulnerable' e 'Enemy' foram adicionadas ao projeto (caso não existissem).\n\n" +
            "⚠️ Agora abra Edit > Project Settings > Physics 2D e desmarque a colisão entre:\n" +
            "PlayerInvulnerable ↔ Enemy",
            "Ok, entendido");
    }

    private static void AddLayerIfMissing(string layerName)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layersProp = tagManager.FindProperty("layers");

        bool found = false;
        for (int i = 8; i < layersProp.arraySize; i++) // user layers start at index 8
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
            for (int i = 8; i < layersProp.arraySize; i++)
            {
                SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);
                if (string.IsNullOrEmpty(sp.stringValue))
                {
                    sp.stringValue = layerName;
                    Debug.Log($"✅ Layer '{layerName}' criada no slot {i}");
                    tagManager.ApplyModifiedProperties();
                    break;
                }
            }
        }
    }
}
