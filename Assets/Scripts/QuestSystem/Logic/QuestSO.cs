using System;
using System.Collections.Generic;
using UnityEngine;

namespace E.Story
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest Data")]
    public class QuestSO : ScriptableObject
    {
        private const string INVENTORYPANEL_PATH = "InventoryPanel";

        [Serializable]
        public class QuestRequire
        {
            // 需求物品名字
            public string name;
            // 需求数量
            public int requireAmount;
            // 当前完成的数量
            public int currentAmount;
        }

        // 任务名字
        public string questName;
        // 任务描述
        [TextArea]
        public string description;

        // 任务状态
        public bool isComplete;

        // 任务需求列表
        public List<QuestRequire> questRequires = new List<QuestRequire>();
        // TODO: 任务奖励列表(与后续背包系统中的物体关联)
        public List<ItemSO> rewards = new List<ItemSO>();

        /// <summary>
        /// 检查任务完成情况
        /// </summary>
        public bool CheckQuestProgress()
        {
            List<QuestRequire> finishedRequire = new List<QuestRequire>();
            foreach(QuestRequire require in questRequires)
            {
                if(require.currentAmount >= require.requireAmount)
                {
                    finishedRequire.Add(require);
                }
            }

            // 所有需求都完成表示完成任务
            isComplete = finishedRequire.Count == questRequires.Count;

            if (isComplete)
            {
                Debug.Log(questName + "任务完成");
            }

            return isComplete;
        }

        /// <summary>
        /// TODO: 获得奖励
        /// </summary>
        public void GiveRewards()
        {
            foreach(ItemSO reward in rewards)
            {
                InventoryManager.GetInstance().AddItem(reward);

                // 开启背包界面时要实时更新显示面板
                if (UIManager.GetInstance().GetPanel<InventoryPanel>(INVENTORYPANEL_PATH))
                {
                    // 先回收 再显示
                    InventoryManager.GetInstance().RecoverInventoryItems();
                    InventoryManager.GetInstance().ShowInventoryItems(UIManager.GetInstance().GetPanel<InventoryPanel>(INVENTORYPANEL_PATH));
                }
            }
        }
    }
}