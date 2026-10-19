using System;
using Manager;
using Models;
using Network;
using SkillBridge.Message;
using UI;
using UnityEngine;

namespace Service
{
    public class QuestService : Singleton<QuestService> , IDisposable
    {
        public QuestService()
        {
            MessageDistributer.Instance.Subscribe<QuestAcceptResponse>(this.OnQuestAccept);
            MessageDistributer.Instance.Subscribe<QuestSubmitResponse>(this.OnQuestSubmit);
            
        }

       
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<QuestAcceptResponse>(this.OnQuestAccept);
            MessageDistributer.Instance.Unsubscribe<QuestSubmitResponse>(this.OnQuestSubmit);
            
        }

        public bool SendQuestAcceptRequest(Quest quest)
        {
            Debug.Log("SendQuestAcceptRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questAccept = new QuestAcceptRequest();
            message.Request.questAccept.QuestId = quest.Define.ID;
            NetClient.Instance.SendMessage(message);
            return true;
        }

        
        private void OnQuestAccept(object sender, QuestAcceptResponse message)
        {
            Debug.LogFormat("OnQuestAccept: RES:{0}  ERR:{1}",message.Result,message.Errormsg);
            if (message.Result == Result.Success)
            {
                QuestManager.Instance.OnQuestAccepted(message.Quest);
            }
            else
            {
                MessageBox.Show("任务接受失败", "错误",MessageBoxType.Error);
            }
        }
        public bool SendQuestSubmitRequest(Quest quest)
        {
            Debug.Log("SendQuestSubmitRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questSubmit = new QuestSubmitRequest();
            message.Request.questSubmit.QuestId = quest.Define.ID;
            NetClient.Instance.SendMessage(message);
            return true;
        }

        private void OnQuestSubmit(object sender, QuestSubmitResponse message)
        {
            Debug.LogFormat("OnQuestSubmit: RES:{0}  ERR:{1}",message.Result,message.Errormsg);
            if (message.Result == Result.Success)
            {
                QuestManager.Instance.OnQuestSubmited(message.Quest);
            }
            else
            {
                MessageBox.Show("任务完成失败", "错误",MessageBoxType.Error);
            }
        }
        

    }
}