using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class UiController : MonoBehaviour
{
    [SerializeField] private float fadeTime = 1;

    [SerializeField] private FadeInOut controlsTutorial;
    [SerializeField] private float showControlsTimer = 2;
    [SerializeField] private FadeInOut interactUi;
    [SerializeField] private DynamicText interactUiText;
    [SerializeField] private FadeInOut winDebug;
    [SerializeField] private CanvasGroup androidControls;

    private PlayerController _player;
    private PlayerInput _playerInput;
    private Coroutine _currentCoroutine;
    private GameController _gameController;
    private ControlSchemeSpecific[] _controleSchemeSpecificComponents;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        _playerInput = _player.GetComponent<PlayerInput>();
        _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        controlsTutorial.ShouldShow = () => (Time.time - _gameController.StartTime > showControlsTimer) && !_player.HasMoved;
        interactUi.ShouldShow = () => _player.InteractionTarget != null && _player.InteractionTarget.CanInteract;
        interactUiText.Text = () => _player.InteractionTarget == null ? "pouet" : _player.InteractionTarget.Verb;
        winDebug.ShouldShow = () => _player.Won;
        _controleSchemeSpecificComponents = FindObjectsOfType<ControlSchemeSpecific>();

        ConfigureUi();
    }

    private void Update()
    {
        // Debug.Log($"{Time.time} - {_gameController.StartTime} > {showControlsTimer} ? {Time.time - _gameController.StartTime > showControlsTimer}");
        // Debug.Log($"player has moved ? {_player.HasMoved}");
        // if(ShouldShowControlsPanel && !_controlsShowing)
        // ShowControls();
        // else if(ShouldHideControlsPanel && _controlsShowing)
        // HideControls();
    }


    public void ConfigureUi()
    {
        List<string> toEnable = new();
        List<string> toDisable = new();
        if (Application.platform == RuntimePlatform.Android)
        {
            toEnable.Add("Android");
            toDisable.Add("PC");
            androidControls.alpha = 1;
        }
        else
        {
            toEnable.Add("PC");
            toDisable.Add("Android");
            androidControls.alpha = 0;
        }

        foreach (var tagToEnable in toEnable)
        {
            foreach (var go in GameObject.FindGameObjectsWithTag(tagToEnable))
            {
                var canvasGroup = go.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                    continue;
                // canvasGroup.alpha = 1;
            }
        }

        foreach (var tagToDisable in toDisable)
        {
            foreach (var go in GameObject.FindGameObjectsWithTag(tagToDisable))
            {
                var canvasGroup = go.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                    continue;
                canvasGroup.alpha = 0;
            }
        }

        var scheme = _playerInput.currentControlScheme;
        foreach (var controlSchemeSpecificComponent in _controleSchemeSpecificComponents)
        {
            controlSchemeSpecificComponent.ChangeControlScheme(scheme);
        }
    }
}