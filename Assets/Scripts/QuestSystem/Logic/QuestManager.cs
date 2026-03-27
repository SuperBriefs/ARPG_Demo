using System;
using System.Collections.Generic;
using UnityEngine;

namespace E.Story
{
    public class QuestManager : SingletonAutoMono<QuestManager>
    {
        private const string PANEL_NAME = "QuestPanel";
        
        [Serializable]
        public class QuestTask
        {
            // 当前任务
            public QuestSO questData;
            // 任务状态
            public bool IsComplete
            {
                get => questData.isComplete;
                set => questData.isComplete = value;
            }
        }

        // 任务列表
        public List<QuestTask> tasks = new List<QuestTask>();

        private bool isOpen;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                isOpen = !isOpen;
                if (isOpen)
                {
                    EventCenter.GetInstance().EventTrigger("锁定玩家");
                    UIManager.GetInstance().ShowPanel<QuestPanel>(PANEL_NAME, E_UI_Layer.Mid);
                }
                else
                {
                    EventCenter.GetInstance().EventTrigger("开启玩家");
                    UIManager.GetInstance().HidePanel(PANEL_NAME);
                }
            }
        }

        /// <summary>
        /// 更新任务完成进度
        /// </summary>
        /// <param name="requireName">需求的名称</param>
        /// <param name="amount">添加的需求个数</param>
        public void UpdateQuestProgress(string requireName, int amount)
        {
            // 可能不同任务有相同的需求名称 所以需要遍历所有需求
            foreach(QuestTask task in tasks)
            {
                foreach(QuestSO.QuestRequire require in task.questData.questRequires)
                {
                    if(require.name == requireName)
                    {
                        require.currentAmount += amount;
                        // 判断任务是否完成
                        if(require.currentAmount >= require.requireAmount)
                        {
                            require.currentAmount = require.requireAmount;
                        }

                        // 每更新一次进度就检查是否完成任务
                        if (task.questData.CheckQuestProgress())
                        {
                            // 播放任务完成的音效
                            MusicMgr.GetInstance().PlaySound("victory", false, (source) =>
                            {
                                StartCoroutine(RecoverAfterPlaying(source));
                            });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 判断当前任务中是否有这个任务了
        /// </summary>
        /// <param name="data">任务数据</param>
        /// <returns>是否已有这个任务</returns>
        public bool HaveQuest(QuestSO data)
        {
            foreach(QuestTask task in tasks)
            {
                if(task.questData.questName == data.questName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取当前的任务数据
        /// </summary>
        /// <param name="data">任务数据</param>
        /// <returns>获取到的任务数据</returns>
        public QuestTask GetTask(QuestSO data)
        {
            foreach(QuestTask task in tasks)
            {
                if(task.questData.questName == data.questName)
                {
                    return task;
                }
            }
            return null;
        }

        /// <summary>
        /// 回收音频
        /// </summary>
        /// <param name="source"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        System.Collections.IEnumerator RecoverAfterPlaying(AudioSource source)
        {
            while (source.isPlaying)
            {
                yield return null;
            }
            MusicMgr.GetInstance().StopSound(source);
        }
    }
}
