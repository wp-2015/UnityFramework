using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABHelper
{
    public class DownloadManager
    {
        private static Stack<Download> _Downloads = new Stack<Download>();

        public static Download CurrentDownload { get; private set; }

        public static void Download(string url, string savePath, Action cb = null)
        {
            _Downloads.Push(new Download(url, savePath, cb));
        }

        public static void Update()
        {
            if(null != CurrentDownload && !CurrentDownload.IsDone)
            {
                CurrentDownload.Update();
            }
            else if(_Downloads.Count > 0)
            {
                CurrentDownload = _Downloads.Pop();
                CurrentDownload.Start();
            }
        }
    }
}
