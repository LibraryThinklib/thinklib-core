using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(TopdownNPCController))]
public class TopdownNPCControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        TopdownNPCController npc = (TopdownNPCController)target;

        EditorGUILayout.LabelField("NPC Type", EditorStyles.boldLabel);
        npc.npcType = (TopdownNPCController.NPCType)EditorGUILayout.EnumPopup("Type", npc.npcType);

        if (npc.npcType == TopdownNPCController.NPCType.Patroller)
        {
            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Patrol Points", EditorStyles.boldLabel);
            SerializedProperty list = serializedObject.FindProperty("patrolPoints");
            EditorGUILayout.PropertyField(list, new GUIContent("Patrol Points"), true);

            npc.patrolSpeed = EditorGUILayout.FloatField("Speed", npc.patrolSpeed);
            npc.patrolTolerance = EditorGUILayout.FloatField("Tolerance", npc.patrolTolerance);

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Path Obstruction Layers", EditorStyles.boldLabel);
            npc.obstructionLayers = EditorGUILayout.MaskField("Obstruction Layers", npc.obstructionLayers, InternalEditorUtility.layers);
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Dialogue", EditorStyles.boldLabel);
        npc.hasDialogues = EditorGUILayout.Toggle("Has Dialogues?", npc.hasDialogues);

        if (npc.hasDialogues)
        {
            SerializedProperty dialogues = serializedObject.FindProperty("dialogues");
            EditorGUILayout.PropertyField(dialogues, new GUIContent("Dialogue List"), true);

            npc.dialogueBubblePrefab = (GameObject)EditorGUILayout.ObjectField("Dialogue Bubble Prefab", npc.dialogueBubblePrefab, typeof(GameObject), false);
            npc.bubbleAnchor = (Transform)EditorGUILayout.ObjectField("Bubble Anchor Point", npc.bubbleAnchor, typeof(Transform), true);
            npc.interactionKey = (KeyCode)EditorGUILayout.EnumPopup("Interaction Key", npc.interactionKey);

            EditorGUILayout.Space(6);
            npc.useTypewriterEffect = EditorGUILayout.Toggle("Use Typewriter Effect", npc.useTypewriterEffect);
            if (npc.useTypewriterEffect)
            {
                npc.typeSpeed = EditorGUILayout.FloatField("Typing Speed", npc.typeSpeed);
            }
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Player Reference", EditorStyles.boldLabel);
        npc.player = (Transform)EditorGUILayout.ObjectField("Player Transform", npc.player, typeof(Transform), true);
        npc.playerMovementScript = (MonoBehaviour)EditorGUILayout.ObjectField("Player Movement Script", npc.playerMovementScript, typeof(MonoBehaviour), true);

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(npc);
    }
}
