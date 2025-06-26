using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class TopdownPlayerAnimatorControllerCreator : MonoBehaviour
{
    [MenuItem("Topdown/Create Topdown Player Animator Controller")]
    public static void CreateTopdownAnimatorController()
    {
        string folderPath = "Assets/Thinklib/Topdown/Player/Animations";

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/Thinklib/Topdown/Player", "Animations");
        }

        string controllerPath = $"{folderPath}/TopdownPlayerAnimatorController.controller";
        var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

        // Parâmetros
        controller.AddParameter("Horizontal", AnimatorControllerParameterType.Float);
        controller.AddParameter("Vertical", AnimatorControllerParameterType.Float);
        controller.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsShooting", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsAttacking", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("IsHurt", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("IsDead", AnimatorControllerParameterType.Bool);

        var root = controller.layers[0].stateMachine;

        // Estados
        var idle = root.AddState("Idle");
        var walk = root.AddState("Walking");
        var shoot = root.AddState("ShootProjectile");
        var melee = root.AddState("MeleeAttack");
        var hurt = root.AddState("Hurt");
        var dead = root.AddState("Dead");

        root.defaultState = idle;

        // Blend Tree de movimentação
        var blendTree = new BlendTree
        {
            name = "WalkBlendTree",
            blendType = BlendTreeType.SimpleDirectional2D,
            blendParameter = "Horizontal",
            useAutomaticThresholds = false
        };

        // Força corretamente o Vertical como segundo parâmetro
        SerializedObject serializedBlendTree = new SerializedObject(blendTree);
        serializedBlendTree.FindProperty("m_BlendParameterY").stringValue = "Vertical";
        serializedBlendTree.ApplyModifiedProperties();

        AssetDatabase.AddObjectToAsset(blendTree, controller);
        walk.motion = blendTree;

        blendTree.AddChild(null, new Vector2(0, 1));   // Cima
        blendTree.AddChild(null, new Vector2(0, -1));  // Baixo
        blendTree.AddChild(null, new Vector2(1, 0));   // Direita
        blendTree.AddChild(null, new Vector2(-1, 0));  // Esquerda
        blendTree.AddChild(null, new Vector2(1, 1));   // Cima-direita
        blendTree.AddChild(null, new Vector2(-1, 1));  // Cima-esquerda
        blendTree.AddChild(null, new Vector2(1, -1));  // Baixo-direita
        blendTree.AddChild(null, new Vector2(-1, -1)); // Baixo-esquerda

        // Transições de movimento
        var idleToWalk = idle.AddTransition(walk);
        idleToWalk.AddCondition(AnimatorConditionMode.If, 0, "IsMoving");
        idleToWalk.hasExitTime = false;

        var walkToIdle = walk.AddTransition(idle);
        walkToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsMoving");
        walkToIdle.hasExitTime = false;

        // Shoot
        ConfigureShootProjectileTransitions(idle, shoot);
        ConfigureShootProjectileTransitions(walk, shoot);

        // Melee Attack
        var anyAttack = root.AddAnyStateTransition(melee);
        anyAttack.AddCondition(AnimatorConditionMode.If, 0, "IsAttacking");
        anyAttack.AddCondition(AnimatorConditionMode.IfNot, 0, "IsDead");
        anyAttack.hasExitTime = false;

        var meleeToIdle = melee.AddTransition(idle);
        meleeToIdle.hasExitTime = true;
        meleeToIdle.exitTime = 1f;

        // Hurt
        var anyHurt = root.AddAnyStateTransition(hurt);
        anyHurt.AddCondition(AnimatorConditionMode.If, 0, "IsHurt");
        anyHurt.AddCondition(AnimatorConditionMode.IfNot, 0, "IsDead");
        anyHurt.hasExitTime = false;

        var hurtToIdle = hurt.AddTransition(idle);
        hurtToIdle.hasExitTime = true;
        hurtToIdle.exitTime = 1f;

        // Dead
        var anyDead = root.AddAnyStateTransition(dead);
        anyDead.AddCondition(AnimatorConditionMode.If, 0, "IsDead");
        anyDead.hasExitTime = false;

        Debug.Log("✅ Topdown Animator Controller criado com sucesso em: " + controllerPath);
    }

    private static void ConfigureShootProjectileTransitions(AnimatorState fromState, AnimatorState shootProjectileState)
    {
        var toShoot = fromState.AddTransition(shootProjectileState);
        toShoot.AddCondition(AnimatorConditionMode.If, 1, "IsShooting");
        toShoot.hasExitTime = false;

        var fromShoot = shootProjectileState.AddTransition(fromState);
        fromShoot.AddCondition(AnimatorConditionMode.IfNot, 0, "IsShooting");
        fromShoot.hasExitTime = true;
        fromShoot.exitTime = 1f;
    }
}
