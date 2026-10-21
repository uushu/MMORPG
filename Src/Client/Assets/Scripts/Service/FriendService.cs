using System;
using Manager;
using Models;
using Network;
using SkillBridge.Message;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Service
{
    public class FriendService :Singleton<FriendService>,IDisposable
    {

        public UnityAction OnFriendUpdate;

        public void Init()
        {
            
        }
        public FriendService()
        {
            // 当别人请求添加自己为好友时需要收到请求
            MessageDistributer.Instance.Subscribe<FriendAddRequest>(this.OnFriendAddRequest);
            MessageDistributer.Instance.Subscribe<FriendAddResponse>(this.OnFriendAddResponse);
            MessageDistributer.Instance.Subscribe<FriendListResponse>(this.OnFriendListResponse);
            MessageDistributer.Instance.Subscribe<FriendRemoveResponse>(this.OnFriendRemoveResponse);

        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<FriendAddRequest>(this.OnFriendAddRequest);
            MessageDistributer.Instance.Unsubscribe<FriendAddResponse>(this.OnFriendAddResponse);
            MessageDistributer.Instance.Unsubscribe<FriendListResponse>(this.OnFriendListResponse);
            MessageDistributer.Instance.Unsubscribe<FriendRemoveResponse>(this.OnFriendRemoveResponse);

        }
        

        /// <summary>
        /// 发送请求添加好友请求
        /// </summary>
        /// <param name="friendId"></param>
        /// <param name="friendName"></param>
        public void SendFriendAddRequest(int friendId, string friendName)
        {
            Debug.Log("SendFriendAddRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendAddReq = new FriendAddRequest();
            message.Request.friendAddReq.FromId = User.Instance.CurrentCharacter.Id;
            message.Request.friendAddReq.FromName = User.Instance.CurrentCharacter.Name;
            message.Request.friendAddReq.ToId = friendId;
            message.Request.friendAddReq.ToName = friendName;
            NetClient.Instance.SendMessage(message);

        }
        
        /// <summary>
        /// 发送请求添加好友响应
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="request"></param>
        public void SendFriendAddResponse(bool accept, FriendAddRequest request)
        {
            Debug.Log("SendFriendAddResponse");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendAddRep = new FriendAddResponse();
            message.Request.friendAddRep.Result = accept ? Result.Success : Result.Failed;
            message.Request.friendAddRep.Errormsg = accept ? "对方同意了你的请求" : "对方拒绝了你的请求";
            message.Request.friendAddRep.Request= request;
            NetClient.Instance.SendMessage(message);

        }
        
        /// <summary>
        /// 收到添加好友请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnFriendAddRequest(object sender, FriendAddRequest message)
        {
            var confirm = MessageBox.Show(string.Format("{0}请求添加你为好友", message.FromName), "好友请求", MessageBoxType.Confirm, "同意", "拒绝");
            confirm.OnYes = () =>
            {
                this.SendFriendAddResponse(true, message);
            };
            confirm.OnNo = () =>
            {
                this.SendFriendAddResponse(false, message);
            };
        }
        
        /// <summary>
        ///  收到添加好友响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnFriendAddResponse(object sender, FriendAddResponse message)
        {
            if(message.Result == Result.Success && message.Request.FromId == User.Instance.CurrentCharacter.Id)
                MessageBox.Show(message.Request.ToName +"同意了你的请求", "添加好友请求");
            else if(message.Result == Result.Success && message.Request.ToId == User.Instance.CurrentCharacter.Id)
                MessageBox.Show("你已和"+message.Request.FromName + "成为好友", "添加好友请求");
            else
                MessageBox.Show(message.Errormsg, "添加好友失败");
        }

        
        private void OnFriendListResponse(object sender, FriendListResponse message)
        {
            Debug.Log("OnFriendList");
            FriendManager.Instance.allFriends = message.Friends;
            
            if(this.OnFriendUpdate != null)
                this.OnFriendUpdate();
        }

        public void SendFriendRemoveRequest(int infoId, int friendId)
        {
            Debug.Log("SendFriendAdd");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendRemove = new FriendRemoveRequest();
            message.Request.friendRemove.Id = infoId;
            message.Request.friendRemove.friendId = friendId;

            NetClient.Instance.SendMessage(message);
        }

        private void OnFriendRemoveResponse(object sender, FriendRemoveResponse message)
        {
            if(message.Result == Result.Success)
                MessageBox.Show("删除好友成功", "删除好友");
            else
                MessageBox.Show("删除好友失败", "删除好友",MessageBoxType.Error);
        }

    }
}
