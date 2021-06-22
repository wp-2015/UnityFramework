using System;
using System.IO;
using UnityEngine;

namespace ABHelper
{
    class PathUtils
    {
        /// <summary>
        /// 确定路径安全(存在...)
        /// </summary>
        public static void ConfirmDirectoryExist(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        public static void ConfirmFileExist(string filePath)
        {
            if(!File.Exists(filePath))
            {
                
                var fs = File.Create(filePath);
                fs.Close();
            }
        }
        /// <summary>
        /// 绝对路径转为Assets的相对路径
        /// </summary>
        public static string FilePathToAssetPath(string filePath)
        {
            return filePath.Substring(filePath.LastIndexOf("Assets"));
        }

        /// <summary>
        /// Assets的相对路径转为绝对路径
        /// </summary>
        public static string AssetPathToFilePath(string assetPath)
        {
            return Application.dataPath.Replace("Assets", "") + assetPath;
        }

        /// <summary>
        /// 绝对路径转相对路径
        /// 例如: absolutePath = @"E:\Program\company\M1Res4Build\branch", basePath = @"E:\Program\company\M1Client\brach" 
        /// return：../../M1Res4Build/branch
        /// </summary>
        public static string MakeRelativePath(string absolutePath, string basePath)
        {
            absolutePath = absolutePath.Replace("\\", "/");
            basePath = basePath.Replace("\\", "/");
            var pathRoots = absolutePath.Split('/');
            var basePathRoots = basePath.Split('/');
            int commonRootLen = FindCommonRoot(pathRoots, basePathRoots);
            var preRootLen = basePathRoots.Length - commonRootLen;
            string preRootStr = "";
            for (int i = 0; i < preRootLen; i++)
            {
                preRootStr += "../";
            }
            preRootStr += string.Join("/", pathRoots, commonRootLen, pathRoots.Length - commonRootLen);
            return preRootStr;
        }

        public static int FindCommonRoot(string[] pathRoots, string[] basePathRoots)
        {
            string commonRoot = "";
            var len = pathRoots.Length < basePathRoots.Length ? pathRoots.Length : basePathRoots.Length;
            int commonRootLen = 0;
            for (int i = 0; i < len; i++)
            {
                if(pathRoots[i] == basePathRoots[i])
                {
                    commonRootLen++;
                    commonRoot += pathRoots[i];
                }
                else
                {
                    return commonRootLen;
                }
            }
            return commonRootLen;
        }

        /// <summary>
        /// 相对路径转绝对路径
        /// 例如: relativePath = @"../../M1Client/brach" basePath = @"E:\Program\company\M1Res4Build\branch"
        /// return: E:\Program\company\M1Client\brach
        /// </summary>
        public static string MakeAbsolutePath(string relativePath, string basePath)
        {
            relativePath = relativePath.Replace("\\", "/");
            basePath = basePath.Replace("\\", "/");
            var pathRoots = relativePath.Split('/');
            DirectoryInfo dirDI = new DirectoryInfo(basePath);
            for(int i = 0; i < pathRoots.Length; i++)
            {
                var pathRoot = pathRoots[i];
                if(pathRoot == "..")
                {
                    dirDI = dirDI.Parent;
                }
                else
                {
                    dirDI = new DirectoryInfo(dirDI.FullName + "/" + pathRoot);
                }
            }
            return dirDI.FullName;
        }

        /// <summary>
        /// 遍历文件目录下所有文件，包含子目录中的文件
        /// </summary>
        public static void TravelDirectory(string dir, Action<string, string> cb)
        {
            DirectoryInfo dirDI = new DirectoryInfo(dir);
            var files = dirDI.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                cb(files[i].DirectoryName, files[i].Name);
            }

            var dirChilds = dirDI.GetDirectories();
            for (int i = 0; i < dirChilds.Length; i++)
            {
                TravelDirectory(dirChilds[i].FullName, cb);
            }
        }
    }
}
