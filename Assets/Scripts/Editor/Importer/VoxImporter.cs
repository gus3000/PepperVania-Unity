using UnityEditor.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, "vox")]
public class VoxImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        Debug.Log($"Importing asset {ctx}");
    }
}