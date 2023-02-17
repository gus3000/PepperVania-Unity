using System;
using UnityEngine;

namespace Animation
{
    [Serializable]
    public class AnimationStep
    {
        [SerializeField] protected float duration = 1f;
        [SerializeField] protected string pouet = "ha";

        public float Duration => duration;


        public virtual void Init()
        {
        }

        // public abstract Vector3 GetPoint(float timeSinceStart);
        public virtual Vector3 GetPoint(float timeSinceStart) => Vector3.zero;
    }
}