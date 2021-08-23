using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RaycastTargetChecker : EditorWindow
{
    [MenuItem("Tools/RaycastTargetChecker", false, 11)]
    static void Do()
    {
        string[] allPath = AssetDatabase.FindAssets("*", new string[] { "Assets" });
        bool changed = false;
        string szLog = DateTime.Now.ToString();
        for (int i = 0; i < allPath.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(allPath[i]);
            EditorUtility.DisplayProgressBar("1", path, (float)i / allPath.Length);
            if (path.EndsWith("prefab"))
            {
                changed = false;
                var obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                //text
                var texts = obj.GetComponentsInChildren<Text>();
                foreach(var text in texts)
                {
                    if(text.raycastTarget)
                    {
                        changed = CheckType(obj, text, ref szLog, path) ? true : changed;
                    }
                }
                //Image
                var images = obj.GetComponentsInChildren<Image>();
                foreach(var image in images)
                {
                    if(image.raycastTarget)
                        changed = CheckType(obj, image, ref szLog, path) ? true : changed;
                }

                if (changed)
                {
                    var instant = GameObject.Instantiate(obj);
                    PrefabUtility.SaveAsPrefabAsset(instant, path);
                    GameObject.DestroyImmediate(instant);
                }
            }
        }
        EditorUtility.ClearProgressBar();
        var logDir = Application.dataPath + "/../Logs";
        DirectoryInfo mydir = new DirectoryInfo(logDir);
        if(!mydir.Exists)
        {
            mydir.Create();
        }

        var filePath = logDir + "/RaycastTargetChecker.csv";
        Debug.LogError(filePath);
        if (!File.Exists(filePath))
        {
            File.Create(filePath);
        }
        File.WriteAllText(filePath, szLog);
        System.Diagnostics.Process.Start(filePath);
    }

    static bool CheckType(GameObject obj, Graphic target, ref string szLog, string path)
    {
        bool res = false;
        var handles = target.gameObject.GetComponents<IEventSystemHandler>();
        if (handles.Length <= 0)
        {
            szLog += "\n" + path + "," + GetPathFromRootNode(target.transform, obj.transform);
            res = true;
            //自动修改
            target.raycastTarget = false;
        }
        
        return res;
    }

    static string GetPathFromRootNode(Transform target, Transform root)
    {
        var iterator = target;
        string res = iterator.name;
        while(iterator != null && iterator != root)
        {
            iterator = iterator.parent;
            res = string.Format("{0}/{1}", iterator.name, res);
        }
        return res;
    }
}
