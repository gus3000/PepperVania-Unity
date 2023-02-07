using UnityEngine;

namespace Extensions
{
    public static class ComponentExtensions
    {
        public static bool CompareTagIncludingParents(this Component component, string tag) => component.gameObject.CompareTagIncludingParents(tag);


    }
}