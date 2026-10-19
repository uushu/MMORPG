using Common.Data;
using SkillBridge.Message;

namespace Models
{
    public class Quest
    {
        public QuestDefine Define; //本地配置信息
        public NQuestInfo Info; //网络信息
        
        public Quest()
        {}

        public Quest(NQuestInfo info)
        {
            this.Info = info;
            this.Define = DataManager.Instance.Quests[info.QuestId];
        }

        public Quest(QuestDefine define)
        {
            this.Define = define;
            this.Info = null;
        }
    }
}