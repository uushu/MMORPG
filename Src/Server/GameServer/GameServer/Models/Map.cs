using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;

using Common;
using Common.Data;

using Network;
using GameServer.Managers;
using GameServer.Entities;
using GameServer.Services;
using System.Security.Cryptography;

namespace GameServer.Models
{
    class Map
    {
        internal class MapCharacter
        {
            public NetConnection<NetSession> connection;
            public Character character;

            public MapCharacter(NetConnection<NetSession> conn, Character cha)
            {
                this.connection = conn;
                this.character = cha;
            }
        }

        public int ID
        {
            get { return this.Define.ID; }
        }
        internal MapDefine Define;


        // 地图中的所有角色 ,以CHaracterId为Key
        Dictionary<int, MapCharacter> MapCharacters = new Dictionary<int, MapCharacter>();


       /// <summary>
       /// 刷怪管理器
       /// </summary>

        SpawnManager SpawnManager = new SpawnManager(); // 刷怪管理器
        public MonsterManager MonsterManager = new MonsterManager();  // 地图管理器

        internal Map(MapDefine define)
        {
            this.Define = define;
            this.SpawnManager.Init(this);
            this.MonsterManager.Init(this);
        }

        internal void Update()
        {
            SpawnManager.Update();
        }


        /// <summary>
        /// 角色进入地图 
        /// </summary>
        /// <param name="character"></param>
        internal void CharacterEnter(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("CharacterEnter: Map:{0} characterId:{1}", this.Define.ID, character.Id);

            character.Info.mapId = this.ID;
            this.MapCharacters[character.Id] = new MapCharacter(conn, character);

            conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            conn.Session.Response.mapCharacterEnter.mapId = this.Define.ID;
           

            foreach (var kv in this.MapCharacters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);
                if (kv.Value.character != character)
                    this.AddCharacterEnterMap(kv.Value.connection, character.Info);
            }

            foreach( var kv in this.MonsterManager.Monsters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.Info);
            }

            conn.SendResponse();
        }

        private void AddCharacterEnterMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            if(conn.Session.Response.mapCharacterEnter == null)
            {
                conn.Session.Response.mapCharacterEnter= new MapCharacterEnterResponse();
                conn.Session.Response.mapCharacterEnter.mapId= this.Define.ID;

            }

            conn.Session.Response.mapCharacterEnter.Characters.Add(character);
            conn.SendResponse();
        }


        /// <summary>
        /// 角色离开地图
        /// </summary>
        /// <param name="character"></param>
        internal void CharacterLeave(Character character)
        {
            Log.InfoFormat("CharacterLeave: Map:{0} characterId:{1}", this.Define.ID, character.Id);

            foreach (var kv in this.MapCharacters)
            {
                this.SendCharacterLeaveMap(kv.Value.connection, character);
            }
            this.MapCharacters.Remove(character.Id);

        }
        void SendCharacterLeaveMap(NetConnection<NetSession> conn, Character character)
        {
            conn.Session.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            conn.Session.Response.mapCharacterLeave.entityId = character.entityId;

            conn.SendResponse();
        }
        /// <summary>
        /// 更新实体在服务器上的信息
        /// </summary>
        /// <param name="entitySync"></param>

        internal void UpdateEntity(NEntitySync entitySync)
        {
            foreach(var kv in this.MapCharacters)
            {
                // 把自己的信息更新到服务器
                if (kv.Value.character.entityId == entitySync.Id)
                {
                    kv.Value.character.Position = entitySync.Entity.Position;
                    kv.Value.character.Direction = entitySync.Entity.Direction;
                    kv.Value.character.Speed = entitySync.Entity.Speed;
                }
                else
                {
                    // 把自己的消息转发给地图上其他客户端
                    MapService.Instance.SendEntitySyncUpdate(kv.Value.connection, entitySync);
                }
            }
        }

        /// <summary>
        /// 怪物进入地图的消息
        /// </summary>
        /// <param name="monster"></param>
        internal void MonsterEnter(Monster monster)
        {
            Log.InfoFormat("MonsterEnter: Map{0},MonsterId{1}", this.Define.ID, monster.Id);
            foreach( var kv in this.MapCharacters)
            {
                this.AddCharacterEnterMap(kv.Value.connection, monster.Info);
            }
        }
    }
}
