using UnityEngine;

namespace FPController
{
    /// <summary>
    /// Preset for First Person Controller.
    /// </summary>
    [CreateAssetMenu(fileName = "FirsPersonPreset", menuName = "FP Preset", order = 1500)]
    public class FirstPersonPreset : ScriptableObject
    {
        /// <summary>
        /// Name, set to game object on reset.
        /// </summary>
        public string Name = "Player";

        /// <summary>
        /// Tag, set to game object on reset.
        /// TODO: Check if Tag is found in unity Tags.
        /// </summary>
        public string Tag = "Player";

        /// <summary>
        /// Charachters target walk speed.
        /// </summary>
        public float WalkSpeed = 5;

        /// <summary>
        /// Charachters target run speed (walk * run multiplier).
        /// </summary>
        public float RunMultiplier = 1.3f;

        /// <summary>
        /// Charachters target crouching speed (walk * crouch multiplier).
        /// </summary>
        public float CrouchMultiplier = 0.6f;

        /// <summary>
        /// Force added to make rigidbody jump.
        /// </summary>
        public float JumpForce = 5f;

        /// <summary>
        /// Charachters (Capsule Colliders) radius.
        /// </summary>
        public float Radius = 0.35f;

        /// <summary>
        /// Charachters (Capsule Colliders) height.
        /// Used for standing after crouching.
        /// </summary>
        public float Height = 1.75f;

        /// <summary>
        /// Maximum angle for slope.
        /// <see cref="FirstPersonController.UpdateFriction(Vector3)"/>
        /// </summary>
        public float MaxSlopeAngle = 45f;

        /// <summary>
        /// Physics Material Max Friction.
        /// Used when on slopes under max angle.
        /// </summary>
        public float MaxFriction = 20f;
    }
}