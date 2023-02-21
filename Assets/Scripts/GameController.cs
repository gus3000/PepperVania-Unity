using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{

    private PlayerController _player;
    private UiController _uiController;
    public float StartTime { get; private set; }

    private void Awake()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        _uiController = GameObject.FindWithTag("UICanvas").GetComponent<UiController>();
        StartTime = Time.time;
    }

    private void Update()
    {

    }

    public void ChangeControls()
    {
        if (_uiController == null)
            return;
        
        _uiController.ConfigureUi();
    }
}