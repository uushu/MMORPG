using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class EquipManager : Singleton<EquipManager>
    {
        public Result EquipItem(NetConnection<NetSession> sender, int slot, int itemID, bool isEquip)
        {
            Character character = sender.Session.Character;
            if (!character.ItemManager.Items.ContainsKey(itemID)|| DataManager.Instance.Items[itemID].LimitClass != character.Info.Class)
            {
                return Result.Failed;
            }

            UpdateEquip(character.Data.Equips, slot, itemID, isEquip);
            DBService.Instance.Save();
            return Result.Success;
        }

        unsafe void UpdateEquip(byte[] equips, int slot, int itemID, bool isEquip)
        {
            fixed(byte* pt = equips)
            {
                int* slotPt = (int*)(pt + slot * sizeof(int));

                if (isEquip)
                    *slotPt = itemID;  //穿
                else
                    *slotPt = 0;  //脱
            }
        }
    }
}
