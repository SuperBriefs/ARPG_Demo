using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace E.Story{
    public class BaseNode : Node
    {
        // UI元素
        protected StoryGraphView graphView;
        protected VisualElement customDataContainer;
        protected Foldout foldout;
        protected Port input;
        protected Port output;

        // 节点GUID
        public string GUID { get; set; }

        // 节点类型
        public NodeType Type { get; set; }

        // 节点标题
        public string Title { get; set; }

        // 节点备注
        public string Note { get; set; }

        // 选项数据列表
        public List<ChoiceData> ChoiceDatas { get; set; }

        // 所属分组
        public BaseGroup Group { get; set; }

        // 输入端口
        public Port Input { get => input; }

        // 输出端口
        public Port Output { get => input; }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("清除输入连接",
                action => DisconnectedInputPorts(),
                HasInputConnection() ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);

            evt.menu.AppendAction("清除输出连接",
                action => DisconnectedOutputPorts(),
                HasOutputConnection() ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);

            evt.menu.AppendAction("清除所有连接",
                action => DisconnectedAllPorts(),
                HasAnyConnection() ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            
            evt.menu.AppendSeparator();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init(StoryGraphView graphView, string title, Vector2 position)
        {
            // 设置字段
            this.graphView = graphView;
            SetPosition(new Rect(position, Vector2.zero));

            // 设置默认属性
            Type = NodeType.Base;
            GUID = UnityEditor.GUID.Generate().ToString();
            Title = title;
            Note = "备注信息";
            ChoiceDatas = new List<ChoiceData>(){ new("下个节点") };

            // 添加USS类名
            mainContainer.AddToClassList("node__main-container");
            titleContainer.AddToClassList("node__title-container");
            inputContainer.AddToClassList("node__input-container");
            outputContainer.AddToClassList("node__output-container");
            extensionContainer.AddToClassList("node__extension-container");
        }

        /// <summary>
        /// 绘制视图
        /// </summary>
        public virtual void Draw()
        {
            DrawMainContainer();
            DrawTitleContainer();
            DrawTitleButtonContainer();
            DrawTopContainer();
            DrawInputContainer();
            DrawOutputContainer();
            DrawExtensionContainer();
        }

        /// <summary>
        /// 绘制主容器
        /// </summary>
        protected virtual void DrawMainContainer()
        {
            
        }

        

        /// <summary>
        /// 绘制标题容器
        /// </summary>
        protected virtual void DrawTitleContainer()
        {
            // 创建标题输入框
            TextField tfdTitle = ElementUtility.CreateTextField(Title, null, callback =>
            {
                // 更新标题
                Title = callback.newValue;
            });

            // 将标题输入框放在最左侧
            titleContainer.Insert(0, tfdTitle);

            // 添加USS类名
            tfdTitle.AddClasses(
                "textfield",
                "textfield__hidden",
                "textfield__node-title"
            );
        }

        /// <summary>
        /// 绘制标题按钮容器
        /// </summary>

        protected virtual void DrawTitleButtonContainer()
        {
            
        }

        /// <summary>
        /// 绘制顶部容器
        /// </summary>
        protected virtual void DrawTopContainer()
        {
            
        }

        /// <summary>
        /// 绘制输入容器
        /// </summary>
        protected virtual void DrawInputContainer()
        {
            // 创建输入端口 
            input = this.CreatePort("上个节点", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            inputContainer.Add(input);
        }

        /// <summary>
        /// 绘制输出容器
        /// </summary>
        protected virtual void DrawOutputContainer()
        {
            // 遍历选项视图列表：创建对应端口
            for(int i = 0; i < ChoiceDatas.Count; i++)
            {
                ChoiceData choiceData = ChoiceDatas[i];
                output = this.CreatePort(choiceData.Text);
                output.userData = choiceData;
                outputContainer.Add(output);
            }
        }

        /// <summary>
        /// 绘制拓展容器
        /// </summary>
        public virtual void DrawExtensionContainer()
        {
            // 创建自定义容器
            customDataContainer = new VisualElement();

            // 创建折叠组
            foldout = ElementUtility.CreateFoldout("节点内容");

            // 创建备注输入框
            TextField tfdNote = ElementUtility.CreateTextArea(Note, null, callback =>
            {
                // 更新标题
                Note = callback.newValue;
            });

            // 将目标放到对应容器
            foldout.Add(tfdNote);
            customDataContainer.Add(foldout);
            extensionContainer.Add(customDataContainer);

            // 添加USS类名
            customDataContainer.AddClasses
            (
                "node__custom-data-container"
            );
            tfdNote.AddClasses
            (
                "textfield",
                "textfield__quote",
                "foldout-item"
            );

            // 此方法必须调用
            RefreshExpandedState();
        }

        /// <summary>
        /// 是否有任意连接
        /// </summary>
        public bool HasAnyConnection()
        {
            return HasInputConnection() || HasOutputConnection();
        }

        /// <summary>
        /// 是否有上行连接
        /// </summary>
        public bool HasInputConnection()
        {
            if(inputContainer.childCount == 0)
            {
                return false;
            }

            Port port = (Port)inputContainer.Children().First();
            return port.connected;
        }

        /// <summary>
        /// 是否有下行连接
        /// </summary>
        public bool HasOutputConnection()
        {
            if(outputContainer.childCount == 0)
            {
                return false;
            }

            foreach(Port port in outputContainer.Children().ToList())
            {
                if (port.connected)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 断开目标端口连接
        /// </summary>
        protected void DisconnectPort(Port port)
        {
            if (port.connected)
            {
                graphView.DeleteElements(port.connections.ToList());
            }
        }

        /// <summary>
        /// 断开所有端口连接
        /// </summary>
        private void DisconnectPorts(VisualElement container)
        {
            foreach(Port port in container.Children())
            {
                DisconnectPort(port);
            }
        }

        /// <summary>
        /// 断开输入连接
        /// </summary>
        private void DisconnectedInputPorts()
        {
            DisconnectPorts(inputContainer);
        }

        /// <summary>
        /// 断开输出连接
        /// </summary>
        private void DisconnectedOutputPorts()
        {
            DisconnectPorts(outputContainer);
        }

        /// <summary>
        /// 断开所有连接
        /// </summary>
        private void DisconnectedAllPorts()
        {
            DisconnectedInputPorts();
            DisconnectedOutputPorts();
        }
    
        /// <summary>
        /// 获取节点数据
        /// </summary>
        /// <returns></returns>
        public virtual NodeData GetNodeData()
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
            };

            return nodeData;
        }
    }
}