using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Camera;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private const string DashAnimationName = "animation.pepper.dash";

    [SerializeField, Tooltip("in m/s")] private float speed = 2;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float dashDurationMultiplier = 1f;
    [SerializeField] private float dashSpeedBoost = 2f;
    [SerializeField] private float dashCooldown = 1f;

    private Animator _animator;
    private PlayerInput _playerInput;
    private IsoFollow _camera;

    // the SerializeField below this are for debug only
    [SerializeField] private Vector3 _velocity;
    private float _angle;
    [SerializeField] private float _baseDashDuration;
    [SerializeField] private bool _isDashing;
    [SerializeField] private float _timeSinceLastDash = 0;
    [SerializeField] private Interactible _interactionTarget = null;


    private static readonly int SpeedAnimHash = Animator.StringToHash("speed");
    private static readonly int DashTriggerHash = Animator.StringToHash("dash");
    private static readonly int DashSpeedHash = Animator.StringToHash("dashSpeed");

    public bool HasMoved { get; private set; }

    void Start()
    {
        Debug.Log("Start");
        _velocity = Vector3.forward;
        _playerInput = GetComponent<PlayerInput>();
        _velocity = Vector3.zero;
        _camera = GameObject.FindWithTag("MainCamera").GetComponent<IsoFollow>();
        _animator = GetComponentInChildren<Animator>();

        var dashAnimation = _animator.runtimeAnimatorController.animationClips.First(clip => clip.name == DashAnimationName);
        _baseDashDuration = dashAnimation.length;
        HasMoved = false;
    }


    void Update()
    {
        HandleInput();
        HandleAnimations();
        HandleMovement();
    }

    void HandleMovement()
    {
        var velocityModifier = 1f;
        if (_isDashing)
            velocityModifier *= dashSpeedBoost;

        transform.Translate(_velocity * (speed * Time.deltaTime * velocityModifier), Space.World);
    }

    void HandleInput()
    {
        // Debug.Log(_playerInput.currentActionMap);
    }

    void HandleAnimations()
    {
        _animator.SetFloat(SpeedAnimHash, _velocity.magnitude);
        _angle = Vector3.SignedAngle(transform.forward, _velocity, Vector3.up);
        transform.Rotate(Vector3.up, _angle * rotationSpeed * Time.deltaTime);
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
        HasMoved = true; //TODO rework this
        Vector2 movement = value.Get<Vector2>();
        //rotate velocity according to camera
        var rawVelocity = new Vector3(movement.x, 0, movement.y);
        var cameraAngle = (int)_camera.Direction;
        var velocity = Quaternion.AngleAxis(cameraAngle, Vector3.up) * rawVelocity;

        if (!_isDashing)
        {
            _velocity = velocity;
            return;
        }


        // Debug.Log($"velocity : {rawVelocity} => {velocity}");
    }

    void OnControlsChanged()
    {
        if (_playerInput == null)
            return; //not started yet

        Debug.Log($"controls changed to {_playerInput.currentControlScheme}");
    }

    void OnInteract()
    {
        Debug.Log("Interact");
        if (_interactionTarget == null)
            return; //TODO maybe play animation ?
        _interactionTarget.Interact();
        Won = true; //DEBUG, remove
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log($"Pepperping {other.name}");
        if (!other.CompareTag("Interactible"))
            return;
        _interactionTarget = other.gameObject.GetComponent<Interactible>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Interactible"))
            return;
        if (_interactionTarget != other.gameObject.GetComponent<Interactible>())
            return;

        _interactionTarget = null;
    }

    public bool Won { get; protected set; }
    
    
    public Interactible InteractionTarget => _interactionTarget;

    private void FixedUpdate()
    {
        // transform.Translate(velocity * (speed * Time.fixedDeltaTime),Space.World);
    }
}