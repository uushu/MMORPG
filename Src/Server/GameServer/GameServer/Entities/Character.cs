using Common;
using Common.Data;
using Common.Utils;
using GameServer.Core;
using GameServer.Managers;

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

        //public Team Team;
        //public double TeamUpdateTS;

        //public Guild Guild;

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
            this.FriendManager.PostProgress(message);
            if(this.StatusManager.HasStatus)
            {
                this.StatusManager.PostProgress(message);
            }
        }

        /// <summary>
        /// 角色离开时调用
        /// </summary>
        internal void Clear()
        {
            // 把自己的信息更新到好友管理器中
            this.FriendManager.UpdateFriendInfo(this.Info, 0);
        }
    }
}
