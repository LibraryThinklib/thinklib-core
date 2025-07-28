using UnityEngine;
using UnityEditor;
using System.IO;

public class PointAndClickMenuItems
{
    [MenuItem("Point and Click/Create New Item")]
    private static void CreateItemAsset()
    {
        CreateAsset<Item>("New Item");
    }

    [MenuItem("Point and Click/Create New Combination Recipe")]
    private static void CreateCombinationRecipeAsset()
    {
        CreateAsset<CombinationRecipe>("New Combination");
    }

    private static void CreateAsset<T>(string defaultName) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path))
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + defaultName + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}