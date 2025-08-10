using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class EnemyAnimatorControllerCreator
{
    private const string Root = "Assets/Thinklib/Platformer/Enemy/Animations";

    [MenuItem("Platformer/Create Enemy Animator Controller")]
    public static void CreateEnemyAnimatorController()
    {
        EnsureFolderPath(Root);

        var controllerPath = AssetDatabase.GenerateUniqueAssetPath($"{Root}/EnemyAnimatorController.controller");
        var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

        controller.AddParameter("IsWalking", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsAttacking", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsShooting", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsDead", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsHurt", AnimatorControllerParameterType.Trigger);

        var sm = controller.layers[0].stateMachine;

        var idle = sm.AddState("Idle");
        var walk = sm.AddState("Walking");
        var attack = sm.AddState("Attacking");
        var shoot = sm.AddState("ShootProjectile");
        var hurt = sm.AddState("Hurt");
        var dead = sm.AddState("Dead");

        sm.defaultState = idle;

        var idleToWalk = idle.AddTransition(walk);
        idleToWalk.AddCondition(AnimatorConditionMode.If, 0, "IsWalking");
        idleToWalk.hasExitTime = false;

        var walkToIdle = walk.AddTransition(idle);
        walkToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsWalking");
        walkToIdle.hasExitTime = false;

        var anyAttack = sm.AddAnyStateTransition(attack);
        anyAttack.AddCondition(AnimatorConditionMode.If, 0, "IsAttacking");
        anyAttack.hasExitTime = false;

        var attackToIdle = attack.AddTransition(idle);
        attackToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsAttacking");
        attackToIdle.hasExitTime = true;
        attackToIdle.exitTime = 1f;
        attackToIdle.duration = 0.1f;

        var anyShoot = sm.AddAnyStateTransition(shoot);
        anyShoot.AddCondition(AnimatorConditionMode.If, 0, "IsShooting");
        anyShoot.hasExitTime = false;

        var shootToIdle = shoot.AddTransition(idle);
        shootToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsShooting");
        shootToIdle.hasExitTime = true;
        shootToIdle.exitTime = 1f;
        shootToIdle.duration = 0.1f;

        var anyHurt = sm.AddAnyStateTransition(hurt);
        anyHurt.AddCondition(AnimatorConditionMode.If, 0, "IsHurt");
        anyHurt.AddCondition(AnimatorConditionMode.IfNot, 0, "IsDead");
        anyHurt.hasExitTime = false;

        var hurtToIdle = hurt.AddTransition(idle);
        hurtToIdle.hasExitTime = true;
        hurtToIdle.exitTime = 1f;
        hurtToIdle.duration = 0.1f;

        var anyDead = sm.AddAnyStateTransition(dead);
        anyDead.AddCondition(AnimatorConditionMode.If, 0, "IsDead");
        anyDead.hasExitTime = false;

        Debug.Log($"✅ Enemy Animator Controller criado em: {controllerPath}");
    }

    private static void EnsureFolderPath(string path)
    {
        var parts = path.Split('/');
        var current = parts[0]; // "Assets"
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
