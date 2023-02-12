using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Animation;
using Camera;
using DefaultNamespace;
using Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerAnimationController))]
public class PlayerController : MonoBehaviour
{
    private const string DashAnimationName = "PepperArmature|Dash";

    [SerializeField, Tooltip("in m/s")] private float speed = 2;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float dashDurationMultiplier = 1f;
    [SerializeField] private float dashSpeedBoost = 2f;
    [SerializeField] private float dashCooldown = 1f;
    private bool canMove => _playerAnimationController.CanMove;

    private Animator _animator;
    private PlayerInput _playerInput;
    private IsoFollow _camera;

    private GameController _gameController;

    // private Rigidbody _rigidbody;
    private CapsuleCollider _playerCollider;

    // the SerializeField below this are for debug only
    [SerializeField] private Vector3 _velocity;
    private float _angle;
    [SerializeField] private float _baseDashDuration;
    [SerializeField] private bool _isDashing;
    [SerializeField] private float _timeSinceLastDash = 0;
    [SerializeField] private Interactible _interactionTarget = null;
    private PlayerAnimationController _playerAnimationController;
    private CharacterController _characterController;
    private float _deadZoneSize = 0.05f;
    private Vector2 _inputMovement;


    private static readonly int SpeedAnimHash = Animator.StringToHash("speed");
    private static readonly int DashTriggerHash = Animator.StringToHash("dash");
    private static readonly int DashSpeedHash = Animator.StringToHash("dashSpeed");


    public bool HasMoved { get; private set; }
    public float DashDurationMultiplier => dashDurationMultiplier;

    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _velocity = Vector3.zero;
        _camera = GameObject.FindWithTag("MainCamera").GetComponent<IsoFollow>();
        _animator = GetComponentInChildren<Animator>();
        _inputMovement = Vector2.zero;
        _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        // _rigidbody = GetComponent<Rigidbody>();
        _playerCollider = GetComponent<CapsuleCollider>();
        _characterController = GetComponent<CharacterController>();
        _playerAnimationController = GetComponent<PlayerAnimationController>();

        var dashAnimation = _animator.runtimeAnimatorController.animationClips.First(clip => clip.name == DashAnimationName);
        _baseDashDuration = dashAnimation.length;
        HasMoved = false;

        Debug.Log($"clamped to 1 : {Vector3.ClampMagnitude(Vector3.forward, 1)}");
        Debug.Log($"clamped to 0 : {Vector3.ClampMagnitude(Vector3.forward, 0)}");
        Debug.Log($"clamped to -1 : {Vector3.ClampMagnitude(Vector3.forward, -1)}");
    }


    void Update()
    {
        HandleInput();
        HandleAnimations();
        HandleMovement();
    }

    void HandleMovement()
    {
        //rotate velocity according to camera
        var rawVelocity = new Vector3(_inputMovement.x, 0, _inputMovement.y);
        var cameraAngle = (int)_camera.Direction;
        var velocity = Quaternion.AngleAxis(cameraAngle, Vector3.up) * rawVelocity;

        if (canMove)
        {
            if (!_isDashing && velocity.magnitude <= _deadZoneSize)
            {
                _velocity = Vector3.zero;
            }
            else if (!_isDashing)
            {
                _velocity = velocity;
            }
        }

        var velocityModifier = 1f;
        if (_isDashing)
            velocityModifier *= dashSpeedBoost;

        // _rigidbody.velocity = _velocity * (speed * velocityModifier);
        // _rigidbody.AddForce(_velocity * (speed * velocityModifier), ForceMode.VelocityChange);
        // Move(_velocity * (speed * Time.deltaTime * velocityModifier));
        // transform.Translate(_velocity * (speed * Time.deltaTime * velocityModifier), Space.World);
        if (_playerAnimationController.IsInAnimatedMovement)
            return;


        _characterController.Move(_velocity * (speed * Time.deltaTime * velocityModifier));
    }

    void Move(Vector3 movement)
    {
        var capsDistance = _playerCollider.height / 2f;
        var center = _playerCollider.bounds.center;
        var tip = center + transform.forward * capsDistance;
        if (UnityEngine.Physics.Raycast(tip, movement, out var raycastHit, movement.magnitude,
                LayerMask.GetMask("Obstacle"), QueryTriggerInteraction.Ignore))
        {
            // Debug.Log($"raycast hit at {raycastHit.distance} with normal {raycastHit.normal}");
            // Debug.Log($"magnitude {movement.magnitude} -> {raycastHit.distance} => {Vector3.ClampMagnitude(movement, raycastHit.distance).magnitude}");
            // Debug.Log($"reducing movement from {movement} to {Vector3.ClampMagnitude(movement, raycastHit.distance)}");
            // var maxMagnitude = raycastHit.distance - capsDistance - epsilon;
            // if (maxMagnitude < 0)
            //     maxMagnitude = 0;
            // movement = Vector3.ClampMagnitude(movement, maxMagnitude); //easy mode
            // movement = FixMovement(movement, tip, raycastHit) * .9f;
            // var fixedMovement = FixMovement(movement, tip, raycastHit);
        }

        transform.Translate(movement, Space.World);
        // _rigidbody.MovePosition(transform.position + movement);
    }

    private void OnDrawGizmosSelected()
    {
        if (_playerCollider == null)
            return;
        // return;
        // var movement = transform.forward;
        var capsDistance = _playerCollider.height / 2f;
        var movement = _velocity * (speed * Time.deltaTime);
        var center = _playerCollider.bounds.center;
        var tip = center + transform.forward * capsDistance;

        Gizmos.color = Color.black;
        Gizmos.DrawCube(center, Vector3.one * 0.05f);
        Gizmos.DrawSphere(tip, .05f);
        Gizmos.DrawRay(center, transform.forward * capsDistance);

        Gizmos.color = Color.white;
        Gizmos.DrawRay(tip, movement);

        if (UnityEngine.Physics.Raycast(tip, movement, out var raycastHit, movement.magnitude,
                LayerMask.GetMask("Obstacle"), QueryTriggerInteraction.Ignore))
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawSphere(raycastHit.point, .1f);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(raycastHit.point, raycastHit.normal);

            //fixmovement
            // var correctMovement = FixMovement(movement, raycastHit);
            var baseMove = raycastHit.point - tip;
            var restMove = movement - baseMove;
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(tip, baseMove);

            // Gizmos.color = Color.blue;
            // Gizmos.DrawRay(raycastHit.point, restMove);
            // Debug.Log($"base : {baseMove}, rest : {restMove}");

            var restMoveFixed = Vector3.ProjectOnPlane(restMove, raycastHit.normal);
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(tip + baseMove, restMoveFixed);

            var correctMovement = baseMove + restMoveFixed;

            Gizmos.color = Color.green;
            Gizmos.DrawRay(tip, correctMovement);
        }
    }

    private Vector3 FixMovement(Vector3 movement, Vector3 tip, RaycastHit raycastHit)
    {
        var minimumDistanceFromWall = 0.1f;
        var baseMove = raycastHit.point - tip;
        var restMove = movement - baseMove;
        var restMoveFixed = Vector3.ProjectOnPlane(restMove, raycastHit.normal);
        var correctMovement = baseMove + restMoveFixed + (raycastHit.normal * minimumDistanceFromWall);
        // if ((tip + correctMovement - raycastHit.point).magnitude < minimumDistanceFromWall)
        // {
        // correctMovement += raycastHit.normal * minimumDistanceFromWall;
        // correctMovement *= (correctMovement.magnitude - minimumDistanceFromWall) / correctMovement.magnitude;
        // }
        return correctMovement;
    }

    void HandleInput()
    {
        // Debug.Log(_playerInput.currentActionMap);
    }

    void HandleAnimations()
    {
        _animator.SetFloat(SpeedAnimHash, _velocity.magnitude);
        if (_velocity.magnitude > 0)
        {
            _angle = Vector3.SignedAngle(transform.forward, _velocity, Vector3.up);
            transform.Rotate(Vector3.up, _angle * rotationSpeed * Time.deltaTime);
        }

        _timeSinceLastDash += Time.deltaTime;
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

    void OnDash()
    {
        if (_isDashing || _timeSinceLastDash < dashCooldown)
            return;
        StartCoroutine(Dash());
    }

    IEnumerator Dash()
    {
        // Debug.Log($"start dash ({Time.time})");
        _animator.SetTrigger(DashTriggerHash);
        _animator.SetFloat(DashSpeedHash, 1 / dashDurationMultiplier);
        _isDashing = true;
        if (_velocity.magnitude == 0)
            _velocity = transform.forward;
        _velocity.Normalize();

        yield return new WaitForSeconds(_baseDashDuration * dashDurationMultiplier);
        _isDashing = false;
        _timeSinceLastDash = 0;
        // Debug.Log($"end dash ({Time.time})");
    }

    void OnMove(InputValue value)
    {
        HasMoved = true;
        _inputMovement = value.Get<Vector2>();
    }

    void OnControlsChanged()
    {
        if (_playerInput == null)
            return; //not started yet

        Debug.Log($"controls changed to {_playerInput.currentControlScheme}");
        _gameController.ChangeControls(_playerInput);
    }

    void OnInteract()
    {
        Debug.Log("Interact");
        if (_interactionTarget == null)
            return; //TODO maybe play animation ?
        _interactionTarget.Interact();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(
            $"trigger enter with {other}, has tag = {other.CompareTagIncludingParents("Interactible")}, has component = {other.GetComponentInParent<Interactible>() != null}, can interact = {other.GetComponentInParent<Interactible>()?.CanInteract}");
        if (!other.CompareTagIncludingParents("Interactible") || !other.GetComponentInParent<Interactible>().CanInteract)
            return;
        Debug.Log("yup, that's interactible, setting interaction target");
        _interactionTarget = other.GetComponentInParent<Interactible>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (_interactionTarget != other.gameObject.GetComponentInParent<Interactible>())
            return;

        _interactionTarget = null;
    }

    // public void PlayAnimation(PlayerAnimation animation)
    // {
    //     StartCoroutine(_playerAnimationController.PlayAnimationSubroutine(animation));
    // }

    public void FollowSequence(AnimationSequence sequence) => StartCoroutine(_playerAnimationController.FollowSequence(sequence));
    


    public bool Won { get; protected set; }


    public Interactible InteractionTarget => _interactionTarget;

    private void FixedUpdate()
    {
        // var velocityModifier = 1f;
        // if (_isDashing)
        //     velocityModifier *= dashSpeedBoost;

        // _rigidbody.velocity = _velocity * (speed * velocityModifier);
        // _rigidbody.AddForce(_velocity * (speed * velocityModifier), ForceMode.VelocityChange);
        // _rigidbody.AddRelativeForce(_velocity * (speed * velocityModifier), ForceMode.VelocityChange);
    }
}