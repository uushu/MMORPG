using Common;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    internal class HelloHJXService : Singleton<HelloHJXService>
    {
        public void Init()
        {
            


        }

        public void Start()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FirstTestRquest>(this.OnFirstTestRquest);
        }

        void OnFirstTestRquest(NetConnection<NetSession>sender,FirstTestRquest request)
        {
            Log.InfoFormat("FirstTestRequest:name:{0}", request.Name);
        }

        public void Stop()
        {

        }
    }
}
