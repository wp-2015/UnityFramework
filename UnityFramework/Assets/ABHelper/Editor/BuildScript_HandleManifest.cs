using UnityEditor;

namespace ABHelper
{
    public partial class BuildScript
    {
        /// <summary>
        /// 生成打包配置文件
        /// </summary>
        public static void MakeABManifest()
        {
            ABManifest abManifest = AssetsUtils.GetAssetFile<ABManifest>(Config.ABManifestAssetPath);
            abManifest.Reset();
            var dirs = abManifest.TargetDirs;
            var ignoreList = abManifest.IgnoreFileType;
            for (int i = 0; i < dirs.Length; i++)
            {
                var dir = PathUtils.AssetPathToFilePath(dirs[i]);
                PathUtils.TravelDirectory(dir, (dirName, Name) =>
                {
                    if (!IsFileValid(Name, ignoreList)) return;
                    dirName = PathUtils.FilePathToAssetPath(dirName).Replace("\\", "/");
                    var abDirs = abManifest.Dirs;
                    var dirIndex = ArrayUtility.FindIndex(abDirs, (string iterator) => { return iterator == dirName; });
                    if (dirIndex == -1)
                    {
                        ArrayUtility.Add(ref abManifest.Dirs, dirName);
                        dirIndex = abManifest.Dirs.Length - 1;
                        abDirs = abManifest.Dirs;
                    }

                    var abNames = abManifest.BundleNames;
                    var bundleName = ((string)dirName).ToLower() + "_ab";
                    var bundleIndex = ArrayUtility.FindIndex(abNames, (string iterator) => { return iterator == bundleName; });
                    if (bundleIndex == -1)
                    {
                        ArrayUtility.Add(ref abManifest.BundleNames, bundleName);
                        bundleIndex = abManifest.BundleNames.Length - 1;
                        abNames = abManifest.BundleNames;
                    }

                    var assets = abManifest.AssetDatas;
                    var assetIndex = ArrayUtility.FindIndex(assets, (AssetData data) =>
                    {
                        return Name == data.Name && dirName == abDirs[data.DirIndex];
                    });

                    if (assetIndex == -1)
                    {
                        var newAssetData = new AssetData();
                        newAssetData.DirIndex = dirIndex;
                        newAssetData.BundleNameInde = bundleIndex;
                        newAssetData.Name = Name;
                        ArrayUtility.Add(ref abManifest.AssetDatas, newAssetData);
                    }
                });
            }
            AssetsUtils.SaveAssetFile(abManifest);
        }
    }
}
