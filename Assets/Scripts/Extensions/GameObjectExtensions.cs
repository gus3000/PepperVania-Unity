using UnityEngine;

namespace Extensions
{
    public static class GameObjectExtensions
    {
        public static bool CompareTagIncludingParents(this GameObject go, string tag)
        {
            while (true)
            {
                Debug.Log($"Testing if {go} has tag {tag}");
                if (go.CompareTag(tag)) return true;
                var parent = go.transform.parent;
                if (parent == null) return false;
                go = parent.gameObject;
            }
        }
    }
}