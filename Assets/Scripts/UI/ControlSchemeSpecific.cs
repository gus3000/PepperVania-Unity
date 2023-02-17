using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ControlSchemeSpecific : MonoBehaviour
    {
        [SerializeField] private string[] controlScheme; 
        
        public void ChangeControlScheme(string newScheme)
        {
            var canvasGroup = GetComponent<CanvasGroup>();
            if (controlScheme.Contains(newScheme))
                canvasGroup.alpha = 1;
            else
                canvasGroup.alpha = 0;

        }
    }
}