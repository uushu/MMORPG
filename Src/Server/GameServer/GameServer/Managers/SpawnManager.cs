using Common.Data;
using GameServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    internal class SpawnManager
    {
        List<Spawner> Rules = new List<Spawner>();
        Map map;
        internal void Init(Map map)
        {
            this.map= map;
            if (DataManager.Instance.SpawnRules.ContainsKey(map.Define.ID))
            {
                foreach( var kv in DataManager.Instance.SpawnRules[map.Define.ID])
                {
                    this.Rules.Add(new Spawner(kv.Value, map));
                }
            }
        }

        internal void Update()
        {
            if (Rules.Count == 0)
                return;
            for(int i = 0;i<Rules.Count; i++)
            {
                this.Rules[i].Update();
            }
        }
    }
}
