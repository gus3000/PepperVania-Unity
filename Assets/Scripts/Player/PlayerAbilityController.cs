using System;
using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerAbilityController : MonoBehaviour
    {
        [SerializeField] private float dashCooldown = 1f;
        [SerializeField] private float dashDurationMultiplier = 1f;
        [SerializeField] private float dashSpeedBoost = 2f;

        //serialized for debug purposes but shouldn't be
        [SerializeField] private float _timeSinceLastDash = 0;
        [SerializeField] private bool _isDashing;
        [SerializeField] private bool _isCrouching;
        

        private PlayerController _playerController;
        private PlayerAnimationController _playerAnimationController;
        public bool IsDashing => _isDashing;

        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
            _playerAnimationController = GetComponent<PlayerAnimationController>();
        }

        public void Crouch()
        {
            if (_isDashing)
                return;
            _isCrouching = true;
        }
        public void Dash()
        {
            if (_timeSinceLastDash < dashCooldown)
                return;
            StartCoroutine(DashCoroutine());
        }

        IEnumerator DashCoroutine()
        {
            // Debug.Log($"start dash ({Time.time})");
            _playerAnimationController.TriggerDash(dashDurationMultiplier);
            _isDashing = true;
            _isCrouching = false;
            var velocity = _playerController.Velocity;
            if (velocity.magnitude == 0)
                velocity = transform.forward;
            velocity.Normalize();
            _playerController.Velocity = velocity;

            yield return new WaitForSeconds(_playerAnimationController.GetDashDuration());
            _isDashing = false;
            _timeSinceLastDash = 0;
            // Debug.Log($"end dash ({Time.time})");
        }

        public bool IsMovementHandledByAbility()
        {
            return _isCrouching || _isDashing;
        }

        public Vector3 GetVelocity()
        {
            if (_isDashing)
                return transform.forward * dashSpeedBoost;
            if (_isCrouching)
                return Vector3.zero;
            return Vector3.zero;
        }

        private void Update()
        {
            _timeSinceLastDash += Time.deltaTime;
        }
    }
}