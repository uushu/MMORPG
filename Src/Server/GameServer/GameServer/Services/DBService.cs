using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common;

namespace GameServer.Services
{
    class DBService : Singleton<DBService>
    {
        ExtremeWorldEntities entities;



        public ExtremeWorldEntities Entities
        {
            get { return this.entities; }
        }

        public void Init()
        {
            entities = new ExtremeWorldEntities();

           
        }

        internal void Save(bool async = false)
        {
            // 回档、短时间内大量保存、定时保存（设个保存间隔）
            if(async)
                entities.SaveChangesAsync();
            else
                entities.SaveChanges();
        }
    }
}
