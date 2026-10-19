using Common.Data;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    internal class QuestManager
    {
        private Character owner;

        public QuestManager(Character character)
        {
            this.owner = character;
        }

        
        internal void GetQuestInfos(List<NQuestInfo> quests)
        {
            foreach (var quest in owner.Data.Quests)
            {
                quests.Add(GetQuestInfo(quest));
            }
        }

        private NQuestInfo GetQuestInfo(TCharacterQuest quest)
        {
            return new NQuestInfo
            {
                QuestId = quest.QuestId,
                QuestGuid = quest.Id,
                Status = (QuestStatus)quest.Status,
                Targets = new int[3]
                {
                    quest.Target1,
                    quest.Target2,
                    quest.Target3,
                }


            };
        }

        internal Result AcceptQuest(NetConnection<NetSession> sender, int questId)
        {
            Character character = sender.Session.Character;
            QuestDefine quest;

            if (DataManager.Instance.Quests.TryGetValue(questId, out quest))
            {
                var dbQuest = DBService.Instance.Entities.CharacterQuests.Create();
                dbQuest.QuestId = questId;
                if (quest.Target1 == QuestTarget.None)
                    dbQuest.Status = (int)QuestStatus.Completed;
                else
                    dbQuest.Status = (int)QuestStatus.InProgress;

                sender.Session.Response.questAccept.Quest = this.GetQuestInfo(dbQuest);
                character.Data.Quests.Add(dbQuest);
                DBService.Instance.Save();

                return Result.Success;
            }
            else
            {
                sender.Session.Response.questAccept.Errormsg = "任务不存在";
                return Result.Failed;
            }
        }

        internal Result SubmitQuest(NetConnection<NetSession> sender, int questId)
        {
            Character character = sender.Session.Character;
            
            QuestDefine quest;
            if (DataManager.Instance.Quests.TryGetValue(questId, out quest))
            {
                var dbQuest = character.Data.Quests.Where(q => q.QuestId == questId).FirstOrDefault();
                if(dbQuest != null)
                {
                    if(dbQuest.Status !=(int)QuestStatus.Completed)
                    {
                        sender.Session.Response.questSubmit.Errormsg = "任务还未完成";
                        return Result.Failed;
                    }

                    dbQuest.Status = (int)QuestStatus.Finished;
                    sender.Session.Response.questSubmit.Quest = this.GetQuestInfo(dbQuest);
                    DBService.Instance.Save();

                    if(quest.RewardExp>0)
                    {
                        //character.Exp += quest.RewardExp;
                    }

                    if(quest.RewardGold>0)
                    {
                        character.Gold += quest.RewardGold;
                    }

                    if(quest.RewardItem1>0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem1, quest.RewardItem1Count);
                    }
                    if (quest.RewardItem2 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem2, quest.RewardItem1Count);
                    }
                    if(quest.RewardItem3>0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem3, quest.RewardItem1Count);
                    }

                    DBService.Instance.Save();
                    return Result.Success;

                }
                sender.Session.Response.questSubmit.Errormsg = "任务不存在[1]";
                return Result.Failed;
            }
            else
            {
                sender.Session.Response.questAccept.Errormsg = "任务不存在[2]";
                return Result.Failed;
            }
        }
    }
}
