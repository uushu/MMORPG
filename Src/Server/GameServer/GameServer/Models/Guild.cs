using Common;
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
    
        public string Name { get { return this.Data.Name; } }
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
                var member = this.GetDBMember(character.Id);
                if(member !=null && (member.Title==(int)GuildTitle.President || member.Title==(int)GuildTitle.VicePresident))
                {
                    info.Applies.AddRange(GetApplyInfos());
                }
            }
            return info;
        }

        List<NGuildApplyInfo> GetApplyInfos()
        {
           List<NGuildApplyInfo> applies= new List<NGuildApplyInfo>();

            foreach(var apply in this.Data.Applies)
            {
                if (apply.Result != (int)GuildApplyResult.None) continue;
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
                    
                }
                else
                {
                    memberInfo.Info = this.GetMemberInfo(member);
                    memberInfo.Status = 0;

                    
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
            var oldApply = this.Data.Applies.FirstOrDefault(v=>v.CharacterID == apply.characterId && v.Result == (int)GuildApplyResult.None);
            if(oldApply != null)
            {
                return false;
            }

            var dbApply = DBService.Instance.Entities.GuildApplies.Create();
            dbApply.TGuildId = apply.GuildId;
            dbApply.CharacterID = apply.characterId;
            dbApply.Class = apply.Class;
            dbApply.Name = apply.Name;
            dbApply.Level = apply.Level;
            dbApply.ApplyTime = DateTime.Now;

            DBService.Instance.Entities.GuildApplies.Add(dbApply);
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
                this.AddMember(oldApply.CharacterID, oldApply.Name, oldApply.Level, oldApply.Class, GuildTitle.None);
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
                dbCharacter.GuildID = this.Id;
            }
                timestamp = TimeUtil.timestamp;
        }

        internal void Leave(Character character)
        {
            Log.InfoFormat("OnGuildLeave: {0}{1}", character.Id, character.Name);
            var member = Data.Members.FirstOrDefault(v => v.CharacterID == character.Id);
            
            // 会长离开：解散公会
            if(character.Id == Data.LeaderID)
            {
                foreach(var m in Data.Members.ToList())
                {
                    var cha = CharacterManager.Instance.GetCharacter(m.CharacterID);
                    // 在线成员
                    if(cha != null)
                    {
                        cha.Data.GuildID = 0;
                        cha.Guild = null;
                        cha.Info.Guild = null;
                    }
                    // 离线修改数据库
                    else
                    {
                        var dbMember = DBService.Instance.Entities.Characters.SingleOrDefault(v => v.ID == m.CharacterID);
                        dbMember.GuildID = 0;
                    }
                    DBService.Instance.Entities.GuildMembers.Remove(m);
                    this.Data.Members.Remove(m);
                }
                foreach(var apply in Data.Applies.ToList())
                {
                    DBService.Instance.Entities.GuildApplies.Remove(apply);
                    this.Data.Applies.Remove(apply);
                }
                DBService.Instance.Entities.Guilds.Remove(this.Data);
                
            }
            // 普通成员离开公会
            else
            {
                DBService.Instance.Entities.GuildMembers.Remove(member);
                this.Data.Members.Remove(member);
                character.Data.GuildID = 0;
                character.Guild = null;
                character.Info.Guild = null;
            }
            timestamp = TimeUtil.timestamp;
        }

        

        TGuildMember GetDBMember(int characterId)
        {
            foreach( var member in this.Data.Members)
            {
                if (member.CharacterID == characterId)
                {
                    return member;
                }
            }
            return null;
        }
        internal bool ExecuteAdmin(GuildAdmainCommand command, int targetId, int characterId)
        {
            var target =GetDBMember(targetId);
            var source = GetDBMember(characterId);
         
            
            switch (command)
            {
                case GuildAdmainCommand.Promote:
                    target.Title = (int)GuildTitle.VicePresident;
                    break;
                case GuildAdmainCommand.Depose:
                    target.Title = (int)GuildTitle.None;
                    break;
                case GuildAdmainCommand.Treansfer:
                    target.Title = (int)GuildTitle.President;
                    source.Title = (int)GuildTitle.None;
                    this.Data.LeaderID = targetId;
                    this.Data.LeaderName = target.Name;
                    break;
                case GuildAdmainCommand.Kickout:
                    if(source.Title == (int)GuildTitle.President && (target.Title == (int)GuildTitle.VicePresident || target.Title == (int)GuildTitle.None) || source.Title == (int)GuildTitle.VicePresident && target.Title == (int)GuildTitle.None)
                    {
                        var dbCharacter = DBService.Instance.Entities.Characters.SingleOrDefault(v => v.ID == targetId);
                        DBService.Instance.Entities.GuildMembers.Remove(target);
                        this.Data.Members.Remove(target);
                        dbCharacter.GuildID = 0;
                    }
                    else 
                        return false;
                    break;
            }
            DBService.Instance.Save();
            timestamp = TimeUtil.timestamp;
            return true;
        }

        public bool SetIntroduce(int characterId,string introduce)
        {
            if(characterId != this.Data.LeaderID)
            {
                return false;
            }
            if(introduce.Length < 3 || introduce.Length > 50)
            {
                return false;
            }

            this.Data.Introduce = introduce;
            DBService.Instance.Save();
            timestamp = TimeUtil.timestamp;
            return true;

        }


    }
}
