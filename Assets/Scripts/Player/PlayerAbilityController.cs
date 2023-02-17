using System;
using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerAbilityController : MonoBehaviour
    {
        [SerializeField] private float dashCooldown = 1f;
        [SerializeField] private float dashDurationMultiplier = 1f;

        //serialized for debug purposes but shouldn't be
        [SerializeField] private bool canMove = true;
        [SerializeField] private float _timeSinceLastDash = 0;
        [SerializeField] private bool _isDashing;

        private PlayerController _playerController;
        private PlayerAnimationController _playerAnimationController;
        public bool CanMove => canMove;
        public bool IsDashing => _isDashing;

        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
            _playerAnimationController = GetComponent<PlayerAnimationController>();
        }

        public void Dash()
        {
            if (!CanMove || _timeSinceLastDash < dashCooldown)
                return;
            StartCoroutine(DashCoroutine());
        }

        IEnumerator DashCoroutine()
        {
            // Debug.Log($"start dash ({Time.time})");
            _playerAnimationController.TriggerDash(dashDurationMultiplier);
            _isDashing = true;
            canMove = false;
            var velocity = _playerController.Velocity;
            if (velocity.magnitude == 0)
                velocity = transform.forward;
            velocity.Normalize();
            _playerController.Velocity = velocity;

            yield return new WaitForSeconds(_playerAnimationController.GetDashDuration());
            _isDashing = false;
            canMove = true;
            _timeSinceLastDash = 0;
            // Debug.Log($"end dash ({Time.time})");
        }

        private void Update()
        {
            _timeSinceLastDash += Time.deltaTime;
        }
    }
}