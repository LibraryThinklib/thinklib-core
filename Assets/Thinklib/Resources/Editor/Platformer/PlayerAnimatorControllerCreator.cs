using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerAnimatorControllerCreator : MonoBehaviour
{
    [MenuItem("Platformer/Create Player Animator Controller")]
    public static void CreateFunctionalAnimatorController()
    {
        string folderPath = "Assets/Thinklib/Platformer/Player/Animations";

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/Thinklib/Platformer/Movement", "Animations");
        }

        string controllerPath = $"{folderPath}/PlayerAnimatorController.controller";
        var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

        // Parâmetros
        controller.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsJumping", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsFalling", AnimatorControllerParameterType.Bool);
        controller.AddParameter("MoveSpeed", AnimatorControllerParameterType.Float);
        controller.AddParameter("IsShooting", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsHurt", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("IsDead", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsAttacking", AnimatorControllerParameterType.Trigger); // novo parâmetro

        var root = controller.layers[0].stateMachine;

        // Estados
        var idle = root.AddState("Idle");
        var walk = root.AddState("Walking");
        var jump = root.AddState("Jumping");
        var fall = root.AddState("Falling");
        var shoot = root.AddState("ShootProjectile");
        var hurt = root.AddState("Hurt");
        var dead = root.AddState("Dead");
        var meleeAttack = root.AddState("MeleeAttack"); // novo estado

        root.defaultState = idle;

        // Movimentação
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

        // Shoot
        ConfigureShootProjectileTransitions(idle, shoot);
        ConfigureShootProjectileTransitions(walk, shoot);
        ConfigureShootProjectileTransitions(jump, shoot);
        ConfigureShootProjectileTransitions(fall, shoot);

        // Hurt
        var anyHurt = root.AddAnyStateTransition(hurt);
        anyHurt.AddCondition(AnimatorConditionMode.If, 0, "IsHurt");
        anyHurt.AddCondition(AnimatorConditionMode.IfNot, 0, "IsDead");
        anyHurt.hasExitTime = false;

        var hurtToIdle = hurt.AddTransition(idle);
        hurtToIdle.hasExitTime = true;
        hurtToIdle.exitTime = 1f;
        hurtToIdle.duration = 0.1f;

        // Dead
        var anyDead = root.AddAnyStateTransition(dead);
        anyDead.AddCondition(AnimatorConditionMode.If, 0, "IsDead");
        anyDead.hasExitTime = false;

        // MeleeAttack (de qualquer estado, exceto se morto)
        var anyAttack = root.AddAnyStateTransition(meleeAttack);
        anyAttack.AddCondition(AnimatorConditionMode.If, 0, "IsAttacking");
        anyAttack.AddCondition(AnimatorConditionMode.IfNot, 0, "IsDead");
        anyAttack.hasExitTime = false;

        var meleeToIdle = meleeAttack.AddTransition(idle);
        meleeToIdle.hasExitTime = true;
        meleeToIdle.exitTime = 1f;
        meleeToIdle.duration = 0.1f;

        Debug.Log("✅ Player Animator Controller criado com sucesso em: " + controllerPath);
    }

    private static void ConfigureShootProjectileTransitions(AnimatorState fromState, AnimatorState shootProjectileState)
    {
        var toShoot = fromState.AddTransition(shootProjectileState);
        toShoot.AddCondition(AnimatorConditionMode.If, 1, "IsShooting");
        toShoot.hasExitTime = false;
        toShoot.duration = 0f;

        var fromShoot = shootProjectileState.AddTransition(fromState);
        fromShoot.AddCondition(AnimatorConditionMode.IfNot, 0, "IsShooting");
        fromShoot.hasExitTime = true;
        fromShoot.exitTime = 1f;
        fromShoot.duration = 0.1f;
    }
}
