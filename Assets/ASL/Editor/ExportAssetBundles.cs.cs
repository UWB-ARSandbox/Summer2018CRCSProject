using UnityEditor;

public class CreateAssetBundles
{
    // ERROR TESTING - THIS NEEDS TO GET PHASED INTO MENU.CS AND MENUHANDLER.CS
    /// <summary>
    /// This script needed to be removed a long time ago. Implemented by previous 
    /// ASL team. Irrelevant and faulty.
    /// </summary>
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/ASL/Resources/StreamingAssets/AssetBundlesPC", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        BuildPipeline.BuildAssetBundles("Assets/ASL/Resources/StreamingAssets/AssetBundlesAndroid", BuildAssetBundleOptions.None, BuildTarget.Android);
        //BuildPipeline.BuildAssetBundles("Assets/ASL/Resources/StreamingAssets/AssetBundlesHololens", BuildAssetBundleOptions.None, BuildTarget.WSAPlayer);
        // Version of Hololens build in LW - BuildPipeline.BuildAssetBundles("Assets/Photon Unity Networking/Resources/AssetBundlesHololens", BuildAssetBundleOptions.None, BuildTarget.WSAPlayer);
    }
}
