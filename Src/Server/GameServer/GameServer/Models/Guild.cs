using Common.Utils;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    internal class Guild 
    {
        public int Id { get { return this.Data.Id; } }
        public Character Leader;
        public string Name { get { return this.Data.Name; } }
        public List<Character> Members = new List<Character>();
        public double timestamp;
        public TGuild Data;


        public Guild(TGuild guild) 
        {
            this.Data = guild;
        
        }

        internal NGuildInfo GetGuildInfo(Character character)
        {
            NGuildInfo info = new NGuildInfo()
            {
                Id = this.Id,
                Name = this.Name,
                Introduce = this.Data.Introduce,
                leaderId = this.Data.LeaderID,
                leaderName = this.Data.LeaderName,
                createTime = (long)TimeUtil.GetTimestamp(this.Data.CreateTime),
                memberCount = this.Data.Members.Count,
            };

            if(character != null)
            {
                info.Members.AddRange(GetMemberInfos());
                if(character.Id == this.Data.LeaderID)
                    info.Applies.AddRange(GetApplyInfos());
            
            }
            return info;
        }

        List<NGuildApplyInfo> GetApplyInfos()
        {
           List<NGuildApplyInfo> applies= new List<NGuildApplyInfo>();

            foreach(var apply in this.Data.Applies)
            {
                applies.Add(new NGuildApplyInfo()
                {
                    characterId = apply.CharacterID,
                    Name = apply.Name,
                    GuildId = apply.TGuildId,
                    Class = apply.Class,
                    Level = apply.Level,
                    Result = (GuildApplyResult) apply.Result,

                });

            }
                return applies;
        }

        List<NGuildMemberInfo> GetMemberInfos()
        {
            List<NGuildMemberInfo> members = new List<NGuildMemberInfo>();
            foreach (var member in this.Data.Members)
            {
                var memberInfo = new NGuildMemberInfo()
                {
                    Id = member.Id,
                    characterId = member.CharacterID,
                    Title = (GuildTitle)member.Title,
                    joinTime = (long)TimeUtil.GetTimestamp(member.JoinTime),
                    lastTime = (long)TimeUtil.GetTimestamp(member.LastTime),


                };
                var character = CharacterManager.Instance.GetCharacter(member.CharacterID);
                if(character != null)
                {
                    memberInfo.Info = character.GetBasicInfo();
                    memberInfo.Status = 1;
                    member.Level = character.Data.Level;
                    member.Class = character.Data.Class;
                    member.LastTime = DateTime.Now;
                    if (member.CharacterID == this.Data.LeaderID)
                        this.Leader = character;
                }
                else
                {
                    memberInfo.Info = this.GetMemberInfo(member);
                    memberInfo.Status = 0;

                    if (member.CharacterID == this.Data.LeaderID)
                        this.Leader = null;
                }
                members.Add(memberInfo);

            }
            return members;
        }

        private NCharacterInfo GetMemberInfo(TGuildMember member)
        {
            return new NCharacterInfo()
            {
                Id = member.CharacterID,
                Name = member.Name,
                Class = (CharacterClass) member.Class,
                Level = member.Level,
            };
            
        }

        internal void PostProcess(Character character, NetMessageResponse message)
        {
            if(message.Guild == null)
            {
                message.Guild = new GuildResponse();
                message.Guild.Result = Result.Success;
                message.Guild.guildInfo = this.GetGuildInfo(character);
            }
        }

        internal bool JoinApply(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(v=>v.CharacterID == apply.characterId);
            if(oldApply != null)
            {
                return false;
            }

            var dbApply = DBService.Instance.Entities.TGuildApplies.Create();
            dbApply.TGuildId = apply.GuildId;
            dbApply.CharacterID = apply.characterId;
            dbApply.Class = apply.Class;
            dbApply.Name = apply.Name;
            dbApply.Level = apply.Level;
            dbApply.ApplyTime = DateTime.Now;

            DBService.Instance.Entities.TGuildApplies.Add(dbApply);
            this.Data.Applies.Add(dbApply);

            DBService.Instance.Save();

            this.timestamp = TimeUtil.timestamp;
            return true;
        }

        internal bool JoinApprove(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(v => v.CharacterID == apply.characterId && v.Result == 0);
            if (oldApply == null)
            {
                return false;
            }

            oldApply.Result = (int) apply.Result;
            if(apply.Result == GuildApplyResult.Accept)
            {
                this.AddMember(apply.characterId, apply.Name, apply.Level, apply.Class, GuildTitle.None);

            }
            DBService.Instance.Save();
            this.timestamp = TimeUtil.timestamp;
            return true; ;

        }

        public void AddMember(int characterId, string name, int level, int @class, GuildTitle title)
        {
            DateTime now = DateTime.Now;

            TGuildMember dbMember = new TGuildMember()
            {
                CharacterID = characterId,
                Name = name,
                Level = level,
                Class = @class,
                Title = (int)title,
                JoinTime = now,
                LastTime = now,
            };

            Data.Members.Add(dbMember);
            var character = CharacterManager.Instance.GetCharacter(characterId);
            if (character != null)
            {
                character.Data.GuildID = this.Id;
            }
            else
            {
                var dbCharacter = DBService.Instance.Entities.Characters.SingleOrDefault(c => c.ID == characterId);
                if(dbCharacter != null)
                {
                    dbCharacter.GuildID = this.Id;
                }
            }
                timestamp = TimeUtil.timestamp;
        }

        internal void Leave(Character character)
        {
           
        }
    }
}
