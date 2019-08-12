using UnityEditor;
using System.IO;

public class CreateAssetBundles {
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAssetBundles()
    {
        string dir = "AssetBundles";
        if (Directory.Exists(dir) == false)
        {
            Directory.CreateDirectory("AssetBundles");
        }
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }
}
