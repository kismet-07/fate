#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace RanMobile.EditorTools
{
    public static class RanLocomotionAnimatorSetup
    {
        private const string AnimationFolder = "Assets/Animations/Locomotion";
        private const string ControllerPath = "Assets/Animations/PlayerLocomotion.controller";

        [MenuItem("Ran/Character/Build Locomotion Animator")]
        public static void Build()
        {
            AnimationClip idle = FindClip("idle");
            AnimationClip walk = FindClip("walk");
            AnimationClip run = FindClip("run");

            if (idle == null || walk == null || run == null)
            {
                Debug.LogError(
                    "Ran locomotion setup requires three AnimationClips in " +
                    AnimationFolder + ": Idle, Walk, and Run. " +
                    $"Found Idle={Format(idle)}, Walk={Format(walk)}, Run={Format(run)}.");
                return;
            }

            EnsureLoop(idle);
            EnsureLoop(walk);
            EnsureLoop(run);

            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            if (controller != null)
                AssetDatabase.DeleteAsset(ControllerPath);

            controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            controller.AddParameter("Moving", AnimatorControllerParameterType.Bool);
            controller.AddParameter("Sprint", AnimatorControllerParameterType.Bool);

            AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
            stateMachine.name = "Locomotion";

            AnimatorState locomotionState = stateMachine.AddState("Locomotion");
            locomotionState.motion = CreateBlendTree(controller, idle, walk, run);
            stateMachine.defaultState = locomotionState;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = controller;
            Debug.Log("Ran locomotion Animator created: " + ControllerPath);
        }

        [MenuItem("Ran/Character/Assign Locomotion Animator To Selected")]
        public static void AssignToSelected()
        {
            if (Selection.activeGameObject == null)
            {
                Debug.LogError("Select the character root GameObject first.");
                return;
            }

            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            if (controller == null)
            {
                Debug.LogError("PlayerLocomotion.controller does not exist. Run Ran > Character > Build Locomotion Animator first.");
                return;
            }

            Animator animator = Selection.activeGameObject.GetComponent<Animator>();
            if (animator == null)
                animator = Undo.AddComponent<Animator>(Selection.activeGameObject);

            Undo.RecordObject(animator, "Assign Ran locomotion Animator");
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;
            EditorUtility.SetDirty(animator);

            Debug.Log("Assigned PlayerLocomotion.controller to " + Selection.activeGameObject.name);
        }

        private static BlendTree CreateBlendTree(
            AnimatorController controller,
            AnimationClip idle,
            AnimationClip walk,
            AnimationClip run)
        {
            BlendTree tree = new BlendTree
            {
                name = "Locomotion Blend Tree",
                blendType = BlendTreeType.Simple1D,
                blendParameter = "Speed",
                useAutomaticThresholds = false
            };

            AssetDatabase.AddObjectToAsset(tree, controller);
            tree.AddChild(idle, 0f);
            tree.AddChild(walk, 2.2f);
            tree.AddChild(run, 4.8f);
            return tree;
        }

        private static AnimationClip FindClip(string keyword)
        {
            string[] guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { AnimationFolder });

            return guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .SelectMany(path => AssetDatabase.LoadAllAssetsAtPath(path)
                    .OfType<AnimationClip>()
                    .Select(clip => new { clip, path }))
                .Where(x => !x.clip.name.StartsWith("__preview__", StringComparison.OrdinalIgnoreCase))
                .Where(x => x.clip.name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                .Select(x => x.clip)
                .FirstOrDefault();
        }

        private static void EnsureLoop(AnimationClip clip)
        {
            if (clip == null)
                return;

            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
            if (!settings.loopTime)
            {
                settings.loopTime = true;
                AnimationUtility.SetAnimationClipSettings(clip, settings);
                EditorUtility.SetDirty(clip);
            }
        }

        private static string Format(AnimationClip clip)
        {
            return clip == null ? "MISSING" : clip.name;
        }
    }
}
#endif
