using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace E.Story{
    public class StoryEditorWindow : EditorWindow
    {
        // 只读字段
        private readonly string defaultFileName = "新故事";
        private readonly string keyLastStoryName = "最近打开故事";
        private readonly string variablesPath = "Assets/Dialogue Tool/Dialogue Story/Editor/Style Sheets/Variabels.uss";
        private readonly string toolbarPath = "Assets/Dialogue Tool/Dialogue Story/Editor/Style Sheets/ToolbarStyle.uss";
        private readonly string graphViewPath = "Assets/Dialogue Tool/Dialogue Story/Editor/Style Sheets/GraphViewStyle.uss";

        // 文件夹路径
        private readonly string eStoryFolderPath = "Assets/Dialogue Tool/Dialogue Story";
        private readonly string exampleFolderPath = "Assets/Dialogue Tool/Dialogue Story/Example";
        private readonly string exampleFolderName = "Example";
        private readonly string storyDatasFolderPath = "Assets/Dialogue Tool/Dialogue Story/Example/Story Datas";
        private readonly string storyDatasFolderName = "Story Datas";

        // 临时变量
        private string fileName;
        private StoryDataSO storyData;


        // 关联的视图
        private StoryGraphView graphView;

        // UI元素
        private Toolbar toolbar;
        private VisualElement panLeft;
        private VisualElement panCenter;
        private VisualElement panRight;
        private static TextField tfdFileName;
        private Button btnSave;
        private Button btnOpen;
        private Button btnNew;
        private Button btnClear;
        private Button btnVars;
        private Button btnMiniMap;

        [MenuItem("Dialogue Tool/打开 Dialogue Story 对话编辑器 %&#")]
        public static void Open()
        {
            StoryEditorWindow wnd = GetWindow<StoryEditorWindow>();
            wnd.titleContent = new GUIContent("Dialogue Story 对话编辑器");
        }

        public void CreateGUI()
        {
            // 调用
            AddToolbar();
            AddGraphView();
            AddStyles();

            // 打开上个故事
            OpenLastStory();
        }

        private void OnInspectorUpdate()
        {
            graphView.UpdateStoryInfo();
        }

        /// <summary>
        /// 添加工具栏
        /// </summary>
        public void AddToolbar()
        {
            tfdFileName = ElementUtility.CreateTextField(defaultFileName, "当前故事", callback =>
            {
                if (callback.newValue.HasSpecialCharacter())
                {
                    string temp = callback.newValue.RemoveSpecialCharacters();
                    tfdFileName.value = temp;
                    fileName = temp;
                }
                else
                {
                    fileName = callback.newValue;
                }
            });
            btnSave = ElementUtility.CreateButton("保存", () => SaveStory());
            btnOpen = ElementUtility.CreateButton("打开", () => OpenStory());
            btnNew = ElementUtility.CreateButton("新建", () => NewStory());
            btnClear = ElementUtility.CreateButton("清空", () => ClearGraphAndCreateDefaultDatas());
            btnVars = ElementUtility.CreateButton("变量面板", () => ToggleVarPanel());
            btnMiniMap = ElementUtility.CreateButton("小地图", () => ToggleMinMap());

            // 创建工具栏
            toolbar = new Toolbar();

            // 创建分组
            panLeft = new VisualElement();
            panCenter = new VisualElement();
            panRight = new VisualElement();

            // 将UI元素添加进工具栏
            panLeft.Add(btnOpen);
            panLeft.Add(btnNew);

            panCenter.Add(tfdFileName);
            panCenter.Add(btnSave);
            panCenter.Add(btnClear);

            panRight.Add(btnVars);
            panRight.Add(btnMiniMap);

            toolbar.Add(panLeft);
            toolbar.Add(panCenter);
            toolbar.Add(panRight);

            // 将工具栏加入窗口
            rootVisualElement.Add(toolbar);

            // 初始化文件名
            fileName = defaultFileName;
        }

        /// <summary>
        /// 当双击资产文件时，打开故事编辑器读取故事数据
        /// </summary>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        [OnOpenAsset()]
        public static bool OnDoubleClick(int instanceID)
        {
            // 获取资产路径
            string fullPath = AssetDatabase.GetAssetPath(instanceID);

            // 检测是否有目标资产类型
            StoryDataSO storyData = IOUtility.LoadAsset<StoryDataSO>(fullPath);
            if(storyData == null)
            {
                return false;
            }

            // 获取窗口实例并打开
            StoryEditorWindow wnd = (StoryEditorWindow)GetWindow(typeof(StoryEditorWindow));
            if(wnd == null)
            {
                Open();
            }
            wnd.RemoveNotification();

            string str = "确认打开新故事并覆盖当前视图内容吗？未保存的数据将无法恢复";
            if(EditorUtility.DisplayDialog("警告", str, "确认", "取消"))
            {
                wnd.storyData = storyData;
                wnd.RecordCurrentStory();

                wnd.graphView.ClearGraph();
                wnd.LoadDatas(storyData);

                // 提示信息
                string message = $"故事已打开";
                wnd.ShowNotification(new GUIContent(message));
            }

            return true;
        }

        /// <summary>
        /// 添加视图
        /// </summary>
        public void AddGraphView()
        {
            // 实例化视图类
            graphView = new StoryGraphView(this);
            // 将视图尺寸拉伸至与窗口相同
            //graphView.StretchToParentSize();
            // 将视图放入UI根组件
            rootVisualElement.Add(graphView);
        }

        /// <summary>
        /// 添加样式文件
        /// </summary>
        public void AddStyles()
        {
            // 引用变量样式文件
            rootVisualElement.AddStyleSheets(variablesPath);
            // 引用工具栏样式文件
            toolbar.AddStyleSheets(toolbarPath);
            // 引用故事视图样式文件
            graphView.AddStyleSheets(graphViewPath);
        }

        /// <summary>
        /// 保存故事
        /// </summary>
        private void SaveStory()
        {
            // 检测文件名是否为空
            if (string.IsNullOrEmpty(fileName))
            {
                string str = "故事名称不能为空。";
                EditorUtility.DisplayDialog("警告", str, "明白");
                return;
            }

            // 创建存档文件夹
            IOUtility.CreateFolder(eStoryFolderPath, exampleFolderName);
            IOUtility.CreateFolder(exampleFolderPath, storyDatasFolderName);

            // 创建图形文件
            storyData = IOUtility.CreateAsset<StoryDataSO>(storyDatasFolderPath, $"{fileName}");
            if(storyData != null)
            {
                storyData.Init(fileName);
            }
            else
            {
                return;
            }

            // 保存数据
            SaveDatas();

            // 提示消息
            string message = $"故事已保存";
            ShowNotification(new GUIContent(message));
        }
        
        /// <summary>
        /// 打开故事
        /// </summary>
        private void OpenStory()
        {
            // 获取文件路径
            string filePath = EditorUtility.OpenFilePanel("打开故事 ", storyDatasFolderPath, "asset");

            // 检测是否为空
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            // 绝对路径转为相对路径
            filePath = FileUtil.GetProjectRelativePath(filePath);

            // 载入文件
            StoryDataSO story = IOUtility.LoadAsset<StoryDataSO>(filePath);

            // 检测是否为空
            if(story == null)
            {
                string temp = "故事不存在：\n\n" + 
                    $"{filePath}\n\n" + 
                    "请确认你选择了正确的文件。";
                EditorUtility.DisplayDialog("警告", temp, "明白");

                return;
            }

            string str = "确认打开新故事并覆盖当前视图内容吗？未保存的数据将无法恢复";
            if(EditorUtility.DisplayDialog("警告", str, "确认", "取消"))
            {
                storyData = story;
                RecordCurrentStory();

                graphView.ClearGraph();
                LoadDatas(storyData);

                // 提示信息
                string message = $"故事已打开";
                ShowNotification(new GUIContent(message));
            }
        }
        
        /// <summary>
        /// 打开上次的故事
        /// </summary>
        private void OpenLastStory()
        {
            // 获取文件路径
            string storyName = EditorPrefs.GetString(keyLastStoryName);

            // 检测是否为空
            if (string.IsNullOrEmpty(storyName))
            {
                return;
            }

            // 载入文件
            StoryDataSO story = IOUtility.LoadAsset<StoryDataSO>(storyDatasFolderPath, storyName);

            // 检测是否为空
            string message;
            if(story == null)
            {
                message = $"未找到上次编辑的故事";
                ShowNotification(new GUIContent(message));
                return;
            }

            storyData = story;
            graphView.ClearGraph();
            LoadDatas(storyData);

            // 提示信息
            message = $"已打开上次编辑的故事";
            ShowNotification(new GUIContent(message));
        }
        
        /// <summary>
        /// 新建故事
        /// </summary>
        private void NewStory()
        {
            string str = "确认新建故事吗？未保存的数据将无法恢复";
            if(EditorUtility.DisplayDialog("警告", str, "确认", "取消"))
            {
                graphView.ClearGraph();
                graphView.AddDefaultNodes();
                graphView.AddDefaultVarData();
                graphView.UpdateStoryInfo();
                // 重置文件名
                UpdateFileName(defaultFileName);

                string message = $"故事已新建";
                ShowNotification(new GUIContent(message));
            }
        }

        /// <summary>
        /// 清空视图并添加默认数据
        /// </summary>
        private void ClearGraphAndCreateDefaultDatas()
        {
            string str = "确认清空当前视图内容吗？未保存的数据将无法恢复。";
            if(EditorUtility.DisplayDialog("警告", str, "确认", "取消"))
            {
                graphView.ClearGraph();
                graphView.AddDefaultNodes();
                graphView.AddDefaultVarData();
                graphView.UpdateStoryInfo();

                string message = $"视图已清空";
                ShowNotification(new GUIContent(message));
            }
        }
        
        /// <summary>
        /// 切换小地图
        /// </summary>
        private void ToggleMinMap()
        {
            graphView.ToggleMinMap();
            // 切换按钮样式
            btnMiniMap.ToggleInClassList("toolbar__button__selected");
        }

        /// <summary>
        /// 切换变量面板
        /// </summary>

        private void ToggleVarPanel()
        {
            graphView.ToggleVarPanel();
            // 切换按钮样式
            btnVars.ToggleInClassList("toolbar__button__selected");
        }

        /// <summary>
        /// 记录当前的故事
        /// </summary>
        private void RecordCurrentStory()
        {
            EditorPrefs.SetString(keyLastStoryName, storyData.FileName);
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        private void SaveDatas()
        {
            SaveGroupDatas(graphView.Groups);
            SaveNodeDatas(graphView.Nodes);
            SaveVarDatas(graphView.Vars);
            SaveNoteDatas(graphView.Notes);

            // 写入硬盘
            IOUtility.SaveAsset(storyData);
        }

        /// <summary>
        /// 保存分组数据
        /// </summary>
        /// <param name="groups"></param>
        private void SaveGroupDatas(List<BaseGroup> groups)
        {
            // 遍历分组列表
            foreach (BaseGroup group in groups)
            {
                // 创建分组数据
                GroupData groupData = group.GetGroupData();
                // 加入列表
                storyData.GroupDatas.Add(groupData);
            }
        }

        /// <summary>
        /// 保存节点数据
        /// </summary>
        /// <param name="nodes"></param>
        private void SaveNodeDatas(List<BaseNode> nodes)
        {
            // 遍历节点列表
            foreach (BaseNode node in nodes)
            {
                // 创建节点数据
                NodeData nodeData = node.GetNodeData();
                // 加入列表
                storyData.NodeDatas.Add(nodeData);
            }
        }

        /// <summary>
        /// 保存变量数据
        /// </summary>
        /// <param name="vars"></param>
        private void SaveVarDatas(List<VarData> vars)
        {
            storyData.VarDatas = DataUtility.CloneVarDatas(vars);
        }

        /// <summary>
        /// 保存便签数据
        /// </summary>
        /// <param name="notes"></param>
        private void SaveNoteDatas(List<BaseNote> notes)
        {
            // 遍历节点列表
            foreach (BaseNote note in notes)
            {
                // 创建节点数据
                NoteData noteData = note.GetNoteData();
                // 加入列表
                storyData.NoteDatas.Add(noteData);
            }
        }

        /// <summary>
        /// 载入数据
        /// </summary>
        /// <param name="storyData"></param>
        private void LoadDatas(StoryDataSO storyData)
        {
            UpdateFileName(storyData.FileName);
            LoadVarDatas(storyData.VarDatas);
            Dictionary<string, BaseGroup> loadedGroups = LoadGroupDatas(storyData.GroupDatas);
            Dictionary<string, BaseNode> loadedNodes = LoadNodeDatas(storyData.NodeDatas, loadedGroups);
            LoadNodesConnections(loadedNodes);
            LoadNoteDatas(storyData.NoteDatas);
        }

        /// <summary>
        /// 更新文件名
        /// </summary>
        /// <param name="newFileName"></param>
        private void UpdateFileName(string newFileName)
        {
            tfdFileName.value = newFileName;
        }
        
        /// <summary>
        /// 载入分组数据
        /// </summary>
        /// <param name="groupDatas"></param>
        /// <returns></returns>

        private Dictionary<string, BaseGroup> LoadGroupDatas(List<GroupData> groupDatas)
        {
            Dictionary<string, BaseGroup> loadedGroups = new Dictionary<string, BaseGroup>();

            // 遍历分组视图列表
            foreach(GroupData groupData in groupDatas)
            {
                BaseGroup group = graphView.CreateGroup(groupData.Title, groupData.Position);
                group.ID = groupData.GUID;

                loadedGroups.Add(group.ID, group);
            }

            return loadedGroups;
        }

        /// <summary>
        /// 载入节点数据
        /// </summary>
        /// <param name="nodeDatas"></param>
        /// <param name="loadedGroups"></param>
        /// <returns></returns>
        private Dictionary<string, BaseNode> LoadNodeDatas(List<NodeData> nodeDatas, Dictionary<string, BaseGroup> loadedGroups)
        {
            Dictionary<string, BaseNode> loadedNodes = new Dictionary<string, BaseNode>();

            // 遍历节点视图列表
            foreach(NodeData nodeData in nodeDatas)
            {
                // 创建节点
                BaseNode node = graphView.CreateNode(nodeData.Title, nodeData.Type, nodeData.Position, null, false);
                node.GUID = nodeData.GUID;
                node.Note = nodeData.Note;
                node.ChoiceDatas = DataUtility.CloneChoiceDatas(nodeData.ChoiceDatas);

                // 检测节点类型并获取特定变量值
                if(node.Type == NodeType.Dialogue)
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
                    GetQuestNode gNode = node as GetQuestNode;
                    gNode.toGetQuest = nodeData.ToGetQuest;
                }
                else if(node.Type == NodeType.CheckQuest)
                {
                    CheckQuestNode cNode = node as CheckQuestNode;
                    cNode.toCheckQuest = nodeData.ToCheckQuest;
                }

                // 绘制节点
                node.Draw();

                // 获取所属分组
                BaseGroup group = null;
                if (!string.IsNullOrEmpty(nodeData.GroupID))
                {
                    group = loadedGroups[nodeData.GroupID];
                    group.AddElement(node);
                }

                // 记录该节点已被加载
                loadedNodes.Add(node.GUID, node);
            }

            return loadedNodes;
        }

        /// <summary>
        /// 载入节点连线
        /// </summary>
        /// <param name="loadedNodes"></param>
        private void LoadNodesConnections(Dictionary<string, BaseNode> loadedNodes)
        {
            // 遍历所有已载入的节点
            foreach(KeyValuePair<string, BaseNode> loadedNode in loadedNodes)
            {
                // 遍历节点的所有输出端口
                foreach(Port outputPort in loadedNode.Value.outputContainer.Children())
                {
                    ChoiceData choiceData = (ChoiceData)outputPort.userData;

                    if (string.IsNullOrEmpty(choiceData.NextNodeID))
                    {
                        continue;
                    }

                    // 获取下个节点的输入端口
                    Port nextNodeInputPort = loadedNodes[choiceData.NextNodeID].Input;

                    // 创建连线
                    graphView.CreateEdge(outputPort, nextNodeInputPort);
                }

                // 刷新端口样式
                loadedNode.Value.RefreshPorts();
            }
        }
        
        /// <summary>
        /// 载入变量数据
        /// </summary>
        /// <param name="oldVarDatas"></param>
        private void LoadVarDatas(List<VarData> oldVarDatas)
        {
            List<VarData> newVarDatas = DataUtility.CloneVarDatas(oldVarDatas);
            graphView.VarPanel.SetVarDatas(newVarDatas);
        }

        /// <summary>
        /// 载入便签数据
        /// </summary>
        /// <param name="notes"></param>
        private void LoadNoteDatas(List<NoteData> notes)
        {
            // 遍历便签数据列表
            foreach(NoteData note in notes)
            {
                graphView.CreateNote(note.Title, note.Content, note.Layout);
            }
        }

        /// <summary>
        /// 当窗口即将销毁时调用
        /// </summary>
        private void OnDestroy()
        {
            bool confirm = EditorUtility.DisplayDialog(
                "警告",
                "需要为您保存当前窗口的所有数据吗？",
                "确认",
                "取消"
            );

            if (confirm)
            {
                SaveStory();
            }
        }
    }
}