using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class TopdownEnemyAnimatorControllerCreator : MonoBehaviour
{
    [MenuItem("Topdown/Create Topdown Enemy Animator Controller")]
    public static void CreateTopdownEnemyAnimatorController()
    {
        string folderPath = "Assets/Thinklib/Topdown/Enemy/Animations";

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/Thinklib/Topdown", "Enemy");
            AssetDatabase.CreateFolder("Assets/Thinklib/Topdown/Enemy", "Animations");
        }

        string controllerPath = $"{folderPath}/TopdownEnemyAnimatorController.controller";
        var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

        // Parameters
        controller.AddParameter("Horizontal", AnimatorControllerParameterType.Float);
        controller.AddParameter("Vertical", AnimatorControllerParameterType.Float);
        controller.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsShooting", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsAttacking", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("IsHurt", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("IsDead", AnimatorControllerParameterType.Bool);

        var root = controller.layers[0].stateMachine;

        // States
        var idle = root.AddState("Idle");
        var walk = root.AddState("Walking");
        var shoot = root.AddState("ShootProjectile");
        var melee = root.AddState("MeleeAttack");
        var hurt = root.AddState("Hurt");
        var dead = root.AddState("Dead");

        root.defaultState = idle;

        // ✅ Add BlendTrees for each
        idle.motion = CreateDirectionalBlendTree(controller, "IdleBlendTree");
        walk.motion = CreateDirectionalBlendTree(controller, "WalkBlendTree");
        hurt.motion = CreateDirectionalBlendTree(controller, "HurtBlendTree");
        shoot.motion = CreateDirectionalBlendTree(controller, "ShootBlendTree");
        melee.motion = CreateDirectionalBlendTree(controller, "MeleeBlendTree");
        dead.motion = CreateDirectionalBlendTree(controller, "DeadBlendTree");

        // Transitions: Idle <-> Walking
        var idleToWalk = idle.AddTransition(walk);
        idleToWalk.AddCondition(AnimatorConditionMode.If, 0f, "IsMoving");
        idleToWalk.hasExitTime = false;

        var walkToIdle = walk.AddTransition(idle);
        walkToIdle.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsMoving");
        walkToIdle.hasExitTime = false;

        // Shoot
        ConfigureShootProjectileTransitions(idle, shoot);
        ConfigureShootProjectileTransitions(walk, shoot);

        // Melee
        var anyAttack = root.AddAnyStateTransition(melee);
        anyAttack.AddCondition(AnimatorConditionMode.If, 0f, "IsAttacking");
        anyAttack.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsDead");
        anyAttack.hasExitTime = false;

        var meleeToIdle = melee.AddTransition(idle);
        meleeToIdle.hasExitTime = true;
        meleeToIdle.exitTime = 1f;

        // Hurt
        var anyHurt = root.AddAnyStateTransition(hurt);
        anyHurt.AddCondition(AnimatorConditionMode.If, 0f, "IsHurt");
        anyHurt.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsDead");
        anyHurt.hasExitTime = false;

        var hurtToIdle = hurt.AddTransition(idle);
        hurtToIdle.hasExitTime = true;
        hurtToIdle.exitTime = 1f;

        // Dead
        var anyDead = root.AddAnyStateTransition(dead);
        anyDead.AddCondition(AnimatorConditionMode.If, 0f, "IsDead");
        anyDead.hasExitTime = false;

        Debug.Log("✅ Topdown Enemy Animator Controller criado com sucesso em: " + controllerPath);
    }

    private static BlendTree CreateDirectionalBlendTree(AnimatorController controller, string blendTreeName)
    {
        var blendTree = new BlendTree
        {
            name = blendTreeName,
            blendType = BlendTreeType.SimpleDirectional2D,
            blendParameter = "Horizontal",
            useAutomaticThresholds = false
        };

        // Force Vertical param
        SerializedObject serializedBlendTree = new SerializedObject(blendTree);
        serializedBlendTree.FindProperty("m_BlendParameterY").stringValue = "Vertical";
        serializedBlendTree.ApplyModifiedProperties();

        AssetDatabase.AddObjectToAsset(blendTree, controller);

        // 8 Directions
        blendTree.AddChild(null, new Vector2(0, 1));    // Up
        blendTree.AddChild(null, new Vector2(0, -1));   // Down
        blendTree.AddChild(null, new Vector2(1, 0));    // Right
        blendTree.AddChild(null, new Vector2(-1, 0));   // Left
        blendTree.AddChild(null, new Vector2(1, 1));    // Up-Right
        blendTree.AddChild(null, new Vector2(-1, 1));   // Up-Left
        blendTree.AddChild(null, new Vector2(1, -1));   // Down-Right
        blendTree.AddChild(null, new Vector2(-1, -1));  // Down-Left

        return blendTree;
    }

    private static void ConfigureShootProjectileTransitions(AnimatorState fromState, AnimatorState shootProjectileState)
    {
        var toShoot = fromState.AddTransition(shootProjectileState);
        toShoot.AddCondition(AnimatorConditionMode.If, 1f, "IsShooting");
        toShoot.hasExitTime = false;

        var fromShoot = shootProjectileState.AddTransition(fromState);
        fromShoot.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsShooting");
        fromShoot.hasExitTime = true;
        fromShoot.exitTime = 1f;
    }
}
