using UnityEngine;

namespace RanMobile.Character
{
    /// <summary>
    /// Lightweight procedural locomotion for the imported character.
    /// No animation clips or Animator Controller are required.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RanCharacterProceduralAnimator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RanCharacterController movement;
        [SerializeField] private Transform hips;
        [SerializeField] private Transform spine;
        [SerializeField] private Transform spine1;
        [SerializeField] private Transform leftUpperLeg;
        [SerializeField] private Transform leftLowerLeg;
        [SerializeField] private Transform leftFoot;
        [SerializeField] private Transform rightUpperLeg;
        [SerializeField] private Transform rightLowerLeg;
        [SerializeField] private Transform rightFoot;
        [SerializeField] private Transform leftUpperArm;
        [SerializeField] private Transform leftLowerArm;
        [SerializeField] private Transform rightUpperArm;
        [SerializeField] private Transform rightLowerArm;

        [Header("Walk")]
        [SerializeField, Min(0f)] private float walkCycleSpeed = 7.5f;
        [SerializeField, Min(0f)] private float legSwing = 28f;
        [SerializeField, Min(0f)] private float lowerLegSwing = 18f;
        [SerializeField, Min(0f)] private float armSwing = 24f;
        [SerializeField, Min(0f)] private float lowerArmSwing = 12f;
        [SerializeField, Min(0f)] private float hipBob = 0.035f;
        [SerializeField, Min(0f)] private float bodySway = 3f;

        [Header("Run Multiplier")]
        [SerializeField, Min(0f)] private float runCycleMultiplier = 1.3f;
        [SerializeField, Min(0f)] private float runSwingMultiplier = 1.25f;

        private Quaternion hipsRotation;
        private Quaternion spineRotation;
        private Quaternion spine1Rotation;
        private Quaternion leftUpperLegRotation;
        private Quaternion leftLowerLegRotation;
        private Quaternion leftFootRotation;
        private Quaternion rightUpperLegRotation;
        private Quaternion rightLowerLegRotation;
        private Quaternion rightFootRotation;
        private Quaternion leftUpperArmRotation;
        private Quaternion leftLowerArmRotation;
        private Quaternion rightUpperArmRotation;
        private Quaternion rightLowerArmRotation;
        private float cycle;
        private float hipsBaseY;

        private void Awake()
        {
            if (movement == null)
                movement = GetComponent<RanCharacterController>();

            AutoFindBones();
            CacheBasePose();
        }

        private void LateUpdate()
        {
            if (movement == null)
                return;

            float speed = movement.CurrentHorizontalSpeed;
            float normalizedSpeed = movement.RunSpeed > 0.001f
                ? Mathf.Clamp01(speed / movement.RunSpeed)
                : 0f;

            if (speed < 0.05f)
            {
                RestorePose();
                return;
            }

            bool running = movement.IsRunning;
            float cycleSpeed = walkCycleSpeed * (running ? runCycleMultiplier : 1f);
            float swingMultiplier = running ? runSwingMultiplier : 1f;

            cycle += Time.deltaTime * cycleSpeed * Mathf.Lerp(0.75f, 1.15f, normalizedSpeed);

            float phase = cycle;
            float left = Mathf.Sin(phase);
            float right = Mathf.Sin(phase + Mathf.PI);

            ApplyLeg(leftUpperLeg, leftUpperLegRotation, left * legSwing * swingMultiplier);
            ApplyLeg(rightUpperLeg, rightUpperLegRotation, right * legSwing * swingMultiplier);

            ApplyLeg(leftLowerLeg, leftLowerLegRotation, Mathf.Max(0f, -left) * lowerLegSwing * swingMultiplier);
            ApplyLeg(rightLowerLeg, rightLowerLegRotation, Mathf.Max(0f, -right) * lowerLegSwing * swingMultiplier);

            ApplyFoot(leftFoot, leftFootRotation, Mathf.Max(0f, -left) * -8f * swingMultiplier);
            ApplyFoot(rightFoot, rightFootRotation, Mathf.Max(0f, -right) * -8f * swingMultiplier);

            ApplyArm(leftUpperArm, leftUpperArmRotation, right * armSwing * swingMultiplier);
            ApplyArm(rightUpperArm, rightUpperArmRotation, left * armSwing * swingMultiplier);

            ApplyArm(leftLowerArm, leftLowerArmRotation, Mathf.Max(0f, right) * lowerArmSwing * swingMultiplier);
            ApplyArm(rightLowerArm, rightLowerArmRotation, Mathf.Max(0f, left) * lowerArmSwing * swingMultiplier);

            if (hips != null)
            {
                Vector3 localPosition = hips.localPosition;
                localPosition.y = hipsBaseY + Mathf.Abs(Mathf.Sin(phase * 2f)) * hipBob * normalizedSpeed;
                hips.localPosition = localPosition;
                hips.localRotation = hipsRotation * Quaternion.Euler(0f, 0f, left * bodySway * normalizedSpeed);
            }

            if (spine != null)
                spine.localRotation = spineRotation * Quaternion.Euler(0f, 0f, -left * bodySway * 0.45f * normalizedSpeed);

            if (spine1 != null)
                spine1.localRotation = spine1Rotation * Quaternion.Euler(0f, 0f, -left * bodySway * 0.3f * normalizedSpeed);
        }

        private void ApplyLeg(Transform bone, Quaternion baseRotation, float xDegrees)
        {
            if (bone != null)
                bone.localRotation = baseRotation * Quaternion.Euler(xDegrees, 0f, 0f);
        }

        private void ApplyFoot(Transform bone, Quaternion baseRotation, float xDegrees)
        {
            if (bone != null)
                bone.localRotation = baseRotation * Quaternion.Euler(xDegrees, 0f, 0f);
        }

        private void ApplyArm(Transform bone, Quaternion baseRotation, float xDegrees)
        {
            if (bone != null)
                bone.localRotation = baseRotation * Quaternion.Euler(xDegrees, 0f, 0f);
        }

        private void RestorePose()
        {
            if (hips != null)
            {
                Vector3 localPosition = hips.localPosition;
                localPosition.y = hipsBaseY;
                hips.localPosition = localPosition;
                hips.localRotation = hipsRotation;
            }

            SetRotation(spine, spineRotation);
            SetRotation(spine1, spine1Rotation);
            SetRotation(leftUpperLeg, leftUpperLegRotation);
            SetRotation(leftLowerLeg, leftLowerLegRotation);
            SetRotation(leftFoot, leftFootRotation);
            SetRotation(rightUpperLeg, rightUpperLegRotation);
            SetRotation(rightLowerLeg, rightLowerLegRotation);
            SetRotation(rightFoot, rightFootRotation);
            SetRotation(leftUpperArm, leftUpperArmRotation);
            SetRotation(leftLowerArm, leftLowerArmRotation);
            SetRotation(rightUpperArm, rightUpperArmRotation);
            SetRotation(rightLowerArm, rightLowerArmRotation);
        }

        private static void SetRotation(Transform bone, Quaternion rotation)
        {
            if (bone != null)
                bone.localRotation = rotation;
        }

        private void CacheBasePose()
        {
            hipsRotation = GetRotation(hips);
            spineRotation = GetRotation(spine);
            spine1Rotation = GetRotation(spine1);
            leftUpperLegRotation = GetRotation(leftUpperLeg);
            leftLowerLegRotation = GetRotation(leftLowerLeg);
            leftFootRotation = GetRotation(leftFoot);
            rightUpperLegRotation = GetRotation(rightUpperLeg);
            rightLowerLegRotation = GetRotation(rightLowerLeg);
            rightFootRotation = GetRotation(rightFoot);
            leftUpperArmRotation = GetRotation(leftUpperArm);
            leftLowerArmRotation = GetRotation(leftLowerArm);
            rightUpperArmRotation = GetRotation(rightUpperArm);
            rightLowerArmRotation = GetRotation(rightLowerArm);
            hipsBaseY = hips != null ? hips.localPosition.y : 0f;
        }

        private static Quaternion GetRotation(Transform bone)
        {
            return bone != null ? bone.localRotation : Quaternion.identity;
        }

        private void AutoFindBones()
        {
            Transform armature = FindBone(transform, "Armature");
            Transform root = armature != null ? armature : transform;

            hips = hips != null ? hips : FindBone(root, "mixamorig:Hips");
            spine = spine != null ? spine : FindBone(root, "mixamorig:Spine");
            spine1 = spine1 != null ? spine1 : FindBone(root, "mixamorig:Spine1");

            leftUpperLeg = leftUpperLeg != null ? leftUpperLeg : FindBone(root, "mixamorig:LeftUpLeg");
            leftLowerLeg = leftLowerLeg != null ? leftLowerLeg : FindBone(root, "mixamorig:LeftLeg");
            leftFoot = leftFoot != null ? leftFoot : FindBone(root, "mixamorig:LeftFoot");

            rightUpperLeg = rightUpperLeg != null ? rightUpperLeg : FindBone(root, "mixamorig:RightUpLeg");
            rightLowerLeg = rightLowerLeg != null ? rightLowerLeg : FindBone(root, "mixamorig:RightLeg");
            rightFoot = rightFoot != null ? rightFoot : FindBone(root, "mixamorig:RightFoot");

            leftUpperArm = leftUpperArm != null ? leftUpperArm : FindBone(root, "mixamorig:LeftArm");
            leftLowerArm = leftLowerArm != null ? leftLowerArm : FindBone(root, "mixamorig:LeftForeArm");
            rightUpperArm = rightUpperArm != null ? rightUpperArm : FindBone(root, "mixamorig:RightArm");
            rightLowerArm = rightLowerArm != null ? rightLowerArm : FindBone(root, "mixamorig:RightForeArm");
        }

        private static Transform FindBone(Transform root, string boneName)
        {
            if (root == null)
                return null;

            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == boneName)
                    return child;
            }

            return null;
        }

        private void OnValidate()
        {
            walkCycleSpeed = Mathf.Max(0f, walkCycleSpeed);
            legSwing = Mathf.Max(0f, legSwing);
            lowerLegSwing = Mathf.Max(0f, lowerLegSwing);
            armSwing = Mathf.Max(0f, armSwing);
            lowerArmSwing = Mathf.Max(0f, lowerArmSwing);
            hipBob = Mathf.Max(0f, hipBob);
            bodySway = Mathf.Max(0f, bodySway);
            runCycleMultiplier = Mathf.Max(0f, runCycleMultiplier);
            runSwingMultiplier = Mathf.Max(0f, runSwingMultiplier);
        }
    }
}
