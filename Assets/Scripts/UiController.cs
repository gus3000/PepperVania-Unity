﻿using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class UiController : MonoBehaviour
{
    [SerializeField] private float fadeTime = 1;

    [SerializeField] private FadeInOut controlsTutorial;
    [SerializeField] private float showControlsTimer = 2;
    [SerializeField] private FadeInOut interactUi;
    [SerializeField] private FadeInOut winDebug;
    
    private PlayerController _player;
    private Coroutine _currentCoroutine;
    private GameController _gameController;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        controlsTutorial.ShouldShow = () => (Time.time - _gameController.StartTime > showControlsTimer) && !_player.HasMoved;
        interactUi.ShouldShow = () => _player.InteractionTarget != null && _player.InteractionTarget.CanInteract;
        winDebug.ShouldShow = () => _player.Won;
    }

    private void Update()
    {
        // if(ShouldShowControlsPanel && !_controlsShowing)
            // ShowControls();
        // else if(ShouldHideControlsPanel && _controlsShowing)
            // HideControls();
    }
    
}