    using System.Collections.Generic;
    using RotaryHeart.Lib.SerializableDictionary;
    using UnityEngine;
    using UnityEngine.Serialization;

    namespace Model
    {
        public class ModelContainer : MonoBehaviour
        {
            // [SerializeField] private Dictionary<string, GameObject> _prefabs;
            // [SerializeField] private GameObject[] prefabs;
            // [SerializeField] private UnityFriendlyDictionary<string, GameObject> prefabsDictionary;
            [SerializeField] private SerializableDictionaryBase<string, GameObject> prefabs;
            [SerializeField] private GameObject tileBasePrefab;

            public GameObject TileBasePrefab => tileBasePrefab;

            public GameObject GetPrefab(string prefix, string tileName)
            {
                prefabs.TryGetValue(tileName, out var go);
                return go == null ? prefabs["unknown"] : go;
                
            }
        }
    }
