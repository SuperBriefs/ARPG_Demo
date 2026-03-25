using UnityEngine;
using UnityEngine.UI;

namespace E.Story
{
    public class UIBranchItem : UIItem<ChoiceData>
    {
        // 主要组件
        private DialogueController dialogueController;
        private DialoguePanel dialoguePanel;

        [Header("UI组件")]
        [SerializeField] private Button btnBranch;
        [SerializeField] private Text txtBranch;

        void Start()
        {
            dialogueController = GetComponentInParent<DialogueController>();
            dialoguePanel = GetComponentInParent<DialoguePanel>();
        }

        public override void Refresh()
        {
            // 更新选项文本
            txtBranch.text = Data.Text;

            // 绑定点击事件
            btnBranch.onClick.RemoveAllListeners();
            btnBranch.onClick.AddListener(() =>
            {
                dialoguePanel.HideBranchs();
                dialogueController.DoNode(Data.NextNodeID);
            });
        }
    }
}