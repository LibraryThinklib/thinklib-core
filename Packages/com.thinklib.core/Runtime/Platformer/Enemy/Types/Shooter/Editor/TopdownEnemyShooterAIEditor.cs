using UnityEditor;
using UnityEngine;
using Thinklib.Platformer.Enemy.Types;

[CustomEditor(typeof(EnemyShooterAI))]
public class EnemyShooterAIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EnemyShooterAI ai = (EnemyShooterAI)target;

        EditorGUILayout.LabelField("References", EditorStyles.boldLabel);
        ai.player = (Transform)EditorGUILayout.ObjectField("Player", ai.player, typeof(Transform), true);

        EditorGUILayout.Space(8);

        EditorGUILayout.LabelField("Shooting Settings", EditorStyles.boldLabel);
        ai.shootingRadius = EditorGUILayout.FloatField("Shooting Radius", ai.shootingRadius);
        ai.timeBetweenShots = EditorGUILayout.FloatField("Time Between Shots", ai.timeBetweenShots);

        EditorGUILayout.Space(8);

        EditorGUILayout.LabelField("Damage Settings", EditorStyles.boldLabel);
        ai.projectileDamage = EditorGUILayout.IntField("Projectile Damage", ai.projectileDamage);

        EditorGUILayout.Space(8);

        EditorGUILayout.LabelField("Shooting Mode", EditorStyles.boldLabel);
        ai.aimAtTarget = EditorGUILayout.Toggle("Aim at Target", ai.aimAtTarget);

        EditorGUILayout.Space(8);

        EditorGUILayout.LabelField("Behavior Mode", EditorStyles.boldLabel);
        ai.isStatic = EditorGUILayout.Toggle("Static", ai.isStatic);
        ai.isPatroller = EditorGUILayout.Toggle("Patroller", ai.isPatroller);

        if (ai.isStatic && ai.isPatroller)
        {
            EditorGUILayout.HelpBox("Both modes are selected. Please choose only one: Static or Patroller.", MessageType.Warning);
        }

        EditorGUILayout.Space(8);

        if (ai.isPatroller)
        {
            EditorGUILayout.LabelField("Patrol Points", EditorStyles.boldLabel);
            ai.pointA = (Transform)EditorGUILayout.ObjectField("Point A", ai.pointA, typeof(Transform), true);
            ai.pointB = (Transform)EditorGUILayout.ObjectField("Point B", ai.pointB, typeof(Transform), true);
            ai.patrolSpeed = EditorGUILayout.FloatField("Speed", ai.patrolSpeed);
            ai.patrolTolerance = EditorGUILayout.FloatField("Tolerance", ai.patrolTolerance);
        }

        EditorUtility.SetDirty(ai);
    }
}
