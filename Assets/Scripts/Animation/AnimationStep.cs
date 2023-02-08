using UnityEngine;

namespace Animation
{
    public abstract class AnimationStep
    {
        protected float duration = 1f;

        public float Duration => duration;


        public virtual void Init()
        {
        }
    }
}