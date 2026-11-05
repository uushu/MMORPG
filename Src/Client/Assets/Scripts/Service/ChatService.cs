using System;
using Manager;
using Network;
using SkillBridge.Message;
using UnityEngine;

namespace Service
{
    public class ChatService :Singleton<ChatService> ,IDisposable
    {
        public void Init()
        {
            
        }

        public ChatService()
        {
            MessageDistributer.Instance.Subscribe<ChatResponse>(this.OnChat);
        }
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ChatResponse>(this.OnChat);
        }
        public void SendChat(ChatChannel sendChannel, string content, int toId, string toName)
        {
            Debug.Log("SendChat");
            
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.Chat = new ChatRequest();
            message.Request.Chat.Chat = new ChatMessage();
            message.Request.Chat.Chat.Channel = sendChannel;
            message.Request.Chat.Chat.Message = content;
            message.Request.Chat.Chat.ToId = toId;
            message.Request.Chat.Chat.ToName = toName;

            NetClient.Instance.SendMessage(message);

        }

        private void OnChat(object sender, ChatResponse message)
        {
            if (message.Result == Result.Success)
            {
                ChatManager.Instance.AddMessages(ChatChannel.Local,message.localMessages);
                ChatManager.Instance.AddMessages(ChatChannel.World,message.worldMessages);
                ChatManager.Instance.AddMessages(ChatChannel.System,message.systemMessages);
                ChatManager.Instance.AddMessages(ChatChannel.Team,message.teamMessages);
                ChatManager.Instance.AddMessages(ChatChannel.Guild,message.guildMessages);
                ChatManager.Instance.AddMessages(ChatChannel.Private,message.privateMessages);
                
            }
            else
            {
                ChatManager.Instance.AddSystemMessage(message.Errormsg);
            }
        }

        

    }
}