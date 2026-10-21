using Common;
using Common.Data;
using GameServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    internal class Spawner
    {
        public SpawnRuleDefine Rule; // 刷怪规则
        private Map map;


        float spawnTime = 0; //刷新时间
        float unspawnTime = 0; //消失时间
        bool spawned = false;
        SpawnPointDefine spawnPoint = null; //具体刷怪点

        public Spawner(SpawnRuleDefine define, Map map)
        {
            this.Rule = define;
            this.map = map;

            if(DataManager.Instance.SpawnPoints.ContainsKey(this.map.ID))
            {
                if (DataManager.Instance.SpawnPoints[this.map.ID].ContainsKey(this.Rule.SpawnPoint))
                {
                    spawnPoint = DataManager.Instance.SpawnPoints[this.map.ID][this.Rule.SpawnPoint];

                }
                else
                {
                    Log.ErrorFormat("SpawnRules[{0}] SpawnPoin[{1}] not existed", this.Rule.ID, this.Rule.SpawnPoint);
                }

            }
        }

        internal void Update()
        {
            if(this.CanSpawn())
            {
                this.Spawn();
            }
        }

        

        private bool CanSpawn()
        {
            if (this.spawned)
            {
                return false;
            }
            if (this.unspawnTime + this.Rule.SpawnPeriod > Time.time)
            {
                return false;
            }
            return true;
        }

        private void Spawn()
        {
            this.spawned = true;
            Log.InfoFormat("Map[{0}]: Spawn[{1}: Monster{2},Lv{3}] At Point:{4}", this.Rule.MapID, this.Rule.ID, this.Rule.SpawnMonID, this.Rule.SpawnLevel, this.Rule.SpawnPoint);
            this.map.MonsterManager.Create(this.Rule.SpawnMonID, this.Rule.SpawnLevel, this.spawnPoint.Position, this.spawnPoint.Direction);
        }
    }
}
