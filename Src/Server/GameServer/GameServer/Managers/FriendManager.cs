using Common;
using GameServer.Entities;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    internal class FriendManager
    {
        private Character owner;
        List<NFriendInfo> friends = new List<NFriendInfo>();
        bool friendChanged = false;

        public FriendManager(Character character)
        {
            this.owner = character;
            this.InitFriends();
        }


        

        private void InitFriends()
        {
            this.friends.Clear();
            foreach(var friend in this.owner.Data.Friends)
            {
                this.friends.Add(GetFriendInfo(friend));
            }
        }

       

        internal void AddFriend(Character friend)
        {
            TCharacterFriend tf = new TCharacterFriend()
            {
                FriendID = friend.Id,
                FriendName = friend.Data.Name,
                Class = friend.Data.Class,
                Level = friend.Data.Level,

            };
            this.owner.Data.Friends.Add(tf);
            friendChanged = true;
        }

        
        internal bool RemoveFriendByFriendId(int friendId)
        {
            var removeFriend = this.owner.Data.Friends.FirstOrDefault(v => v.FriendID == friendId);
            if (removeFriend != null)
            {
                DBService.Instance.Entities.CharacterFriends.Remove(removeFriend);
            }
            friendChanged = true;
            return true;

        }

        internal bool RemoveFriendById(int id)
        {
            var removeItem = this.owner.Data.Friends.FirstOrDefault(v => v.Id == id);
            if (removeItem != null)
            {
                DBService.Instance.Entities.CharacterFriends.Remove(removeItem);
            }
            friendChanged = true;
            return true;

        }

        internal void GetFriendInfos(List<NFriendInfo> list)
        {
            foreach (var friend in friends)
            {
                list.Add(friend);
            }
        }
        internal NFriendInfo GetFriendInfo(int friendId)
        {
            foreach(var friend in friends)
            {
                if(friend.friendInfo.Id == friendId)
                    return friend;
            }
            return null;
        }

        private NFriendInfo GetFriendInfo(TCharacterFriend friend)
        {
            NFriendInfo friendInfo = new NFriendInfo();
            var character = CharacterManager.Instance.GetCharacter(friend.FriendID);
            friendInfo.friendInfo = new NCharacterInfo();
            friendInfo.Id = friend.Id;

            if(character != null)
            {
                friendInfo.friendInfo = character.GetBasicInfo();
                friendInfo.friendInfo.Name = character.Info.Name;
                friendInfo.friendInfo.Class = character.Info.Class;
                friendInfo.friendInfo.Level = character.Info.Level;
                
                if(friend.Level != character.Info.Level)
                    friend.Level = character.Info.Level;

                character.FriendManager.UpdateFriendInfo(this.owner.Info, 1);
                friendInfo.Status = 1;
            }
            else
            {
                friendInfo.friendInfo.Id = friend.FriendID;
                friendInfo.friendInfo.Name = friend.FriendName;
                friendInfo.friendInfo.Class = (CharacterClass)friend.Class;
                friendInfo.friendInfo.Level = friend.Level;
                friendInfo.Status = 0;
            }
            Log.InfoFormat("{0}:{1} GetFriendInfo :{2}:{3} Status:{4}", this.owner.Id, this.owner.Info.Name, friendInfo.friendInfo.Id, friendInfo.friendInfo.Name, friendInfo.Status);
            return friendInfo;
        }

        

        internal void UpdateFriendInfo(NCharacterInfo info, int status)
        {
            foreach (var friend in this.friends)
            {
                if(friend.friendInfo.Id == info.Id)
                {
                    friend.Status = status;
                    break;
                }
            }
            friendChanged = true;
        }

        public void OffLineNotify()
        {
            foreach(var friendInfo in this.friends)
            {
                var friend = CharacterManager.Instance.GetCharacter(friendInfo.friendInfo.Id);
                if(friend!=null)
                {
                    friend.FriendManager.UpdateFriendInfo(this.owner.Info, 0);
                }
            }
        }
        internal void PostProgress(NetMessageResponse message)
        {
           if(friendChanged)
            {
                this.InitFriends();
                if(message.friendList == null)
                {
                    message.friendList = new FriendListResponse();
                    message.friendList.Friends.AddRange(this.friends);
                }
                friendChanged = false; 
            }
        }
    }
}
