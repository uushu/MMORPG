using System;
using Manager;
using Models;
using Network;
using SkillBridge.Message;
using UI;
using UnityEngine;

namespace Service
{
    public class ItemService : Singleton<ItemService> ,IDisposable
    {
        public ItemService()
        {
            MessageDistributer.Instance.Subscribe<ItemBuyResponse>(this.OnItemBuy);
            MessageDistributer.Instance.Subscribe<ItemEquipResponse>(this.OnItemEquip);
            
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ItemBuyResponse>(this.OnItemBuy);

            MessageDistributer.Instance.Unsubscribe<ItemEquipResponse>(this.OnItemEquip);

        }

      

        /// <summary>
        /// 发送购买道具请求
        /// </summary>
        /// <param name="shopID"></param>
        /// <param name="shopItemID"></param>
        public void SendBuyItem(int shopID, int shopItemID)
        {
            Debug.Log("SendBuyItem");

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemBuy = new ItemBuyRequest();
            message.Request.itemBuy.shopId = shopID;
            message.Request.itemBuy.shopItemId = shopItemID;
            NetClient.Instance.SendMessage(message);

        }
        
        /// <summary>
        /// 接收购买道具响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnItemBuy(object sender, ItemBuyResponse message)
        {
            MessageBox.Show("购买结果：" + message.Result + "\n" + message.Errormsg, "购买完成");
            
        }

        private Item pendingEquip = null;
        private bool isEquip;

        /// <summary>
        ///  发送穿脱装备请求
        /// </summary>
        /// <param name="equip"></param>
        /// <param name="isEquip"></param>
        /// <returns></returns>
        public bool SendEquipItem(Item equip, bool isEquip)
        {
            if (pendingEquip != null)
                return false;
            pendingEquip = equip;
            this.isEquip = isEquip;

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemEquip = new ItemEquipRequest();
            message.Request.itemEquip.itemId = equip.Id;
            message.Request.itemEquip.isEquip = isEquip;
            message.Request.itemEquip.Slot = (int)equip.EquipInfo.Slot;
            
            NetClient.Instance.SendMessage(message);
            return true;
        }

        
        private void OnItemEquip(object sender, ItemEquipResponse message)
        {
            if (message.Result == Result.Success)
            {
                if (pendingEquip != null)
                {
                    if(this.isEquip)
                        EquipManager.Instance.OnEquipItem(pendingEquip);
                    else
                        EquipManager.Instance.OnUnEquipItem(pendingEquip.EquipInfo.Slot);
                    pendingEquip = null;
                }
            }
        }

    }
}