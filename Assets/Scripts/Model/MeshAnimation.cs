using System;
using UnityEngine;

namespace DefaultNamespace.Model
{
    [Serializable]
    public class MeshAnimation
    {
        [SerializeField] private AnimationPhase _phase;
        [SerializeField] private Mesh[] _meshes;

        public AnimationPhase Phase => _phase;

        public Mesh[] Meshes => _meshes;
    }
}