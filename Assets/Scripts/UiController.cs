using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class UiController : MonoBehaviour
{
    [SerializeField] private CanvasGroup controlsTutorial;
    [SerializeField] private bool triggerShowControls;
    [SerializeField] private bool triggerHideControls;
    [SerializeField] private float fadeTime = 1;
    private PlayerController _player;
    private Coroutine _currentCoroutine;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        controlsTutorial.alpha = 0;
    }

    private void Update()
    {
        if (triggerShowControls)
        {
            triggerShowControls = false;
            ToggleControls(true);
        }

        if (triggerHideControls)
        {
            triggerHideControls = false;
            ToggleControls(false);
        }
    }

    private void ToggleControls(bool enable)
    {
        // StartCoroutine(ShowControls());
        // StartCoroutine(HideControls());
        if (_currentCoroutine != null)
            StopCoroutine(_currentCoroutine);
        if (enable)
            _currentCoroutine = StartCoroutine(SetAlphaTo(controlsTutorial, 1));
        else
            _currentCoroutine = StartCoroutine(SetAlphaTo(controlsTutorial, 0));
    }

    private IEnumerator SetAlphaTo(CanvasGroup group, float alpha)
    {
        var diff = alpha - controlsTutorial.alpha;
        while (Mathf.Abs(controlsTutorial.alpha - alpha) > Time.smoothDeltaTime)
        {
            controlsTutorial.alpha += Mathf.Sign(diff) * Time.deltaTime / fadeTime;
            yield return null;
        }
    }

    public void ShowControls()
    {
        ToggleControls(true);
    }

    public void HideControls()
    {
        ToggleControls(false);
    }
}