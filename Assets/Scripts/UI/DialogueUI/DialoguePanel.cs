using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace E.Story
{
    public class DialoguePanel : BasePanel
    {
        [Header("主要组件")]
        [SerializeField] private DialogueController dialogueController;

        [Header("UI组件")]
        // 对话面板
        [SerializeField] private Text roleNameText;
        [SerializeField] private Text contextText;
        [SerializeField] private Image portraitsImage;
        [SerializeField] private Button continueButton;

        // 分支面板
        [SerializeField] private GameObject branchsBack;
        [SerializeField] private UIBranchList uiBranchList;

        protected override void OnClick(string btnName)
        {
            if(btnName == continueButton.name)
            {
                dialogueController.NextStep();
            }
        }

        /// <summary>
        /// 显示立绘
        /// </summary>
        /// <param name="sprite">立绘</param>
        public void ShowPortrait(Sprite sprite, int index = 0)
        {
            portraitsImage.sprite = sprite;
            portraitsImage.enabled = true;
        }

        /// <summary>
        /// 隐藏立绘
        /// </summary>
        public void HidePortrait(int index = 0)
        {
            portraitsImage.enabled = false;
        }

        /// <summary>
        /// 显示选项面板
        /// </summary>
        /// <param name="choiceDatas">选项数据</param>
        public void ShowBranchs(List<ChoiceData> choiceDatas)
        {
            branchsBack.SetActive(true);
            uiBranchList.SetData(choiceDatas);
        }

        /// <summary>
        /// 隐藏选项面板
        /// </summary>
        public void HideBranchs()
        {
            branchsBack.SetActive(false);
        }

        /// <summary>
        /// 显示对话面板
        /// </summary>
        /// <param name="rolename">角色名称</param>
        /// <param name="sentenceData">对话内容</param>
        public void UpdateDialogue(string rolename, SentenceData sentenceData)
        {
            roleNameText.text = rolename;
            contextText.text = sentenceData.Text;
        }
    }
}