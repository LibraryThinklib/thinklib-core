// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

﻿using UnityEditor;
using UnityEngine;

public class ThinklibTagInstaller : MonoBehaviour
{
    [MenuItem("Thinklib/Install Required Tags")]
    public static void InstallTags()
    {
        string[] requiredTags = new string[]
        {
            "Player", "Ground", "Enemy", "Tower", "NPC"
        };

        foreach (string tag in requiredTags)
        {
            AddTagIfMissing(tag);
        }

        EditorUtility.DisplayDialog("Thinklib Setup",
            "The required tags have been added if they were missing.",
            "Ok, got it");
    }

    private static void AddTagIfMissing(string tagName)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t != null && t.stringValue.Equals(tagName)) return; // Already exists
        }

        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t != null && string.IsNullOrEmpty(t.stringValue))
            {
                t.stringValue = tagName;
                Debug.Log($"✅ Tag '{tagName}' created in slot {i}");
                tagManager.ApplyModifiedProperties();
                return;
            }
        }

        // If no empty slot found, expand array and add
        tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
        tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tagName;
        tagManager.ApplyModifiedProperties();
        Debug.Log($"✅ Tag '{tagName}' added at the end of the tag list.");
    }
}
