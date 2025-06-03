using UnityEditor;
using UnityEngine;
using Thinklib.Platformer.Enemy.Types;

[CustomEditor(typeof(EnemyShooterAI))]
public class EnemyShooterAIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EnemyShooterAI ai = (EnemyShooterAI)target;

        EditorGUILayout.LabelField("Refer�ncias", EditorStyles.boldLabel);
        ai.player = (Transform)EditorGUILayout.ObjectField("Player", ai.player, typeof(Transform), true);

        EditorGUILayout.Space(8);

        EditorGUILayout.LabelField("Configura��o de Disparo", EditorStyles.boldLabel);
        ai.raioDeDisparo = EditorGUILayout.FloatField("Raio de Disparo", ai.raioDeDisparo);
        ai.tempoEntreTiros = EditorGUILayout.FloatField("Tempo entre Tiros", ai.tempoEntreTiros);

        EditorGUILayout.Space(8);

        EditorGUILayout.LabelField("Modo de Disparo", EditorStyles.boldLabel);
        ai.mirarNoAlvo = EditorGUILayout.Toggle("Mirar no Alvo", ai.mirarNoAlvo);

        EditorGUILayout.Space(8);

        EditorGUILayout.LabelField("Modo de Funcionamento", EditorStyles.boldLabel);
        ai.isEstatico = EditorGUILayout.Toggle("Est�tico", ai.isEstatico);
        ai.isPatrulheiro = EditorGUILayout.Toggle("Patrulheiro", ai.isPatrulheiro);

        if (ai.isEstatico && ai.isPatrulheiro)
        {
            EditorGUILayout.HelpBox("Voc� marcou os dois modos ao mesmo tempo. Use apenas um: Est�tico ou Patrulheiro.", MessageType.Warning);
        }

        EditorGUILayout.Space(8);

        if (ai.isPatrulheiro)
        {
            EditorGUILayout.LabelField("Pontos de Patrulha", EditorStyles.boldLabel);
            ai.pontoA = (Transform)EditorGUILayout.ObjectField("Ponto A", ai.pontoA, typeof(Transform), true);
            ai.pontoB = (Transform)EditorGUILayout.ObjectField("Ponto B", ai.pontoB, typeof(Transform), true);
            ai.velocidadePatrulha = EditorGUILayout.FloatField("Velocidade", ai.velocidadePatrulha);
            ai.tolerancia = EditorGUILayout.FloatField("Toler�ncia", ai.tolerancia);
        }

        EditorUtility.SetDirty(ai);
    }
}
