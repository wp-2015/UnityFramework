using UnityEditor;
using UnityEngine;

namespace ABHelper
{
    class GUIUtils
    {
        /// <summary>
        /// 在GUI面板种有时候我想做一个TAB的退格效果，但是还没有找到比较文雅的做法。
        /// 这里封装一下，方便以后集体修改
        /// </summary>
        public static void MakeGUITab(int width)
        {
            EditorGUILayout.LabelField("", GUILayout.Width(width));
        }
    }
}
