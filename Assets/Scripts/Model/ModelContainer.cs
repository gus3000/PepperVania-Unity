using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace Model
{
    public class ModelContainer : MonoBehaviour
    {
        [SerializeField] private GameObject tileBasePrefab;

        public GameObject TileBasePrefab => tileBasePrefab;

#if UNITY_EDITOR
        [SerializeField] private SerializableDictionaryBase<string, GameObject> objects;
        public GameObject GetObject(string scriptName)
        {
            if (!objects.ContainsKey(scriptName))
            {
                Debug.LogWarning($"no prefab with key {scriptName}");
                return null;
            }
            return objects[scriptName];
        }
#endif
    }
}