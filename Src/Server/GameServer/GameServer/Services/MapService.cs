using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class MapService : Singleton<MapService>
    {
        public MapService()
        {
            
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapEntitySyncRequest>(this.OnMapEntitySync);

            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapTeleportRequest>(this.OnMapTeleport);
        }

        public void Init()
        {
            MapManager.Instance.Init();
        }
        


        internal void SendEntitySyncUpdate(NetConnection<NetSession> connection, NEntitySync entitySync)
        {
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.mapEntitySync = new MapEntitySyncResponse();
            message.Response.mapEntitySync.entitySyncs.Add(entitySync);

            byte[] data =PackageHandler.PackMessage(message);
            connection.SendData(data, 0, data.Length);
        }
        private void OnMapEntitySync(NetConnection<NetSession> sender, MapEntitySyncRequest request)
        {
            Character character = sender.Session.Character;
            if (character == null)
                return;
            Log.InfoFormat("OnMapEntitySync: characterId {0}:{1} EntityId: {2} Event:{3} Entity:{4}", character.Id, character.Info.Name, request.entitySync.Id, request.entitySync.Event , request.entitySync.Entity.String());
            MapManager.Instance[character.Info.mapId].UpdateEntity(request.entitySync);
        }

        private void OnMapTeleport(NetConnection<NetSession> sender, MapTeleportRequest request)
        {
            Character character=sender.Session.Character;
            Log.InfoFormat("OnMapTeleport: characterID {0}:{1} TeleportId:{2}", character.Id, character.Data, request.teleporterId);

            if(!DataManager.Instance.Teleporters.ContainsKey(request.teleporterId))
            {
                Log.WarningFormat("Source TeleportID {0} not existed", request.teleporterId);
                return;
            }

            TeleporterDefine td = DataManager.Instance.Teleporters[request.teleporterId];
            if (td.LinkTo == 0 || !DataManager.Instance.Teleporters.ContainsKey(td.LinkTo))
            {
                Log.WarningFormat("Source TeleportID [{0}] LinkToID [{1}] not existed", request.teleporterId, td.LinkTo);
            }

            TeleporterDefine target = DataManager.Instance.Teleporters[td.LinkTo];

            MapManager.Instance[td.MapID].CharacterLeave(character);
            character.Position = target.Position;
            character.Direction = target.Direction;
            MapManager.Instance[target.MapID].CharacterEnter(sender, character);



        }

    }
}
