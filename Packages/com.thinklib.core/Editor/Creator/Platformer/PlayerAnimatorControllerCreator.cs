using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class PlayerAnimatorControllerCreator
{
    private const string Root = "Assets/Thinklib/Platformer/Player/Animations";

    [MenuItem("Platformer/Create Player Animator Controller")]
    public static void CreateFunctionalAnimatorController()
    {
        EnsureFolderPath(Root);

        var controllerPath = AssetDatabase.GenerateUniqueAssetPath($"{Root}/PlayerAnimatorController.controller");
        var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

        controller.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsJumping", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsFalling", AnimatorControllerParameterType.Bool);
        controller.AddParameter("MoveSpeed", AnimatorControllerParameterType.Float);
        controller.AddParameter("IsShooting", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsHurt", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("IsDead", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsAttacking", AnimatorControllerParameterType.Trigger);

        var sm = controller.layers[0].stateMachine;

        var idle = sm.AddState("Idle");
        var walk = sm.AddState("Walking");
        var jump = sm.AddState("Jumping");
        var fall = sm.AddState("Falling");
        var shoot = sm.AddState("ShootProjectile");
        var hurt = sm.AddState("Hurt");
        var dead = sm.AddState("Dead");
        var melee = sm.AddState("MeleeAttack");

        sm.defaultState = idle;

        var idleToWalk = idle.AddTransition(walk);
        idleToWalk.AddCondition(AnimatorConditionMode.If, 0, "IsMoving");
        idleToWalk.hasExitTime = false;

        var walkToIdle = walk.AddTransition(idle);
        walkToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsMoving");
        walkToIdle.hasExitTime = false;

        var walkToJump = walk.AddTransition(jump);
        walkToJump.AddCondition(AnimatorConditionMode.If, 0, "IsJumping");
        walkToJump.hasExitTime = false;

        var idleToJump = idle.AddTransition(jump);
        idleToJump.AddCondition(AnimatorConditionMode.If, 0, "IsJumping");
        idleToJump.hasExitTime = false;

        var jumpToFall = jump.AddTransition(fall);
        jumpToFall.AddCondition(AnimatorConditionMode.If, 0, "IsFalling");
        jumpToFall.hasExitTime = false;

        var fallToIdle = fall.AddTransition(idle);
        fallToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsFalling");
        fallToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsJumping");
        fallToIdle.hasExitTime = false;

        ConfigureShoot(shoot, idle);
        ConfigureShoot(shoot, walk);
        ConfigureShoot(shoot, jump);
        ConfigureShoot(shoot, fall);

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

        var anyAttack = sm.AddAnyStateTransition(melee);
        anyAttack.AddCondition(AnimatorConditionMode.If, 0, "IsAttacking");
        anyAttack.AddCondition(AnimatorConditionMode.IfNot, 0, "IsDead");
        anyAttack.hasExitTime = false;

        var meleeToIdle = melee.AddTransition(idle);
        meleeToIdle.hasExitTime = true;
        meleeToIdle.exitTime = 1f;
        meleeToIdle.duration = 0.1f;

        Debug.Log($"✅ Player Animator Controller criado em: {controllerPath}");
    }

    private static void ConfigureShoot(AnimatorState shoot, AnimatorState from)
    {
        var toShoot = from.AddTransition(shoot);
        toShoot.AddCondition(AnimatorConditionMode.If, 0, "IsShooting");
        toShoot.hasExitTime = false;
        toShoot.duration = 0f;

        var back = shoot.AddTransition(from);
        back.AddCondition(AnimatorConditionMode.IfNot, 0, "IsShooting");
        back.hasExitTime = true;
        back.exitTime = 1f;
        back.duration = 0.1f;
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
