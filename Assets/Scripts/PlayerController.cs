using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Camera;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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
    [SerializeField] private bool canMove = true;

    private Animator _animator;
    private PlayerInput _playerInput;
    private IsoFollow _camera;
    private GameController _gameController;

    // the SerializeField below this are for debug only
    [SerializeField] private Vector3 _velocity;
    private float _angle;
    [SerializeField] private float _baseDashDuration;
    [SerializeField] private bool _isDashing;
    [SerializeField] private float _timeSinceLastDash = 0;
    [SerializeField] private Interactible _interactionTarget = null;
    private float _deadZoneSize = 0.05f;
    private Vector2 _inputMovement;


    private static readonly int SpeedAnimHash = Animator.StringToHash("speed");
    private static readonly int DashTriggerHash = Animator.StringToHash("dash");
    private static readonly int DashSpeedHash = Animator.StringToHash("dashSpeed");
    private static readonly int Sleeping = Animator.StringToHash("sleeping");
    private static readonly int Rest = Animator.StringToHash("rest");

    public bool HasMoved { get; private set; }

    void Start()
    {
        _velocity = Vector3.forward;
        _playerInput = GetComponent<PlayerInput>();
        _velocity = Vector3.zero;
        _camera = GameObject.FindWithTag("MainCamera").GetComponent<IsoFollow>();
        _animator = GetComponentInChildren<Animator>();
        _inputMovement = Vector2.zero;
        _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        
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

        transform.Translate(_velocity * (speed * Time.deltaTime * velocityModifier), Space.World);
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
        Debug.Log($"trigger enter with {other}");
        if (!other.CompareTag("Interactible") || !other.GetComponent<Interactible>().CanInteract)
            return;
        _interactionTarget = other.gameObject.GetComponent<Interactible>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (_interactionTarget != other.gameObject.GetComponent<Interactible>())
            return;

        _interactionTarget = null;
    }

    public void PlayAnimation(PlayerAnimation animation)
    {
        StartCoroutine(PlayAnimationSubroutine(animation));
    }

    private IEnumerator PlayAnimationSubroutine(PlayerAnimation animation)
    {
        Debug.Log($"Play animation {animation}");
        var oldCanMove = canMove;
        canMove = false;
        switch (animation)
        {
            case PlayerAnimation.Sleep:
                break;
            case PlayerAnimation.Rest:
                _animator.SetBool(Rest, true);
                // _animator.SetBool(Sleeping, true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(animation), animation, null);
        }

        yield return null;
        var state = _animator.GetCurrentAnimatorStateInfo(0);
        while (!state.IsName("Climb Tree"))
        {
            Debug.Log("waiting for anim to start");
            yield return null;
            state = _animator.GetCurrentAnimatorStateInfo(0);
        }

        Debug.Log("anim started !");
        // Debug.Break();

        canMove = oldCanMove;
    }

    public IEnumerator ForceMoveTo(Vector3 pos, float angle)
    {
        var trans = transform;
        while ((pos - trans.position).magnitude > Time.deltaTime)
        {
            Debug.Log($"going from {trans.position} to {pos}");
            _velocity = (pos - trans.position).normalized;
            yield return null;
        }

        _velocity = Vector3.zero;


        trans.rotation = Quaternion.Euler(0, angle, 0);
        yield return null;
        // while (Mathf.Abs(trans.rotation.eulerAngles.y - angle) > Time.deltaTime)
        // {
        // _angle = Vector3.SignedAngle(transform.forward, new Vector3(0, angle, 0), Vector3.up);
        // transform.Rotate(Vector3.up, _angle * rotationSpeed * Time.deltaTime);
        // }
    }

    public IEnumerator StartAnimationAt(Vector3 startPos, PlayerAnimation animation)
    {
        canMove = false;
        yield return ForceMoveTo(startPos, 0);
        yield return PlayAnimationSubroutine(animation);

        canMove = true;
    }

    public bool Won { get; protected set; }


    public Interactible InteractionTarget => _interactionTarget;

    private void FixedUpdate()
    {
        // transform.Translate(velocity * (speed * Time.fixedDeltaTime),Space.World);
    }
}