using Models;
using SkillBridge.Message;

namespace Manager
{
    public class GuildManager : Singleton<GuildManager>
    {
        public int MaxMemberCount = 255;
        public NGuildInfo guildInfo;
        public NGuildMemberInfo myMemberInfo;

        public bool HasGuild
        {
            get { return this.guildInfo != null; }
        }
        public void Init(NGuildInfo info)
        {
            this.guildInfo = info;
            if (guildInfo == null)
            {
                myMemberInfo = null;
                return;
            }

            foreach (var member in guildInfo.Members)
            {
                if (member.characterId == User.Instance.CurrentCharacter.Id)
                {
                    myMemberInfo = member;
                    break;
                }
            }
        }

        public void ShowGuild()
        {
            if (HasGuild)
            {
                UIManager.Instance.Show<UIGuildMain>();
            }
            else
            {
                var window = UIManager.Instance.Show<UIGuildSelect>();
                window.OnClose += PopNoGuild_OnClose;
            }
        }

        private void PopNoGuild_OnClose(UIWindow sender, UIWindow.WindowResult result)
        {
            if(result == UIWindow.WindowResult.Yes)
            {
                // 创建公会
                UIManager.Instance.Show<UIGuildCreate>();
            }
            else if (result == UIWindow.WindowResult.No)
            {
                // 加入公会
                UIManager.Instance.Show<UIGuildJoin>();
            }
        }
    }
}