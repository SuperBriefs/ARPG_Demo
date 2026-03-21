using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace E.Story{
    public static class ElementUtility
    {
        /// <summary>
        /// 创建标签
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>标签</returns>
        public static Label CreateLabel(string text)
        {
            Label label = new Label(text);
            return label;
        }

        /// <summary>
        /// 创建按钮
        /// </summary>
        public static Button CreateButton(string text, Action onClick = null)
        {
            Button button = new Button(onClick)
            {
                text = text,
            };

            return button;
        }
        
        /// <summary>
        /// 创建折叠组
        /// </summary>
        public static Foldout CreateFoldout(string title, bool collapsed = false)
        {
            Foldout foldout = new Foldout()
            {
                text = title,
                value = !collapsed
            };

            return foldout;
        }

        /// <summary>
        /// 创建单行文本输入框
        /// </summary>
        public static TextField CreateTextField(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textField = new TextField()
            {
                value = value,
                label = label
            };

            if(onValueChanged != null)
            {
                textField.RegisterValueChangedCallback(onValueChanged);
            }

            return textField;
        }

        /// <summary>
        /// 创建多行文本输入框
        /// </summary>
        public static TextField CreateTextArea(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textArea = CreateTextField(value, label, onValueChanged);
            textArea.multiline = true;
            return textArea;
        }

        /// <summary>
        /// 创建整数字段
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="label">标签</param>
        /// <param name="onValueChanged">值变化事件</param>
        /// <returns></returns>
        public static IntegerField CreateIntField(int value = 0, string label = null, EventCallback<ChangeEvent<int>> onValueChanged = null)
        {
            IntegerField field = new IntegerField()
            {
                value = value,
                label = label,
            };

            if(onValueChanged != null)
            {
                field.RegisterValueChangedCallback(onValueChanged);
            }

            return field;
        }

        /// <summary>
        /// 创建下拉菜单字段
        /// </summary>
        /// <param name="choices">选项列表</param>
        /// <param name="defaultIndex">默认索引值</param>
        /// <param name="onValueChanged">值变化事件</param>
        /// <returns>下拉菜单字段</returns>
        public static PopupField<string> CreatePopupField(List<string> choices, int defaultIndex, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            PopupField<string> field = new PopupField<string>(choices, defaultIndex);

            if(onValueChanged != null)
            {
                field.RegisterValueChangedCallback(onValueChanged);
            }

            return field;
        }

        /// <summary>
        /// 创建下拉菜单字段
        /// </summary>
        /// <param name="choices">选项列表</param>
        /// <param name="defaultIndex">默认索引值</param>
        /// <param name="onValueChanged">值变化事件</param>
        /// <returns>下拉菜单字段</returns>
        public static PopupField<string> CreatePopupField(List<string> choices, string defaultValue, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            PopupField<string> field = new PopupField<string>(choices, defaultValue);

            if(onValueChanged != null)
            {
                field.RegisterValueChangedCallback(onValueChanged);
            }

            return field;
        }

        /// <summary>
        /// 创建枚举下拉字段
        /// </summary>
        /// <param name="defaultValue">默认值</param>
        /// <param name="label">标签</param>
        /// <param name="onValueChanged">值变化事件</param>
        /// <returns>枚举下拉字段</returns>
        public static EnumField CreateEnumField(Enum defaultValue, string label = null, EventCallback<ChangeEvent<Enum>> onValueChanged = null)
        {
            EnumField field = new EnumField(label, defaultValue);

            if(onValueChanged != null)
            {
                field.RegisterValueChangedCallback(onValueChanged);
            }

            return field;
        }

        /// <summary>
        /// 创建对象选择字段
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="obj">对象默认值</param>
        /// <param name="label">标签</param>
        /// <param name="onValueChanged">值变化事件</param>
        /// <returns>对象选择字段</returns>
        public static ObjectField CreateObjectField(Type type, UnityEngine.Object obj, string label = null, EventCallback<ChangeEvent<UnityEngine.Object>> onValueChanged = null)
        {
            ObjectField field = new ObjectField(label)
            {
                objectType = type,
                allowSceneObjects = false,
                value = obj
            };

            if (onValueChanged != null)
            {
                field.RegisterValueChangedCallback(onValueChanged);
            }

            return field;
        }

        /// <summary>
        /// 创建图片预览区域
        /// </summary>
        /// <param name="sprite">图片</param>
        /// <returns>图片预览区域</returns>
        public static Image CreateImage(Sprite sprite)
        {
            Image image = new Image()
            {
                sprite = sprite,
            };

            return image;
        }
        
        /// <summary>
        /// 创建端口
        /// </summary>
        /// <param name="node">当前的节点</param>
        /// <param name="portName">端口名称</param>
        /// <param name="orientation">连线的方式（默认水平）</param>
        /// <param name="direction">连线的方向（默认输出）</param>
        /// <param name="capacity">允许连线数量（默认单条连接）</param>
        /// <returns>端口</returns>
        public static Port CreatePort(this BaseNode node, string portName = "", Orientation orientation = Orientation.Horizontal, Direction direction = Direction.Output, Port.Capacity capacity = Port.Capacity.Single)
        {
            Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));
            port.portName = portName;
            
            return port;
        }
    }
}
