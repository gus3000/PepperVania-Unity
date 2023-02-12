using UnityEngine;
using UnityEngine.Serialization;

namespace Physics
{
    
    // inspired by https://gustavcorpas.medium.com/building-a-customizable-jump-in-unity-using-animation-curves-a168a618428d
    [CreateAssetMenu(fileName = "Jump", menuName = "Player/Movement/Jump")]
    public class JumpPhysics : ScriptableObject
    {
        [SerializeField] private float initalJumpForce;
        [SerializeField] private float jumpDuration;
        [SerializeField] private AnimationCurve gravityRise;
        [SerializeField] private AnimationCurve gravityFall;
        [SerializeField] private float gravityOnRelease;

        public float getGravityDelta(float currentVelocity, bool holdingJump, float timeSinceStart)
        {
            if (currentVelocity > 0 && holdingJump)
                return gravityRise.Evaluate(timeSinceStart / jumpDuration);
            if (currentVelocity > 0)
                return -gravityOnRelease;
            return gravityFall.Evaluate(timeSinceStart / jumpDuration);
        }
    }
}