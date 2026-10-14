using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Data;
using SkillBridge.Message;
using UnityEngine;

namespace Models
{
    class User : Singleton<User>
    {
        public event Action<long> GoldChanged;
        SkillBridge.Message.NUserInfo userInfo;
        public SkillBridge.Message.NUserInfo Info
        {
            get { return userInfo; }
        }


        public void SetupUserInfo(SkillBridge.Message.NUserInfo info)
        {
            this.userInfo = info;
        }


        public MapDefine CurrentMapData { get; set; }

        public NCharacterInfo CurrentCharacter { get; set; }

        public GameObject CurrentCharacterObject { get; set; }

        public void AddGold(int goldChange)
        {
            this.CurrentCharacter.Gold += goldChange;
            GoldChanged?.Invoke(this.CurrentCharacter.Gold);
        }
    }
    
}