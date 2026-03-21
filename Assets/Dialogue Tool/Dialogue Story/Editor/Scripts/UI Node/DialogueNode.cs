using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace E.Story
{
    // 对话节点
    public class DialogueNode : SingleInSingleOutNode
    {
        // 角色名称
        public string RoleName { get; set; }

        // 角色立绘
        public Sprite Portrait { get; set; }

        // 句子列表
        public List<SentenceData> SentenceDatas { get; set; }

        public override void Init(StoryGraphView graphView, string title, Vector2 position)
        {
            base.Init(graphView, title, position);

            // 重设属性默认值
            Type = NodeType.Dialogue;

            RoleName = "角色名称";
            SentenceDatas = new List<SentenceData>()
            {
                new SentenceData("发言内容")
            };
        }

        public override void DrawExtensionContainer()
        {
            // 创建自定义容器
            customDataContainer = new VisualElement();
            // 创建折叠框
            foldout = ElementUtility.CreateFoldout("节点内容");
            // 创建角色信息容器
            VisualElement roleInfoRowContainer = new VisualElement();
            VisualElement roleInfoColContainer = new VisualElement();
            // 创建角色名称输入框
            TextField tfdRoleName = ElementUtility.CreateTextField(RoleName, null, callback =>
            {
                RoleName = callback.newValue;
            });

            // 创建角色立绘预览
            Image imgPortrait = ElementUtility.CreateImage(Portrait);
            // 创建角色立绘选择字段
            ObjectField objPortrait = ElementUtility.CreateObjectField(typeof(Sprite), Portrait, null, (callback) =>
            {
                Portrait = callback.newValue as Sprite;
                imgPortrait.sprite = Portrait;
            });

            // 放置UI元素
            roleInfoColContainer.Add(tfdRoleName);
            roleInfoColContainer.Add(objPortrait);
            roleInfoRowContainer.Add(roleInfoColContainer);
            roleInfoRowContainer.Add(imgPortrait);
            foldout.Add(roleInfoRowContainer);

            // 创建添加按钮
            Button btnAdd = ElementUtility.CreateButton("添加句子", () =>
            {
                SentenceData sentenceData = new SentenceData("新句子");
                SentenceDatas.Add(sentenceData);

                VisualElement lineContainer = CreateSentenceData(sentenceData);
                foldout.Add(lineContainer);
            });

            // 放置UI元素
            foldout.Add(btnAdd);
            customDataContainer.Add(foldout);
            extensionContainer.Add(customDataContainer);

            // 遍历列表并创建句子条目
            foreach(SentenceData sentenceData in SentenceDatas)
            {
                VisualElement lineContainer = CreateSentenceData(sentenceData);
                foldout.Add(lineContainer);
            }

            // 添加USS类名
            customDataContainer.AddClasses
            (
                "node__custom-data-container"
            );
            roleInfoRowContainer.AddClasses
            (
                "row-container",
                "foldout-item"
            );
            roleInfoColContainer.AddClasses
            (
                "col-container",
                //"row-item__left-center",
                "full-width"
            );
            tfdRoleName.AddClasses
            (
                "col-item__top-center",
                "textfield",
                "textfield__quote"
            );
            btnAdd.AddClasses
            (
                "foldout-item"
            );

            RefreshExpandedState();
        }

        /// <summary>
        /// 创建句子数据
        /// </summary>
        private VisualElement CreateSentenceData(object userData)
        {
            // 获取句子数据
            SentenceData sentenceData = (SentenceData)userData;

            // 创建行容器
            VisualElement lineContainer = new VisualElement();
            lineContainer.userData = userData;
            // 创建句子输入框
            TextField tfdSentence = ElementUtility.CreateTextArea(sentenceData.Text, null, callback =>
            {
                sentenceData.Text = callback.newValue;
            });
            // 创建删除按钮
            Button btnDelete = ElementUtility.CreateButton("X", () =>
            {
                if(SentenceDatas.Count == 1)
                {
                    Debug.Log("请至少保留一条句子");
                    return;
                }

                // 从数据列表移除
                SentenceDatas.Remove(sentenceData);
                // 从UI移除
                foldout.Remove(lineContainer);
            });

            // 放置UI元素
            lineContainer.Add(tfdSentence);
            lineContainer.Add(btnDelete);

            // 添加USS类名
            lineContainer.AddClasses
            (
                "row-container",
                "foldout-item"
            );

            tfdSentence.AddClasses
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

        public override NodeData GetNodeData()
        {
            List<ChoiceData> choiceDatas = DataUtility.CloneChoiceDatas(ChoiceDatas);

            NodeData nodeData = new NodeData()
            {
                GUID = GUID,
                Type = Type,
                Position = GetPosition().position,
                Title = Title,
                Note = Note,
                ChoiceDatas = choiceDatas,
                GroupID = Group?.ID,
                RoleName = RoleName,
                Portrait = Portrait,
                SentenceDatas = SentenceDatas
            };

            return nodeData;
        }
    }
}
