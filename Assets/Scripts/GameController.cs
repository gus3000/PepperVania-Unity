using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private float showControlsTimer = 2;

    private PlayerController _player;
    private UiController _uiController;
    private float _startTime;

    private bool ShouldShowControlsPanel
    {
        get
        {
            var timeSinceStart = Time.time - _startTime;
            return timeSinceStart > showControlsTimer && !_player.HasMoved;
        }
    }

    private bool ShouldHideControlsPanel => _player.HasMoved; 

    private void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        _uiController = GameObject.FindWithTag("UICanvas").GetComponent<UiController>();
        _startTime = Time.time;
    }

    private void Update()
    {
        if(ShouldShowControlsPanel)
            _uiController.ShowControls();
        else if(ShouldHideControlsPanel)
            _uiController.HideControls();
    }
}