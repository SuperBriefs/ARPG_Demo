using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace E.Story
{
    public class VarPanel : Blackboard
    {
        private new StoryGraphView graphView;

        public List<VarData> VarDatas { get; set; }

        public List<string> VarDataKeys
        {
            get
            {
                List<string> keys = new List<string>();
                foreach(VarData varData in VarDatas)
                {
                    string key = varData.Key;
                    keys.Add(key);
                }
                return keys;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="graphView"></param>
        public void Init(StoryGraphView graphView)
        {
            this.graphView = graphView;
            title = "变量";
            subTitle = "";
            scrollable = true;
            visible = false;
            VarDatas = new List<VarData>();

            addItemRequested = (bb) =>
            {
                VarData varData = new VarData("新变量");
                VarDatas.Add(varData);

                VisualElement lineContainer = CreateVarData(varData);
                Add(lineContainer);

                CheckDuplicateVarKey();
                graphView.OnEditVarDatas();
            };

            moveItemRequested = (bb, index, ve) =>
            {
                Debug.Log("moveItemRequested");
            };

            editTextRequested = (bb, ve, text) =>
            {
                Debug.Log("editTextRequested");
            };
        }

        /// <summary>
        /// 绘制
        /// </summary>
        public void Draw()
        {
            Clear();

            CheckDuplicateVarKey();

            // 遍历
            foreach(VarData varData in VarDatas)
            {
                VisualElement lineContainer = CreateVarData(varData);
                Add(lineContainer);
            }
        }

        /// <summary>
        /// 设置变量数据
        /// </summary>
        /// <param name="varDatas">变量数据</param>
        public void SetVarDatas(List<VarData> varDatas)
        {
            VarDatas = varDatas;
            graphView.OnEditVarDatas();

            Draw();
        }

        /// <summary>
        /// 清除面板
        /// </summary>
        public void ClearPanel()
        {
            VarDatas.Clear();
            graphView.OnEditVarDatas();

            Draw();
        }
        
        /// <summary>
        /// 检测重复变量名
        /// </summary>
        private void CheckDuplicateVarKey()
        {
            List<string> keys = new List<string>();
            List<string> duplicateKeys = new List<string>();

            // 记录重复的变量名
            foreach(VisualElement lineContainer in Children())
            {
                TextField tfdKey = lineContainer.Q<TextField>();

                if (keys.Contains(tfdKey.value))
                {
                    duplicateKeys.Add(tfdKey.value);
                }
                else
                {
                    keys.Add(tfdKey.value);
                }
            }

            // 如果有重复，高亮所有相关条目的背景色
            foreach(VisualElement lineContainer in Children())
            {
                TextField tfdKey = lineContainer.Q<TextField>();

                if (duplicateKeys.Contains(tfdKey.value))
                {
                    lineContainer.AddClasses("error-bg");
                }
                else
                {
                    lineContainer.RemoveFromClassList("error-bg");
                }
            }

            // 如果有重复，显示副标题
            if(duplicateKeys.Count > 0)
            {
                this.Q<Label>("subTitleLabel").style.color = new StyleColor(new Color(1, 0.133f, 0.133f, 1));
                subTitle = "请勿设置同名变量";
            }
            else
            {
                this.Q<Label>("subTitleLabel").style.color = new StyleColor(new Color(1, 0.133f, 0.133f, 1));
                subTitle = "";
            }
        }

        /// <summary>
        /// 创建变量数据UI
        /// </summary>
        /// <param name="varData"></param>
        /// <returns></returns>
        private VisualElement CreateVarData(VarData varData)
        {
            // 创建行容器
            VisualElement lineContainer = new();
            lineContainer.userData = userData;
            lineContainer.AddClasses("row-container");

            // 创建变量名输入框
            TextField tfdVarKey = ElementUtility.CreateTextArea(varData.Key, null, callback =>
            {
                varData.Key = callback.newValue;

                CheckDuplicateVarKey();
                graphView.OnEditVarDatas();
            });
            tfdVarKey.AddClasses
            (
                "textfield",
                "textfield__quote"
            );
            lineContainer.Add(tfdVarKey);

            // 创建变量值输入框
            IntegerField tfdVarValue = ElementUtility.CreateIntField(varData.Value, null, callback =>
            {
                varData.Value = callback.newValue;
            });
            tfdVarValue.AddClasses
            (
                "textfield",
                "textfield__quote"
            );
            lineContainer.Add(tfdVarValue);

            // 创建删除按钮
            Button btnDelete = ElementUtility.CreateButton("X", () =>
            {
                VarDatas.Remove(varData);
                Remove(lineContainer);

                CheckDuplicateVarKey();
                graphView.OnEditVarDatas();
            });
            btnDelete.AddClasses
            (
                //"row-item__right"
            );
            lineContainer.Add(btnDelete);

            return lineContainer;
        }

        /// <summary>
        /// 获取变量索引
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetVarIndex(string key)
        {
            foreach(VarData varData in VarDatas)
            {
                if(varData.Key == key)
                {
                    return VarDatas.IndexOf(varData);
                }
            }

            return 0;
        }
    }
}