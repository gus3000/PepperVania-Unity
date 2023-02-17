using System;
using Animation;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Animation
{
    [CustomPropertyDrawer(typeof(AnimationStep), true)]
    public class AnimationStepDrawer : PropertyDrawer
    {
        private AnimationStepType _type;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            int lines = 0;

            EditorGUI.BeginChangeCheck();
            var type = (AnimationStepType)EditorGUI.EnumPopup(GetRect(position, lines++), GetCurrentType());
            if (EditorGUI.EndChangeCheck())
            {
                Debug.Log($"type changed to {type}");
                property.SetValue(new CurveAnimationStep());
            }
            
            if (_type != AnimationStepType.None)
            {
                EditorGUI.PropertyField(GetRect(position, lines++), property.FindPropertyRelative("duration"), new GUIContent("duration"));
                EditorGUI.PropertyField(GetRect(position, lines++), property.FindPropertyRelative("pouet"), new GUIContent("pouet"));
            }

            // EditorGUILayout.PropertyField(property.FindPropertyRelative("duration"), new GUIContent("duration"));
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var lines = GetNumberOfLines();

            return lines * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * (lines - 1);
        }

        private Rect GetRect(Rect basePosition, int line)
        {
            return new Rect(basePosition.x, basePosition.y + GetLineHeight(line), basePosition.width, EditorGUIUtility.singleLineHeight);
        }

        private float GetLineHeight(int line)
        {
            return line * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        }

        private AnimationStepType GetCurrentType()
        {
            //TODO
            // Debug.Log(fieldInfo.FieldType);
            if (fieldInfo.FieldType == typeof(AnimationStep)) return AnimationStepType.None;
            if (fieldInfo.FieldType == typeof(CurveAnimationStep)) return AnimationStepType.Curve;
            if (fieldInfo.FieldType == typeof(PlayerAnimAnimationStep)) return AnimationStepType.PlayerAnim;

            return AnimationStepType.None;
        }

        private int GetNumberOfLines()
        {
            switch (GetCurrentType())
            {
                case AnimationStepType.Curve:
                    return 8;
                case AnimationStepType.PlayerAnim:
                    return 3;
                default:
                    return 1;
            }
        }

        enum AnimationStepType
        {
            None,
            Curve,
            PlayerAnim
        }
    }
}