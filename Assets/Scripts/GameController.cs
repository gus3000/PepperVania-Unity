using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private PlayerController _player;
    private UiController _uiController;
    public float StartTime { get; private set; }

    private void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        _uiController = GameObject.FindWithTag("UICanvas").GetComponent<UiController>();
        StartTime = Time.time;
    }

    private void Update()
    {

    }
}