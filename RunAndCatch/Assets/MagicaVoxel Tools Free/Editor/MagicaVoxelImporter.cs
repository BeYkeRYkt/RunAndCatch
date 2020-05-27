using UnityEditor.Experimental.AssetImporters;

namespace MagicaVoxelTools
{
    [ScriptedImporter(1, "vox")]
    public class MagicaVoxelImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            //Not really going to auto import anything, let user handle what needs to be imported
        }
    }
}