using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class EnemyAnimatorControllerCreator : MonoBehaviour
{
    [MenuItem("Tools/Create Enemy Animator Controller")]
    public static void CreateEnemyAnimatorController()
    {
        string folderPath = "Assets/Thinklib/Platformer/Enemy/Animations";

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/Thinklib/Platformer/Enemy", "Animations");
        }

        string controllerPath = $"{folderPath}/EnemyAnimatorController.controller";
        var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

        controller.AddParameter("IsWalking", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsAttacking", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsShooting", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsDead", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsHurt", AnimatorControllerParameterType.Trigger);

        var root = controller.layers[0].stateMachine;

        var idle = root.AddState("Idle");
        var walk = root.AddState("Walking");
        var attack = root.AddState("Attacking");
        var shoot = root.AddState("ShootProjectile");
        var hurt = root.AddState("Hurt");
        var dead = root.AddState("Dead");

        root.defaultState = idle;

        var idleToWalk = idle.AddTransition(walk);
        idleToWalk.AddCondition(AnimatorConditionMode.If, 0, "IsWalking");
        idleToWalk.hasExitTime = false;

        var walkToIdle = walk.AddTransition(idle);
        walkToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsWalking");
        walkToIdle.hasExitTime = false;

        var anyAttack = root.AddAnyStateTransition(attack);
        anyAttack.AddCondition(AnimatorConditionMode.If, 0, "IsAttacking");
        anyAttack.hasExitTime = false;

        var attackToIdle = attack.AddTransition(idle);
        attackToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsAttacking");
        attackToIdle.hasExitTime = true;
        attackToIdle.exitTime = 1f;
        attackToIdle.duration = 0.1f;

        var anyShoot = root.AddAnyStateTransition(shoot);
        anyShoot.AddCondition(AnimatorConditionMode.If, 0, "IsShooting");
        anyShoot.hasExitTime = false;

        var shootToIdle = shoot.AddTransition(idle);
        shootToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsShooting");
        shootToIdle.hasExitTime = true;
        shootToIdle.exitTime = 1f;
        shootToIdle.duration = 0.1f;

        var anyHurt = root.AddAnyStateTransition(hurt);
        anyHurt.AddCondition(AnimatorConditionMode.If, 0, "IsHurt");
        anyHurt.hasExitTime = false;

        var hurtToIdle = hurt.AddTransition(idle);
        hurtToIdle.hasExitTime = true;
        hurtToIdle.exitTime = 1f;
        hurtToIdle.duration = 0.1f;

        var anyDead = root.AddAnyStateTransition(dead);
        anyDead.AddCondition(AnimatorConditionMode.If, 0, "IsDead");
        anyDead.hasExitTime = false;

        Debug.Log("Enemy Animator Controller criado com sucesso em: " + controllerPath);
    }
}
