using System;
using System.Linq;
using UnityEngine;

namespace Animation
{
    public class AnimationSequence : MonoBehaviour
    {
        [SerializeField] private AnimationStep[] _animationSteps;
        [SerializeField] private AnimationStep _stepDebug;

        public string pouet;

        // [SerializeField] private Vector3[] sequence;
        // private BezierCurve _curve;

        private int _currentStep;

        private void Start()
        {
            // _curve = GetComponent<BezierCurve>();
            Debug.Log($"Total duration : {TotalDuration}");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.black;
        }

        public Vector3 GetPoint(float timeSinceStart)
        {
            if (IsOver(timeSinceStart))
                return Vector3.zero;

            var (step, timeSinceAnimStart) = GetCurrentAnimationStep(timeSinceStart);
            return step.GetPoint(timeSinceAnimStart);
            // return _curve.GetPointAt(timeSinceStart / duration);
        }

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

        public (AnimationStep, float) GetCurrentAnimationStep(float timeSinceStart)
        {
            float acc = 0f;
            foreach (var animationStep in _animationSteps)
            {
                acc += animationStep.Duration;
                if (timeSinceStart < acc)
                    return (animationStep, timeSinceStart - acc);
            }

            return (null, timeSinceStart);
        }


        public float TotalDuration => _animationSteps.Select(a => a.Duration).Sum();

        public bool IsOver(float timeSinceStart) => timeSinceStart > TotalDuration;
    }
}