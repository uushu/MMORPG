using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Entities;
using SkillBridge.Message;

namespace GameServer.Managers
{
    internal class StatusManager
    {
        Character Owner;

        private List<NStatus> Status { get; set; }

        public bool HasStatus
        {
            get { return this.Status.Count > 0; }
        }

        public StatusManager(Character owner)
        {
            this.Owner = owner;
            this.Status = new List<NStatus>();
        }

        public void AddStatus(StatusType type, int id,int value,StatusAction action)
        {
            this.Status.Add(new NStatus()
            {
                Type = type,
                Id = id,
                Value = value,
                Action = action
            });
        }

        public void AddGoldChange(int goldDetail)
        {
            if (goldDetail > 0)
            {
                this.AddStatus(StatusType.Money, 0, goldDetail, StatusAction.Add);
            }
            if(goldDetail < 0)
            {
                this.AddStatus(StatusType.Money, 0, -goldDetail, StatusAction.Delete);
            }

        }
        public void AddItemChange(int id,int count,StatusAction action)
        {
            this.AddStatus(StatusType.Item ,id ,count,action);
        }

        public void ApplyResponse(NetMessageResponse message)
        {
            if (message.statusNotify == null)
                message.statusNotify = new StatusNotify();
            foreach (var status in this.Status)
            {
                message.statusNotify.Status.Add(status);
            }

            this.Status.Clear();
        }
    }
}
