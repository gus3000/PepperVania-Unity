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
        [SerializeField] private SerializableDictionaryBase<string, MonoScript> scripts;
        public MonoScript GetScript(string scriptName)
        {
            if (!scripts.ContainsKey(scriptName))
            {
                Debug.LogWarning($"no known script with key {scriptName}");
                return null;
            }
            return scripts[scriptName];
        }
#endif
    }
}