using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ABHelper
{
    public struct AssetInfo
    {
        public int folderIndex;
        public int bundleIndex;
    }
    public class BundleInfo
    {
        public AssetBundle bundle;
        public BundleInfo(AssetBundle bundle)
        {
            this.bundle = bundle;
        }
        public int ReferenceCount{get;private set;}
        public void AddReference()
        {
            ReferenceCount++;
        }
        public void SubReference()
        {
            ReferenceCount--;
        }
    }
    public class ABManager
    {
        private static string[] _BuildedFoldArray   = new string[0];
        private static string[] _BundleNameArray    = new string[0];
        private static Dictionary<string, BundleInfo> _BundleReference = new Dictionary<string, BundleInfo>();
        /// <summary>
        /// 游戏中用的索引使用资源名字，因为感觉资源名字重复的几率比较小，将来查找资源时比较快
        /// </summary>
        private static Dictionary<string, List<AssetInfo>> _AssetRelevanceBundle = new Dictionary<string, List<AssetInfo>>();

        private static AssetBundleManifest _AssetBundleManifest;
        /// <summary>
        /// 读取全部的bundle已经bundle中的asset文件配置
        /// 便于在实例asset时加载对应的bundle
        /// </summary>
        public static void Init()
        {
            var buildedFolderFileName = Config.GetNativePath(Config.BuildedFolderFileName);
            _BuildedFoldArray = UtilsRuntime.TxtToList(buildedFolderFileName).ToArray();
           
            var bundleNameFile = Config.GetNativePath(Config.BundleNameFileName);
            _BundleNameArray = UtilsRuntime.TxtToList(bundleNameFile).ToArray();

            var assetRelevanceBundleFile = Config.GetNativePath(Config.AssetRelevanceBundle);
            using (var s = new StreamReader(assetRelevanceBundleFile))
            {
                string line;
                while ((line = s.ReadLine()) != null)
                {
                    if (line == string.Empty)
                        continue;
                    var bundleInfos = line.Split(':');
                    if(bundleInfos.Length > 1)
                    {
                        var assetInfos = bundleInfos[1].Split(',');
                        var assetName = assetInfos[1];
                        int folderIndex = int.Parse(assetInfos[0]);
                        int bundleIndex = int.Parse(bundleInfos[0]);
                        List<AssetInfo> assetList;
                        if(!_AssetRelevanceBundle.TryGetValue(assetName, out assetList))
                        {
                            assetList = new List<AssetInfo>();
                            _AssetRelevanceBundle[assetName] = assetList;
                        }
                        assetList.Add(new AssetInfo() { folderIndex = folderIndex, bundleIndex = bundleIndex });
                    }
                }
            }

            AssetBundle abPlatform = AssetBundle.LoadFromFile(Config.GetNativePath(Config.CurrentPlatformName));
            if (null != abPlatform)
            {
                _AssetBundleManifest = abPlatform.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
            else
                Debug.LogError("找不到总资源文件");
        }

        public static UnityEngine.Object Load(string assetName)
        {
            var bundleName = GetAssetBundleName(assetName);
            var ab = LoadAssetBundle(bundleName);
            if(null != ab)
                return ab.LoadAsset<GameObject>(assetName);
            return null;
        }

        public static void UnLoad(string assetName)
        {
            var bundleName = GetAssetBundleName(assetName);
            UnLoadAssetBundle(bundleName);
        }

        public static void UnLoadUnusefulAssetBundle()
        {
            foreach(var bundleInfoDic in _BundleReference)
            {
                var bundleInfo = bundleInfoDic.Value;
                if (bundleInfo.ReferenceCount <= 0)
                {
                    bundleInfo.bundle.Unload(true);
                }
                _BundleReference.Remove(bundleInfoDic.Key);
            }
        }

        private static string GetAssetBundleName(string assetName)
        {
            var dir = Path.GetDirectoryName(assetName).ToLower();
            var name = Path.GetFileName(assetName);
            List<AssetInfo> assetList;
            if (_AssetRelevanceBundle.TryGetValue(name, out assetList))
            {
                if (assetList.Count < 1)
                    Debug.LogError("找不到asset " + assetName);
                //如果这个资源不重名，则可以直接返回这个资源的bundle
                if (assetList.Count == 1)
                {
                    return _BundleNameArray[assetList[0].bundleIndex];
                }
                else
                {
                    var folderIndex = Array.FindIndex(_BuildedFoldArray, (iterator) => { return iterator == dir; });
                    if (folderIndex != -1)
                    {
                        for (int i = 0; i < assetList.Count; i++)
                        {
                            var assetInfo = assetList[i];
                            if (assetInfo.folderIndex == folderIndex)
                            {
                                return _BundleNameArray[assetInfo.bundleIndex];
                            }
                        }
                    }
                    else
                        Debug.LogError("找不到asset " + assetName);
                }
            }
            else
                Debug.LogError("找不到asset " + assetName);
            return null;
        }

        private static void UnLoadAssetBundle(string bundleName, HashSet<string> existCache = null)
        {
            if (null == existCache)
                existCache = new HashSet<string>();
            if (existCache.Contains(bundleName))
                return;
            existCache.Add(bundleName);
            if (null == _AssetBundleManifest)
            {
                Debug.LogError("未读取AssetBundleManifest的依赖文件");
                return;
            }
            var dependencies = _AssetBundleManifest.GetAllDependencies(bundleName);
            for (int i = 0; i < dependencies.Length; i++)
            {
                UnLoadAssetBundle(dependencies[i], existCache);
            }


            if (_BundleReference.TryGetValue(bundleName, out BundleInfo info))
            {
                info.SubReference();
            }
            else
                Debug.LogError(string.Format("想要卸载的bundle：{0} 不存在缓存中", bundleName));
        }

        private static AssetBundle LoadAssetBundle(string bundleName, HashSet<string> existCache = null)
        {
            if (null == existCache)
                existCache = new HashSet<string>();
            if (existCache.Contains(bundleName))
                return null;
            existCache.Add(bundleName);
            if (null == _AssetBundleManifest)
            {
                Debug.LogError("未读取AssetBundleManifest的依赖文件");
                return null;
            }
            var dependencies = _AssetBundleManifest.GetAllDependencies(bundleName);
            for (int i = 0; i < dependencies.Length; i++)
            {
                LoadAssetBundle(dependencies[i], existCache);
            }
            AssetBundle res;
            if (_BundleReference.TryGetValue(bundleName, out BundleInfo info))
            {
                res = info.bundle;
            }
            else
            {
                res = AssetBundle.LoadFromFile(Config.GetNativePath(bundleName));
                info = new BundleInfo(res);
                _BundleReference.Add(bundleName, info);
            }
            info.AddReference();
            return res;
        }

        public static List<string> GetAllAsset()
        {
            List<string> res = new List<string>();
            foreach(var asset in _AssetRelevanceBundle)
            {
                var name = asset.Key;
                var infoList = asset.Value;
                for(int i = 0; i < infoList.Count; i++)
                {
                    res.Add(_BuildedFoldArray[infoList[i].folderIndex] + "/" + name);
                }
            }
            return res;
        }
        public static Dictionary<string, int> GetAllAssetBundleReference()
        {
            Dictionary<string, int> res = new Dictionary<string, int>();
            foreach(var bundleDic in _BundleReference)
            {
                res[bundleDic.Key] = bundleDic.Value.ReferenceCount;
            }
            return res;
        }
    }
}
