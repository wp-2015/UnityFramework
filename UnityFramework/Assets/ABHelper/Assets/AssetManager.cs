using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ABHelper
{
    public class AssetManager
    {
        public static UnityEngine.Object Load(string path)
        {
#if UNITY_EDITOR
            if(!IsUseAssetBundle)
            {
                 return Editor.Instantiate<GameObject>(AssetDatabase.LoadAssetAtPath<GameObject>(path));
            }
#endif
            return ABManager.Load(path);
        }

        public static void UnLoad(string path)
        {
#if UNITY_EDITOR
            if (IsUseAssetBundle)
#endif
            {
                ABManager.UnLoad(path);
            }
        }

        public static void UnLoadAllUnuseAsset()
        {
#if UNITY_EDITOR
            if (IsUseAssetBundle)
#endif
            {
                ABManager.UnLoadUnusefulAssetBundle();
            }
        }

#if UNITY_EDITOR
        [MenuItem("ABHelper/资源加载方式/使用AssetBundle", true)]
        public static bool UseBundle()
        {
            return !IsUseAssetBundle && !Application.isPlaying;
        }
        [MenuItem("ABHelper/资源加载方式/使用AssetBundle", false)]
        public static void UseBundleShow()
        {
            IsUseAssetBundle = true;
        }
        [MenuItem("ABHelper/资源加载方式/不使用AssetBundle", true)]
        public static bool NotUseBundle()
        {
            return IsUseAssetBundle && !Application.isPlaying;
        }
        [MenuItem("ABHelper/资源加载方式/不使用AssetBundle", false)]
        public static void NotUseBundleShow()
        {
            IsUseAssetBundle = false;
        }

        private static bool IsUseAssetBundle
        {
            get
            {
                return PlayerPrefs.GetString("UseAssetBundle") == "ON";
            }
            set
            {
                string res = value ? "ON" : "OFF";
                PlayerPrefs.SetString("UseAssetBundle", res);
            }
        } 
#endif
    }
}
