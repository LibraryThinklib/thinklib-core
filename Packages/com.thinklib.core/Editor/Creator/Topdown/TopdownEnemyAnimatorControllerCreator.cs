using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class TopdownEnemyAnimatorControllerCreator
{
    private const string Root = "Assets/Thinklib/Topdown/Enemy/Animations";

    [MenuItem("Topdown/Create Enemy Animator Controller")]
    public static void CreateTopdownEnemyAnimatorController()
    {
        EnsureFolderPath(Root);

        var controllerPath = AssetDatabase.GenerateUniqueAssetPath($"{Root}/TopdownEnemyAnimatorController.controller");
        var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

        // Parameters
        controller.AddParameter("Horizontal", AnimatorControllerParameterType.Float);
        controller.AddParameter("Vertical", AnimatorControllerParameterType.Float);
        controller.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsShooting", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsAttacking", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("IsHurt", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("IsDead", AnimatorControllerParameterType.Bool);

        var sm = controller.layers[0].stateMachine;

        // States
        var idle = sm.AddState("Idle");
        var walk = sm.AddState("Walking");
        var shoot = sm.AddState("ShootProjectile");
        var melee = sm.AddState("MeleeAttack");
        var hurt = sm.AddState("Hurt");
        var dead = sm.AddState("Dead");

        sm.defaultState = idle;

        // 2D directional blend trees (8 direções) com clips placeholders
        idle.motion = CreateDirectionalBlendTree(controller, "Idle");
        walk.motion = CreateDirectionalBlendTree(controller, "Walk");
        shoot.motion = CreateDirectionalBlendTree(controller, "Shoot");
        melee.motion = CreateDirectionalBlendTree(controller, "Melee");
        hurt.motion = CreateDirectionalBlendTree(controller, "Hurt");
        dead.motion = CreateDirectionalBlendTree(controller, "Dead");

        // Idle <-> Walking
        var idleToWalk = idle.AddTransition(walk);
        idleToWalk.AddCondition(AnimatorConditionMode.If, 0f, "IsMoving");
        idleToWalk.hasExitTime = false;

        var walkToIdle = walk.AddTransition(idle);
        walkToIdle.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsMoving");
        walkToIdle.hasExitTime = false;

        // Shoot (Bool)
        ConfigureShootTransitions(idle, shoot);
        ConfigureShootTransitions(walk, shoot);

        // Melee (Trigger)
        var anyAttack = sm.AddAnyStateTransition(melee);
        anyAttack.AddCondition(AnimatorConditionMode.If, 0f, "IsAttacking");
        anyAttack.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsDead");
        anyAttack.hasExitTime = false;

        var meleeToIdle = melee.AddTransition(idle);
        meleeToIdle.hasExitTime = true;
        meleeToIdle.exitTime = 1f;

        // Hurt (Trigger)
        var anyHurt = sm.AddAnyStateTransition(hurt);
        anyHurt.AddCondition(AnimatorConditionMode.If, 0f, "IsHurt");
        anyHurt.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsDead");
        anyHurt.hasExitTime = false;

        var hurtToIdle = hurt.AddTransition(idle);
        hurtToIdle.hasExitTime = true;
        hurtToIdle.exitTime = 1f;

        // Dead (Bool)
        var anyDead = sm.AddAnyStateTransition(dead);
        anyDead.AddCondition(AnimatorConditionMode.If, 0f, "IsDead");
        anyDead.hasExitTime = false;

        Debug.Log($"✅ Topdown Enemy Animator Controller criado em: {controllerPath}");
    }

    private static BlendTree CreateDirectionalBlendTree(AnimatorController controller, string baseName)
    {
        var bt = new BlendTree
        {
            name = $"{baseName}BlendTree",
            blendType = BlendTreeType.SimpleDirectional2D,
            blendParameter = "Horizontal",
            blendParameterY = "Vertical"
        };

        // Subasset para o controller (deixa tudo dentro do mesmo .controller)
        AssetDatabase.AddObjectToAsset(bt, controller);

        // 8 direções com clips placeholders
        bt.AddChild(PlaceholderClip(controller, $"{baseName}_Up"), new Vector2(0, 1));
        bt.AddChild(PlaceholderClip(controller, $"{baseName}_Down"), new Vector2(0, -1));
        bt.AddChild(PlaceholderClip(controller, $"{baseName}_Right"), new Vector2(1, 0));
        bt.AddChild(PlaceholderClip(controller, $"{baseName}_Left"), new Vector2(-1, 0));
        bt.AddChild(PlaceholderClip(controller, $"{baseName}_UpRight"), new Vector2(1, 1));
        bt.AddChild(PlaceholderClip(controller, $"{baseName}_UpLeft"), new Vector2(-1, 1));
        bt.AddChild(PlaceholderClip(controller, $"{baseName}_DownRight"), new Vector2(1, -1));
        bt.AddChild(PlaceholderClip(controller, $"{baseName}_DownLeft"), new Vector2(-1, -1));

        return bt;
    }

    private static Motion PlaceholderClip(AnimatorController controller, string clipName)
    {
        var clip = new AnimationClip { name = clipName };
        AssetDatabase.AddObjectToAsset(clip, controller);
        return clip;
    }

    private static void ConfigureShootTransitions(AnimatorState from, AnimatorState shoot)
    {
        var toShoot = from.AddTransition(shoot);
        toShoot.AddCondition(AnimatorConditionMode.If, 0f, "IsShooting"); // Bool: threshold ignorado
        toShoot.hasExitTime = false;
        toShoot.duration = 0f;

        var back = shoot.AddTransition(from);
        back.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsShooting");
        back.hasExitTime = true;
        back.exitTime = 1f;
        back.duration = 0.1f;
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
