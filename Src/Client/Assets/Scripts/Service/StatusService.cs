using System;
using System.Collections.Generic;
using Common.Data;
using Models;
using Network;
using SkillBridge.Message;
using UnityEngine;

namespace Manager
{
    public class StatusService : Singleton<StatusService> ,IDisposable
    {
        public delegate bool StatusNotifyHandler(NStatus status);
        Dictionary<StatusType,StatusNotifyHandler> events = new Dictionary<StatusType, StatusNotifyHandler>();
        HashSet<StatusNotifyHandler> handler = new HashSet<StatusNotifyHandler>();
        public void Init()
        {
            
        }
        public void RegisterStatusNotify(StatusType type, StatusNotifyHandler action)
        {
            if (handler.Contains(action))
                return;
            if (!events.ContainsKey(type))
            {
                events[type] = action;
            }
            else
            {
                events[type] += action;
            }
            handler.Add(action);
        }

        public StatusService()
        {
            MessageDistributer.Instance.Subscribe<StatusNotify>(this.OnStatusNotify);
        }
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<StatusNotify>(this.OnStatusNotify);
        }

        private void OnStatusNotify(object sender, StatusNotify notify)
        {
            foreach (var status in notify.Status)
            {
                Notify(status);
            }
        }

        private void Notify(NStatus status)
        {
            Debug.LogFormat("StatusNotify: [{0} {1} {2} :{3}]", status.Type, status.Action, status.Id,status.Value);
            if (status.Type == StatusType.Money)
            {
                if(status.Action == StatusAction.Add)
                {
                    User.Instance.AddGold(status.Value);
                }
                else if(status.Action ==StatusAction.Delete)
                {
                    User.Instance.AddGold(-status.Value);
                }
            }

            StatusNotifyHandler handler;
            if(events.TryGetValue(status.Type, out handler))
            {
                handler(status);
            }

        }
    }
}