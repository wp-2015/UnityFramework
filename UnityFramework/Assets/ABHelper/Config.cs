using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ABHelper
{
    public static class Config
    {
        public const string VersionFileName         = "Version.txt";                // 记录各个bundle的hash，方便对比差异，确定下载
        public const string AssetFileInfo           = "AssetFileInfo.txt";          // version以及 Asset名字:HashCode
        public const string AssetRelevanceBundle    = "AssetRelevanceBundle.txt";   // 记录asset与bundle的包含关系，哪个bundle里面有哪个bundle
        public const string BuildedFolderFileName   = "BuildedFolder.txt";          // 记录所有的打包的文件夹路径
        public const string BundleNameFileName      = "BundleName.txt";             // 记录所有的打包的bundle的名称

        private const string ABFolderName           = "/AssetBundle/";              // bundle文件夹名称
        /// <summary>
        /// rootPath确定到root
        /// </summary>
        public static string GetServerPath(string rootPath, string fileName)
        {
            return rootPath + "/" + CurrentPlatformName + "/" + fileName;
        }
        /// <summary>
        /// 本地保存路径
        /// </summary>
        private static string _NativePathRoot;
        /// <summary>
        /// 保存路径确定为：persistentDataPath + "AssetBundle" + 平台 + path
        /// </summary>
        public static string GetNativePath(string path)
        {
            if(string.IsNullOrEmpty(_NativePathRoot))
            {
                _NativePathRoot = Application.persistentDataPath + ABFolderName + CurrentPlatformName;
            }

            return _NativePathRoot + "/" + path;
            //return Path.Combine(Application.persistentDataPath, GetPlatform(Application.platform));//这里使用Path.Combine结合符为\而不是/
        }

        private static string _CurrentPlatformName;
        public static string CurrentPlatformName
        {
            get
            {
                if(string.IsNullOrEmpty(_CurrentPlatformName))
                {
                    _CurrentPlatformName = GetPlatform(Application.platform);
                }
                return _CurrentPlatformName;
            }
        }
        public static string GetPlatform(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return "Windows";
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                    return "OSX";
                default:
                    return null;
            }
        }
    }
}
