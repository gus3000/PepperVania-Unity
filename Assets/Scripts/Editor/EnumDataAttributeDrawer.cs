using Attribute;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(EnumDataAttribute))]
    public class EnumDataAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var enumData = (EnumDataAttribute)attribute;
            var path = property.propertyPath;
            var array = property.serializedObject.FindProperty(path.Substring(0, path.LastIndexOf('.')));
            if (array == null)
            {
                EditorGUI.LabelField(position, "Use EnumDataAttribute on arrays only");
                return;
            }

            array.arraySize = enumData.names.Length;
            int index = GetIndex(path);
            label.text = enumData.names[index];
            EditorGUI.PropertyField(position, property, label, true);
        }

        protected int GetIndex(string path)
        {
            return System.Convert.ToInt32(path.Substring(path.LastIndexOf('[') + 1).Replace("]", ""));
        }
    }
}