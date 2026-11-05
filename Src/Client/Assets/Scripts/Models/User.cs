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
        NUserInfo userInfo;
        public NUserInfo Info
        {
            get { return userInfo; }
        }

        public void SetupUserInfo(NUserInfo info)
        {
            this.userInfo = info;
        }


        public MapDefine CurrentMapData { get; set; }

        public NCharacterInfo CurrentCharacter { get; set; }

        public PlayerInputController CurrentCharacterObject { get; set; }

        public void AddGold(int goldChange)
        {
            this.CurrentCharacter.Gold += goldChange;
        }

        public int CurrentRide = 0;

        public void Ride(int id)
        {
            if (CurrentRide != id)
            {
                CurrentRide = id;
                CurrentCharacterObject.SendEntityEvent(EntityEvent.Ride, CurrentRide);
            }
            else
            {
                CurrentRide = 0;
                CurrentCharacterObject.SendEntityEvent(EntityEvent.Ride, 0);
            }
        }
    }
    
}