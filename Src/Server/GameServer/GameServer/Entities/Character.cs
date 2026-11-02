using Common;
using Common.Data;
using Common.Utils;
using GameServer.Core;
using GameServer.Managers;
using GameServer.Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    /// <summary>
    /// Character
    /// 玩家角色类
    /// </summary>
    class Character : CharacterBase ,IPostResponser
    {
        public TCharacter Data;


        public ItemManager ItemManager;
        public StatusManager StatusManager;
        public QuestManager QuestManager;
        public FriendManager FriendManager;

        public Team Team;
        public double TeamUpdateTS; //队伍信息更新时间戳

        public Guild Guild;
        public double GuildUpdateTS;

        //public Chat Chat;

        public Character(CharacterType type,TCharacter cha):
            base(new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ),new Core.Vector3Int(100,0,0))
        {
            this.Data = cha;
            this.Id = cha.ID;
            this.Info = new NCharacterInfo();
            this.Info.Type = type;
            this.Info.Id = cha.ID;
            this.Info.EntityId = this.entityId;
            this.Info.Name = cha.Name;
            this.Info.Level = 1;    //cha.Level;
            this.Info.ConfigId = cha.TID;
            this.Info.Class = (CharacterClass)cha.Class;
            this.Info.mapId = cha.MapID;
            this.Info.Gold = cha.Money;
            this.Info.Entity = this.EntityData;
            this.Define = DataManager.Instance.Characters[this.Info.ConfigId];

            this.ItemManager = new ItemManager(this);
            this.ItemManager.GetItemInfos(this.Info.Items);

            this.Info.Bag = new NBagInfo();
            this.Info.Bag.unLocked = this.Data.Bag.UnLocked;
            this.Info.Bag.Items = this.Data.Bag.Items;
            this.Info.Equips = this.Data.Equips;
            this.StatusManager = new StatusManager(this);


            this.QuestManager = new QuestManager(this);
            this.QuestManager.GetQuestInfos(this.Info.Quests);

            this.FriendManager = new FriendManager(this);
            this.FriendManager.GetFriendInfos(this.Info.Friends);
            this.Guild = GuildManager.Instance.GetGuild(this.Data.GuildID);
        }

        public long Gold
        {
            get
            {
                return this.Data.Money;
            }
            set
            {
                if (this.Data.Money == value)
                    return;
                this.StatusManager.AddGoldChange((int)(value - this.Data.Money));
                this.Data.Money = value;
            }
        }

        /// <summary>
        /// 各种后处理信息
        /// </summary>
        /// <param name="message"></param>
        public void PostProcess(NetMessageResponse message)
        {
            Log.InfoFormat("PostProcess: Character: characterID{0}:{1}", this.Id, this.Info.Name);
            
            // 好友状态信息更新
            this.FriendManager.PostProgress(message);
            // 组队状态信息更新
            lock(TeamManager.Instance.SyncRoot)
            {
                if (this.Team != null)
                {
                    Log.InfoFormat("PostProcess: Character: characterID{0}:{1} {2}<{3}", this.Id, this.Info.Name, TeamUpdateTS, Team.timestamp);
                    if (TeamUpdateTS < this.Team.timestamp)
                    {
                        TeamUpdateTS = this.Team.timestamp;
                        this.Team.PostProcess(message);
                    }
                }

            }

            // 公会状态信息更新
            if (this.Guild != null)
            {
                Log.InfoFormat("PostProcess > Guild: CharacterID:{0}:{1},{2}<{3}", this.Id, this.Info.Name, this.GuildUpdateTS ,this.Guild.timestamp);
                if(this.Info.Guild ==null)
                {
                    this.Info.Guild = this.Guild.GetGuildInfo(this);
                    if(message.mapCharacterEnter != null)
                    {
                        GuildUpdateTS = Guild.timestamp;
                    }
                }
                if(GuildUpdateTS < this.Guild.timestamp & message.mapCharacterEnter == null)
                {
                    GuildUpdateTS = this.Guild.timestamp;
                    this.Guild.PostProcess(this, message);
                }
            }

            // 各种状态信息更新
            if (this.StatusManager.HasStatus)
            {
                this.StatusManager.PostProgress(message);
            }
        }

        /// <summary>
        /// 角色离开时调用
        /// </summary>
        internal void Clear()
        {
            this.FriendManager.OffLineNotify();
        }

       
        internal NCharacterInfo GetBasicInfo()
        {
            return new NCharacterInfo
            {
                Id = this.Id,
                Name = this.Info.Name,
                Class = this.Info.Class,
                Level = this.Info.Level,
            };
        }
    }
}
