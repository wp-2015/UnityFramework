using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;

namespace ABHelper
{
    public class UtilsEditor
    {
        /*Start*************************************************数据结构存取文件**************************************************/
        /// <summary>
        /// 数组存入txt文件
        /// </summary>
        /// <param name="txtPath"></param>
        /// <param name="content"></param>
         public static void ListToTxt(string txtPath, List<string> content)
        {
            if (File.Exists(txtPath))
                File.Delete(txtPath);
            using (var s = new StreamWriter(txtPath))
            {
                foreach (var item in content)
                {
                    s.WriteLine(item);
                }
                s.Flush();
                s.Close();
            }
        }

        public static string[] TxtToArray(string path)
        {
            string[] res = new string[0];
            using (var s = new StreamReader(path))
            {
                string line;
                while ((line = s.ReadLine()) != null)
                {
                    if (line == string.Empty)
                        continue;
                    ArrayUtility.Add(ref res, line);
                }
            }
            return res;
        }


        /// <summary>
        /// 字典存入txt文件
        /// </summary>
        public static void DicToTxt(string txtPath, Dictionary<string, string> content)
        {
            if (File.Exists(txtPath))
                File.Delete(txtPath);
            using (var s = new StreamWriter(txtPath))
            {
                foreach (var item in content)
                    s.WriteLine(item.Key + ':' + item.Value);
                s.Flush();
                s.Close();
            }
        }
        /*END*************************************************字典存取txt文件**************************************************/

        /// <summary>
        /// 获取文件的唯一凭证，用来验证文件是否发生变化
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileCertificate(string path)
        {
            try
            {
                using(FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    MD5 md5Hash = MD5.Create();
                    // Convert the input string to a byte array and compute the hash.
                    byte[] data = md5Hash.ComputeHash(fileStream);

                    // Create a new Stringbuilder to collect the bytes
                    // and create a string.
                    StringBuilder sBuilder = new StringBuilder();

                    // Loop through each byte of the hashed data 
                    // and format each one as a hexadecimal string.
                    for (int i = 0; i < data.Length; i++)
                    {
                        sBuilder.Append(data[i].ToString("x2"));
                    }
                    return sBuilder.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(path + "    GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
    }
}
