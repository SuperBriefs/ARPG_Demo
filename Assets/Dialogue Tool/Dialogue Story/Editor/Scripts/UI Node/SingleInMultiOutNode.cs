using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace E.Story
{
    //单进多出节点
    public class SingleInMultiOutNode : BaseNode
    {
        public override void Init(StoryGraphView graphView, string title, Vector2 position)
        {
            base.Init(graphView, title, position);
            
            // 重设属性默认值
            Type = NodeType.SingleInMultiOut;

            // 清除并添加默认选项
            ChoiceDatas.Clear();
            ChoiceDatas.Add(new ChoiceData("选项文本"));
        }

        protected override void DrawOutputContainer()
        {
            // 遍历选项视图列表：创建对应端口
            for(int i = 0; i < ChoiceDatas.Count; i++)
            {
                ChoiceData choiceData = ChoiceDatas[i];
                output = this.CreateOutputPort(choiceData);
                outputContainer.Add(output);
            }
        }

        public override void DrawExtensionContainer()
        {
            // 创建添加选项按钮
            Button btnAdd = ElementUtility.CreateButton("添加选项", () =>
            {
                ChoiceData choiceData = new ChoiceData("选项文本");
                ChoiceDatas.Add(choiceData);

                output = CreateOutputPort(choiceData);
                outputContainer.Add(output);
            });

            // 放置UI元素
            extensionContainer.Add(btnAdd);

            // 添加USS类名
            btnAdd.AddClasses(
                "foldout-item"
            );

            RefreshExpandedState();
        }

        /// <summary>
        /// 创建选项端口
        /// </summary>
        private Port CreateOutputPort(object userData)
        {
            // 获取选项数据
            ChoiceData choiceData = (ChoiceData)userData;

            // 创建输出端口
            Port outputPort = this.CreatePort();
            // 创建删除按钮
            Button btnDelete = ElementUtility.CreateButton("X", () =>
            {
                if(ChoiceDatas.Count == 1)
                {
                    Debug.Log("请至少保留一条选项");
                    return;
                }

                ChoiceDatas.Remove(choiceData);
                graphView.RemoveElement(outputPort);
            });
            // 创建选项文本框
            TextField tfdChoice = ElementUtility.CreateTextField(choiceData.Text, null, callback =>
            {
                choiceData.Text = callback.newValue;
            });
            // 放置UI元素
            outputPort.Add(btnDelete);
            outputPort.Add(tfdChoice);

            // 添加USS类名
            btnDelete.AddClasses
            (
                "row-item__right"
            );
            tfdChoice.AddClasses
            (
                "textfield",
                "textfield__node-output-port",
                "textfield__hidden"
            );

            return outputPort;
        }
    }
}