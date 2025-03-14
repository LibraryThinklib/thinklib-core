using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class AnimatorControllerCreator : MonoBehaviour
{
    [MenuItem("Tools/Create Functional Animator Controller")]
    public static void CreateFunctionalAnimatorController()
    {
        string folderPath = "Assets/Thinklib/Platformer/Movement/Animations";

        // Garantir que o diretório exista
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/Thinklib/Platformer/Movement", "Animations");
        }

        string controllerPath = $"{folderPath}/PlayerAnimatorController.controller";
        var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

        // Adicionando parâmetros ao Animator
        controller.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsJumping", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsFalling", AnimatorControllerParameterType.Bool);
        controller.AddParameter("MoveSpeed", AnimatorControllerParameterType.Float);
        controller.AddParameter("IsShooting", AnimatorControllerParameterType.Bool); // Parâmetro para disparo

        var rootStateMachine = controller.layers[0].stateMachine;

        // Estados do Animator
        var idleState = rootStateMachine.AddState("Idle");
        var walkState = rootStateMachine.AddState("Walking");
        var jumpState = rootStateMachine.AddState("Jumping");
        var fallState = rootStateMachine.AddState("Falling");
        var shootProjectileState = rootStateMachine.AddState("ShootProjectile");

        rootStateMachine.defaultState = idleState;

        // Transições entre estados principais
        var idleToWalk = idleState.AddTransition(walkState);
        idleToWalk.AddCondition(AnimatorConditionMode.If, 0, "IsMoving");
        idleToWalk.hasExitTime = false;

        var walkToIdle = walkState.AddTransition(idleState);
        walkToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsMoving");
        walkToIdle.hasExitTime = false;

        var walkToJump = walkState.AddTransition(jumpState);
        walkToJump.AddCondition(AnimatorConditionMode.If, 0, "IsJumping");
        walkToJump.hasExitTime = false;

        var idleToJump = idleState.AddTransition(jumpState);
        idleToJump.AddCondition(AnimatorConditionMode.If, 0, "IsJumping");
        idleToJump.hasExitTime = false;

        var jumpToFall = jumpState.AddTransition(fallState);
        jumpToFall.AddCondition(AnimatorConditionMode.If, 0, "IsFalling");
        jumpToFall.hasExitTime = false;

        var fallToIdle = fallState.AddTransition(idleState);
        fallToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsFalling");
        fallToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsJumping");
        fallToIdle.hasExitTime = false;

        // Configuração de transições para ShootProjectile a partir de cada estado principal
        ConfigureShootProjectileTransitions(idleState, shootProjectileState);
        ConfigureShootProjectileTransitions(walkState, shootProjectileState);
        ConfigureShootProjectileTransitions(jumpState, shootProjectileState);
        ConfigureShootProjectileTransitions(fallState, shootProjectileState);

        Debug.Log("Animator Controller criado com sucesso em: " + controllerPath);
    }

    private static void ConfigureShootProjectileTransitions(AnimatorState fromState, AnimatorState shootProjectileState)
    {
        // Transição do estado principal para o estado de disparo
        var toShootTransition = fromState.AddTransition(shootProjectileState);
        toShootTransition.AddCondition(AnimatorConditionMode.If, 1, "IsShooting");
        toShootTransition.hasExitTime = false;
        toShootTransition.duration = 0;

        // Transição do estado de disparo de volta ao estado principal
        var fromShootTransition = shootProjectileState.AddTransition(fromState);
        fromShootTransition.AddCondition(AnimatorConditionMode.IfNot, 0, "IsShooting");
        fromShootTransition.hasExitTime = true;
        fromShootTransition.exitTime = 1.0f; // Tempo necessário para concluir a animação
    }
}
