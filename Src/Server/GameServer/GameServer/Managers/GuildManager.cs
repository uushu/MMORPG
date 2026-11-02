using Common;
using Common.Utils;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    internal class GuildManager  : Singleton<GuildManager>
    {
     
        public Dictionary<int, Guild> Guilds = new Dictionary<int, Guild>();
        private HashSet<string> GuildNames = new HashSet<string>();
        public void Init()
        {
            this.Guilds.Clear();
            foreach(var guild in DBService.Instance.Entities.Guilds)
            {
                this.AddGuild(new Guild(guild));
            }
        }

        private void AddGuild(Guild guild)
        {
            this.Guilds.Add(guild.Id, guild);
            this.GuildNames.Add(guild.Name);
            guild.timestamp = TimeUtil.timestamp;
        }

        internal bool CheckNameExisted(string guildName)
        {
            return GuildNames.Contains(guildName);
        }

        internal void CreateGuild(string guildName, string guildIntroduce, Character character)
        {
            DateTime now = DateTime.Now;
            TGuild dbGuild = DBService.Instance.Entities.Guilds.Create();
            dbGuild.Name = guildName;
            dbGuild.Introduce = guildIntroduce;
            dbGuild.LeaderID = character.Id;
            dbGuild.LeaderName = character.Name;
            dbGuild.CreateTime = now;
            DBService.Instance.Entities.Guilds.Add(dbGuild);

            Guild guild = new Guild(dbGuild);
            guild.AddMember(character.Id, character.Name,character.Data.Level, character.Data.Class, GuildTitle.President);
            character.Guild = guild;
            DBService.Instance.Save();

            character.Data.GuildID = dbGuild.Id;
            character.Gold -= 50000;
            DBService.Instance.Save();
            this.AddGuild(guild);
        }

        internal Guild GetGuild(int guildID)
        {
            if(guildID ==0)
            {
                return null;
            }
            Guild guild = null;
            this.Guilds.TryGetValue(guildID, out guild);
            return guild;
        }

        internal List<NGuildInfo> GetGuildsInfo()
        {
            List<NGuildInfo> result =new List<NGuildInfo>();
            foreach(var guild in this.Guilds)
            {
                result.Add(guild.Value.GetGuildInfo(null));
            }    
            return result;
        }

        public void RemoveGuild(Guild guild)
        {
            this.Guilds.Remove(guild.Id);
            this.GuildNames.Remove(guild.Name);
        }
    }
}
