using Common;
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
    internal class TeamService:Singleton<TeamService>
    {
       
        public TeamService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamLeaveRequest>(this.OnTeamLeaveRequest);

        }

        internal void Init()
        {
            TeamManager.Instance.Init();
        }

        /// <summary>
        /// 收到组队请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnTeamInviteRequest(NetConnection<NetSession> sender, TeamInviteRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnTeamInviteRequest: FromID:{0},FromName:{1},ToID:{2},ToName:{3}", request.FromId, request.FromName, request.ToId, request.ToName);

            // TODO: 做一些前置数据校验 (什么前置数据?)

            // 开始逻辑
            NetConnection<NetSession> target = SessionManager.Instance.GetSession(request.ToId);
            // 服务器创建 请求失败响应
            if(target == null)
            {
                sender.Session.Response.teamInviteRep = new TeamInviteResponse();
                sender.Session.Response.teamInviteRep.Request = request;
                sender.Session.Response.teamInviteRep.Result = Result.Failed;
                sender.Session.Response.teamInviteRep.Errormsg = "好友不在线";
                sender.SendResponse();
                return;
            }

            if(target.Session.Character.Team != null)
            {
                sender.Session.Response.teamInviteRep = new TeamInviteResponse();
                sender.Session.Response.teamInviteRep.Request = request;
                sender.Session.Response.teamInviteRep.Result = Result.Failed;
                sender.Session.Response.teamInviteRep.Errormsg = "好友已经加入队伍";
                sender.SendResponse();
                return;
            }

            if (character.Team != null && character.Team.Members.Count >= 5)
            {
                sender.Session.Response.teamInviteRep = new TeamInviteResponse();
                sender.Session.Response.teamInviteRep.Request = request;
                sender.Session.Response.teamInviteRep.Result = Result.Failed;
                sender.Session.Response.teamInviteRep.Errormsg = "队伍已满员";
                sender.SendResponse();
                return;
            }

            // 转发请求
            Log.InfoFormat("ForwardTeamInviteRequest:  FromID:{0},FromName:{1},ToID:{2},ToName:{3}", request.FromId, request.FromName, request.ToId, request.ToName);
            target.Session.Response.teamInviteReq = request;
            target.SendResponse();
        }

        /// <summary>
        /// 收到组队响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnTeamInviteResponse(NetConnection<NetSession> sender, TeamInviteResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnTeamInviteReponse: characterID:{0},Result:{1},FromID:{2},ToID:{3}", character.Id,response.Result,response.Request.FromId,response.Request.ToId);

            sender.Session.Response.teamInviteRep = response;
            var requester = SessionManager.Instance.GetSession(response.Request.FromId);
            if (response.Result == Result.Success)
            {
                lock(TeamManager.Instance.SyncRoot)
                {
                    // 请求组队方下线
                    if (requester == null)
                    {
                        sender.Session.Response.teamInviteRep.Result = Result.Failed;
                        sender.Session.Response.teamInviteRep.Errormsg = "请求者已下线";

                    }
                    else if (character.Team != null)
                    {
                        sender.Session.Response.teamInviteRep.Result = Result.Failed; ;
                        sender.Session.Response.teamInviteRep.Errormsg = "你已经加入队伍";
                    }
                    else if (requester.Session.Character.Team != null && requester.Session.Character.Team.Members.Count >= 5)
                    {
                        sender.Session.Response.teamInviteRep.Result = Result.Failed; ;
                        sender.Session.Response.teamInviteRep.Errormsg = "队伍人数已满";
                    }
                    else
                    {
                        TeamManager.Instance.AddTeamMember(requester.Session.Character, character);
                    }
                }
                if(response.Result == Result.Success)
                {
                    requester.Session.Response.teamInviteRep = response;
                    requester.SendResponse();
                }
            }
            else
            {
                response.Errormsg = "已拒绝" + response.Request.FromName + "的组队请求";
                if(requester != null)
                {
                    requester.Session.Response.teamInviteRep = new TeamInviteResponse();
                    requester.Session.Response.teamInviteRep.Result = Result.Failed;
                    requester.Session.Response.teamInviteRep.Errormsg =  character.Data.Name + "拒绝了你的组队请求";
                    requester.SendResponse();
                }

            }
            sender.SendResponse();
        }

        private void OnTeamLeaveRequest(NetConnection<NetSession> sender, TeamLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnTeamLeaveRequest: characterID:{0},TeamID:{1}", character.Id, request.TeamId);

            LeaveTeam(character);

            sender.Session.Response.teamLeave = new TeamLeaveResponse();
            sender.Session.Response.teamLeave.characterId = character.Id;
            sender.Session.Response.teamLeave.Result = Result.Success;
            sender.SendResponse();
        }

        public void LeaveTeam(Character character)
        {
            Character[] members = new Character[0];
            bool isBand = false;
            lock (TeamManager.Instance.SyncRoot)
            {
                if (character.Team != null)
                {
                    members = character.Team.Members.ToArray();
                    isBand = character == character.Team.Leader;
                    character.Team.Leave(character);
                }

            }

            foreach (var member in members)
            {
                if (member == character) continue;
                var item = SessionManager.Instance.GetSession(member.Id);
                if (item == null) continue;

                var response = item.Session.Response;
                if (isBand)
                {
                    response.teamInfo = new TeamInfoResponse();
                    response.teamInfo.Team = null;
                }
                response.teamLeave = new TeamLeaveResponse();
                response.teamLeave.characterId = character.Id;
                response.teamLeave.Errormsg = isBand ? character.Info.Name + "解散了队伍" : character.Info.Name + "离开了队伍";
                item.SendResponse();
            }
        }
    }
}
