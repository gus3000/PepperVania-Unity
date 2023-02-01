using System;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class DynamicText : MonoBehaviour
    {
        private TextMeshProUGUI _textComponent;
        private string _oldtext;
        public Func<string> Text { get; set; }

        private void Start()
        {
            _textComponent = GetComponent<TextMeshProUGUI>();
            Text = () => "";
            _oldtext = "";
        }

        private void Update()
        {
            var newText = Text();
            if (newText != _oldtext)
            {
                _oldtext = newText;
                _textComponent.text = newText;
            }
        }
    }
}