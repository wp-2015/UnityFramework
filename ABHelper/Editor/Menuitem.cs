/// <summary>
/// 编辑器扩展需要用到的Menuitem
/// </summary>

using UnityEditor;
using UnityEngine;

namespace ABHelper
{
    public static class Menuitem
    {
        [MenuItem("ABHelper/BuildWindow", false, 100)]
        public static void MainWindowOpen()
        {
            MainWindow.Open();
        }
    }
}