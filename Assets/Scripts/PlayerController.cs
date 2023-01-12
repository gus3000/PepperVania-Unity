using System;
using Camera;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField, Tooltip("in m/s")] private float speed = 2;
        [SerializeField] private float rotationSpeed = 10f;

        [SerializeField] private Vector3 velocity;
        public float angle;
        public float velocityAngle;
        private Animator _animator;
        private PlayerInput _playerInput;
        private IsoFollow camera;

        private static readonly int Speed = Animator.StringToHash("speed");

        void Start()
        {
            Debug.Log("Start");
            velocity = Vector3.forward;
            _playerInput = GetComponent<PlayerInput>();
            velocity = Vector3.zero;
            camera = GameObject.FindWithTag("MainCamera").GetComponent<IsoFollow>();
            _animator = GetComponentInChildren<Animator>();
        }


        void Update()
        {
            HandleInput();
            HandleAnimations();
            HandleMovement();
        }

        void HandleMovement()
        {
            transform.Translate(velocity * (speed * Time.deltaTime), Space.World);
        }

        void HandleInput()
        {
            // Debug.Log(_playerInput.currentActionMap);
        }

        void HandleAnimations()
        {
            if (_animator == null)
            {
                throw new Exception("animator null");
            }

            if (velocity == null)
            {
                throw new Exception("velocity is null");
            }

            _animator.SetFloat(Speed, velocity.magnitude);
            angle = Vector3.SignedAngle(transform.forward, velocity, Vector3.up);
            transform.Rotate(Vector3.up, angle * rotationSpeed * Time.deltaTime);
            // angle = Vector3.Angle(Vector3.up, transform.forward);
            // velocityAngle = Vector3.Angle(Vector3.up,velocity);
            // transform.rotation = Quaternion.Euler();
            // transform.forward = Vector3.Lerp(transform.forward, velocity, 1);
        }

        void OnJump()
        {
            Debug.Log("Jump");
            Debug.Log(_playerInput.currentControlScheme);
        }

        void OnMove(InputValue value)
        {
            Vector2 movement = value.Get<Vector2>();
            //rotate velocity according to camera
            var rawVelocity = new Vector3(movement.x, 0, movement.y);
            var cameraAngle = (int)camera.Direction;
            velocity = Quaternion.AngleAxis(cameraAngle, Vector3.up) * rawVelocity;

            // Debug.Log($"velocity : {rawVelocity} => {velocity}");
        }

        private void FixedUpdate()
        {
            // transform.Translate(velocity * (speed * Time.fixedDeltaTime),Space.World);
        }
    }
}