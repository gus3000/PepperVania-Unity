using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Animation
{
    [Serializable]
    public class CurveAnimationStep : AnimationStep
    {
        [SerializeField] private BezierCurve curve;

        public override void Init()
        {
            base.Init();
            
        }

        public Quaternion GetRotation(float timeSinceStart)
        {
            var delta = 0.001f;
            var percent = timeSinceStart / duration;
            var currentPoint = curve.GetPointAt(percent);
            var lastPoint = curve.GetPointAt(percent - delta);
            return Quaternion.LookRotation(currentPoint - lastPoint);
        }

        public override Vector3 GetPoint(float timeSinceStart)
        {
            return curve.GetPointAt(timeSinceStart);
        }
    }
}