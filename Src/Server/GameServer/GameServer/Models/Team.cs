using Common;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    internal class Team
    {
        public int Id;
        public Character Leader;
        public List<Character> Members = new List<Character>();
        public double timestamp;

        public Team(Character leader) 
        {
            this.AddMember(leader);
        }

        public void AddMember(Character member)
        {
            Log.InfoFormat("AddMember:{0}:{1}", member.Id, member.Info.Name);
            if (this.Members.Count == 0)
            {
                this.Leader = member;
            }
            this.Members.Add(member);
            member.Team = this;
            timestamp = Time.timestamp;
        }

        public void Leave(Character member)
        {
            Log.InfoFormat("LeaveTeam:{0}:{1}", member.Id, member.Info.Name);
        
            if (member == Leader)
            {
               foreach(var teamMember in this.Members)
               {
                    teamMember.Team = null;
                    teamMember.TeamUpdateTS = 0;
               }

                this.Members.Clear();
                this.Leader = null;
            }
            else
            {
                this.Members.Remove(member);
                member.Team = null;
                member.TeamUpdateTS = 0;
            }
            timestamp = Time.timestamp;
        }

        internal void PostProcess(NetMessageResponse message)
        {
            if(message.teamInfo == null)
            {
                message.teamInfo = new TeamInfoResponse();
                message.teamInfo.Result = Result.Success;
                message.teamInfo.Team = new NTeamInfo();
                message.teamInfo.Team.Id = this.Id;
                message.teamInfo.Team.leaderId = this.Leader.Id;

                foreach(var member in this.Members)
                {
                    message.teamInfo.Team.Members.Add(member.GetBasicInfo());
                }
            }
        }
    }
}
