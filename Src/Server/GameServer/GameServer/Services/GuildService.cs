using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

            if(response.Result == Result.Success)
            {
                guild.JoinApprove(response.Apply);
            }
            var requester = SessionManager.Instance.GetSession(response.Apply.characterId);
          
            if(requester != null)
            {
                requester.Session.Character.Guild = guild;
                requester.Session.Response.guildJoinRep = response;
                requester.Session.Response.guildJoinRep.Result = Result.Success;
                requester.Session.Response.guildJoinRep.Errormsg = "加入公会成功";
                requester.SendResponse();
            }

            guild.PostProcess(character, sender.Session.Response);
            sender.SendResponse();

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
            sender.Session.Response.guildLeave = new GuildLeaveResponse();
            
            character.Guild.Leave(character);
            sender.Session.Response.guildLeave.Result = Result.Success;
            DBService.Instance.Save();
            sender.SendResponse();
        }


       


    }
}
