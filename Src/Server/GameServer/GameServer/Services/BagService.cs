using Common;
using GameServer.Entities;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    internal class BagService:Singleton<BagService>
    {
        public BagService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<BagSaveRequest>(this.OnBagSave);

        }


        public void Init()
        {

        }
        private void OnBagSave(NetConnection<NetSession> sender, BagSaveRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("BagSaveRequest: character:{0} Unclocked{1}", character.Id, message.bagInfo.unLocked);

            if(message.bagInfo != null)
            {
                character.Data.Bag.Items = message.bagInfo.Items;
                DBService.Instance.Save();
            }
        }

       
    }
}
