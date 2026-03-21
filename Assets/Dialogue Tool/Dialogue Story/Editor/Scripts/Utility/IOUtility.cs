using UnityEditor;
using UnityEngine;

namespace E.Story
{
    // 输入输出实用类
    public static class IOUtility
    {
        /// <summary>
        /// 创建文件夹
        /// </summary>
        public static void CreateFolder(string path, string fileName)
        {
            // 检测目标文件是否存在
            if (AssetDatabase.IsValidFolder($"{path}/{fileName}"))
            {
                return;
            }

            AssetDatabase.CreateFolder(path, fileName);
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="fullPath">路径</param>
        public static void RemoveFolder(string fullPath)
        {
            FileUtil.DeleteFileOrDirectory($"{fullPath}.meta");
            FileUtil.DeleteFileOrDirectory($"{fullPath}/");
        }

        /// <summary>
        /// 创建资产文件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="path">路径</param>
        /// <param name="assetName">资产名称</param>
        /// <returns>资产文件</returns>
        public static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            // 尝试加载目标文件
            T asset = LoadAsset<T>(path, assetName);

            // 若文件不存在，则创建一个
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, fullPath);
            }
            // 若文件存在，提示使用者，避免操作失误覆盖数据
            else
            {
                string str = "故事名称重复，请确认是否覆盖。";
                if(!EditorUtility.DisplayDialog("警告", str, "确认", "取消"))
                {
                    // 不创建资产文件
                    asset = null;
                }
            }

            return asset;
        }

        /// <summary>
        /// 载入资产文件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="path">路径</param>
        /// <param name="assetName">文件名</param>
        /// <returns>资产文件</returns>
        public static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";
            return AssetDatabase.LoadAssetAtPath<T>(fullPath);
        }

        /// <summary>
        /// 载入资产文件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="fullPath">路径</param>
        /// <returns>资产文件</returns>
        public static T LoadAsset<T>(string fullPath) where T : ScriptableObject
        {
            return AssetDatabase.LoadAssetAtPath<T>(fullPath);
        }

        /// <summary>
        /// 删除资产文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="assetName">文件名</param>
        public static void RemoveAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
        }

        /// <summary>
        /// 将资产文件写入硬盘
        /// </summary>
        /// <param name="asset">资产文件</param>
        public static void SaveAsset(Object asset)
        {
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}