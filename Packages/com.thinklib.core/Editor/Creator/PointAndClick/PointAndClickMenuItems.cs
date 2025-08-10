using UnityEditor;
using UnityEngine;
using System.IO;

public static class PointAndClickMenuItems
{
    private const string ItemsRoot   = "Assets/Thinklib/PointAndClick/Items";
    private const string RecipesRoot = "Assets/Thinklib/PointAndClick/Recipes";

    [MenuItem("Point & Click/Create New Item")]
    private static void CreateItemAsset()
    {
        CreateAsset<Item>("New Item", ItemsRoot);
    }

    [MenuItem("Point & Click/Create New Combination Recipe")]
    private static void CreateCombinationRecipeAsset()
    {
        CreateAsset<CombinationRecipe>("New Combination", RecipesRoot);
    }

    private static void CreateAsset<T>(string defaultName, string defaultRoot) where T : ScriptableObject
    {
        var asset = ScriptableObject.CreateInstance<T>();

        var folder = ResolveTargetFolder(defaultRoot);
        EnsureFolderPath(folder);

        var path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{defaultName}.asset");
        AssetDatabase.CreateAsset(asset, path);

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        Debug.Log($"✅ Created {typeof(T).Name} at: {path}");
    }

    private static string ResolveTargetFolder(string defaultRoot)
    {
        var sel  = Selection.activeObject;
        var path = sel ? AssetDatabase.GetAssetPath(sel) : null;

        if (string.IsNullOrEmpty(path))
            return defaultRoot;

        if (AssetDatabase.IsValidFolder(path))
            return path.StartsWith("Assets") ? path : defaultRoot;

        var dir = Path.GetDirectoryName(path)?.Replace("\\", "/");
        if (!string.IsNullOrEmpty(dir) && dir.StartsWith("Assets"))
            return dir;

        return defaultRoot;
    }

    private static void EnsureFolderPath(string path)
    {
        var parts = path.Split('/');
        var current = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            var next = parts[i];
            var combined = $"{current}/{next}";
            if (!AssetDatabase.IsValidFolder(combined))
                AssetDatabase.CreateFolder(current, next);
            current = combined;
        }
    }
}
