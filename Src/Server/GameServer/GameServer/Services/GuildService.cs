using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    internal class GuildService : Singleton<GuildService>
    {

        public GuildService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildCreateRequest>(this.OnGuildCreate);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinReq);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinRep);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildListRequest>(this.OnGuildList);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildLeaveRequest>(this.OnGuildLeave);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildAdminRequest>(this.OnGuildAdmin);



        }

     

        public void Init()
        {
            GuildManager.Instance.Init();
        }

        /// <summary>
        /// 接收创建公会请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>

        private void OnGuildCreate(NetConnection<NetSession> sender, GuildCreateRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildCreate: GuildName {0},character:{1}{2}", request.guildName, character.Id, character.Name);
            sender.Session.Response.guildCreate = new GuildCreateResponse();
            if(character.Guild != null)
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "已有公会";
                sender.SendResponse();
                return;
            }

            if(character.Gold < 50000)
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "创建公会金币不足";
                sender.SendResponse();
                return;
            }

            if (GuildManager.Instance.CheckNameExisted(request.guildName))
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "公会名称已存在";
                sender.SendResponse();
                return;
            }

            GuildManager.Instance.CreateGuild(request.guildName, request.guildIntroduce,character);
            sender.Session.Response.guildCreate.guildInfo = character.Guild.GetGuildInfo(character);
            sender.Session.Response.guildCreate.Result = Result.Success;
            sender.SendResponse();
           
        }

        /// <summary>
        /// 收到公会请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildJoinReq(NetConnection<NetSession> sender, GuildJoinRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildJoinReq: Guild:{0},character:{1}{2}", request.Apply.GuildId, request.Apply.characterId, request.Apply.Name);
            var guild = GuildManager.Instance.GetGuild(request.Apply.GuildId);

            if(guild == null)
            {
                sender.Session.Response.guildJoinRep = new GuildJoinResponse();
                sender.Session.Response.guildJoinRep.Result = Result.Failed;
                sender.Session.Response.guildJoinRep.Errormsg = "公会不存在";

                sender.SendResponse();
                return;

            }
  
            request.Apply.characterId = character.Id;
            request.Apply.Name = character.Name;
            request.Apply.Level = character.Data.Level;
            request.Apply.Class = character.Data.Class;

            if(guild.JoinApply(request.Apply))
            {
                sender.Session.Response.guildJoinRep = new GuildJoinResponse();
                sender.Session.Response.guildJoinRep.Result = Result.Success;
                sender.Session.Response.guildJoinRep.Errormsg = "申请已提交，等待审批";
                sender.SendResponse();

                var leader = SessionManager.Instance.GetSession(guild.Data.LeaderID);
                if(leader != null)
                {
                    leader.Session.Response.guildJoinReq = request;
                    leader.SendResponse(); 
                }

            }
            else
            {
                sender.Session.Response.guildJoinRep = new GuildJoinResponse();
                sender.Session.Response.guildJoinRep.Result = Result.Failed;
                sender.Session.Response.guildJoinRep.Errormsg = "请勿重复申请";
                sender.SendResponse();
            }


        }

        /// <summary>
        /// 收到公会申请响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnGuildJoinRep(NetConnection<NetSession> sender, GuildJoinResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildJoinRep: Guild:{0},character:{1}{2}", response.Apply.GuildId, response.Apply.characterId, response.Apply.Name);
            var guild = GuildManager.Instance.GetGuild(character.Data.GuildID);
            bool accept = response.Apply.Result == GuildApplyResult.Accept;

            sender.Session.Response.guildJoinRep = new GuildJoinResponse();
            sender.Session.Response.guildJoinRep.Apply = response.Apply;

            if(!guild.JoinApprove(response.Apply))
            {
                sender.Session.Response.guildJoinRep.Result = Result.Failed;
                sender.Session.Response.guildJoinRep.Errormsg = "申请不存在或已经被处理";
                sender.SendResponse();
                return;
            }
            sender.Session.Response.guildJoinRep.Result = Result.Success;
            sender.Session.Response.guildJoinRep.Errormsg = accept?  "已同意该入会申请" : "已拒绝该入会申请";
            sender.SendResponse();

            var requester = SessionManager.Instance.GetSession(response.Apply.characterId);

            if (requester != null)
            {
                if(accept) requester.Session.Character.Guild = guild;
                requester.Session.Response.guildJoinRep = response;
                requester.Session.Response.guildJoinRep.Result = accept? Result.Success : Result.Failed;
                requester.Session.Response.guildJoinRep.Errormsg = accept ? "加入公会成功" : "加入公会失败";
                requester.SendResponse();

            }
        }

        private void OnGuildList(NetConnection<NetSession> sender, GuildListRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildList: character:{0}{1}", character.Id, character.Name);
            sender.Session.Response.guildList = new GuildListResponse();
            sender.Session.Response.guildList.Guilds.AddRange(GuildManager.Instance.GetGuildsInfo());
            sender.Session.Response.guildList.Result = Result.Success;
            sender.SendResponse();
        }

        private void OnGuildLeave(NetConnection<NetSession> sender, GuildLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildLeave: character:{0}{1}", character.Id, character.Name);
           
            var guild = character.Guild;
            bool isLeader = guild.Data.LeaderID == character.Id;
            var members = isLeader
                ? guild.Data.Members.Select(m => m.CharacterID).ToList()
                : null;
            guild.Leave(character);
            DBService.Instance.Save();
            if(isLeader)
            {
                GuildManager.Instance.RemoveGuild(guild);
                foreach(var memberId in members)
                {
                    if (memberId == character.Id) continue;
                    var target = SessionManager.Instance.GetSession(memberId);
                    if(target != null)
                    {
                        target.Session.Response.Guild = new GuildResponse();
                        target.Session.Response.Guild.Result = Result.Success;
                        target.SendResponse();
                    }
                }
            }

            sender.Session.Response.Guild = new GuildResponse();
            sender.Session.Response.Guild.Result = Result.Success;

            sender.Session.Response.guildLeave = new GuildLeaveResponse();
            sender.Session.Response.guildLeave.Result = Result.Success;
            sender.Session.Response.guildLeave.Errormsg =
                isLeader ? "公会解散成功" : "离开公会成功";

            sender.SendResponse();
        }

        private void OnGuildAdmin(NetConnection<NetSession> sender, GuildAdminRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildAdmin: character:{0}{1}", character.Id, character.Name);
            sender.Session.Response.guildAdmin = new GuildAdminResponse();

            if(character.Guild == null)
            {
                sender.Session.Response.guildAdmin.Result = Result.Failed;
                sender.Session.Response.guildAdmin.Errormsg = "违法操作";
                sender.SendResponse();
                return;
            }
            sender.Session.Response.guildAdmin.Command = message.Command;
            if (message.targetId == character.Id)
            {
                sender.Session.Response.guildAdmin.Result = Result.Failed;
                sender.Session.Response.guildAdmin.Errormsg = "不能对自己进行操作"; 
                sender.SendResponse();
                return;
            }

            if(message.Command == GuildAdmainCommand.SetIntroduce)
            {
                if(!character.Guild.SetIntroduce(character.Id,message.Introduce))
                {
                    sender.Session.Response.guildAdmin.Result = Result.Failed;
                    sender.Session.Response.guildAdmin.Errormsg = "修改公会宗旨失败";
                    sender.SendResponse();
                    return;
                }
                sender.Session.Response.guildAdmin.Result = Result.Success;
                sender.Session.Response.guildAdmin.Errormsg = "修改公会宗旨成功";
                sender.SendResponse();
                return;
            }
            
            

            string targetErrormsg;
            switch (message.Command)
            {
                case GuildAdmainCommand.Promote:
                    sender.Session.Response.guildAdmin.Errormsg = "已将该成员晋升为副会长";
                    targetErrormsg = "你已被晋升为副会长";
                    break;
                case GuildAdmainCommand.Depose:
                    sender.Session.Response.guildAdmin.Errormsg = "已罢免该成员的公会职位";
                    targetErrormsg = "你的公会职位已被罢免";
                    break;
                case GuildAdmainCommand.Kickout:
                    sender.Session.Response.guildAdmin.Errormsg = "已将该成员踢出公会";
                    targetErrormsg = "你已被公会踢出";
                    break;
                case GuildAdmainCommand.Treansfer:
                    sender.Session.Response.guildAdmin.Errormsg = "已将会长转让给该成员";
                    targetErrormsg = "你已成为公会会长";
                    break;
                default:
                    sender.Session.Response.guildAdmin.Result = Result.Failed;
                    sender.Session.Response.guildAdmin.Errormsg = "未知公会管理操作";
                    sender.SendResponse();
                    return;
            }

            if (!character.Guild.ExecuteAdmin(message.Command, message.targetId, character.Id))
            {
                sender.Session.Response.guildAdmin.Result = Result.Failed;
                sender.Session.Response.guildAdmin.Errormsg = "操作失败";
                sender.SendResponse();
                return;
            }

            var target = SessionManager.Instance.GetSession(message.targetId);
            if(target != null)
            {
                if(message.Command == GuildAdmainCommand.Kickout)
                {
                    target.Session.Character.Guild = null;
                    target.Session.Character.Info.Guild = null;
                    target.Session.Response.Guild = new GuildResponse();
                    target.Session.Response.Guild.Result = Result.Success;
                }
                else
                {
                    target.Session.Response.guildAdmin = new GuildAdminResponse();
                    target.Session.Response.guildAdmin.Result = Result.Success;
                    target.Session.Response.guildAdmin.Command = message.Command;
                    target.Session.Response.guildAdmin.Errormsg = targetErrormsg;  
                }
                target.SendResponse();

            }
            sender.Session.Response.guildAdmin.Result = Result.Success;
            sender.SendResponse();
        }




    }
}
