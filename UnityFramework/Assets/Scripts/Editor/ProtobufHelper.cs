using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class ProtobufHelper
{
    [MenuItem("Tools/生成Protobuf协议代码")]
    static void SyncCode()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            EditorUtils.RunCommand("../Common/protobuf", "encode2C.bat");
        }
        else
        {
            "bash ./gen_client.sh".Bash("../Common/protobuf");
        }
        GenertateType();
    }

    private static void GenertateType()
    {
        string path = Application.dataPath + "../../../Common/protobuf/Proto/ProtoType.proto";
        string[] lines = File.ReadAllLines(path);
        string lineText = @"{0}MSGTYPE.{1}, typeof({2}){3},";
        string types = "";
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains("S2C"))
            {
                string temp = lines[i];
                temp = temp.Replace("\t", "");
                temp = temp.Replace(" ", "");
                temp = temp.Replace("_", "");
                Debug.LogError(temp);
                string[] splits = temp.Split('=');
                Debug.LogError(splits[0]);
                Debug.LogError(lineText);
                types += string.Format(lineText,"{", splits[0], splits[0].Replace("Mt", ""), "}") + '\n';
            }
        }

        string text = @"using System;
using System.Collections.Generic;
using Protocol;

/// <summary>
/// 所有需要发送和接收的协议都要在这个注册
/// </summary>
public static class MessageTypeMap
{
    static public Dictionary<MSGTYPE, Type> messageTypeDic = new Dictionary<MSGTYPE, Type>()
    {" + '\n' + '\t' + '\t'
                    + types + '\n' +
     "\t" +   @"};"+"\n" + "}";
        File.WriteAllText(Application.dataPath + @"/Scripts/PB/MessageTypeMap.cs", text);
    }

    private static string _setTextTransform(string str)
    {
        str = str.ToLower();
        string[] temp = str.Split('_');
        string result = string.Empty;
        for (int i = 0; i < temp.Length; i++)
        {
            result += temp[i].Substring(0, 1).ToUpper() + temp[i].Substring(1);
        }

        return result;
    }
}