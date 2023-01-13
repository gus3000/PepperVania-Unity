using System;
using System.Collections;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeInOut : MonoBehaviour
    {
        [SerializeField] private float fadeTime = 1f;

        private bool _showing;
        private CanvasGroup _canvasGroup;
        private Coroutine _currentCoroutine;

        public Func<bool> ShouldShow { get; set; }

        private void Start()
        {
            _showing = false;
            ShouldShow = () => false;
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Update()
        {
            // Debug.Log($"{name} should show ? {ShouldShow()}");
            if (ShouldShow() && !_showing)
                ToggleShowing(true);
            else if (!ShouldShow() && _showing)
                ToggleShowing(false);
        }

        private void ToggleShowing(bool enable)
        {
            _showing = enable;
            if (_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);
            if (enable)
                _currentCoroutine = StartCoroutine(SetAlphaTo(1));
            else
                _currentCoroutine = StartCoroutine(SetAlphaTo(0));
        }

        private IEnumerator SetAlphaTo(float alpha)
        {
            var diff = alpha - _canvasGroup.alpha;
            while (Mathf.Abs(_canvasGroup.alpha - alpha) > Time.smoothDeltaTime)
            {
                _canvasGroup.alpha += Mathf.Sign(diff) * Time.deltaTime / fadeTime;
                yield return null;
            }
        }
    }
}