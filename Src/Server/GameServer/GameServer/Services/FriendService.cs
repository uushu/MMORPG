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
    internal class FriendService : Singleton<FriendService>
    {
        private readonly object _lock = new object();

        readonly Dictionary<Tuple<int, int>, FriendAddRequest> pendingFriendAddReqs = new Dictionary<Tuple<int, int>, FriendAddRequest>();
        public FriendService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddRequest>(this.OnFriendAddRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddResponse>(this.OnFriendAddResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendRemoveRequest>(this.OnFriendRemoveRequest);

        }

        internal void Init()
        {
            
        }


        /// <summary>
        /// 收到添加好友请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        /// <exception cref="NotImplementedException"></exception>

        private void OnFriendAddRequest(NetConnection<NetSession> sender, FriendAddRequest request)
        {
            lock(_lock)
            {
                // 当前角色
                Character character = sender.Session.Character;
                Log.InfoFormat("OnFriendRequest: FromId{0} FromName{1} ToId{2} ToName{3}", request.FromId, request.FromName, request.ToId, request.ToName);

                // 1. 查找好友ID
                if (request.ToId == 0)
                {
                    // 用好友名称查找 ,只处理在线角色
                    foreach (var cha in CharacterManager.Instance.Characters)
                    {
                        if (cha.Value.Data.Name == request.ToName)
                        {
                            request.ToId = cha.Key;
                            break;
                        }
                    }
                    // TODO: 离线查询，到数据库中查找
                }
                // 2. 判断是否已成为好友，并查找好友的Session
                if (request.ToId > 0)
                {
                    if (character.FriendManager.GetFriendInfo(request.ToId) != null)
                    {
                        SendFriendAddResult(sender,request,Result.Failed, "你们已经是好友了");
                        return;
                    }
                }

                // 3. 判断好友是否离线或不存在
                NetConnection<NetSession> friend = SessionManager.Instance.GetSession(request.ToId);
                if (friend == null || CharacterManager.Instance.GetCharacter(request.ToId)==null)
                {
                    SendFriendAddResult(sender, request, Result.Failed, "好友不存在或已离线"); 
                    return;
                }

                // 4.是否重复发送申请
                var key = Tuple.Create(character.Id, request.ToId);
                if(pendingFriendAddReqs.ContainsKey(key))
                {
                    SendFriendAddResult(sender, request, Result.Failed, "申请已发送，等待对方处理");
                    return;
                }

                request.ToName = friend.Session.Character.Data.Name;
                pendingFriendAddReqs[key] = request;

                // 5. 服务器转发申请
                Log.InfoFormat("ForFriendAddRequest: FromID{0} FromName{1} ToId{2} ToName{3}", request.FromId, request.FromName, request.ToId, request.ToName);
                
                friend.Session.Response.friendAddReq = request;
                friend.SendResponse();
            }
         
        }

        /// <summary>
        /// 收到添加好友响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnFriendAddResponse(NetConnection<NetSession> sender, FriendAddResponse response)
        {
            lock(_lock)
            {
                Character character = sender.Session.Character;
           
                // 查询申请是否存在
                var key = Tuple.Create(response.Request.FromId, character.Id);
                FriendAddRequest request;
                if (!pendingFriendAddReqs.TryGetValue(key, out request))
                    return;

                pendingFriendAddReqs.Remove(key);
                response.Request= request;

                // 双方相互申请添加好友时，查看是否已经建立了好友关系
                if (response.Result == Result.Success && character.Data.Friends.Any(item => item.FriendID == request.FromId))
                    return;

                Log.InfoFormat("OnFriendAddResponse: character{0} Result{1} FromId{2} FromName{3} ToId{4} ToName{5}", character.Id, response.Result, response.Request.FromId, response.Request.FromName, response.Request.ToId, response.Request.ToName);
                sender.Session.Response.friendAddRep = response;

                var requester = SessionManager.Instance.GetSession(response.Request.FromId);
                
                if (response.Result == Result.Success)
                {
                    
                    if (requester == null || CharacterManager.Instance.GetCharacter(request.FromId)==null)
                    {
                        sender.Session.Response.friendAddRep.Result = Result.Failed;
                        sender.Session.Response.friendAddRep.Errormsg = "请求者已下线";
                    }

                    else
                    {
                        character.FriendManager.AddFriend(requester.Session.Character);
                        requester.Session.Character.FriendManager.AddFriend(character);
                        DBService.Instance.Save();
                        requester.Session.Response.friendAddRep = response;
                        requester.Session.Response.friendAddRep.Result = Result.Success;
                        requester.Session.Response.friendAddRep.Errormsg = "添加好友成功";
                        requester.SendResponse();
                    }

                }
                else
                {
                    response.Errormsg = "已拒绝" + response.Request.FromName + "的好友申请";

               
                    if (requester != null)
                    {
                        SendFriendAddResult(requester,request,Result.Failed, character.Data.Name + "拒绝了你的好友申请");
                       
                    }
                }
                sender.SendResponse();
            }
        }

        private void SendFriendAddResult(NetConnection<NetSession> connection , FriendAddRequest request, Result result,string message)
        {
            connection.Session.Response.friendAddRep = new FriendAddResponse();
            connection.Session.Response.friendAddRep.Request = request;
            connection.Session.Response.friendAddRep.Result = result;
            connection.Session.Response.friendAddRep.Errormsg = message;

            connection.SendResponse();
        }


        private void OnFriendRemoveRequest(NetConnection<NetSession> sender, FriendRemoveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendRemove: characterId{0} FriendRemoveId{1}", character.Id, request.Id);
            sender.Session.Response.friendRemove = new FriendRemoveResponse();
            sender.Session.Response.friendRemove.Id = request.Id;

            NetConnection<NetSession> friend = SessionManager.Instance.GetSession(request.friendId);
            // 删除自己的好友
            if (character.FriendManager.RemoveFriendById(request.Id))
            {
                sender.Session.Response.friendRemove.Result = Result.Success;
                
                //删除好友中的自己
                // 好友在线
                if (friend != null)
                {
                    friend.Session.Character.FriendManager.RemoveFriendByFriendId(character.Id);
                }
                else
                {
                    this.RemoveFriend(request.friendId, character.Id);
                }
            }
            else
                sender.Session.Response.friendRemove.Result = Result.Failed;
            DBService.Instance.Save();

            if(friend != null)
            {
                friend.SendResponse();
            }
            sender.SendResponse();
        }

        void RemoveFriend( int characterId , int friendId)
        {
            var removeItem = DBService.Instance.Entities.CharacterFriends.FirstOrDefault(v => v.TCharacterID == characterId && v.FriendID == friendId);
            if(removeItem != null)
            {
                DBService.Instance.Entities.CharacterFriends.Remove(removeItem);
            }
        }

        /// <summary>
        /// 清除发给接受者的待处理申请请求
        /// </summary>
        /// <param name="characterId"></param>
        public void RemovePendingRequests(int characterId)
        {
            lock(_lock)
            {
                var keys = pendingFriendAddReqs.Keys.Where(key => key.Item2 == characterId).ToList();
                foreach(var key in keys)
                {
                    pendingFriendAddReqs.Remove(key);
                }
            }
        }
    }
}
