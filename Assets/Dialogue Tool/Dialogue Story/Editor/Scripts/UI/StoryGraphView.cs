using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Android.Gradle;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace E.Story{
    public class StoryGraphView : GraphView
    {
        // 关联的UI元素
        private StoryEditorWindow storyEditorWindow;
        private NodeCreationBox nodeCreationBox;
        private NavMap miniMap;

        private Label lblVarCount;
        private Label lblNodeCount;
        private Label lblGroupCount;
        private Label lblNoteCount;
        private Label lblScale;

        private VarPanel varPanel;

        public VarPanel VarPanel { get => varPanel; }

        public List<BaseGroup> Groups
        {
            get
            {
                // 遍历获取元素
                List<BaseGroup> groups = new List<BaseGroup>();
                graphElements.ForEach(element =>
                {
                    if(element is BaseGroup group)
                    {
                        groups.Add(group);
                        return;
                    }
                });
                return groups;
            }
        }

        public List<BaseNode> Nodes
        {
            get
            {
                // 遍历获取元素
                List<BaseNode> baseNodes = new List<BaseNode>();
                nodes.ForEach(element =>
                {
                    if (element is BaseNode node)
                    {
                        baseNodes.Add(node);
                        return;
                    }
                });
                return baseNodes;
            }
        }

        public List<VarData> Vars
        {
            get => VarPanel.VarDatas;
        }

        public List<BaseNote> Notes
        {
            get
            {
                // 遍历获取元素
                List<BaseNote> notes = new List<BaseNote>();
                graphElements.ForEach(element =>
                {
                    if (element is BaseNote note)
                    {
                        notes.Add(note);
                        return;
                    }
                });
                return notes;
            }
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="window"></param>
        public StoryGraphView(StoryEditorWindow window)
        {
            // 实例化此类时绑定窗口
            storyEditorWindow = window;

            //调用方法
            AddGridBackground();
            AddManipulators();
            AddNodeCreationBox();
            AddMiniMap();
            AddVarPanel();
            AddStoryInfo();

            // 定义事件
            OnOpenNodeCreationBox();
            OnGraphViewChanged();
            OnElementsReadyDelete();
            OnGroupElementAdded();
            OnGroupElementRemoved();
            OnGroupRename();
            OnCopyElements();
            OnPasteElements();

            // 添加默认数据
            AddDefaultNodes();
            AddDefaultVarData();
        }

        /// <summary>
        /// 构建右键菜单
        /// </summary>
        /// <param name="evt"></param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            evt.menu.AppendAction("添加节点", action =>
            {
                // 获取光标当前屏幕位置
                Vector2 screenMousePosition = action.eventInfo.mousePosition + storyEditorWindow.position.position + new Vector2(50, 35);
                
                // 触发请求事件
                nodeCreationRequest?.Invoke(new NodeCreationContext
                {
                    screenMousePosition = screenMousePosition,
                    index = -1
                });
            });

            evt.menu.AppendAction("添加分组", action =>
            {
                CreateGroup("分组", GetLocalMousePosition(action.eventInfo.mousePosition));
            });

            evt.menu.AppendAction("添加便签", action =>
            {
                CreateNote("新便签", "", new Rect(GetLocalMousePosition(action.eventInfo.localMousePosition), new Vector2(200, 200)));
            });
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            // 获取所有端口
            List<Port> result = ports.ToList();
            // 执行筛选
            result = result.Where(
                // 两个端口的逻辑方向不能相同（即数据流向是输入还是输出）
                endport => endport.direction != startPort.direction
                           // 两个端口不能为同一个端口
                           && endport.node != startPort.node
            ).ToList();

            return result;
        }

        /// <summary>
        /// 添加网格背景
        /// </summary>
        private void AddGridBackground()
        {
            // 实例化网格背景
            GridBackground gridBackground = new GridBackground();
            // 将网格背景尺寸拉伸至与视图相同
            gridBackground.StretchToParentSize();
            // 将网格背景插入视图
            Insert(0, gridBackground);
        }

        /// <summary>
        /// 添加视图操作组件
        /// </summary>
        private void AddManipulators()
        {
            // 视图缩放
            SetupZoom(0.2f, 2.0f);
            // 添加视图拖拽组件
            this.AddManipulator(new ContentDragger());
            // 添加选中对象拖拽组件
            this.AddManipulator(new SelectionDragger());
            // 添加框选组件
            this.AddManipulator(new RectangleSelector());
        }

        /// <summary>
        /// 创建节点
        /// </summary>
        public BaseNode CreateNode(string title, NodeType type, Vector2 position, Group group = null, bool shouldDraw = true)
        {
            // 获取节点类型
            Type nodeType = Type.GetType($"E.Story.{type}Node");

            // 创建对应节点
            BaseNode node = Activator.CreateInstance(nodeType) as BaseNode;
            node.Init(this, title, position);

            // 是否立即绘制
            if (shouldDraw)
            {
                node.Draw();
            }

            if(group == null)
            {
                // 加入视图
                AddElement(node);
            }
            else
            {
                // 加入分组
                group.AddElement(node);
            }

            return node;
        }

        /// <summary>
        /// 创建连线
        /// </summary>
        public Edge CreateEdge(Port lastOutput, Port nextInput)
        {
            Edge edge = lastOutput.ConnectTo(nextInput);
            AddElement(edge);
            return edge;
        }

        /// <summary>
        /// 创建分组
        /// </summary>
        public BaseGroup CreateGroup(string title, Vector2 position, bool moveSelectedNodes = true)
        {
            BaseGroup group = new BaseGroup(title, position);

            AddElement(group);

            if (moveSelectedNodes)
            {
                // 如果选中了若干节点后创建分组，则将这些节点放入新的分组
                foreach(GraphElement item in selection)
                {
                    if(item is BaseNode baseNode)
                    {
                        group.AddElement(baseNode);
                    }
                }
            }

            return group;
        }

        /// <summary>
        /// 创建便签
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="contents">内容</param>
        /// <param name="position">坐标</param>
        /// <returns>便签</returns>
        public BaseNote CreateNote(string title, string contents, Rect layout)
        {
            BaseNote note = new(title, contents, layout);
            AddElement(note);
            return note;
        }

        /// <summary>
        /// 添加默认节点
        /// </summary>
        public void AddDefaultNodes()
        {
            CreateNode("开始", NodeType.Start, new Vector2(200, 200));
            CreateNode("结束", NodeType.End, new Vector2(800, 200));
        }

        /// <summary>
        /// 添加默认变量
        /// </summary>
        public void AddDefaultVarData()
        {
            VarData varData1 = new VarData("变量一");
            VarData varData2 = new VarData("变量二");
            VarData varData3 = new VarData("变量三");
            varPanel.VarDatas.Add(varData1);
            varPanel.VarDatas.Add(varData2);
            varPanel.VarDatas.Add(varData3);
            varPanel.Draw();
        }

        /// <summary>
        /// 添加节点对话框
        /// </summary>
        private void AddNodeCreationBox()
        {
            nodeCreationBox = ScriptableObject.CreateInstance<NodeCreationBox>();
            nodeCreationBox.Init(this);
        }

        /// <summary>
        /// 添加小地图
        /// </summary>
        private void AddMiniMap()
        {
            miniMap = new NavMap();
            miniMap.Init();
            miniMap.SetPosition(new(15, 10, 200, 200));

            Add(miniMap);
        }

        /// <summary>
        /// 添加变量面板
        /// </summary>
        private void AddVarPanel()
        {
            varPanel = new VarPanel();
            varPanel.Init(this);
            varPanel.Draw();

            Add(varPanel);
        }

        /// <summary>
        /// 添加故事数据
        /// </summary>
        private void AddStoryInfo()
        {
            VisualElement panInfo = new VisualElement();
            VisualElement panStoryInfo = new VisualElement();
            lblVarCount = new Label();
            lblNodeCount = new Label();
            lblGroupCount = new Label();
            lblNoteCount = new Label();
            lblScale = new Label();

            panStoryInfo.Add(lblVarCount);
            panStoryInfo.Add(lblNodeCount);
            panStoryInfo.Add(lblGroupCount);
            panStoryInfo.Add(lblNoteCount);
            panStoryInfo.Add(lblScale);

            panInfo.Add(panStoryInfo);
            Add(panInfo);

            panInfo.AddClasses("info-container");
            panStoryInfo.AddClasses("row-container");
        }

        /// <summary>
        /// 当打开添加节点对话框时
        /// </summary>
        private void OnOpenNodeCreationBox()
        {
            // 定义请求事件
            nodeCreationRequest = context =>
            {
                // 打开搜索框
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), nodeCreationBox);
            };
        }

        /// <summary>
        /// 当视图发生变化时
        /// </summary>
        private void OnGraphViewChanged()
        {
            // 定义事件
            graphViewChanged = (changes) =>
            {
                // 当即将创建连线的时候
                if(changes.edgesToCreate != null)
                {
                    // 遍历所有创建的连线
                    foreach(Edge edge in changes.edgesToCreate)
                    {
                        OnCreateEdge(edge);
                    }
                }

                // 当即将删除元素时
                if(changes.elementsToRemove != null)
                {
                    /* 避免当开始节点和结束节点位于分组里时删除分组被一并删除 */
                    // 获取开始节点
                    GraphElement startNodes = changes.elementsToRemove.FirstOrDefault(n => n is StartNode);
                    // 将其移出删除列表
                    changes.elementsToRemove.Remove(startNodes);
                    // 获取结束节点列表
                    List<GraphElement> endNodes = changes.elementsToRemove.Where(n => n is EndNode).ToList();
                    // 如果与视图中所有结束节点的总数相同
                    if (endNodes.Count == GetEndNodesAmount())
                    {
                        // 获取最后一个结束节点
                        GraphElement lastEndNode = endNodes[endNodes.Count - 1];
                        // 将其移出删除列表
                        changes.elementsToRemove.Remove(lastEndNode);
                    }

                    // 遍历所有元素，执行对应事件
                    foreach(GraphElement element in changes.elementsToRemove)
                    {
                        // 如果是节点 
                        if(element is BaseNode node)
                        {
                            OnDeleteNode(node);
                        }
                        // 如果是分组
                        else if(element is BaseGroup group)
                        {
                            OnDeleteGroup(group);
                        }
                        // 如果是连线
                        else if(element is Edge edge)
                        {
                            OnDeleteEdge(edge);
                        }
                        // 如果是便签
                        else if (element is BaseNote note)
                        {
                            OnDeleteNote(note);
                        }
                    }
                }

                // 当有元素移动后
                if(changes.movedElements != null)
                {
                    Debug.Log(changes.moveDelta);
                }

                return changes;
            };
        }

        /// <summary>
        /// 当元素准备删除时
        /// </summary>
        private void OnElementsReadyDelete()
        {
            // 在此处对选中的元素进行过滤
            deleteSelection = (operationName, askUser) =>
            {
                List<ISelectable> readyToDelete = new List<ISelectable>();

                // 遍历当前选中的目标
                foreach(GraphElement element in selection)
                {
                    // 如果是节点
                    if(element is BaseNode node)
                    {
                        // 过滤掉最后一个开始节点
                        if(node is StartNode)
                        {
                            if(GetStartNodesAmount() == 1)
                            {
                                string str = "不可以删除最后一个开始节点";
                                EditorUtility.DisplayDialog("警告", str, "明白");
                                continue;
                            }
                        }

                        // 过滤掉最后一个结束节点
                        if(node is EndNode)
                        {
                            if(GetEndNodesAmount() == 1)
                            {
                                string str = "不可以删除最后一个结束节点";
                                EditorUtility.DisplayDialog("警告", str, "明白");
                                continue;
                            }
                        }

                        // 加入待删除列表
                        readyToDelete.Add(node);
                    }
                    // 如果是分组
                    else if(element is BaseGroup group)
                    {
                        readyToDelete.Add(group);
                    }
                    // 如果是连线
                    else if(element is Edge edge)
                    {
                        readyToDelete.Add(edge);
                    }
                    // 如果是便签
                    else if(element is BaseNote note)
                    {
                        readyToDelete.Add(note);
                    }
                }

                // 更新选中目标
                selection = readyToDelete;
                // 执行删除
                DeleteSelection();
            };
        }

        /// <summary>
        /// 当往分组里面加入节点时
        /// </summary>
        private void OnGroupElementAdded()
        {
            elementsAddedToGroup = (group, elements) =>
            {
                BaseGroup baseGroup = (BaseGroup)group;
                foreach(GraphElement element in elements)
                {
                    // 如果添加的是节点
                    if(element is BaseNode node)
                    {
                        // 记录节点所属分组
                        node.Group = baseGroup;
                    }
                }
            };
        }

        /// <summary>
        /// 当往分组里面移出节点时
        /// </summary>
        private void OnGroupElementRemoved()
        {
            elementsRemovedFromGroup = (group, elements) =>
            {
                BaseGroup baseGroup = (BaseGroup)group;
                foreach(GraphElement element in elements)
                {
                    // 如果移除的是节点
                    if(element is BaseNode node)
                    {
                        // 记录节点所属分组
                        node.Group = null;
                    }
                }
            };
        }

        /// <summary>
        /// 当分组重命名时
        /// </summary>
        private void OnGroupRename()
        {
            groupTitleChanged = (group, newTitle) =>
            {
                BaseGroup baseGroup = (BaseGroup)group;

                // 清除无效字符
                string temp = newTitle;
                temp = temp.RemoveWhitespaces();
                temp = temp.RemoveSpecialCharacters();
                baseGroup.title = temp;
            };
        }
        
        /// <summary>
        /// 获取开始节点数量
        /// </summary>
        private int GetStartNodesAmount()
        {
            return graphElements.Where(e => e is StartNode).ToList().Count();
        }
        
        /// <summary>
        /// 获取结束节点数量
        /// </summary>
        private int GetEndNodesAmount()
        {
            return graphElements.Where(e => e is EndNode).ToList().Count();
        }

        /// <summary>
        /// 当即将创建连线时
        /// </summary>
        /// <param name="edge"></param>
        private void OnCreateEdge(Edge edge)
        {
            // 获取连线输入端的节点（下一个节点）
            BaseNode nextNode = (BaseNode)edge.input.node;
            // 获取连线输出端的节点（上一个节点）引用的选项数据
            ChoiceData choiceData = (ChoiceData)edge.output.userData;
            // 记录下一个节点的GUID
            choiceData.NextNodeID = nextNode.GUID;
        }

        /// <summary>
        /// 即将删除节点时
        /// </summary>
        /// <param name="node"></param>
        private void OnDeleteNode(BaseNode node)
        {
            Debug.Log("正在删除节点");
        }

        /// <summary>
        /// 即将删除分组时
        /// </summary>
        /// <param name="group"></param>
        private void OnDeleteGroup(BaseGroup group)
        {
            Debug.Log("正在删除分组");
        }

        /// <summary>
        /// 即将删除连线时
        /// </summary>
        /// <param name="edge"></param>
        private void OnDeleteEdge(Edge edge)
        {
            Debug.Log("正在删除连线");

            // 当连线的输出端口为空时，跳过
            if (edge.output == null) 
            {
                return;
            }

            // 获取连线输出端的节点（上一个节点）引用的选项数据
            ChoiceData choiceData = (ChoiceData)edge.output.userData;
            // 清空记录
            choiceData.NextNodeID = "";
        }
        
        /// <summary>
        /// 即将删除便签时
        /// </summary>
        /// <param name="note">将被删除的便签</param>
        public void OnDeleteNote(BaseNote note)
        {
            Debug.Log("正在删除便签");
        }

        /// <summary>
        /// 当复制元素时
        /// </summary>
        private void OnCopyElements()
        {
            serializeGraphElements = (elements) =>
            {
                CopyDatas copyDatas = new CopyDatas();
                foreach(GraphElement element in elements)
                {
                    // 检测是否是节点
                    if(element is BaseNode node)
                    {
                        // 过滤开始节点
                        if(node.Type == NodeType.Start)
                        {
                            continue;
                        }

                        NodeData nodeData = node.GetNodeData();
                        copyDatas.nodeDatas.Add(nodeData);
                    }
                    // 检测是否是分组
                    else if(element is BaseGroup group)
                    {
                        GroupData groupData = group.GetGroupData();
                        copyDatas.groupDatas.Add(groupData);
                    }
                    // 检测是否是便签
                    else if(element is BaseNote note)
                    {
                        NoteData noteData = note.GetNoteData();
                        copyDatas.noteDatas.Add(noteData);
                    }
                }

                // 序列化成字符串
                string temp = JsonUtility.ToJson(copyDatas, true);
                return temp;
            };
        }

        /// <summary>
        /// 当粘贴元素时
        /// </summary>
        private void OnPasteElements()
        {
            unserializeAndPaste = (operationName, data) =>
            {
                ClearSelection();

                // 将字符串反序列化
                CopyDatas copyDatas = JsonUtility.FromJson<CopyDatas>(data);

                // 创建字典，使新元素与旧ID匹配
                Dictionary<GroupData, BaseGroup> pasteGroups = new Dictionary<GroupData, BaseGroup>();
                Dictionary<NodeData, BaseNode> pasteNodes = new Dictionary<NodeData, BaseNode>();

                // 遍历所有分组数据，创建分组
                foreach (GroupData groupData in copyDatas.groupDatas)
                {
                    // 更新分组标题
                    string newTitle = groupData.Title;
                    // 偏移分组位置
                    Vector2 newPosition = groupData.Position + new Vector2(50, 50);

                    // 创建分组
                    BaseGroup group = CreateGroup(newTitle, newPosition, false);
                    // 加入字典
                    pasteGroups.Add(groupData, group);
                }

                // 遍历所有节点数据，创建节点
                foreach (NodeData nodeData in copyDatas.nodeDatas)
                {
                    // 更新节点标题
                    string newTitle = nodeData.Title;
                    // 偏移节点位置
                    Vector2 newPosition = nodeData.Position + new Vector2(50, 50);

                    // 创建节点
                    BaseNode node = CreateNode(newTitle, nodeData.Type, newPosition, null, false);

                    // 加入字典
                    pasteNodes.Add(nodeData, node);

                    // 复制节点信息
                    node.Note = nodeData.Note;
                    node.ChoiceDatas = DataUtility.CloneChoiceDatas(nodeData.ChoiceDatas);

                    // 检测节点类型并获取特定变量值
                    if (node.Type == NodeType.Dialogue)
                    {
                        DialogueNode dNode = node as DialogueNode;
                        dNode.RoleName = nodeData.RoleName;
                        dNode.Portrait = nodeData.Portrait;
                        dNode.SentenceDatas = DataUtility.CloneSentenceDatas(nodeData.SentenceDatas);
                    }
                    else if(node.Type == NodeType.BGI)
                    {
                        BGINode bNode = node as BGINode;
                        bNode.BGI = nodeData.BGI;
                    }
                    else if(node.Type == NodeType.EditVar)
                    {
                        EditVarNode eNode = node as EditVarNode;
                        eNode.EditVarDatas = DataUtility.CloneEditVarDatas(nodeData.EditVarDatas);
                    }
                    else if(node.Type == NodeType.Skip)
                    {
                        SkipNode sNode = node as SkipNode;
                        sNode.nextStoryDataSO = nodeData.NextStoryDataSO;
                    }
                    else if(node.Type == NodeType.GetQuest)
                    {
                        GetQuestNode sNode = node as GetQuestNode;
                        sNode.toGetQuest = nodeData.ToGetQuest;
                    }
                    else if(node.Type == NodeType.CheckQuest)
                    {
                        CheckQuestNode cNode = node as CheckQuestNode;
                        cNode.toCheckQuest = nodeData.ToCheckQuest;
                    }

                    // 绘制节点
                    node.Draw();
                }

                // 遍历所有节点
                foreach(KeyValuePair<NodeData, BaseNode> pasteNode in pasteNodes)
                {
                    NodeData nodeData = pasteNode.Key;
                    BaseNode node = pasteNode.Value;

                    // 更新分组信息
                    if (!string.IsNullOrEmpty(nodeData.GroupID))
                    {
                        foreach(GroupData id in pasteGroups.Keys)
                        {
                            if(nodeData.GroupID == id.GUID)
                            {
                                pasteGroups[id].AddElement(node);
                                node.Title = nodeData.Title;
                                break;
                            }
                        }
                    }

                    // 创建连线
                    foreach (Port outputPort in node.outputContainer.Children())
                    {
                        ChoiceData choiceData = (ChoiceData)outputPort.userData;

                        if (string.IsNullOrEmpty(choiceData.NextNodeID))
                        {
                            return;
                        }

                        // 获取下个节点的数据
                        NodeData nextNodeData = pasteNodes.Keys.FirstOrDefault(x => x.GUID == choiceData.NextNodeID);

                        if(nextNodeData == null)
                        {
                            choiceData.NextNodeID = "";
                            continue;
                        }

                        // 获取下个节点的输入端口
                        BaseNode nextNode = pasteNodes[nextNodeData];
                        Port nextNodeInputPort = (Port)nextNode.inputContainer.Children().First();

                        // 记录GUID
                        choiceData.NextNodeID = nextNode.GUID;

                        // 创建连线
                        CreateEdge(outputPort, nextNodeInputPort);
                    }
                    node.RefreshPorts();
                }

                // 遍历所有便签数据，创建便签
                foreach(NoteData noteData in copyDatas.noteDatas)
                {
                    // 偏移位置
                    Vector2 newPosition = noteData.Layout.position + new Vector2(50, 50);
                    // 创建便签
                    BaseNote note = CreateNote(noteData.Title, noteData.Content, new Rect(newPosition, noteData.Layout.size));
                }
            };
        }

        /// <summary>
        /// 获取本地光标坐标
        /// </summary>
        public Vector2 GetLocalMousePosition(Vector2 screenMousePosition, bool isNodeCreationBox = false)
        {
            Vector2 windowMousePosition;

            if (isNodeCreationBox)
            {
                // 将光标的屏幕坐标转换为光标在当前窗口内的坐标
                windowMousePosition = screenMousePosition - storyEditorWindow.position.position;
            }
            else
            {
                windowMousePosition = screenMousePosition;
            }

            // 将光标在当前窗口的坐标转换为光标在节点视图内的坐标
            Vector2 localMousePosition = contentViewContainer.WorldToLocal(windowMousePosition);

            return localMousePosition;
        }

        /// <summary>
        /// 清除视图
        /// </summary>
        public void ClearGraph()
        {
            graphElements.ForEach(e => RemoveElement(e));

            varPanel.ClearPanel();
        }

        /// <summary>
        /// 切换小地图
        /// </summary>
        public void ToggleMinMap()
        {
            miniMap.visible = !miniMap.visible;
        }

        /// <summary>
        /// 切换变量面板
        /// </summary>
        public void ToggleVarPanel()
        {
            varPanel.visible = !varPanel.visible;
        }

        /// <summary>
        /// 当变量数据更新时，更新视图
        /// </summary>
        public void OnEditVarDatas()
        {
            // 获取需要重绘的节点
            List<BaseNode> nodesToReDraw = new List<BaseNode>();
            nodes.ForEach(node =>
            {
                if(node is EditVarNode || node is BranchNode)
                {
                    nodesToReDraw.Add((BaseNode)node);
                    return;
                }
            });

            // 重绘节点
            foreach(BaseNode node in nodesToReDraw)
            {
                node.DrawExtensionContainer();
            }
        }
    
        /// <summary>
        /// 更新故事信息
        /// </summary>
        public void UpdateStoryInfo()
        {
            UpdateVarCountInfo();
            UpdateNodeCountInfo();
            UpdateGroupCountInfo();
            UpdateNoteCountInfo();
            UpdateScaleInfo();
        }

        /// <summary>
        /// 更新变量数量信息
        /// </summary>
        public void UpdateVarCountInfo()
        {
            lblVarCount.text = varPanel.VarDatas.Count + "变量";
        }

        /// <summary>
        /// 更新节点数量信息
        /// </summary>
        public void UpdateNodeCountInfo()
        {
            lblNodeCount.text = nodes.Count() + "节点";
        }

        /// <summary>
        /// 更新分组数量信息
        /// </summary>
        private void UpdateGroupCountInfo()
        {
            int count = 0;
            graphElements.ForEach(element =>
            {
                if (element is BaseGroup group)
                {
                    count++;
                    return;
                }
            });
            lblGroupCount.text = count + "分组";
        }

        /// <summary>
        /// 更新便签数量信息
        /// </summary>
        private void UpdateNoteCountInfo()
        {
            int count = 0;
            graphElements.ForEach(element =>
            {
                if (element is BaseNote note)
                {
                    count++;
                    return;
                }
            });
            lblNoteCount.text = count + "便签";
        }

        /// <summary>
        /// 更新缩放信息
        /// </summary>
        private void UpdateScaleInfo()
        {
            lblScale.text = string.Format("{0:F2}", scale) + "x";
        }
    }
}
