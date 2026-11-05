using System.Collections.Generic;
using System.Text;
using Models;
using Service;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.Events;

namespace Manager
{
    public class ChatManager:Singleton<ChatManager>
    {
        public enum Channel
        {
            All = 0,    // 所有
            Local = 1,  // 本地
            World = 2,  // 世界
            Team = 3,   // 队伍
            Guild = 4,  // 公会
            Private = 5, // 私聊
        }

        private ChatChannel[] ChannelFirlter = new ChatChannel[6]
        {
            ChatChannel.Local | ChatChannel.World | ChatChannel.Team | ChatChannel.Guild | ChatChannel.Private | ChatChannel.System,
            ChatChannel.Local,
            ChatChannel.World,
            ChatChannel.Team,
            ChatChannel.Guild,
            ChatChannel.Private,
           
        };
        public  UnityAction OnChat;
        public Channel displayChannel;
        public Channel sendChannel;
        
        public int PrivateID;
        public string PrivateName = "";

        public List<ChatMessage>[] Messages = new List<ChatMessage>[6]
        {
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>()
        };

        public void Init()
        {
            foreach(var message in Messages)
            {
                message.Clear();
            }
        }

        public ChatChannel SendChannel
        {
            get
            {
                switch(sendChannel)
                {
                    case Channel.Local:
                        return ChatChannel.Local;
                    case Channel.World:
                        return ChatChannel.World;
                    case Channel.Team:
                        return ChatChannel.Team;
                    case Channel.Guild:
                        return ChatChannel.Guild;
                    case Channel.Private:
                        return ChatChannel.Private;
                }

                return ChatChannel.Local;
            }
        }

        
        public void StartPrivateChat(int targetId, string targetName)
        {
            this.PrivateID = targetId;
            this.PrivateName = targetName;
            this.sendChannel = Channel.Private;
            if (this.OnChat != null)
                this.OnChat();
        }

        public void SendChat(string content, int toId, string toName ="")
        {
            ChatService.Instance.SendChat(this.SendChannel,content,toId,toName);
        }

       
        public bool SetSendChannel(Channel index)
        {
            if (index == Channel.Team)
            {
                if(TeamManager.Instance.TeamInfo == null)
                {
                    this.AddSystemMessage("你没有加入任何队伍");
                    return false;
                }
            }
            
            if (index == Channel.Guild)
            {
                if(GuildManager.Instance.guildInfo == null)
                {
                    this.AddSystemMessage("你没有加入任何公会");
                    return false;
                }
            }
            
            this.sendChannel = index;
            Debug.LogFormat("Set Channel :{0}",this.sendChannel);
            return true;
        }

        public void AddMessages(ChatChannel channel, List<ChatMessage> messages)
        {
            for (int chat = 0; chat < 6; chat++)
            {
                if ((this.ChannelFirlter[chat] & channel) == channel)
                {
                    this.Messages[chat].AddRange(messages);
                }
                if(this.OnChat != null)
                    this.OnChat();
            }
        }

        public void AddSystemMessage(string message,string from ="")
        {
            this.Messages[(int)Channel.All].Add(new ChatMessage()
            {
                Channel = ChatChannel.System,
                Message = message,
                FromName = from
                
            });
            if (this.OnChat != null)
                this.OnChat();
        }

        public string GetCurrentMessage()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var message in Messages[(int)displayChannel])
            {
                sb.AppendLine(FormatMessage(message));
            }
            return sb.ToString();
        }


        private string FormatMessage(ChatMessage message)
        {
           switch(message.Channel)
            {
                case ChatChannel.Local:
                    return string.Format("[本地]{0}{1}", FormatFromPlayer(message), message.Message);
                case ChatChannel.World:
                    return string.Format("<color=cyan>[世界]{0}{1}</color>", FormatFromPlayer(message), message.Message);
                case ChatChannel.System:
                    return string.Format("<color=yellow>[系统]{0}</color>", message.Message);
                case ChatChannel.Team:
                    return string.Format("<color=blue>[队伍]{0}{1}</color>", FormatFromPlayer(message), message.Message);
                case ChatChannel.Guild:
                    return string.Format("<color=purple>[公会]{0}{1}</color>", FormatFromPlayer(message), message.Message);
                case ChatChannel.Private:
                    return string.Format("<color=green>[私聊]{0}{1}</color>", FormatFromPlayer(message), message.Message);
            }
            return "";
        }

        private string FormatFromPlayer(ChatMessage message)
        {
            if (message.FromName == User.Instance.CurrentCharacter.Name)
            {
                return "<a name=\"\" class=\"player\">[我]</a>";
            }
            else
            {
                return string.Format("<a name=\"c:{0}:{1}\" class=\"player\">[{1}]</a>", message.FromId, message.FromName);
            }
        }
    }
}