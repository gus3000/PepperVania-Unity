using System;
using UnityEngine;

[Serializable]
public class MeshList
{
    [SerializeField] private Mesh[] mesh;

    public MeshList(Mesh[] mesh)
    {
        this.mesh = mesh;
    }

    public int Length => mesh.Length;

    public Mesh this[int index] => mesh[index];
}