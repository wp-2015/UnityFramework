using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AutoBuild : EditorWindow
{
    [MenuItem("Tools/Android一键打包", false, 100)]
    static void Open()
    {
        var editorWindow = GetWindow<AutoBuild>(false, "一键打包");
        editorWindow.position = new Rect(500, 200, 382, 106);
        if (editorWindow.position.position == Vector2.zero)
        {
            _ = Screen.currentResolution;
        }
        editorWindow.Show();
    }

    private string[] PlatformsName = new string[] { "Android", "Windows", "IOS" };
    private int iCurrentPlatformsIndex;
    private string szSourceSDKFolderName = "Sources";

    private string szCustomBuildPipelinePath;
    private string szSDKPluginsRootPath;
    private string szPluginsPath;

    private static DirectoryInfo[] ChannelFolders;

    private void OnEnable()
    {
        szCustomBuildPipelinePath = Application.dataPath.Replace("Assets", "CustomBuildPipeline");
        szPluginsPath = Application.dataPath + "/Plugins";
        iCurrentPlatformsIndex = PlayerPrefs.GetInt("CurrentPlatformsIndex", 0);
        szSDKPluginsRootPath = string.Format("{0}/SDKPlugins", szCustomBuildPipelinePath);
        DirectoryInfo and = new DirectoryInfo(szSDKPluginsRootPath);
        ChannelFolders = and.GetDirectories();
    }

    private void OnGUI()
    {
        GUILayout.Label("平台选择");
        GUILayout.BeginHorizontal();
        for(int i = 0; i < PlatformsName.Length; i++)
        {
            var index = i;
            GUI.enabled = iCurrentPlatformsIndex != index;
            if (GUILayout.Button(PlatformsName[index]))
            {
                iCurrentPlatformsIndex = index;
                PlayerPrefs.SetInt("CurrentPlatformsIndex", iCurrentPlatformsIndex);
            }
            GUI.enabled = true;
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(6);
        GUILayout.Label("渠道选择");
        
        int colCount = 2;
        var length = ChannelFolders.Length;
        for (int i = 0; i < length; i+=colCount)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; (j < colCount) && (i + j < length); j++)
            {
                var dirName = ChannelFolders[i + j].Name;
                if (GUILayout.Button(dirName, GUILayout.Width(position.width / 2 - 6)))
                {
                    var pluginsPath = string.Format("{0}/{1}", szPluginsPath, PlatformsName[iCurrentPlatformsIndex]);
                    ClearFolder(pluginsPath);
                    var sdkPluginsSourcesPath = string.Format("{0}/{1}/{2}", szSDKPluginsRootPath, szSourceSDKFolderName, PlatformsName[iCurrentPlatformsIndex]);
                    CopyFolder(sdkPluginsSourcesPath, pluginsPath);

                    if (dirName != szSourceSDKFolderName)
                    {
                        var sdkPluginsPath = string.Format("{0}/{1}/{2}", szSDKPluginsRootPath, dirName, PlatformsName[iCurrentPlatformsIndex]);
                        CopyFolder(sdkPluginsPath, pluginsPath);
                    }

                    BuildAPP();
                }
            }
            GUILayout.EndHorizontal();
        }
    }

    private void BuildAPP()
    {
        var scenes = EditorBuildSettings.scenes;
        List<string> packedScene = new List<string>();
        foreach (var scene in scenes)
        {
            if (scene.enabled)
            {
                packedScene.Add(scene.path);
            }
        }
        var platformName = PlatformsName[iCurrentPlatformsIndex];
        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = packedScene.ToArray(),
            locationPathName = string.Format("{0}/App/{1}/App{2}", szCustomBuildPipelinePath, platformName, GetExtension(platformName)),
            //assetBundleManifestPath = GetAssetBundleManifestFilePath(),
            target = GetBuildTarget(PlatformsName[iCurrentPlatformsIndex]),//BuildTarget.Android,//EditorUserBuildSettings.activeBuildTarget,
            options = BuildOptions.None,//EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None
        };
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    private void ClearFolder(string folder)
    {
        var dir = CheckFolder(folder);
        dir.Delete(true);
        dir.Create();
    }

    private DirectoryInfo CheckFolder(string folder)
    {
        DirectoryInfo dir = new DirectoryInfo(folder);
        if (!dir.Exists)
            dir.Create();
        return dir;
    }

    private BuildTarget GetBuildTarget(string platform)
    {
        switch(platform)
        {
            case "Android":
                return BuildTarget.Android;
        }
        return BuildTarget.NoTarget;
    }

    private string GetExtension(string platform)
    {
        switch (platform)
        {
            case "Android":
                return ".apk";
                
        }
        return "";
    }

    public static void CopyFolder(string sourceFolder, string destFolder)
    {
        //如果目标路径不存在,则创建目标路径
        if (!System.IO.Directory.Exists(destFolder))
        {
            System.IO.Directory.CreateDirectory(destFolder);
        }
        //得到原文件根目录下的所有文件
        string[] files = System.IO.Directory.GetFiles(sourceFolder);
        foreach (string file in files)
        {
            string name = System.IO.Path.GetFileName(file);
            string dest = System.IO.Path.Combine(destFolder, name);
            System.IO.File.Copy(file, dest, true);//复制文件
        }
        //得到原文件根目录下的所有文件夹
        string[] folders = System.IO.Directory.GetDirectories(sourceFolder);
        foreach (string folder in folders)
        {
            string name = System.IO.Path.GetFileName(folder);
            string dest = System.IO.Path.Combine(destFolder, name);
            CopyFolder(folder, dest);//构建目标路径,递归复制文件
        }
    }
}
