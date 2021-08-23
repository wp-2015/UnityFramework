/// <summary>
/// Bundle的打包配置文件。
/// 采用索引的方式避免出现大量的string GC
/// </summary>

using System;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace ABHelper
{
    /// <summary>
    /// 每个需要打AB的Asset的配置
    /// </summary>
    [Serializable]
    public struct AssetData
    {
        public int      BundleNameInde;     //包含该Asset的Bundle在所有Bundle名称列表种的索引
        public int      DirIndex;           //目录索引
        public string   Name;
    }
     
    /// <summary>
    /// 所有需要打AB的Asset的配置
    /// </summary>
    [CreateAssetMenu(fileName ="ABManifest", menuName = "ABHelper/创建ABManifest")]
    public class ABManifest : ScriptableObject
    {
        public string[]     BundleNames     = new string[0];    //所有的Bundle的名称
        public string[]     Dirs            = new string[0];    //所有的资源目录
        public AssetData[]  AssetDatas      = new AssetData[0]; //所有需要打包的Asset的配置

        public string       OutputFolder    = "";               //打包后输出目录
        public string[]     TargetDirs      = new string[0];    //准备打包的资源目录  
        public string[]     IgnoreFileType  = new string[0];    //打包时忽略的文件类型(后缀)

        public TargetPlatform           BuildTargetPlatform             = TargetPlatform.Window;        //打包的平台
        public BuildAssetBundleOptions  BuildOptions                    = BuildAssetBundleOptions.None; //打包选项

        public BuildTargetGroup TargetGroup
        {
            get
            {
                switch(BuildTargetPlatform)
                {
                    case TargetPlatform.Window:
                        return BuildTargetGroup.Standalone;
                    case TargetPlatform.IOS:
                        return BuildTargetGroup.iOS;
                    case TargetPlatform.Android:
                        return BuildTargetGroup.Android;
                }
                return BuildTargetGroup.Standalone;
            }
        }

        public string OutputPlatformFolder
        {
            get
            {
                // 返回自定义设置的ab文件输出目录加上平台的文件夹路径
                return PathUtils.MakeAbsolutePath(OutputFolder, Application.dataPath) + "/" + Config.GetPlatformName();
            }
        }

        public void Reset()
        {
            BundleNames = new string[0];
            Dirs = new string[0];
            AssetDatas = new AssetData[0];
        }

        /// <summary>
        /// 检索需要打包的所有文件，去除忽略文件。用于后期添加忽略文件配置后操作
        /// </summary>
        public void CheckIgnoreFile()
        {
            for (int i = 0; i < IgnoreFileType.Length; i++)
            {
                for (int j = 0; j < AssetDatas.Length; )
                {
                    if (AssetDatas[j].Name.EndsWith(IgnoreFileType[i]))
                    {
                        ArrayUtility.RemoveAt(ref AssetDatas, j);
                        continue;
                    }
                    else
                    {
                        j++;
                    }
                }
            }
        }

        /// <summary>
        /// Asset配置是否目前合法
        /// </summary>
        public void CheckAssetDatasValid()
        {
            for(int i = 0; i < AssetDatas.Length; )
            {
                var asset = AssetDatas[i];
                var path = Dirs[asset.DirIndex] + "/" + asset.Name;
                if(!File.Exists(PathUtils.AssetPathToFilePath(path)))
                {
                    ArrayUtility.RemoveAt(ref AssetDatas, i);
                }
                else
                {
                    i++;
                }
            }
        }
    }
}