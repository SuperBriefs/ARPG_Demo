using UnityEditor;
using UnityEngine.UIElements;

namespace E.Story{
    public static class StyleUtility
    {
        /// <summary>
        /// 添加类名
        /// </summary>
        public static VisualElement AddClasses(this VisualElement element, params string[] classNames)
        {
            foreach(string item in classNames)
            {
                element.AddToClassList(item);
            }

            return element;
        }

        /// <summary>
        /// 添加样式表USS
        /// </summary>
        public static VisualElement AddStyleSheets(this VisualElement element, params string[] filePath)
        {
            foreach(string item in filePath)
            {
                //载入文件
                StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(item);
                //添加引用
                element.styleSheets.Add(styleSheet);
            }

            return element;
        }
    }
}
