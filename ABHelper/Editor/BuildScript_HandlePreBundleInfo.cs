using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace ABHelper
{
    public partial class BuildScript
    {
        /// <summary>
        /// 读取上一次打包中所有bundle名称，所有文件夹名称，所有文件所在的bundle映射
        /// 这里为了避免需要缓存的数据过大、字符串过多。将重复的文件夹路径、bundle名称做了映射，所以有了保存所有bundle名称的文件、所有文件夹路径的文件
        /// </summary>
        private static void LoadAssetRelevanceBundle(ABManifest abManifest, string bundleRelevanceAssetPath, string buildedfolderFilePath, string bundleNameFilePath)
        {
            if (!File.Exists(bundleRelevanceAssetPath) || !File.Exists(buildedfolderFilePath) || !File.Exists(bundleNameFilePath))
                return;
            string[] buildedFolder = UtilsEditor.TxtToArray(buildedfolderFilePath);

            string[] bundleNameList = UtilsEditor.TxtToArray(bundleNameFilePath);

            using (var s = new StreamReader(bundleRelevanceAssetPath))
            {
                string line;
                while ((line = s.ReadLine()) != null)
                {
                    if (line == string.Empty)
                        continue;
                    var fields = line.Split(':');
                    if (fields.Length > 1)
                    {
                        var bundleName = bundleNameList[int.Parse(fields[0])];
                        var assetName = fields[1];
                        List<string> assetToBuild;
                        if (!BundleRelevanceAsset.TryGetValue(bundleName, out assetToBuild))
                        {
                            assetToBuild = new List<string>();
                            BundleRelevanceAsset[bundleName] = assetToBuild;
                        }
                        var fileEle = assetName.Split(',');
                        if (fileEle.Length > 1)
                        {
                            var fileTurePath = buildedFolder[int.Parse(fileEle[0])] + "/" + fileEle[1];
                            assetToBuild.Add(fileTurePath);
                            AssetRelevanceBundle[fileTurePath] = bundleName;
                        }
                    }

                }
            }
        }

        /// <summary>
        /// 保存这次打包中所有bundle名称，所有文件夹名称，所有文件所在的bundle映射
        /// 这里为了避免需要缓存的数据过大、字符串过多。将重复的文件夹路径、bundle名称做了映射，所以有了保存所有bundle名称的文件、所有文件夹路径的文件
        /// </summary>
        /// <param name="bundleRelevanceAssetPath"></param>
        /// <param name="buildedfolderFilePath"></param>
        /// <param name="bundleNameFilePath"></param>
        public static void SaveAssetRelevanceBundle(string bundleRelevanceAssetPath, string buildedfolderFilePath, string bundleNameFilePath)
        {
            string[] folders = new string[0];
            string[] bundleNames = new string[0];
            List<string> assetRelevanceBundleList = new List<string>();
            foreach (var assetWithBundle in AssetRelevanceBundle)
            {
                var assetName = assetWithBundle.Key;
                var bundleName = assetWithBundle.Value;
                var folderPath = Path.GetDirectoryName(assetName).Replace("\\\\", "/").Replace('\\', '/');
                var folderIndex = ArrayUtility.FindIndex(folders, (iterator) => { return iterator == folderPath; });
                if (folderIndex == -1)
                {
                    ArrayUtility.Add(ref folders, folderPath);
                    folderIndex = folders.Length - 1;
                }
                var bundleNameIndex = ArrayUtility.FindIndex(bundleNames, (iterator) => { return iterator == bundleName; });
                if (bundleNameIndex == -1)
                {
                    ArrayUtility.Add(ref bundleNames, bundleName);
                    bundleNameIndex = bundleNames.Length - 1;
                }
                assetRelevanceBundleList.Add(bundleNameIndex + ":" + folderIndex + "," + Path.GetFileName(assetName));
            }
            UtilsEditor.ListToTxt(bundleNameFilePath, new List<string>(bundleNames));
            UtilsEditor.ListToTxt(buildedfolderFilePath, new List<string>(folders));
            UtilsEditor.ListToTxt(bundleRelevanceAssetPath, assetRelevanceBundleList);
        }

        /// <summary>
        /// 读取上次打包中所有文件和其唯一标识，用来判断这个文件有没有改动，是不是需要在这次打包中
        /// </summary>
        /// <param name="abManifest"></param>
        /// <param name="filePath"></param>
        private static void LoadFileWithHash(ABManifest abManifest, string filePath)
        {
            if (!File.Exists(filePath))
                return;
            using (var s = new StreamReader(filePath))
            {
                string line;
                while ((line = s.ReadLine()) != null)
                {
                    if (line == string.Empty)
                        continue;
                    var fields = line.Split(':');
                    if (fields.Length > 1)
                        FileWithHash.Add(fields[0], fields[1]);
                }
            }
        }
    }
}
