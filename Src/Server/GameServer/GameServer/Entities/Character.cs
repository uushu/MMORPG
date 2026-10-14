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
    class Character : CharacterBase
    {
        public TCharacter Data;


        public ItemManager ItemManager;
        public StatusManager StatusManager;
        //public QuestManager QuestManager;
        //public FriendManager FriendManager;

        //public Team Team;
        //public double TeamUpdateTS;

        //public Guild Guild;

        //public Chat Chat;

        public Character(CharacterType type,TCharacter cha):
            base(new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ),new Core.Vector3Int(100,0,0))
        {
            this.Data = cha;
            this.Info = new NCharacterInfo();
            this.Info.Type = type;
            this.Info.Id = cha.ID;
            this.Info.Name = cha.Name;
            this.Info.Level = 1;//cha.Level;
            this.Info.Tid = cha.TID;
            this.Info.Class = (CharacterClass)cha.Class;
            this.Info.mapId = cha.MapID;
            this.Info.Gold = cha.Money;
            this.Info.Entity = this.EntityData;

            this.ItemManager = new ItemManager(this);
            this.ItemManager.GetItemInfos(this.Info.Items);

            this.Info.Bag = new NBagInfo();
            this.Info.Bag.unLocked = this.Data.Bag.UnLocked;
            this.Info.Bag.Items = this.Data.Bag.Items;
            this.Info.Equips = this.Data.Equips;
            this.StatusManager = new StatusManager(this);

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

        
    }
}
