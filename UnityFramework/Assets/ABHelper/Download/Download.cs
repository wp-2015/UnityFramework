using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace ABHelper
{
    public class Download
    {
        public string URL   { get; set; }
        public string Error { get; private set; }
        public bool IsDone { get; private set; }
        public float Progress { get; private set; }
        private string SavePath { get; set; }
        private Action _FinishCB;

        private enum State
        {
            HeadRequest,
            BodyRequest,
            FinishRequest,
            Completed,
        }
        private UnityWebRequest request { get; set; }
        private State state { get; set; }
        public long maxlen { get; private set; }

        public Download(string url, string savePath, Action cb = null)
        {
            _FinishCB = cb;
            URL = url;
            SavePath = savePath;
        }

        public void Start()
        {
            request = UnityWebRequest.Get(URL);
#if UNITY_2017_1_OR_NEWER
            request.SendWebRequest();
#else
            request.Send();
#endif
            Progress = 0;
            IsDone = false;
        }

        public void Update()
        {
            if (IsDone) return;
            if (request.isHttpError || request.isNetworkError)
            {
                UnityEngine.Debug.Log(request.error);
            }
            Progress = request.downloadProgress;
            if (!request.isDone) return;
            var dir = Path.GetDirectoryName(SavePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            using(FileStream fs = new FileStream(SavePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                var buff = request.downloadHandler.data;
                if (buff != null)
                {
                    fs.Write(buff, 0, buff.Length);
                }
            }
            //FileStream fs = new FileStream(SavePath, FileMode.OpenOrCreate, FileAccess.Write);
            //var buff = request.downloadHandler.data;
            //if (buff != null)
            //{
            //    fs.Write(buff, 0, buff.Length);
            //}
            
            request.Dispose();
            IsDone = true;
            _FinishCB?.Invoke();
        }
    }
}
