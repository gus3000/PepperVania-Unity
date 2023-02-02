    using System;
    using UnityEngine;

    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private Transform headTrackerFocus;
        
        private PlayerController _playerController;
        private Vector3 baseHeadTrackerPosition;

        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
            baseHeadTrackerPosition = headTrackerFocus.transform.localPosition;
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
    }
