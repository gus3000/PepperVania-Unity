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
        ConfigureUi();
    }

    private void ConfigureUi()
    {
        List<string> toEnable = new ();
        List<string> toDisable = new ();
        if (Application.platform == RuntimePlatform.Android)
        {
            toEnable.Add("Android");
            toDisable.Add("PC");
        }
        else
        {
            toEnable.Add("PC");
            toDisable.Add("Android");
        }

        // foreach (var tagToEnable in toEnable)
        // {
        //     foreach (var go in GameObject.FindGameObjectsWithTag(tagToEnable))
        //     {
        //         var canvasGroup = go.GetComponent<CanvasGroup>();
        //         if (canvasGroup == null)
        //             continue;
        //         canvasGroup.alpha = 1;
        //     }
        // }
        
        foreach (var tagToDisable in toDisable)
        {
            foreach (var go in GameObject.FindGameObjectsWithTag(tagToDisable))
            {
                go.SetActive(false);
            }
        }
    }

    private void Update()
    {

    }
}