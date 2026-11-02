using System;
using System.Collections.Generic;
using Manager;
using Network;
using SkillBridge.Message;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Service
{
    public class GuildService : Singleton<GuildService> ,IDisposable
    {
        public UnityAction OnGuildUpdate;  // 公会内部更新委托
        public UnityAction<bool> OnGuildCreateResult;
        public UnityAction<List<NGuildInfo>> OnGuildListResult ;  // 加入公会列表委托
        public void Init()
        {
            
        }

        public GuildService()
        {
            MessageDistributer.Instance.Subscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinReq);
            MessageDistributer.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinRep);
            MessageDistributer.Instance.Subscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Subscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Subscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Subscribe<GuildAdminResponse>(this.OnGuildAdmin);
            
        }

      

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Unsubscribe<GuildJoinRequest>(this.OnGuildJoinReq);
            MessageDistributer.Instance.Unsubscribe<GuildJoinResponse>(this.OnGuildJoinRep);
            MessageDistributer.Instance.Unsubscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Unsubscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Unsubscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Unsubscribe<GuildAdminResponse>(this.OnGuildAdmin);
            
        }

       
        /// <summary>
        /// 发送公会创建请求
        /// </summary>
        /// <param name="inputNameText"></param>
        /// <param name="inputIntroduceText"></param>
        public void SendGuildCreate(string inputNameText, string inputIntroduceText)
        {
            Debug.Log("SendGuildCreate");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildCreate = new GuildCreateRequest();
            message.Request.guildCreate.guildName = inputNameText;
            message.Request.guildCreate.guildIntroduce = inputIntroduceText;
            
            NetClient.Instance.SendMessage(message);
        }
        
        /// <summary>
        /// 收到公会创建响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnGuildCreate(object sender, GuildCreateResponse message)
        {
            Debug.LogFormat("OnGuildLeave: {0}", message.Result);
            if (OnGuildCreateResult != null)
            {
                this.OnGuildCreateResult(message.Result == Result.Success);
            }

            if (message.Result == Result.Success)
            {
                GuildManager.Instance.Init(message.guildInfo);
                MessageBox.Show(string.Format("{0}公会创建成功", message.guildInfo.Name), "公会创建");
            }
            else
            {
                MessageBox.Show(message.Errormsg, "公会创建", MessageBoxType.Error);
            }
        }


       
        
        /// <summary>
        /// 发送申请加入公会请求
        /// </summary>
        /// <param name="guildId"></param>
        public void SendGuildJoinRequest(int guildId)
        {
            Debug.Log("SendGuildJoinRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinReq = new GuildJoinRequest();
            message.Request.guildJoinReq.Apply = new NGuildApplyInfo();
            message.Request.guildJoinReq.Apply.GuildId = guildId;
            NetClient.Instance.SendMessage(message);
        }
        
        /// <summary>
        /// 收到加入公会的申请
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnGuildJoinReq(object sender, GuildJoinRequest request)
        {
            Debug.Log("OnGuildJoinReq");
            var confirm = MessageBox.Show(string.Format("{0}申请加入公会", request.Apply.Name),"公会申请",MessageBoxType.Confirm,"同意","拒绝");
            confirm.OnYes = () =>
            {
                this.SendGuildJoinResponse(true, request);
            };
            confirm.OnNo = () =>
            {
                this.SendGuildJoinResponse(false, request);
            };
        }

        
        
        /// <summary>
        /// 发送申请加入公会结果
        /// </summary>
        /// <param name="guildId"></param>
        public void SendGuildJoinResponse(bool accept, GuildJoinRequest request)
        {
            Debug.Log("SendGuildJoinResponse");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinRep = new GuildJoinResponse();
            message.Request.guildJoinRep.Result = Result.Success;
            message.Request.guildJoinRep.Apply = request.Apply;
            message.Request.guildJoinRep.Apply.Result = accept ? GuildApplyResult.Accept : GuildApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
        }
        
        /// <summary>
        /// 收到加入公会响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnGuildJoinRep(object sender, GuildJoinResponse response)
        {
            Debug.LogFormat("OnGuildJoinRep:{0}", response.Result);
            if (response.Result == Result.Success)
            {
                MessageBox.Show(response.Errormsg, "公会");
            }
            else
            {
                MessageBox.Show(response.Errormsg, "公会", MessageBoxType.Error);
            }
        }


        /// <summary>
        /// 发送公会列表请求
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void SendGuildListRequest()
        {
            Debug.Log("SendGuildListRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildList = new GuildListRequest();
            NetClient.Instance.SendMessage(message);
        }
       
        
       
        private void OnGuildList(object sender, GuildListResponse response)
        {
            if (OnGuildListResult != null)
                this.OnGuildListResult(response.Guilds);
        }

        
        private void OnGuild(object sender, GuildResponse message)
        {
            Debug.LogFormat("OnGuild:{0}", message.Result);
            GuildManager.Instance.Init(message.guildInfo);
            if (this.OnGuildUpdate != null)
            {
                this.OnGuildUpdate();
            }
        }

        public void SendGuildLeaveRequest()
        {
            Debug.Log("SendGuildLeaveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildLeave = new GuildLeaveRequest();
            NetClient.Instance.SendMessage(message);
        }
        private void OnGuildLeave(object sender, GuildLeaveResponse message)
        {
            if (message.Result == Result.Success)
            {
                MessageBox.Show(message.Errormsg, "公会");
            }
            else 
                MessageBox.Show(message.Errormsg, "公会",MessageBoxType.Error);
        }

        #region 公会管理协议

        
        /// <summary>
        ///  发送加入公会的审批
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="apply"></param>
        public void SendGuildJoinApply(bool accept, NGuildApplyInfo apply)
        {
            Debug.Log("SendGuildJoinApply");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinRep = new GuildJoinResponse();
            message.Request.guildJoinRep.Result = Result.Success;
            message.Request.guildJoinRep.Apply = apply;
            message.Request.guildJoinRep.Apply.Result = accept ? GuildApplyResult.Accept : GuildApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 发送公会管理指令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="targetId"></param>
        public void SendAdminCommand(GuildAdmainCommand command, int targetId ,string introduce = "")
        {
            Debug.Log("SendAdminCommandRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildAdmin = new GuildAdminRequest();
            message.Request.guildAdmin.Command = command;
            message.Request.guildAdmin.targetId = targetId;
            message.Request.guildAdmin.Introduce = introduce;
            NetClient.Instance.SendMessage(message);
        }
        
        private void OnGuildAdmin(object sender, GuildAdminResponse message)
        {
            Debug.LogFormat("OnGuildAdmin:{0} {1}",message.Command,message.Result );

            if(message.Result == Result.Success)
            {
                MessageBox.Show(message.Errormsg, "操作结果", MessageBoxType.Information);

            }
            else
            {
                MessageBox.Show(message.Errormsg, "操作结果", MessageBoxType.Error);
            }
        }


        #endregion
        
        
    }
}