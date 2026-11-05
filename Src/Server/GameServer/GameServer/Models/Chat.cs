using GameServer.Entities;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    internal class Chat
    {
        private Character Owner;

        public int localIndex;
        public int worldIndex;
        public int systemIndex;
        public int teamIndex;
        public int guildIndex;

        public Chat(Character character)
        {
            this.Owner = character;
        }

        internal void PostProcess(NetMessageResponse message)
        {
            if(message.Chat == null)
            {
                message.Chat = new ChatResponse();
                message.Chat.Result = Result.Success;
            }

            this.localIndex = ChatManager.Instance.GetLocalMessage(this.Owner.Info.mapId,this.localIndex, message.Chat.localMessages);
            this.worldIndex = ChatManager.Instance.GetWorldMessage(this.worldIndex, message.Chat.worldMessages);
            this.systemIndex = ChatManager.Instance.GetSystemMessage(this.systemIndex, message.Chat.systemMessages);
            
            if(this.Owner.Team != null)
                this.teamIndex = ChatManager.Instance.GetTeamMessage(this.Owner.Team.Id, this.teamIndex, message.Chat.teamMessages);

            if(this.Owner.Guild != null)
                this.guildIndex = ChatManager.Instance.GetGuildMessage(this.Owner.Guild.Id, this.guildIndex, message.Chat.guildMessages);


        }
    }
}
