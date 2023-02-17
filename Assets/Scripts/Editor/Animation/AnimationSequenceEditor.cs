using System;
using Animation;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Animation
{
    // [CustomEditor(typeof(AnimationSequence), true)]
    // [CanEditMultipleObjects]
    // public class AnimationSequenceEditor : UnityEditor.Editor
    // {
    //     private SerializedProperty _animationStepsProperty;
    //
    //     private void OnEnable()
    //     {
    //         _animationStepsProperty = serializedObject.FindProperty("_animationSteps");
    //     }
    //
    //     public override void OnInspectorGUI()
    //     {
    //         serializedObject.Update();
    //         // EditorGUILayout.Slider(duration, 0, 100, new GUIContent("pouet"));
    //         // EditorGUILayout.PropertyField(_animationStepsProperty, new GUIContent("pouet"));
    //         for (int i = 0; i < _animationStepsProperty.arraySize; i++)
    //         {
    //             var step = _animationStepsProperty.GetArrayElementAtIndex(i);
    //             EditorGUILayout.PropertyField(step);
    //             // EditorGUILayout.HelpBox(step.editable.ToString(), MessageType.Info);
    //             
    //         }
    //         serializedObject.ApplyModifiedProperties();
    //     }
    // }
}