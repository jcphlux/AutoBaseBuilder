using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class MultiPlatformExportAssetBundles_01 //Based off the original by Xyth - https://github.com/7D2D/Templates-and-Utilities/blob/29758ef38db5dc291004c3b5facce826a45b6df9/MultiPlatformExportAssetBundles.zip
{
    [MenuItem("Assets/Build Normal (Compressed) AssetBundle From Selection - Smallest - Slowest")]
    static void ExportResource()
    {
        // Bring up save panel
        string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "unity3d");
        if (path.Length != 0)
        {
            // include the following Graphic APIs
            PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, new GraphicsDeviceType[] { GraphicsDeviceType.Direct3D11, GraphicsDeviceType.OpenGLCore, GraphicsDeviceType.Vulkan });

            // Build the resource file from the active selection.
            Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

            #pragma warning disable CS0618 // Type or member is obsolete: Thanks to Eric Beaudoin for this bit
            BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, BuildTarget.StandaloneWindows64);
            Selection.objects = selection;
            #pragma warning restore CS0618 //  Type or member is obsolete
        }
    }
}

public class MultiPlatformExportAssetBundles_02
{
    [MenuItem("Assets/Build Chunk-Compressed AssetBundle From Selection  - Smaller - Fast Initial Load - Assets Loaded  as Needed")]
    static void ExportResource()
    {
        string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "unity3d");
        if (path.Length != 0)
        {
            PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, new GraphicsDeviceType[] { GraphicsDeviceType.Direct3D11, GraphicsDeviceType.OpenGLCore, GraphicsDeviceType.Vulkan });

            Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

            #pragma warning disable CS0618
            BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
            Selection.objects = selection;
            #pragma warning restore CS0618
        }
    }
}

public class MultiPlatformExportAssetBundles_03
{
    [MenuItem("Assets/Build Uncompressed AssetBundle From Selection - Largest - Fastest")]
    static void ExportResource()
    {
        string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "unity3d");
        if (path.Length != 0)
        {
            PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, new GraphicsDeviceType[] { GraphicsDeviceType.Direct3D11, GraphicsDeviceType.OpenGLCore, GraphicsDeviceType.Vulkan });

            Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

            #pragma warning disable CS0618
            BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows64);
            Selection.objects = selection;
            #pragma warning restore CS0618
        }
    }
}