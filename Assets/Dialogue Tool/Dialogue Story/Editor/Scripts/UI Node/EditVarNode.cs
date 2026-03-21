using System.Collections.Generic;
using Codice.Client.Common.TreeGrouper;
using Packages.Rider.Editor.UnitTesting;
using UnityEngine;
using UnityEngine.UIElements;

namespace E.Story
{
    public class EditVarNode : SingleInSingleOutNode
    {
        // 编辑列表
        public List<EditVarData> EditVarDatas { get; set; }

        public override void Init(StoryGraphView graphView, string title, Vector2 position)
        {
            base.Init(graphView, title, position);

            // 重设属性默认值
            Type = NodeType.EditVar;
            EditVarDatas = new List<EditVarData>()
            {
                new EditVarData()
            };
        }

        public override void DrawExtensionContainer()
        {
            extensionContainer.Clear();

            // 创建自定义容器
            customDataContainer = new VisualElement();
            // 创建折叠框
            foldout = ElementUtility.CreateFoldout("节点内容");
            // 创建添加按钮
            Button btnAdd = ElementUtility.CreateButton("添加目标变量", () =>
            {
                EditVarData editVarData = new EditVarData();
                EditVarDatas.Add(editVarData);

                VisualElement lineContainer = CreateEditVarData(editVarData);
                foldout.Add(lineContainer);
            });

            // 放置UI元素
            foldout.Add(btnAdd);
            customDataContainer.Add(foldout);
            extensionContainer.Add(customDataContainer);

            // 遍历列表并创建变量编辑条目
            foreach(EditVarData editVarData in EditVarDatas)
            {
                VisualElement lineContainer = CreateEditVarData(editVarData);
                foldout.Add(lineContainer);
            }

            // 添加USS类名
            btnAdd.AddClasses
            (
                "foldout-item"
            );
            customDataContainer.AddClasses
            (
                "node__custom-data-container"
            );

            RefreshExpandedState();
        }

        public override NodeData GetNodeData()
        {
            NodeData nodeData = base.GetNodeData();

            List<EditVarData> editVarDatas = DataUtility.CloneEditVarDatas(EditVarDatas);

            nodeData.EditVarDatas = editVarDatas;

            return nodeData;
        }

        /// <summary>
        /// 创建编辑变量数据
        /// </summary>
        /// <param name="userData"></param>
        /// <returns></returns>
        private VisualElement CreateEditVarData(object userData)
        {
            // 获取编辑变量数据
            EditVarData editVarData = (EditVarData)userData;

            // 创建行容器
            VisualElement lineContainer = new VisualElement();
            lineContainer.userData = userData;
            // 创建变量下拉字段
            int varCount = graphView.VarPanel.VarDatas.Count;
            Label label = null;
            PopupField<string> pfdKey = null;
            if(varCount == 0)
            {
                label = ElementUtility.CreateLabel("请至少设置一个变量");
            }
            else
            {
                List<string> varKeys = graphView.VarPanel.VarDataKeys;
                int defaultIndex = editVarData.VarIndex;
                // 检测索引是否越界
                if(varCount <= defaultIndex)
                {
                    defaultIndex = -1;
                }
                pfdKey = ElementUtility.CreatePopupField(varKeys, defaultIndex, callback =>
                {
                    editVarData.VarIndex = graphView.VarPanel.GetVarIndex(callback.newValue);
                });
            }
            // 创建操作类型下拉字段
            EnumField efdOperation = ElementUtility.CreateEnumField(editVarData.Operation, null, callback =>
            {
                editVarData.Operation = (OperationType)callback.newValue;
            });
            // 创建值字段
            IntegerField ifdValue = ElementUtility.CreateIntField(editVarData.Value, null, callback =>
            {
                editVarData.Value = callback.newValue;
            });

            // 创建删除按钮
            Button btnDelete = ElementUtility.CreateButton("X", () =>
            {
                if(EditVarDatas.Count == 1)
                {
                    Debug.LogWarning("需至少保留一条编辑变量");
                    return;
                }
                EditVarDatas.Remove(editVarData);
                foldout.Remove(lineContainer);
            });

            // 放置UI元素
            if(label!= null)
            {
                lineContainer.Add(label);
            }
            if(pfdKey != null)
            {
                lineContainer.Add(pfdKey);
                lineContainer.Add(efdOperation);
                lineContainer.Add(ifdValue);
                lineContainer.Add(btnDelete);
            }

            // 添加USS类
            label?.AddClasses
            (
                "help-text"
            );
            pfdKey?.AddClasses
            (
                "row-item__left-center"
            );
            lineContainer?.AddClasses
            (
                "row-container",
                "foldout-item"
            );
            efdOperation?.AddClasses
            (
                "row-item__left-center"
            );
            ifdValue?.AddClasses
            (
                "textfield",
                "textfield__quote",
                "row-item__left-center"
            );
            btnDelete?.AddClasses
            (
                "row-item__right"
            );

            return lineContainer;
        }
    }
}
