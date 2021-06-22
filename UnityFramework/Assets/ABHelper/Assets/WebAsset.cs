using System;
using UnityEngine;
using UnityEngine.Networking;

namespace ABHelper
{
    public class WebAsset
    {
        private Action<System.Object> _CB;
        private bool _IsFinish;
#if UNITY_2018_3_OR_NEWER
        private UnityWebRequest _www;
#else
        private WWW _www;
#endif
        public WebAsset(string url, Action<System.Object> cb)
        {
#if UNITY_2018_3_OR_NEWER
            _www = new UnityWebRequest(url);
            _www.downloadHandler = new DownloadHandlerBuffer();
            _www.SendWebRequest();
#else
            _www = new WWW(name);
#endif
            _CB = cb;
        }

        public void Unload()
        {
            _www.Dispose();
            _www = null;
        }

        public bool IsDone()
        {
            return _www.isDone;
        }

        public void Update()
        {
            if(IsDone() && !_IsFinish)
            {
                _CB(_www.downloadHandler.text);
                Stop();
            }
        }

        public void Stop()
        {
            _IsFinish = true;
        }
    }
}
