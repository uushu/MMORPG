using Common;
using GameServer.Entities;
using GameServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    internal class TeamManager :Singleton<TeamManager>
    {
        public readonly object SyncRoot = new object();
        public List<Team> Teams = new List<Team>(); // 遍历所有队伍，存放对象Team引用，本身在服务器内存中只有一份，占用一个指针内存
        public Dictionary<int, Team> TeamMembers = new Dictionary<int, Team>(); //精准查询每个队伍的信息
        public void Init()
        {

        }

        public Team GetTeamByCharacter(int characterId)
        {
            Team team = null;
            this.TeamMembers.TryGetValue(characterId, out team);
            return team;
        }

        internal void AddTeamMember(Character leader, Character member)
        {
            if(leader.Team == null)
                leader.Team = CreateTeam(leader);

            leader.Team.AddMember(member);
        }

        private Team CreateTeam(Character leader)
        {
            Team team = null;
            for (int i = 0; i < this.Teams.Count; i++)
            {
                team = this.Teams[i];
                if (team.Members.Count == 0)
                {
                    team.AddMember(leader);
                    return team;

                }
                
            }
            team = new Team(leader);
            this.Teams.Add(team);
            team.Id = this.Teams.Count;
            return team;
        }
    }
}
