using System.Collections.Generic;
using SuperTiled2Unity;
using SuperTiled2Unity.Editor;
using UnityEngine;

namespace Editor.Importer
{
    class TileComparerByTileName : IEqualityComparer<SuperObject>
    {
        private static string GetTileName(SuperObject o)
        {
            return o.gameObject.GetSuperPropertyValueString(TilesTo3dObjectImporter.TileNameKey, "");
        }

        public bool Equals(SuperObject x, SuperObject y)
        {
            // Debug.Log($"Comparing {x} and {y}");
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;

            var xTileName = GetTileName(x);
            var yTileName = GetTileName(y);

            // Debug.Log($"comparing {xTileName} and {yTileName}");
            return xTileName == yTileName;
        }

        public int GetHashCode(SuperObject obj)
        {
            return GetTileName(obj).GetHashCode();
        }
    }
}