using System;
using System.Collections.Generic;
using Codice.CM.Common;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace E.Story
{
    public class BranchNode : SingleInMultiOutNode
    {
        public override void Init(StoryGraphView graphView, string title, Vector2 position)
        {
            base.Init(graphView, title, position);

            // 重设属性默认值
            Type = NodeType.Branch;

            ChoiceDatas.Clear();
            ChoiceDatas.Add(new ChoiceData("选项文本一"));
            ChoiceDatas.Add(new ChoiceData("选项文本二"));
        }

        protected override void DrawOutputContainer()
        {
            // 遍历选项视图列表：创建对应端口
            for(int i = 0; i < ChoiceDatas.Count; i++)
            {
                ChoiceData choiceData = ChoiceDatas[i];
                // 限制最长显示字符数量
                string text = choiceData.Text;
                if (text.Length > 30)
                {
                    text = text[..27] + "...";
                }
                output = this.CreatePort(text);
                output.userData = choiceData;
                outputContainer.Add(output);
            }
        }

        public override void DrawExtensionContainer()
        {
            // 创建自定义容器
            customDataContainer = new VisualElement();
            // 创建折叠框
            foldout = ElementUtility.CreateFoldout("节点内容");
            // 创建添加按钮
            Button btnAdd = ElementUtility.CreateButton("添加选项", () =>
            {
                ChoiceData choiceData = new ChoiceData("选项文本");
                ChoiceDatas.Add(choiceData);

                VisualElement lineContainer = CreateChoiceData(choiceData);
                foldout.Add(lineContainer);
                
                OnAddChoiceText(choiceData);
            });

            // 放置UI元素
            foldout.Add(btnAdd);
            customDataContainer.Add(foldout);
            extensionContainer.Add(customDataContainer);

            // 遍历列表并创建选项条目
            foreach(ChoiceData choiceData in ChoiceDatas)
            {
                VisualElement lineContainer = CreateChoiceData(choiceData);
                foldout.Add(lineContainer);
            }

            // 添加USS类名
            btnAdd.AddClasses(
                "foldout-item"
            );
            customDataContainer.AddClasses(
                "node__custom-data-container"
            );

            RefreshExpandedState();
        }

        /// <summary>
        /// 创建选项数据UI
        /// </summary>
        private VisualElement CreateChoiceData(object userData)
        {
            // 获取选项数据
            ChoiceData choiceData = (ChoiceData)userData;

            // 创建选项容器
            VisualElement choiceContainer = new VisualElement();
            // 创建行容器
            VisualElement lineContainer = new VisualElement();
            lineContainer.userData = userData;
            // 创建条件列表容器
            VisualElement conditionsContainer = new VisualElement();
            // 创建句子输入框
            TextField tfdChoice = ElementUtility.CreateTextArea(choiceData.Text, null, callback =>
            {
                choiceData.Text = callback.newValue;
                OnEditChoiceText(choiceData);
            });

            // 创建添加条件按钮
            Button btnAdd = ElementUtility.CreateButton("添加条件", () =>
            {
                ConditionData conditionData = new ConditionData(0, 0, CompareType.等于);
                choiceData.Conditions.Add(conditionData);

                // 创建条件容器
                VisualElement conditionContainer = CreateCondition(conditionsContainer, conditionData, choiceData.Conditions);
                conditionsContainer.Add(conditionContainer);
            });
            // 创建条件检索模式下拉字段
            EnumField efdDetectMode = ElementUtility.CreateEnumField(choiceData.DetectMode, null, callback =>
            {
                choiceData.DetectMode = (ConditionDetectMode)callback.newValue;
            });

            // 创建删除按钮
            Button btnDelete = ElementUtility.CreateButton("X", () =>
            {
                if(ChoiceDatas.Count == 1)
                {
                    Debug.Log("请至少保留一个选项");
                    return;
                }

                // 从数据列表移除
                ChoiceDatas.Remove(choiceData);
                // 从UI移除
                foldout.Remove(choiceContainer);
                
                OnRemoveChoiceText(choiceData);
            });
            // 遍历列表并创建条件条目
            foreach (ConditionData conditionData in choiceData.Conditions)
            {
                VisualElement conditionContainer = CreateCondition(conditionsContainer, conditionData, choiceData.Conditions);
                conditionsContainer.Add(conditionContainer);
            }

            // 放置UI元素
            lineContainer.Add(tfdChoice);
            lineContainer.Add(btnAdd);
            lineContainer.Add(efdDetectMode);
            lineContainer.Add(btnDelete);
            choiceContainer.Add(lineContainer);
            choiceContainer.Add(conditionsContainer);

            // 添加USS类名
            choiceContainer.AddClasses
            (
                "foldout-item"
            );
            lineContainer.AddClasses
            (
                "row-container"
            );
            tfdChoice.AddClasses
            (
                "textfield",
                "textfield__quote",
                "row-item__left-center"
            );
            btnAdd.AddClasses
            (
                "row-item__left-center"
            );
            efdDetectMode.AddClasses
            (
                "row-item__left-center"
            );
            btnDelete.AddClasses
            (
                "row-item__right"
            );

            return choiceContainer;
        }

        /// <summary>
        /// 创建条件数据
        /// </summary>
        /// <param name="conditionContainer">条件容器</param>
        /// <param name="conditionData">条件数据</param>
        /// <param name="conditions">条件数据列表</param>
        /// <returns></returns>
        private VisualElement CreateCondition(VisualElement conditionContainer, ConditionData conditionData, List<ConditionData> conditions)
        {
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
                int defaultIndex = conditionData.VarIndex;
                // 检测索引是否越界
                if(varCount <= defaultIndex)
                {
                    defaultIndex = -1;
                }
                pfdKey = ElementUtility.CreatePopupField(varKeys, defaultIndex, callback =>
                {
                    conditionData.VarIndex = graphView.VarPanel.GetVarIndex(callback.newValue);
                });
            }
            // 创建操作类型下拉字段
            EnumField efdCompare = ElementUtility.CreateEnumField(conditionData.Compare, null, callback =>
            {
                conditionData.Compare = (CompareType)callback.newValue;
            });
            // 创建值字段
            IntegerField ifdValue = ElementUtility.CreateIntField(conditionData.Value, null, callback =>
            {
                conditionData.Value = callback.newValue;
            });

            // 创建删除按钮
            Button btnDelete = ElementUtility.CreateButton("X", () =>
            {
                conditions.Remove(conditionData);
                conditionContainer.Remove(lineContainer);
            });

            // 放置UI元素
            if (label != null)
            {
                lineContainer.Add(label);
            }
            if (pfdKey != null)
            {
                lineContainer.Add(pfdKey);
                lineContainer.Add(efdCompare);
                lineContainer.Add(ifdValue);
                lineContainer.Add(btnDelete);
            }

            // 添加USS类名
            label?.AddClasses
            (
                "help-text"
            );
            pfdKey?.AddClasses
            (
                "row-item__left-center"
            );
            lineContainer.AddClasses
            (
                "row-container",
                "indent"
            );
            efdCompare.AddClasses
            (
                "row-item__left-center"
            );
            ifdValue.AddClasses
            (
                "textfield",
                "textfield__quote",
                "row-item__left-center"
            );
            btnDelete.AddClasses
            (
                "row-item__right"
            );

            return lineContainer;
        }

        /// <summary>
        /// 当编辑选项文本时
        /// </summary>
        private void OnEditChoiceText(ChoiceData choiceData)
        {
            // 遍历获取端口元素
            foreach(Port port in outputContainer.Children())
            {
                if(port.userData == choiceData)
                {
                    // 限制最长显示字符数量
                    string text = choiceData.Text;
                    if (text.Length > 30)
                    {
                        text = text[..27] + "...";
                    }
                    port.portName = text;
                    break;
                }
            }
        }

        /// <summary>
        /// 当添加选项文本时
        /// </summary>
        private void OnAddChoiceText(ChoiceData choiceData)
        {
            Port newPort = this.CreatePort(choiceData.Text);
            newPort.userData = choiceData;
            outputContainer.Add(newPort);
        }

        /// <summary>
        /// 当删除选项文本时
        /// </summary>
        private void OnRemoveChoiceText(ChoiceData choiceData)
        {
            Port portToRemove = null;
            // 遍历获取端口元素
            foreach(Port port in outputContainer.Children())
            {
                if(port.userData == choiceData)
                {
                    portToRemove = port;
                    break;
                }
            }

            // 删除该端口上的连线
            DisconnectPort(portToRemove);

            // 删除多余端口
            outputContainer.Remove(portToRemove);
        }
    }
}