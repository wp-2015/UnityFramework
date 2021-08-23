using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABHelper
{
    public class WebAssetManager
    {
        public static List<WebAsset> _WebAssetList = new List<WebAsset>();
        public static WebAsset Load(string url, Action<System.Object> cb)
        {
            WebAsset webAsset = new WebAsset(url, cb);
            _WebAssetList.Add(webAsset);
            return webAsset;
        }

        public static void Update()
        {
            for (int i = 0; i < _WebAssetList.Count; i++)
            {
                _WebAssetList[i].Update();
            }
        }

        public static void UnLoad(WebAsset webAsset)
        {
            webAsset.Unload();
            _WebAssetList.Remove(webAsset);
        }
    }
}
