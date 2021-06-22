using UnityEditor;

namespace ABHelper
{
    public static partial class Config
    {
        /************************************Path************************************/
        public const string ABManifestAssetPath     = "Assets/ABManifest.asset";
        public const string AssetFileInfo           = "AssetFileInfo.txt";              // version以及 Asset名字:HashCode
        public const string AssetRelevanceBundle    = "AssetRelevanceBundle.txt";       // 记录asset与bundle的包含关系，哪个bundle里面有哪个asset
        public const string BuildedFolderFileName   = "BuildedFolder.txt";              // 记录所有的打包的文件夹路径
        public const string BundleNameFileName      = "BundleName.txt";                 // 记录所有的打包的bundle的名称
        public const string VersionFileName         = "Version.txt";                    // 记录所有需要更新的文件的hash，用来检测该文件是否需要从服务器下载

        public static string GetPlatformName()
        {
            return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
        }
        private static string GetPlatformForAssetBundles(BuildTarget target)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (target)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.WebGL:
                    return "WebGL";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
#if UNITY_2017_3_OR_NEWER
                case BuildTarget.StandaloneOSX:
                    return "OSX";
#else
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                    return "OSX";
#endif
                default:
                    return null;
            }
        }
    }
}
