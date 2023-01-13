using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Attribute;
using DefaultNamespace.Model;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class VoxelAnimation : MonoBehaviour
{
    // // [SerializeField, PopulateMeshAnimation] private Dictionary<AnimationPhase, Mesh[]> _animations;
    // [SerializeField, EnumData(typeof(AnimationPhase))]
    // private MeshList[] animationPhases;
    //
    // [SerializeField] private AnimationPhase currentAnimation;
    //
    // [InspectorButton("OnPopulate")] public bool populate;
    // public string modelName = "";
    // [SerializeField] private float delay = .5f;
    //
    // [SerializeField] private int currentFrame = 0;
    //
    // private float _timeAccumulator = 0f;
    //
    // private MeshFilter _meshFilter;
    //
    //
    // void Start()
    // {
    //     _timeAccumulator = 0;
    //     _meshFilter = GetComponent<MeshFilter>();
    //
    // }
    //
    // void Update()
    // {
    //     _timeAccumulator += Time.deltaTime;
    //     while (_timeAccumulator > delay)
    //     {
    //         _timeAccumulator -= delay;
    //         var currentPhaseMeshes = animationPhases[(int)currentAnimation];
    //         currentFrame = (currentFrame + 1) % currentPhaseMeshes.Length;
    //         _meshFilter.mesh = currentPhaseMeshes[currentFrame];
    //     }
    // }
    //
    // private void OnPopulate()
    // {
    //     Debug.Log("populate !!!!");
    //     if (modelName == "")
    //     {
    //         Debug.LogError("model name not set");
    //         return;
    //     }
    //
    //     var assetFolderPath = $"Assets/Characters/{modelName}";
    //     var filePaths = Directory.GetFiles(assetFolderPath);
    //     var baseRegex = new Regex($"^{modelName.ToLower()}$");
    //     var objFiles = Directory
    //         .EnumerateFiles(assetFolderPath)
    //         .Where(filePath => Path.GetExtension(filePath) == ".obj")
    //         .ToList();
    //
    //     var baseObjPath = objFiles.First(filePath => baseRegex.IsMatch(Path.GetFileNameWithoutExtension(filePath)));
    //     GetComponent<MeshFilter>().sharedMesh = GetMesh(baseObjPath);
    //     
    //     foreach (AnimationPhase phase in Enum.GetValues(typeof(AnimationPhase)))
    //     {
    //         var regex = $"^{modelName.ToLower()}_{phase.ToString().ToLower()}_\\d+$";
    //         // Debug.Log($"searching for animation {phase} with regex {regex}");
    //         var files = objFiles
    //             .Where(filePath => Regex.IsMatch(Path.GetFileNameWithoutExtension(filePath), regex))
    //             .ToList();
    //         if (files.Count == 0)
    //             continue;
    //         Debug.Log($"found : {phase}");
    //         var meshes = new List<Mesh>();
    //         foreach (var file in files)
    //         {
    //             Debug.Log($"Adding {file} to animation {phase}");
    //             meshes.Add(GetMesh(file));
    //         }
    //
    //         animationPhases[(int)phase] = new MeshList(meshes.ToArray());
    //     }
    // }
    //
    // private Mesh GetMesh(string objPath)
    // {
    //     var objAsset = AssetDatabase.LoadAssetAtPath(objPath, typeof(Mesh));
    //     return objAsset as Mesh;
    // }
}