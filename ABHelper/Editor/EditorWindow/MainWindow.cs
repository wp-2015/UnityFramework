using System;
using UnityEditor;
using UnityEngine;

namespace ABHelper
{
    public class MainWindow : EditorWindow
    {
        private string _OutputFolderAbsolutePath    = "";
        private string _BuildOptionsShowStr         = "";
        public static void Open()
        {
            var editorWindow = GetWindow<MainWindow>(false, "ABHelper");
            editorWindow.position = new Rect(500, 200, 800, 600);
            if (editorWindow.position.position == Vector2.zero)
            {
                Resolution res = Screen.currentResolution;
            }
            editorWindow.Show();
        }

        public ABManifest _ABManifest;

        private void OnEnable()
        {
            _ABManifest = AssetsUtils.GetAssetFile<ABManifest>(Config.ABManifestAssetPath);
            _OutputFolderAbsolutePath = PathUtils.MakeAbsolutePath(_ABManifest.OutputFolder, Application.dataPath);
            _BuildOptionsShowStr = _ABManifest.BuildOptions.ToString();
        }

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("清空配置文件"))
            {
                ArrayUtility.Clear(ref _ABManifest.BundleNames);
                ArrayUtility.Clear(ref _ABManifest.AssetDatas);
                AssetsUtils.SaveAssetFile(_ABManifest);
            }
            if(GUILayout.Button("重新读取配置文件"))
            {
                _ABManifest = AssetsUtils.GetAssetFile<ABManifest>(Config.ABManifestAssetPath);
            }
            EditorGUILayout.EndHorizontal();

            #region/*******************************需要打包的文件夹*******************************/ 
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("需要打包的资源文件夹");
            EditorGUILayout.EndHorizontal();

            for(int i = 0; i < _ABManifest.TargetDirs.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUIUtils.MakeGUITab(20);
                if (GUILayout.Button("选择文件夹", GUILayout.Width(88)))
                {
                    var dir = EditorUtility.OpenFolderPanel("选择需要打包的资源文件夹", "Assets", "");
                    _ABManifest.TargetDirs[i] = PathUtils.FilePathToAssetPath(dir);

                    AssetsUtils.SaveAssetFile(_ABManifest);
                }
                if (GUILayout.Button("删除", GUILayout.Width(60)))
                {
                    ArrayUtility.RemoveAt<string>(ref _ABManifest.TargetDirs, i);
                    AssetsUtils.SaveAssetFile(_ABManifest);
                } 
                else
                {
                    EditorGUILayout.LabelField(_ABManifest.TargetDirs[i]);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            GUIUtils.MakeGUITab(10);
            if (GUILayout.Button("+", GUILayout.Width(100)))
            {
                ArrayUtility.Add<string>(ref _ABManifest.TargetDirs, "");
                AssetsUtils.SaveAssetFile(_ABManifest);
            }
            EditorGUILayout.EndHorizontal();
            #endregion/*******************************需要打包的文件夹*******************************/ 

            EditorGUILayout.Space();
            
            #region/*******************************输出Bundle文件夹*******************************/
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("选择输出文件夹", GUILayout.Width(100)))
            {
                var folder = EditorUtility.OpenFolderPanel("AB文件输出路径选择", "", "");
                _ABManifest.OutputFolder = PathUtils.MakeRelativePath(folder, Application.dataPath);
                AssetsUtils.SaveAssetFile(_ABManifest);
                _OutputFolderAbsolutePath = PathUtils.MakeAbsolutePath(_ABManifest.OutputFolder, Application.dataPath);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUIUtils.MakeGUITab(30);
            EditorGUILayout.LabelField("绝对路径:", GUILayout.Width(66));
            EditorGUILayout.LabelField(_OutputFolderAbsolutePath);
            GUIUtils.MakeGUITab(5);
            EditorGUILayout.LabelField("相对于Assets文件夹的相对路径:", GUILayout.Width(166));
            EditorGUILayout.LabelField(_ABManifest.OutputFolder);
            EditorGUILayout.EndHorizontal();
            #endregion/*******************************输出Bundle文件夹*******************************/ 

            EditorGUILayout.Space();

            #region/*******************************忽略文件后缀*******************************/
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("忽略文件后缀", GUILayout.Width(66));
            if (GUILayout.Button("+", GUILayout.Width(100)))
            {
                ArrayUtility.Add<string>(ref _ABManifest.IgnoreFileType, "");
                AssetsUtils.SaveAssetFile(_ABManifest);
            }
            if (GUILayout.Button("重新过滤忽略文件", GUILayout.Width(100)))
            {
                _ABManifest.CheckIgnoreFile();
                AssetsUtils.SaveAssetFile(_ABManifest);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUIUtils.MakeGUITab(30);
            var ignoreFileType = _ABManifest.IgnoreFileType;
            for(int i = 0; i < ignoreFileType.Length; i++)
            {
                ignoreFileType[i] = EditorGUILayout.TextField(ignoreFileType[i], GUILayout.Width(50));
                if(GUILayout.Button("-", GUILayout.Width(30)))
                {
                    ArrayUtility.RemoveAt(ref _ABManifest.IgnoreFileType, i);
                    AssetsUtils.SaveAssetFile(_ABManifest);
                }
            }
            
            EditorGUILayout.EndHorizontal();
            #endregion/*******************************忽略文件后缀*******************************/ 

            EditorGUILayout.Space();

            #region/*******************************打包选项*******************************/
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("打包选项");

            EditorGUILayout.LabelField("添加选项", GUILayout.Width(50));
            var buildOption = _ABManifest.BuildOptions;
            buildOption = (BuildAssetBundleOptions)EditorGUILayout.EnumFlagsField(buildOption);
            if (buildOption != _ABManifest.BuildOptions)
            {
                _ABManifest.BuildOptions = buildOption;
                AssetsUtils.SaveAssetFile(_ABManifest);
                _BuildOptionsShowStr = buildOption.ToString();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUIUtils.MakeGUITab(30);
            EditorGUILayout.LabelField(_BuildOptionsShowStr);
            EditorGUILayout.EndHorizontal();
            #endregion/*******************************打包选项*******************************/ 

            EditorGUILayout.Space();

            #region/*******************************打包平台选择*******************************/
            var targetPlatform = _ABManifest.BuildTargetPlatform;
            targetPlatform = (TargetPlatform)EditorGUILayout.EnumPopup("打包平台: ", targetPlatform);
            if(targetPlatform != _ABManifest.BuildTargetPlatform)
            {
                _ABManifest.BuildTargetPlatform = targetPlatform;
                AssetsUtils.SaveAssetFile(_ABManifest);
            }
            #endregion/*******************************打包平台选择*******************************/ 

            EditorGUILayout.Space();

            #region/*******************************打包功能*******************************/
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("生成配置文件"))
            {
                BuildScript.MakeABManifest();
            }
            if (GUILayout.Button("预览配置文件"))
            {
                _isShowConfig = !_isShowConfig;
            }
            if (GUILayout.Button("生成Bundle"))
            {
                BuildScript.CheckPlatformAndBuild();
            }
            if (!string.IsNullOrEmpty(_OutputFolderAbsolutePath) && GUILayout.Button("打开Bundle文件"))
            {
                EditorUtility.OpenWithDefaultApp(_OutputFolderAbsolutePath);
            }
            EditorGUILayout.EndHorizontal();
            #endregion/*******************************打包功能*******************************/

            #region/*******************************预览配置文件*******************************/
            EditorGUILayout.BeginHorizontal();
            ShowConfig();
            EditorGUILayout.EndHorizontal();
            #endregion/*******************************预览配置文件*******************************/ 

            EditorGUILayout.Space();

            
        }
        Vector2 _showConfigScrollPos;
        bool _isShowConfig;

        void ShowConfig()
        {
            if (!_isShowConfig) return;
            _showConfigScrollPos = EditorGUILayout.BeginScrollView(_showConfigScrollPos, GUILayout.Height(300));
            var dirs = _ABManifest.Dirs;
            var bundleNames = _ABManifest.BundleNames;
            var assets = _ABManifest.AssetDatas;
            var targetDirs = _ABManifest.TargetDirs;
            for(int i = 0; i < targetDirs.Length; i++)
            {
                var targetDir = targetDirs[i];
                GUILayout.Label("文件夹:" + targetDir);
                for (int j = 0; j < assets.Length; j++)
                {
                    var asset = assets[j];
                    var dir = dirs[asset.DirIndex];
                    if(dir.Contains(targetDir))
                    {
                        var bundleName = bundleNames[asset.BundleNameInde];
                        EditorGUILayout.BeginHorizontal();
                        GUIUtils.MakeGUITab(20);
                        GUILayout.Label(bundleName + "  " + asset.Name);
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }
}