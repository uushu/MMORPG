using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    internal class ChatService : Singleton<ChatService>
    {
        public ChatService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ChatRequest>(this.OnChat);
        }
        public void Init()
        {
            ChatManager.Instance.Init();
        }
        private void OnChat(NetConnection<NetSession> sender, ChatRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnChat: character:{0},Channel:{1},Message:{2}", character.Id, request.Chat.Channel, request.Chat.Message);
            if(request.Chat.Channel == ChatChannel.Private)
            {
                var chatTo = SessionManager.Instance.GetSession(request.Chat.ToId);
                if (chatTo == null)
                {
                    sender.Session.Response.Chat = new ChatResponse();
                    sender.Session.Response.Chat.Result = Result.Failed;
                    sender.Session.Response.Chat.Errormsg = "对方不在线";
                    sender.SendResponse();
                }
                else
                {
                    if(chatTo.Session.Response.Chat == null)
                    {
                        chatTo.Session.Response.Chat = new ChatResponse();
                    }
                    request.Chat.FromId = character.Id;
                    request.Chat.FromName = character.Name;
                    chatTo.Session.Response.Chat.Result = Result.Success;
                    chatTo.Session.Response.Chat.privateMessages.Add(request.Chat);
                    chatTo.SendResponse();

                    if (sender.Session.Response.Chat == null)
                    {
                        sender.Session.Response.Chat = new ChatResponse();
                    }
                    sender.Session.Response.Chat.Result= Result.Success;
                    sender.Session.Response.Chat.privateMessages.Add(request.Chat);
                    sender.SendResponse();
                }
            }
            else
            {
                sender.Session.Response.Chat = new ChatResponse();
                sender.Session.Response.Chat.Result = Result.Success;
                ChatManager.Instance.AddMessage(character, request.Chat);
                sender.SendResponse();
            }

        }
    }
}
