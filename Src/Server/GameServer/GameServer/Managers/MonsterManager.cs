using GameServer.Entities;
using GameServer.Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    internal class MonsterManager
    {
        public Dictionary<int, Monster> Monsters = new Dictionary<int, Monster>();
        public Map Map;
        internal void Init(Map map)
        {
            this.Map = map;
        }


        public Monster Create(int spawnMonId,int spawnLevel,NVector3 position, NVector3 dirction)
        {
            Monster monster = new Monster(spawnMonId, spawnLevel, position, dirction);
            EntityManager.Instance.AddEntity(this.Map.ID, monster);
            monster.Info.Id = monster.entityId;
            monster.Info.mapId = this.Map.ID;
            Monsters[Map.ID] = monster;

            this.Map.MonsterEnter(monster);
            return monster;
        }
    }
}
