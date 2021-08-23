using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace ABHelper
{
    public enum TargetPlatform { Window = BuildTarget.StandaloneWindows64, Android = BuildTarget.Android, IOS = BuildTarget.iOS }
    public partial class BuildScript
    {
        /// <summary>
        /// 记录已经打好bundle的文件的hash。用来判断新文件是否需要打bundle(如果该文件没有变化则不需要再打一次bundle)
        /// </summary>
        private static Dictionary<string, string> FileWithHash = new Dictionary<string, string>();
        
        /// <summary>
        /// 记录已经打好bundle的文件的bundleName和路径
        /// 这里为了避免需要缓存的数据过大、字符串过多。将重复的文件夹路径做了映射
        /// </summary>
        private static Dictionary<string, List<string>> BundleRelevanceAsset   = new Dictionary<string, List<string>>();
        private static Dictionary<string, string>       AssetRelevanceBundle   = new Dictionary<string, string>();

        public static void CheckPlatformAndBuild()
        {
            var abManifest = AssetsUtils.GetAssetFile<ABManifest>(Config.ABManifestAssetPath);
            if ((int)abManifest.BuildTargetPlatform == (int)EditorUserBuildSettings.activeBuildTarget)
            {
                Build();
            }
            else
            {
                EditorUserBuildSettings.activeBuildTargetChanged = delegate ()
                {
                    Build();
                };
                EditorUserBuildSettings.SwitchActiveBuildTarget(abManifest.TargetGroup, (BuildTarget)abManifest.BuildTargetPlatform);
            }
        }

        public static void Build()
        {
            EditorUtility.DisplayProgressBar("ABHelper", "清理旧配置", 0);
            FileWithHash.Clear();
            BundleRelevanceAsset.Clear();
            AssetRelevanceBundle.Clear();
            EditorUtility.DisplayProgressBar("ABHelper", "重新生成配置文件", 0);
            //打包之前需要确定manifest文件
            BuildScript.MakeABManifest();
            EditorUtility.DisplayProgressBar("ABHelper", "重新生成配置文件", 1);
            var abManifest = AssetsUtils.GetAssetFile<ABManifest>(Config.ABManifestAssetPath);
            //检查配置文件的有效行
            abManifest.CheckAssetDatasValid();

            EditorUtility.DisplayProgressBar("ABHelper", "读取AssetFileInfo文件", 1);
            //读取fileWithHash文件
            var fileWithHashPath = abManifest.OutputPlatformFolder + "/" + Config.AssetFileInfo;
            LoadFileWithHash(abManifest, fileWithHashPath);
            EditorUtility.DisplayProgressBar("ABHelper", "读取上次打包所有资源配置文件", 1);
            //读取Asset与bundle的包含关系文件
            var bundleRelevanceAssetPath = abManifest.OutputPlatformFolder + "/" + Config.AssetRelevanceBundle;
            var buildedfolderFilePath = abManifest.OutputPlatformFolder + "/" + Config.BuildedFolderFileName;
            var bundleNameFilePath = abManifest.OutputPlatformFolder + "/" + Config.BundleNameFileName;
            LoadAssetRelevanceBundle(abManifest, bundleRelevanceAssetPath, buildedfolderFilePath, bundleNameFilePath);
            EditorUtility.DisplayProgressBar("ABHelper", "计算需要增量打包的文件", 0);
            //统计所有需要打包的资源文件
            var assets = abManifest.AssetDatas;
            var dirs = abManifest.Dirs;
            var abName = abManifest.BundleNames;
            var map = new Dictionary<string, List<string>>();
            var len = assets.Length;
            for (int i = 0; i < assets.Length; i++)
            {
                var asset = assets[i];
                var dir = dirs[asset.DirIndex];
                var path = dir + "/" + asset.Name;
                EditorUtility.DisplayProgressBar("ABHelper", "计算需要增量打包的文件 正在进行:" + path, (float)i/ len);
                string md5 = UtilsEditor.GetFileCertificate(PathUtils.AssetPathToFilePath(path));
                var bundleName = abName[asset.BundleNameInde];
                if (FileWithHash.TryGetValue(path, out string oldMd5) && oldMd5 == md5) //该文件没有改动(以前打bundle的记录文件中有该文件，并且md5hash也没有变化)
                {
                    continue;
                }
                // 该文件所在的bundle需要重新打bundle，或者该文件为新的需要打bundle的文件
                List<string> assetToBuild;
                if (!map.TryGetValue(bundleName, out assetToBuild))
                {
                    assetToBuild = new List<string>();
                    map[bundleName] = assetToBuild;
                }
                assetToBuild.Add(path);
                FileWithHash[path] = md5;
            }
            var waitforBuild = new Dictionary<string, List<string>>();
            foreach (var item in map)
            {
                var bundleName = item.Key;
                var assetToBuild = item.Value;
                foreach(var asset in assetToBuild)
                {
                    AssetRelevanceBundle[asset] = bundleName;
                }
                waitforBuild[bundleName] = assetToBuild;
                List<string> oldAssetToBuild;//上次该bundle下的所有asset列表
                if(BundleRelevanceAsset.TryGetValue(bundleName, out oldAssetToBuild)) //需要重新打该bundle下的所有资源
                {
                    foreach(var asset in assetToBuild)
                    {
                        if(!oldAssetToBuild.Contains(asset))
                        {
                            oldAssetToBuild.Add(asset);
                        }
                    }
                    waitforBuild[bundleName] = oldAssetToBuild;
                }
            }

            //需要打包的资源文件放入buildMap中
            List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
            foreach (var ab in waitforBuild)
            {
                foreach(var a in ab.Value)
                {
                    Debug.Log(ab.Key + "    " + a);
                }
                builds.Add(new AssetBundleBuild()
                {
                    assetBundleName = ab.Key,
                    assetNames = ab.Value.ToArray()
                });
            }

            EditorUtility.DisplayProgressBar("ABHelper", "开始打包", 1);
            //调用打包接口
            var outputFolder = abManifest.OutputPlatformFolder;//PathUtils.MakeAbsolutePath(abManifest.OutputFolder, Application.dataPath);
            PathUtils.ConfirmDirectoryExist(outputFolder);
            var manifest = BuildPipeline.BuildAssetBundles(outputFolder, builds.ToArray(), 
                                abManifest.BuildOptions, EditorUserBuildSettings.activeBuildTarget);

            EditorUtility.DisplayProgressBar("ABHelper", "生成本次打包记录文件", 1);
            UtilsEditor.DicToTxt(fileWithHashPath, FileWithHash);
            SaveAssetRelevanceBundle(bundleRelevanceAssetPath, buildedfolderFilePath, bundleNameFilePath);
            MakeAllFileMD5(abManifest.OutputPlatformFolder);
            if (EditorUtility.DisplayDialog("ABHelper", "AssetBundle打包完成, 是否需要打开输出文件", "打开", "取消"))
            {
                EditorUtility.OpenWithDefaultApp(outputFolder);
            }
            EditorUtility.ClearProgressBar();
        }

        public static void MakeAllFileMD5(string path)
        {
            var normalPath = path.Replace(@"\", "/") + "/";
            List<string> versions = new List<string>();
            PathUtils.TravelDirectory(path, (dir, name) =>
            {
                if (name.Contains(Config.AssetFileInfo)) return;
                var fullName = (dir + "/" + name).Replace(@"\", "/");
                versions.Add(fullName.Replace(normalPath, "") + ":" + UtilsEditor.GetFileCertificate(fullName));
            });
            UtilsEditor.ListToTxt(path + "/" + Config.VersionFileName, versions);
        }

        /// <summary>
        /// 需要打包的文件是否合法(是否需要过滤的)
        /// </summary>
        public static bool IsFileValid(string fileName, string[] ignoreList)
        {
            for(int i = 0; i < ignoreList.Length; i++)
            {
                if(fileName.EndsWith(ignoreList[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}