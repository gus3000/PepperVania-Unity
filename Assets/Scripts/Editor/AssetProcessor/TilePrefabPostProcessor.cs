using UnityEditor;
using UnityEngine;

namespace Editor.AssetProcessor
{
    public class TilePrefabPostProcessor : AssetPostprocessor
    {
        void OnPostprocessPrefab(GameObject go)
        {
            // Debug.Log($"Post process {go} at path ");
        }
    }
}