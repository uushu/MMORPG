using System;
using Manager;
using Models;
using Network;
using SkillBridge.Message;
using UI;
using UnityEngine;

namespace Service
{
    public class TeamService : Singleton<TeamService>,IDisposable
    {
        public void Init()
        {
            
        }
        public TeamService()
        {
            MessageDistributer.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer.Instance.Subscribe<TeamInfoResponse>(this.OnTeamInfoResponse);
            MessageDistributer.Instance.Subscribe<TeamLeaveResponse>(this.OnTeamLeaveResponse);

        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer.Instance.Unsubscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer.Instance.Unsubscribe<TeamInfoResponse>(this.OnTeamInfoResponse);
            MessageDistributer.Instance.Unsubscribe<TeamLeaveResponse>(this.OnTeamLeaveResponse);
        }

        
        public void SendTeamInviteRequest(int friendInfoId, string friendInfoName)
        {
            Debug.Log("SendTeamInviteRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamInviteReq = new TeamInviteRequest();
            message.Request.teamInviteReq.FromId = User.Instance.CurrentCharacter.Id;
            message.Request.teamInviteReq.FromName = User.Instance.CurrentCharacter.Name;
            message.Request.teamInviteReq.ToId = friendInfoId;
            message.Request.teamInviteReq.ToName = friendInfoName;
            NetClient.Instance.SendMessage(message);
        }

        public void SendTeamInviteResponse(bool accept, TeamInviteRequest request)
        {
            Debug.Log("SendTeamInviteResponse");
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.teamInviteRep = new TeamInviteResponse();
            message.Response.teamInviteRep.Result = accept ? Result.Success : Result.Failed;
            message.Response.teamInviteRep.Errormsg = accept ? "组队成功" : "对方拒绝了组队请求";
            message.Response.teamInviteRep.Request = request;
            NetClient.Instance.SendMessage(message);
        }

        
        
        private void OnTeamInviteRequest(object sender, TeamInviteRequest request)
        {
            var confirm = MessageBox.Show(string.Format("{0}邀请你加入队伍", request.FromName), "组队请求", MessageBoxType.Confirm, "接受", "拒绝");
            confirm.OnYes = () =>
            {
                this.SendTeamInviteResponse(true, request);
            };
            confirm.OnNo = () =>
            {
                this.SendTeamInviteResponse(false, request);
            };
        }
        
        /// <summary>
        /// 收到组队邀请的响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnTeamInviteResponse(object sender, TeamInviteResponse message)
        {
            if (message.Result == Result.Success)
            {
                if(message.Request.FromId == User.Instance.CurrentCharacter.Id)
                    MessageBox.Show("成功邀请" + message.Request.ToName +"加入队伍", "组队成功");
                else if(message.Request.ToId == User.Instance.CurrentCharacter.Id)
                    MessageBox.Show("你已加入" + message.Request.FromName +"的队伍", "组队成功");
            }
            else
                MessageBox.Show(message.Errormsg, "组队失败");
        }
        
        private void OnTeamInfoResponse(object sender, TeamInfoResponse message)
        {
            Debug.Log("OnTeamInfoResponse");
            TeamManager.Instance.UpdateTeamInfo(message.Team);
        }

        public void SendTeamLeaveRequest(int teamInfoId)
        {
            Debug.Log("SendTeamLeaveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamLeave= new TeamLeaveRequest();
            message.Request.teamLeave.TeamId = TeamManager.Instance.TeamInfo.Id;
            message.Request.teamLeave.characterId = User.Instance.CurrentCharacter.Id;
            NetClient.Instance.SendMessage(message);

        }
        private void OnTeamLeaveResponse(object sender, TeamLeaveResponse message)
        {
            if (message.Result != Result.Success)
            {
                MessageBox.Show("退出队伍失败", "错误", MessageBoxType.Error);
                return;
            }
            
            if (message.characterId != User.Instance.CurrentCharacter.Id)
            {
                MessageBox.Show(message.Errormsg, "提示");
                return;
            }
            
            TeamManager.Instance.UpdateTeamInfo(null);
            MessageBox.Show("退出队伍成功", "提示");
               
        }
        
    }
}