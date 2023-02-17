    using System;
    using System.Collections;
    using System.Linq;
    using Animation;
    using UnityEngine;

    public class PlayerAnimationController : MonoBehaviour
    {
        private const string DashAnimationName = "PepperArmature|Dash";

        [SerializeField] private Transform headTrackerFocus;
        [SerializeField] private bool canMove = true;

        private PlayerController _playerController;
        private Animator _animator;
        private Vector3 baseHeadTrackerPosition;
        private float _baseDashDuration;

        private static readonly int Sleeping = Animator.StringToHash("sleeping");
        private static readonly int Rest = Animator.StringToHash("rest");
        private static readonly int DashTriggerHash = Animator.StringToHash("dash");
        private static readonly int DashSpeedHash = Animator.StringToHash("dashSpeed");

        public bool CanMove => canMove;
        public bool IsInAnimatedMovement { get; private set; }

        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
            _animator = GetComponentInChildren<Animator>();
            baseHeadTrackerPosition = headTrackerFocus.transform.localPosition;
            IsInAnimatedMovement = false;
            
            var dashAnimation = _animator.runtimeAnimatorController.animationClips.First(clip => clip.name == DashAnimationName);
            _baseDashDuration = dashAnimation.length;
        }

        private void Update()
        {
            var target = _playerController.InteractionTarget;
            if (target != null)
            {
                headTrackerFocus.transform.position = target.FocusPoint;
            }
            else
            {
                headTrackerFocus.transform.localPosition = baseHeadTrackerPosition;
            }
        }

        public IEnumerator FollowSequence(AnimationSequence sequence)
        {
            Debug.Log("Starting sequence");
            var startTime = Time.time;
            var step = sequence.First();
            while (step != null)
            {
                yield return FollowAnimationStep(step);
                step = sequence.Next();
            }
            // while (!sequence.IsOver(timeSinceStart))
            // {
            //     timeSinceStart = Time.time - startTime;
            //     transform.position = sequence.GetPoint(timeSinceStart);
            //     // transform.rotation = sequence.GetRotation(timeSinceStart);
            //     yield return null;
            // }
            Debug.Log("End of sequence");
        }

        public IEnumerator FollowAnimationStep(AnimationStep step)
        {
            var timeSinceStart = 0f;
            Debug.Log($"starting step {step}");
            yield return new WaitForSeconds(step.Duration);
        }

        public void TriggerDash(float dashDurationMultiplier = 1)
        {
            _animator.SetTrigger(DashTriggerHash);
            _animator.SetFloat(DashSpeedHash, 1 / dashDurationMultiplier);
        }

        public float GetDashDuration()
        {
            return _baseDashDuration * _animator.GetFloat(DashSpeedHash);
        }
    }
