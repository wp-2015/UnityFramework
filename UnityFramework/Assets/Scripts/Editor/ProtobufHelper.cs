using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

public class ProtobufHelper
{
    [MenuItem("Tools/生成Protobuf协议代码")]
    static void SyncCode()
    {
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            EditorUtils.RunCommand("../Common/protobuf", "encode2C.bat");
        }
        else
        {
            "bash ./gen_client.sh".Bash("../Common/protobuf");
        }
    }
}