    using System;
    using System.Collections;
    using Animation;
    using UnityEngine;

    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private Transform headTrackerFocus;
        [SerializeField] private bool canMove = true;

        private PlayerController _playerController;
        private Animator _animator;
        private Vector3 baseHeadTrackerPosition;

        private static readonly int Sleeping = Animator.StringToHash("sleeping");
        private static readonly int Rest = Animator.StringToHash("rest");
        public bool CanMove => canMove;
        public bool IsInAnimatedMovement { get; private set; }

        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
            _animator = GetComponentInChildren<Animator>();
            baseHeadTrackerPosition = headTrackerFocus.transform.localPosition;
            IsInAnimatedMovement = false;
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
            var timeSinceStart = 0f;
            while (!sequence.IsOver(timeSinceStart))
            {
                timeSinceStart = Time.time - startTime;
                transform.position = sequence.GetPoint(timeSinceStart);
                // transform.rotation = sequence.GetRotation(timeSinceStart);
                yield return null;
            }
            Debug.Log("End of sequence");
        }
    }
