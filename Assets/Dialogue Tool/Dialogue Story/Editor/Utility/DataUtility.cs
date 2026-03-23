using System.Collections.Generic;

namespace E.Story
{
    // 数据处理实用类
    public static class DataUtility
    {
        /// <summary>
        /// 克隆条件数据列表
        /// </summary>
        /// <param name="oldDatas"></param>
        /// <returns></returns>
        public static List<ConditionData> CloneConditionDatas(List<ConditionData> oldDatas)
        {
            List<ConditionData> newDatas = new List<ConditionData>();
            foreach(ConditionData data in oldDatas)
            {
                ConditionData newData = new ConditionData(data.VarIndex, data.Value, data.Compare);
                newDatas.Add(newData);
            }

            return newDatas;
        } 

        /// <summary>
        /// 克隆选择数据列表
        /// </summary>
        /// <param name="oldDatas">选择数据列表</param>
        /// <returns>选择数据列表</returns>
        public static List<ChoiceData> CloneChoiceDatas(List<ChoiceData> oldDatas)
        {
            List<ChoiceData> newDatas = new List<ChoiceData>();
            foreach (ChoiceData data in oldDatas)
            {
                List<ConditionData> conditionDatas = CloneConditionDatas(data.Conditions);
                ChoiceData newData = new ChoiceData(data.Text, data.NextNodeID, data.DetectMode, conditionDatas);
                newDatas.Add(newData);
            }

            return newDatas;
        }

        /// <summary>
        /// 克隆句子数据列表
        /// </summary>
        /// <param name="oldDatas">句子数据</param>
        /// <returns>句子数据</returns>
        public static List<SentenceData> CloneSentenceDatas(List<SentenceData> oldDatas)
        {
            if(oldDatas == null)
            {
                return null;
            }

            List<SentenceData> newDatas = new List<SentenceData>();
            foreach(SentenceData data in oldDatas)
            {
                SentenceData newData = new SentenceData(data.Text);
                newDatas.Add(newData);
            }

            return newDatas;
        }

        /// <summary>
        /// 克隆变量数据列表
        /// </summary>
        /// <param name="oldDatas">变量数据列表</param>
        /// <returns>变量数据列表</returns>
        public static List<VarData> CloneVarDatas(List<VarData> oldDatas)
        {
            if(oldDatas == null)
            {
                return null;
            }

            List<VarData> newDatas = new List<VarData>();
            foreach(VarData data in oldDatas)
            {
                VarData newData = new VarData(data.Key, data.Value);
                newDatas.Add(newData);
            }

            return newDatas;
        }
        
        /// <summary>
        /// 克隆编辑变量列表数据
        /// </summary>
        /// <param name="oldDatas"></param>
        /// <returns></returns>
        public static List<EditVarData> CloneEditVarDatas(List<EditVarData> oldDatas)
        {
            if(oldDatas == null)
            {
                return null;
            }

            List<EditVarData> newDatas = new List<EditVarData>();
            foreach(EditVarData data in oldDatas)
            {
                EditVarData newData = new EditVarData
                {
                    VarIndex = data.VarIndex,
                    Value = data.Value,
                    Operation = data.Operation,
                };
                newDatas.Add(newData);
            }

            return newDatas;
        }
    }
}