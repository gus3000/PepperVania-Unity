using System.Collections.Generic;
using SuperTiled2Unity;
using SuperTiled2Unity.Editor;
using UnityEngine;

namespace Editor.Importer
{
    class TileComparerByTileId : IEqualityComparer<SuperObject>
    {

        public bool Equals(SuperObject x, SuperObject y)
        {
            // Debug.Log($"Comparing {x} and {y}");
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;

            // Debug.Log($"comparing {xTileName} and {yTileName}");
            return x.m_TileId == y.m_TileId;
        }

        public int GetHashCode(SuperObject obj)
        {
            return (int)obj.m_TileId;
        }
    }
}