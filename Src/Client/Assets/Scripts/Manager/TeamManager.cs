using Models;
using SkillBridge.Message;

namespace Manager
{
    public class TeamManager:Singleton<TeamManager>
    {
        public NTeamInfo TeamInfo;

        public void Init()
        {
            
        }

        public void UpdateTeamInfo(NTeamInfo team)
        {
            TeamInfo = team;
            ShowTeamUI(team != null);
        }

        private void ShowTeamUI(bool show)
        {
            if(UIMain.Instance != null)
            {
                UIMain.Instance.ShowTeamUI(show);
            }
        }
    }
}