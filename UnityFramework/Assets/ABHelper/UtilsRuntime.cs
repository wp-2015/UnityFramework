using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABHelper
{
    public class UtilsRuntime
    {
        /// <summary>
        /// txt文件读取成字典
        /// </summary>
        public static void TxtToDic(string txtPath, IDictionary<string, string> content)
        {
            if (content == null)
                throw new ArgumentNullException("content");
            if (!File.Exists(txtPath))
                return;
            using (var s = new StreamReader(txtPath))
            {
                string line;
                while ((line = s.ReadLine()) != null)
                {
                    if (line == string.Empty)
                        continue;
                    var fields = line.Split(':');
                    if (fields.Length > 1)
                        content.Add(fields[0], fields[1]);
                }
            }
        }

        public static void MakeFileSafe(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
        }

        public static List<string> TxtToList(string path)
        {
            List<string> res = new List<string>();
            using (var s = new StreamReader(path))
            {
                string line;
                while ((line = s.ReadLine()) != null)
                {
                    if (line == string.Empty)
                        continue;
                    res.Add(line);
                }
            }
            return res;
        }
    }
}
