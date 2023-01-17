using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using SuperTiled2Unity;
using SuperTiled2Unity.Editor;
using SuperTiled2Unity.Editor.ClipperLib;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.Importer
{
    public class TilesTo3dObjectImporter : CustomTmxImporter
    {
        public const string TileNameKey = "tileName";
        public const string EmptyKey = "empty";

        private const string LayerToKeep = "terrain";
        private const string PrefabsDirectory = "Assets/Models/Terrain";
        private const string PrefabPrefix = "slate";

        private ModelContainer _modelContainer;

        private Dictionary<uint, GameObject> _generatedPrefabs;
        private Dictionary<uint, Mesh> _generatedMeshes;

        private int tileWidth = 1;
        private int tileHeight = 1;

        public override void TmxAssetImported(TmxAssetImportedArgs args)
        {
            var map = args.ImportedSuperMap;
            var importer = args.AssetImporter;
            Debug.Log("----------------------- IMPORT -----------------------");
            Debug.Log($"Map : {map.m_Width}x{map.m_Height} : {map.m_TiledVersion}");
            tileWidth = map.m_TileWidth;
            tileHeight = map.m_TileHeight;

            var tileMap = new Dictionary<Vector2, SuperObject>();

            var tiles = map.GetComponentsInChildren<SuperObject>();
            foreach (var tile in tiles)
            {
                if (tile.GetComponentInParent<SuperTileLayer>().m_TiledName != LayerToKeep)
                    continue;
                // Debug.Log($"tile {tile} => {tile.m_TileId}");
                // Debug.Log($"type : {tile.m_Type}");
                tileMap.Add(new Vector2(tile.m_X, tile.m_Y), tile);
            }


            var distinctTiles = tileMap.Values.Distinct(new TileComparerByTileId()).ToList();
            // Debug.Log($"total tiles = {tileMap.Count}, distinct = {distinctTiles.Count()}");
            PreparePrefabs(PrefabPrefix, distinctTiles);
            // Debug.Log($"prefabs (count {_generatedPrefabs.Count}): ");

            // bool debugStop = false;
            // if (_generatedMeshes == null)
            // {
            //     Debug.Log("generated meshes null :(");
            //     debugStop = true;
            // }
            // if(_generatedPrefabs == null)
            // {
            //     Debug.Log("generated prefabs null :(");
            //     debugStop = true;
            // }
            //
            // foreach (var (tileId, prefab) in _generatedPrefabs)
            // {
            //     // Debug.Log($"prefab {tileId} =>");
            //     if(prefab == null)
            //         Debug.Log($"{tileId} prefab is null");
            //     else
            //         Debug.Log($"{tileId} is NOT NULL <-----------------");
            // }
            //
            // debugStop = true;
            //
            // if (debugStop)
            // {
            //     Debug.Log("an error was encoutered, stopping now");
            //     return;
            // }
            
            // foreach (var (tileId, mesh) in _generatedMeshes)
            // {
            //     Debug.Log($"Mesh {tileId} => {mesh}");
            // }
            //
            // foreach (var (tileId, prefab) in _generatedPrefabs)
            // {
            //     Debug.Log($"Prefab {tileId} => {prefab.name}");
            // }


            var objectLayer = new GameObject("Terrain");
            var grid = map.GetComponentInChildren<Grid>();
            objectLayer.transform.parent = grid.transform;

            var objectLayerTransform = objectLayer.transform;

            foreach (var (pos2d, tile) in tileMap)
            {
                var go = ComputeTile(pos2d, tile, map);
                if (go == null)
                    continue;

                go.transform.parent = objectLayerTransform;
            }

            // Clean2DTiles(grid);

            // Debug.Log($"map tile height : {map.m_TileHeight}");
            foreach (var tileId in tileMap.Values.Distinct())
            {
                // Debug.Log($"Unique tile id : {tileId}");
            }

            // DestroyPrefabsInScene();
        }

        private void PreparePrefabs(string prefabPrefix, IEnumerable<SuperObject> tiles)
        {
            _modelContainer = GetModelContainer();
            if (_modelContainer == null)
                throw new Exception("Unable to find model container");
            _generatedPrefabs = new();
            _generatedMeshes = new();

            DestroyPrefabsInScene();

            var tileBasePrefab = _modelContainer.TileBasePrefab;
            var prefabTransformParent = GameObject.FindWithTag("GameController").transform;

            var meshRenderer = tileBasePrefab.GetComponentInChildren<MeshRenderer>();
            var texture = meshRenderer.sharedMaterial.mainTexture;
            var singleTileOffset = new DoublePoint((float)tileWidth / texture.width, (float)tileHeight / texture.height);
            var tilesPerLine = texture.width / tileWidth;
            // Debug.Log($"tile dimensions : {tileWidth}x{tileHeight}");
            // Debug.Log($"single tile offset : {singleTileOffset.X}x{singleTileOffset.Y}");

            foreach (var tile in tiles)
            {
                // Debug.Log($"Adding tile {tile}");
                // var prefab = _modelContainer.GetPrefab(prefabPrefix, tile.m_Type);

                GameObject go = GameObject.Instantiate(tileBasePrefab, prefabTransformParent, true);


                var plane = go.transform.GetChild(0).GetChild(0);
                // Debug.Log($"plane name = {plane}");

                var meshFilter = plane.GetComponent<MeshFilter>();
                // var mesh = Object.Instantiate(meshFilter.sharedMesh);
                // var originalMesh = meshFilter.mesh;
                // var mesh = new Mesh
                // {
                // name = "clone",
                // vertices = originalMesh.vertices,
                // triangles = originalMesh.triangles,
                // normals = originalMesh.normals,
                // uv = originalMesh.uv
                // };
                // meshFilter.sharedMesh = mesh; //FIXME does not actually replace the mesh
                var meshCopy = Object.Instantiate(meshFilter.sharedMesh);
                var tileId = tile.m_TileId;
                var offset = new Vector2(
                    (float)((double)tileId % tilesPerLine * singleTileOffset.X),
                    -(float)((int)(tileId / tilesPerLine) * singleTileOffset.Y)
                );
                Vector2[] newUv = meshCopy.uv.Select(uv => uv + offset).ToArray();
                meshCopy.uv = newUv;

                meshFilter.sharedMesh = meshCopy;

                StringBuilder sb = new();
                foreach (var v in meshCopy.uv)
                {
                    sb.Append($"{v}\n");
                }

                go.name = $"{tile.m_TileId} => {offset.x}x{offset.y}";
                go.transform.Translate(tileId % tilesPerLine, 0, -tileId / tilesPerLine);

                AssetDatabase.CreateAsset(meshCopy, MeshPathFromTileId(tileId));
                PrefabUtility.SaveAsPrefabAsset(go, PrefabPathFromTileId(tileId), out var success);
                if (!success)
                {
                    Debug.LogWarning($"Unable to create prefab {tileId}");
                    continue;
                }
                // Debug.Log($"Created {tileId} prefab at {AssetDatabase.GetAssetPath(go)}, should be at {PrefabPathFromTileId(tileId)}");
                _generatedMeshes[tile.m_TileId] = meshCopy;
                _generatedPrefabs[tile.m_TileId] = go;

            }
            AssetDatabase.SaveAssets();
        }

        private void DestroyPrefabsInScene()
        {
            var prefabTransformParent = GameObject.FindWithTag("GameController").transform;
            while (prefabTransformParent.childCount > 0)
            {
                Object.DestroyImmediate(prefabTransformParent.GetChild(0).gameObject);
            }
        }

        private ModelContainer GetModelContainer()
        {
            var loadedObjects = AssetDatabase.FindAssets("t:Prefab");
            foreach (var guid in loadedObjects)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var modelContainer = prefab.GetComponent<ModelContainer>();
                if (modelContainer != null)
                    return modelContainer;
            }

            return null;
        }

        private GameObject ComputeTile(Vector2 coords, SuperObject tile, SuperMap map)
        {
            var tileName = tile.gameObject.GetSuperPropertyValueString(TileNameKey, "");
            var empty = tile.gameObject.GetSuperPropertyValueBool(EmptyKey, false);
            if (empty)
                return null;
            if (!_generatedPrefabs.ContainsKey(tile.m_TileId))
            {
                Debug.LogWarning($"unknown key {tile.m_TileId}");
                return null;
            }

            var go = Object.Instantiate(_generatedPrefabs[tile.m_TileId]);

            var x = coords.x / map.m_TileWidth;
            var z = map.m_Height - 1 - (coords.y / map.m_TileHeight);

            go.transform.position = new Vector3(x, 0, z);
            go.name = $"{go.transform.position} {tileName} {tile.m_Id} {tile.m_TileId}";
            var plane = go.transform.GetChild(0).GetChild(0);
            var meshFilter = plane.GetComponent<MeshFilter>();
            var mesh = meshFilter.sharedMesh;
            var planePrefab = _generatedPrefabs[tile.m_TileId].transform.GetChild(0).GetChild(0).GetComponent<MeshFilter>();
            meshFilter.sharedMesh = planePrefab.sharedMesh;
            // Debug.Log($"computing tile {tile.m_Id}, plane mesh ? {mesh}");
            return go;
        }

        private static string BasicPathFromTileId(uint tileId) => $"Assets/Prefab/Generated/tile{tileId}";
        private static string MeshPathFromTileId(uint tileId) => $"{BasicPathFromTileId(tileId)}.mesh";
        private static string AssetPathFromTileId(uint tileId) => $"{BasicPathFromTileId(tileId)}.asset";
        private static string PrefabPathFromTileId(uint tileId) => $"{BasicPathFromTileId(tileId)}.prefab";

        private void Clean2DTiles(Grid grid)
        {
            var layers = grid.GetComponentsInChildren<SuperTileLayer>();
            foreach (var layer in layers)
            {
                Object.DestroyImmediate(layer.gameObject);
            }
        }
    }
}