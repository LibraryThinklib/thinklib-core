// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public static class ThinklibImportResources
{
    [MenuItem("Thinklib/Import Resources", false, 101)]
    public static void ImportResources()
    {
        const string srcRoot = "Packages/com.thinklib.core/Runtime/Resources/Prefabs";
        const string dstRoot = "Assets/Thinklib/Resources/Prefabs";

        if (!Directory.Exists(srcRoot))
        {
            EditorUtility.DisplayDialog("Thinklib", $"Não encontrei:\n{srcRoot}", "OK");
            return;
        }

        int copied = 0;
        foreach (var src in Directory.GetFiles(srcRoot, "*.prefab", SearchOption.AllDirectories))
        {
            var rel = src.Substring(srcRoot.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var dst = Path.Combine(dstRoot, rel);

            Directory.CreateDirectory(Path.GetDirectoryName(dst)!);
            File.Copy(src, dst, true);
            copied++;
        }

        AssetDatabase.Refresh();
        Debug.Log($"[Thinklib] Recursos importados: {copied} prefabs.");
    }
}
#endif
