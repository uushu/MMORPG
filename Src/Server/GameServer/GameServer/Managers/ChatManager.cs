using Common;
using Common.Utils;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    internal class ChatManager : Singleton<ChatManager>
    {
        public List<ChatMessage> System = new List<ChatMessage>();
        public List<ChatMessage> World = new List<ChatMessage>();
        public Dictionary<int, List<ChatMessage>> Local= new Dictionary<int, List<ChatMessage>>();
        public Dictionary<int, List<ChatMessage>> Team= new Dictionary<int, List<ChatMessage>>();
        public Dictionary<int, List<ChatMessage>> Guild= new Dictionary<int, List<ChatMessage>>();
      

        public void Init()
        {

        }

        public void AddMessage(Character from ,ChatMessage message)
        {
            message.FromId = from.Id;
            message.FromName = from.Name;
            message.Time = TimeUtil.timestamp;

            switch(message.Channel)
            {
                case ChatChannel.Local:
                    this.AddLocalMessage(from.Info.mapId,message);
                    break;
                case ChatChannel.World:
                    this.AddWorldMessage(message);
                    break;
                case ChatChannel.System:
                    this.AddSystemMessage(message);
                    break;
                case ChatChannel.Team:
                    this.AddTeamMessage(from.Team.Id,message);
                    break;
                case ChatChannel.Guild:
                    this.AddGuildMessage(from.Guild.Id,message);
                    break;
            
            }
        }

        private void AddLocalMessage(int mapId, ChatMessage message)
        {
            if(!this.Local.TryGetValue(mapId, out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                this.Local.Add(mapId, messages);
            }
            messages.Add(message);
        }

        private void AddWorldMessage(ChatMessage message)
        {
            this.World.Add(message);
        }

        private void AddSystemMessage(ChatMessage message)
        {
            this.System.Add(message);
        }

        private void AddTeamMessage(int id, ChatMessage message)
        {
            if (!this.Team.TryGetValue(id, out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                this.Team.Add(id, messages);
            }
            messages.Add(message);
        }

        private void AddGuildMessage(int id, ChatMessage message)
        {
            if (!this.Guild.TryGetValue(id, out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                this.Guild.Add(id, messages);
            }
            messages.Add(message);
        }


        public int GetLocalMessage(int mapId,int index ,List<ChatMessage> result)
        {
            if(!this.Local.TryGetValue(mapId, out List<ChatMessage> messages))
                return 0;
            return GetNewMessages(index,result,messages);
        }

        public int GetWorldMessage(int index,List<ChatMessage> result)
        {
            return GetNewMessages(index,result,this.World);
        }
        public int GetSystemMessage(int index, List<ChatMessage> result)
        {
            return GetNewMessages(index, result, this.System);
        }
        public int GetTeamMessage(int mapId, int index, List<ChatMessage> result)
        {
            if (!this.Team.TryGetValue(mapId, out List<ChatMessage> messages))
                return 0;
            return GetNewMessages(index, result, messages);
        }
        public int GetGuildMessage(int mapId, int index, List<ChatMessage> result)
        {
            if (!this.Guild.TryGetValue(mapId, out List<ChatMessage> messages))
                return 0;
            return GetNewMessages(index, result, messages);
        }

        private int GetNewMessages(int index, List<ChatMessage> result, List<ChatMessage> messages)
        {
            // 当聊天数量特别多的时候，只拉前20条
            if(index == 0)
            {
                if(messages.Count > GameDefine.ChatMaxRecoredNums)
                {
                    index = messages.Count - GameDefine.ChatMaxRecoredNums;
                }
            }

            for(;index<messages.Count;index++)
            {
                result.Add(messages[index]);
            }
            return index;
        }
    }
}
