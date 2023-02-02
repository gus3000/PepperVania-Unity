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
            _modelContainer = GetModelContainer();
            if (_modelContainer == null)
                throw new Exception("Unable to find model container");

            var map = args.ImportedSuperMap;
            var importer = args.AssetImporter;
            Debug.Log("----------------------- IMPORT -----------------------");
            Debug.Log($"Map : {map.m_Width}x{map.m_Height} : {map.m_TiledVersion}");
            tileWidth = map.m_TileWidth;
            tileHeight = map.m_TileHeight;

            var grid = map.GetComponentInChildren<Grid>();
            var gridTransform = grid.transform;
            var terrainGameObject = new GameObject("Terrain")
            {
                transform =
                {
                    parent = gridTransform
                }
            };

            var tileLayerTransform = terrainGameObject.transform;
            var tiledObjectContainer = new GameObject("Objects")
            {
                transform =
                {
                    parent = gridTransform
                }
            };

            var tileMap = new Dictionary<Vector2, SuperObject>();

            var tiles = map.GetComponentsInChildren<SuperObject>();
            foreach (var superObject in tiles)
            {
                var tileLayer = superObject.GetComponentInParent<SuperTileLayer>();
                var objectLayer = superObject.GetComponentInParent<SuperObjectLayer>();

                if (tileLayer != null && tileLayer.m_TiledName == LayerToKeep)
                {
                    tileMap.Add(new Vector2(superObject.m_X, superObject.m_Y), superObject);
                }
                else if (objectLayer != null)
                {
                    // Debug.Log($"Attempting to summon object {superObject} of type {superObject.m_Type}");
                    var objectPrefab = _modelContainer.GetObject(superObject.m_Type);
                    // Debug.Log($"attaching script {superObject.m_Type} : {script.GetClass()}");
                    Debug.Log($"putting object at {superObject.m_X},{superObject.m_Y}");
                    if (objectPrefab == null)
                    {
                        Debug.LogWarning($"Attempt to summon object of type {superObject.m_Type} failed because there's no such prefab");
                        continue;
                    }
                    
                    // Debug.Log($"{superObject.name} properties : {String.Join(", ",properties.m_Properties)}");

                    var go = Object.Instantiate(
                        objectPrefab,
                        GetObjectPosition(superObject, map),
                        Quaternion.Euler(0, -superObject.m_Rotation, 0),
                        tiledObjectContainer.transform
                    );
                    
                    var properties = superObject.GetComponent<SuperCustomProperties>();
                    foreach (var prop in properties.m_Properties)
                    {
                        go.BroadcastProperty(prop);
                    }
                }

                // Debug.Log($"tile {tile} => {tile.m_TileId}");
                // Debug.Log($"type : {tile.m_Type}");
            }


            var distinctTiles = tileMap.Values.Distinct(new TileComparerByTileId()).ToList();
            // Debug.Log($"total tiles = {tileMap.Count}, distinct = {distinctTiles.Count()}");
            PreparePrefabs(PrefabPrefix, distinctTiles);

            foreach (var (pos2d, tile) in tileMap)
            {
                var go = ComputeTile(pos2d, tile, map);
                if (go == null)
                    continue;

                go.transform.parent = tileLayerTransform;
            }

            Clean2DTiles(grid);
            
            // Debug.Log($"map tile height : {map.m_TileHeight}");
            // foreach (var tileId in tileMap.Values.Distinct())
            // {
            // Debug.Log($"Unique tile id : {tileId}");
            // }

            DestroyPrefabsInScene();
        }


        private void PreparePrefabs(string prefabPrefix, IEnumerable<SuperObject> tiles)
        {
            _generatedPrefabs = new();
            _generatedMeshes = new();

            DestroyPrefabsInScene();

            var tileBasePrefab = _modelContainer.TileBasePrefab;
            var prefabTransformParent = GameObject.FindWithTag("GameController").transform;

            var meshRenderer = tileBasePrefab.GetComponentInChildren<MeshRenderer>();
            var texture = meshRenderer.sharedMaterial.mainTexture;
            var singleTileOffset = new DoublePoint((float)tileWidth / texture.width, (float)tileHeight / texture.height);
            var tilesPerLine = texture.width / tileWidth;

            foreach (var tile in tiles)
            {
                var empty = tile.gameObject.GetSuperPropertyValueBool(EmptyKey, false);
                GameObject go;
                if (empty)
                {
                    _generatedPrefabs[tile.m_TileId] = _modelContainer.GetObject(tile.m_Type);
                    Debug.Log($"preparing empty object prefab {_generatedPrefabs[tile.m_TileId].name}");
                    continue;
                }

                go = Object.Instantiate(tileBasePrefab, prefabTransformParent, true);

                var plane = go.transform.GetChild(0).GetChild(0);

                var meshFilter = plane.GetComponent<MeshFilter>();
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

                _generatedMeshes[tile.m_TileId] = meshCopy;


                // Debug.Log($"Created {tileId} prefab at {AssetDatabase.GetAssetPath(go)}, should be at {PrefabPathFromTileId(tileId)}");
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
            if (empty && tile.m_Type == "")
                return null;
            if (!_generatedPrefabs.ContainsKey(tile.m_TileId))
            {
                Debug.LogWarning($"unknown key {tile.m_TileId}");
                return null;
            }

            var go = Object.Instantiate(_generatedPrefabs[tile.m_TileId]);

            go.transform.position = GetTilePosition(coords, map);
            go.name = $"{go.transform.position} {tileName} {tile.m_Id} {tile.m_TileId}";
            if (empty)
            {
                Debug.Log($"Not skipping tile of type {tile.m_Type}");
                return go;
            }
            var plane = go.transform.GetChild(0).GetChild(0);
            var meshFilter = plane.GetComponent<MeshFilter>();
            var mesh = meshFilter.sharedMesh;
            var planePrefab = _generatedPrefabs[tile.m_TileId].transform.GetChild(0).GetChild(0).GetComponent<MeshFilter>();
            meshFilter.sharedMesh = planePrefab.sharedMesh;
            // Debug.Log($"computing tile {tile.m_Id}, plane mesh ? {mesh}");
            return go;
        }

        private Vector3 GetObjectPosition(SuperObject superObject, SuperMap map)
        {
            var xDelta = superObject.m_Width / 2;
            var yDelta = superObject.m_Height / 2;

            // var x = (superObject.m_X + xDelta) / map.m_TileWidth;
            // var z = map.m_Height - ((superObject.m_Y - yDelta) / map.m_TileHeight);
            var x = (superObject.m_X + Mathf.Cos(superObject.m_Rotation) * xDelta) / map.m_TileWidth;
            var z = map.m_Height - (superObject.m_Y + Mathf.Sin(superObject.m_Rotation) * yDelta) / map.m_TileHeight;


            return new Vector3(x, 0, z);
        }

        private Vector3 GetTilePosition(Vector2 coords, SuperMap map)
        {
            var x = coords.x / map.m_TileWidth;
            var z = map.m_Height - 1 - (coords.y / map.m_TileHeight);

            return new Vector3(x, 0, z);
        }

        private static string BasicPathFromTileId(uint tileId) => $"Assets/Prefab/Generated/tile{tileId}";
        private static string MeshPathFromTileId(uint tileId) => $"{BasicPathFromTileId(tileId)}.mesh";
        private static string AssetPathFromTileId(uint tileId) => $"{BasicPathFromTileId(tileId)}.asset";
        private static string PrefabPathFromTileId(uint tileId) => $"{BasicPathFromTileId(tileId)}.prefab";

        private void Clean2DTiles(Grid grid)
        {
            var tileLayers = grid.GetComponentsInChildren<SuperTileLayer>();
            foreach (var layer in tileLayers)
            {
                Object.DestroyImmediate(layer.gameObject);
            }

            var objectLayers = grid.GetComponentsInChildren<SuperObjectLayer>();
            foreach (var layer in objectLayers)
            {
                Object.DestroyImmediate(layer.gameObject);
            }
        }
    }
}