using System.Collections.Generic;
using Entities;
using SkillBridge.Message;

namespace Manager
{
    interface IEntityNotify
    {
        void OnEntityRemoved();
        void OnEntityChanged(Entity entity);
        void OnEntityEvent(EntityEvent entitySyncEvent);
    }
    internal class EntityManager : Singleton<EntityManager> 
    {
        Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
        Dictionary<int, IEntityNotify> notifies = new Dictionary<int, IEntityNotify>();

        public void RegisterEntityChangeNotify(int entityId , IEntityNotify notify)
        {
            this.notifies[entityId] = notify;
        }

        public void AddEntity(Entity entity)
        {
            entities[entity.entityId] = entity;
        }

        public void RemoveEntity(NEntity entity)
        {
            entities.Remove(entity.Id);
            if (notifies.ContainsKey(entity.Id))
            {
                notifies[entity.Id].OnEntityRemoved();
                notifies.Remove(entity.Id);
            }
        }


        public void OnEntitySync(NEntitySync entitySync)
        {
            Entity entity = null;
            entities.TryGetValue(entitySync.Id, out entity);
            if (entity != null)
            {
                if (entitySync.Entity != null)
                {
                    entity.EntityData = entitySync.Entity; 
                }

                if (notifies.ContainsKey(entitySync.Id))
                {
                    notifies[entitySync.Id].OnEntityChanged(entity);
                    notifies[entitySync.Id].OnEntityEvent(entitySync.Event);
                    
                }
            }
        }
    }
}