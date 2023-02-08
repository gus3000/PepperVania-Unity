using System;
using System.Linq;
using UnityEngine;

namespace Animation
{
    public class AnimationSequence : MonoBehaviour
    {
        [SerializeField] private float duration = 1f;

        // [SerializeField] private Vector3[] sequence;
        private BezierCurve _curve;
        private AnimationStep[] _animationSteps;

        private int _currentStep;
        private void Start()
        {
            _curve = GetComponent<BezierCurve>();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.black;
        }

        public Vector3 GetPoint(float timeSinceStart) => _curve.GetPointAt(timeSinceStart / duration);

        public AnimationStep First()
        {
            if (_animationSteps.Length == 0)
                return null;

            _currentStep = 0;
            return _animationSteps[_currentStep];
        }

        public AnimationStep Next()
        {
            _currentStep++;
            if (_currentStep >= _animationSteps.Length)
                return null;
            
            return _animationSteps[_currentStep];
        }

        public AnimationStep GetCurrentAnimationStep(float timeSinceStart)
        {
            float acc = 0f;
            foreach (var animationStep in _animationSteps)
            {
                acc += animationStep.Duration;
                if (timeSinceStart < acc)
                    return animationStep;
            }

            return null;
        }

        public Quaternion GetRotation(float timeSinceStart)
        {
            var delta = 0.001f;
            var percent = timeSinceStart / duration;
            var currentPoint = _curve.GetPointAt(percent);
            var lastPoint = _curve.GetPointAt(percent - delta);
            return Quaternion.LookRotation(currentPoint - lastPoint);
        }

        public bool IsOver(float timeSinceStart) => timeSinceStart > duration;
    }
}