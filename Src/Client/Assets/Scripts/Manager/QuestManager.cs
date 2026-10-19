using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.Data;
using Models;
using Service;
using SkillBridge.Message;
using UI;

namespace Manager
{
    public enum NpcQuestStatus
    {
        None = 0,
        Complete ,
        Available ,
        InProgress ,
    }

    public class QuestManager : Singleton<QuestManager>
    {
        public List<NQuestInfo> questInfo;
        public Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();

        public Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>> npcQuest = new Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>>();

        public event Action<Quest> onQuestStatusChanged;

        public void Init(List<NQuestInfo> quests)
        {
            this.questInfo = quests;
            allQuests.Clear();
            this.npcQuest.Clear();
            InitQuests();
        }

        // 保存每个npc身上的任务状态
        private void InitQuests()
        {
            // 初始化已接任务
            foreach (var info in questInfo)
            {
                //创建并初始化 任务
                Quest quest = new Quest(info);
                this.allQuests[quest.Info.QuestId] = quest;
            }

            this.CheckAcailableQuests();

            // 把可接的、未完成的、已完成的任务加到allQuests之后，再到Npc给任务根据状态分类
            foreach (var kv in this.allQuests)
            {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC,kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC,kv.Value);
                
            }
            
            
        }

        private void CheckAcailableQuests()
        {
            //检索条件
            foreach (var kv in DataManager.Instance.Quests)
            {
                if (kv.Value.LimitClass != CharacterClass.None &&
                    kv.Value.LimitClass != User.Instance.CurrentCharacter.Class)
                    continue;
                if(kv.Value.LimitLevel > User.Instance.CurrentCharacter.Level)
                    continue;
                if(this.allQuests.ContainsKey(kv.Key))
                    continue;
                if (kv.Value.PreQuest > 0)
                {
                    Quest preQuest;
                    if (this.allQuests.TryGetValue(kv.Value.PreQuest, out preQuest))
                    {
                        if(preQuest.Info == null)
                            continue; //前置任务未接
                        if(preQuest.Info.Status != QuestStatus.Finished)
                            continue; //前置任务未完成
                    }
                    else
                        continue;
                }

                Quest quest = new Quest(kv.Value);
                this.allQuests[quest.Define.ID] = quest;

            }
        }


        /// <summary>
        ///  判断任务状态，添加到角色的不同状态任务列表
        /// </summary>
        /// <param name="npcID"></param>
        /// <param name="quest"></param>
        private void AddNpcQuest(int npcID, Quest quest)
        {
            if (!this.npcQuest.ContainsKey(npcID))
                this.npcQuest[npcID] = new Dictionary<NpcQuestStatus, List<Quest>>();

            List<Quest> available;
            List<Quest> inProgress;
            List<Quest> complates;
            
            // 初始化不同状态任务列表
            if(!this.npcQuest[npcID].TryGetValue(NpcQuestStatus.Available ,out available))
            {
                available = new List<Quest>();
                this.npcQuest[npcID][NpcQuestStatus.Available] = available;
            }
            if(!this.npcQuest[npcID].TryGetValue(NpcQuestStatus.InProgress ,out inProgress))
            {
                inProgress = new List<Quest>();
                this.npcQuest[npcID][NpcQuestStatus.InProgress] = inProgress;
            }
            if(!this.npcQuest[npcID].TryGetValue(NpcQuestStatus.Complete ,out complates))
            {
                complates = new List<Quest>();
                this.npcQuest[npcID][NpcQuestStatus.Complete] = complates;
            }


            // 根据任务状态加入相应列表
            if (quest.Info == null)
            {
                if(npcID == quest.Define.AcceptNPC && !this.npcQuest[npcID][NpcQuestStatus.Available].Contains(quest))
                {
                    this.npcQuest[npcID][NpcQuestStatus.Available].Add(quest);
                }
            }
            else
            {
                if(quest.Define.SubmitNPC == npcID && quest.Info.Status == QuestStatus.InProgress)
                {
                    if(!this.npcQuest[npcID][NpcQuestStatus.InProgress].Contains(quest))
                        this.npcQuest[npcID][NpcQuestStatus.InProgress].Add(quest);
                }
                if(quest.Define.SubmitNPC == npcID && quest.Info.Status == QuestStatus.Completed)
                {
                    if(!this.npcQuest[npcID][NpcQuestStatus.Complete].Contains(quest))
                        this.npcQuest[npcID][NpcQuestStatus.Complete].Add(quest);
                }
            }
            
        }


        public NpcQuestStatus GetQuestStatus(int npcID)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (this.npcQuest.TryGetValue(npcID, out status))
            {
                // 优先级固定
                if (status[NpcQuestStatus.Complete].Count > 0)
                    return NpcQuestStatus.Complete;
                if (status[NpcQuestStatus.Available].Count > 0)
                    return NpcQuestStatus.Available;
                if (status[NpcQuestStatus.InProgress].Count > 0)
                    return NpcQuestStatus.InProgress;
            }

            return NpcQuestStatus.None;
        }

        public bool OpenNpcQuest(int npcID)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (this.npcQuest.TryGetValue(npcID, out status))
            {
                if (status[NpcQuestStatus.Complete].Count > 0)
                    return showQuestDialog(status[NpcQuestStatus.Complete].First());
                if (status[NpcQuestStatus.Available].Count > 0)
                    return showQuestDialog(status[NpcQuestStatus.Available].First());
                if (status[NpcQuestStatus.InProgress].Count > 0)
                    return showQuestDialog(status[NpcQuestStatus.InProgress].First());
            }

            return false;
        }

        private bool showQuestDialog(Quest quest)
        {
            if (quest.Info == null || quest.Info.Status == QuestStatus.Completed)
            {
                UIQuestDialog dialog = UIManager.Instance.Show<UIQuestDialog>();
                dialog.SetQuest(quest);
                dialog.OnClose += OnQuestDialogClose;
                return true;
            }
            
            if (quest.Info != null || quest.Info.Status == QuestStatus.InProgress)
            {
                if (!string.IsNullOrEmpty(quest.Define.DialogInComplete))
                    MessageBox.Show(quest.Define.DialogInComplete);
            }

            return true;
        }

        private void OnQuestDialogClose(UIWindow sender, UIWindow.WindowResult result)
        {
            UIQuestDialog dlg = (UIQuestDialog) sender;
            if (result == UIWindow.WindowResult.Yes)
            {
                if (dlg.quest.Info == null)
                    QuestService.Instance.SendQuestAcceptRequest(dlg.quest);
                else if(dlg.quest.Info.Status == QuestStatus.Completed)
                    QuestService.Instance.SendQuestSubmitRequest(dlg.quest);
            }
            else if (result == UIWindow.WindowResult.No)
            {
                MessageBox.Show(dlg.quest.Define.DialogDeny);
            }
        }

        public void OnQuestAccepted(NQuestInfo info)
        {
            var quest = RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogAccept);
        }

        public void OnQuestSubmited(NQuestInfo info)
        {
            var quest = RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogFinish);
        }

        private Quest RefreshQuestStatus(NQuestInfo info)
        {
            this.npcQuest.Clear();
            Quest quest;
            if (this.allQuests.ContainsKey(info.QuestId))
            {
                // 更新任务状态
                this.allQuests[info.QuestId].Info = info;
                quest = this.allQuests[info.QuestId];
                
            }
            else
            {
                quest = new Quest(info);
                this.allQuests.Add(info.QuestId, quest);
            }
            
            this.CheckAcailableQuests();

            // 把可接的、未完成的、已完成的任务加到allQuests之后，再到Npc给任务根据状态分类
            foreach (var kv in this.allQuests)
            {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC,kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC,kv.Value);
                
            }

            if (onQuestStatusChanged != null)
                onQuestStatusChanged(quest);

            return quest;
        }
    }
    
}