using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using Service;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UITeam : MonoBehaviour
{
	public Text title;
	public ListView list;
	public UITeamItem[] members;

	private void Start()
	{
		if (TeamManager.Instance.TeamInfo == null)
		{
			this.gameObject.SetActive(false);
			return;
		}

		foreach (var v in members)
		{
			this.list.AddItem(v);
		}
	}

	private void OnEnable()
	{

		UpdateTeamUI();
	}

	
	public void ShowTeam(bool show)
	{
		this.gameObject.SetActive(show);
		if(show)
			UpdateTeamUI();
	}
	
	private void UpdateTeamUI()
	{
		if(TeamManager.Instance.TeamInfo == null)
            return;
        title.text = string.Format("我的队伍 ({0}/5)",TeamManager.Instance.TeamInfo.Members.Count);

        for (int i = 0; i < 5; i++)
        {
	        if (i < TeamManager.Instance.TeamInfo.Members.Count)
	        {
		        members[i].SetMemberInfo(i,TeamManager.Instance.TeamInfo.Members[i],TeamManager.Instance.TeamInfo.leaderId == TeamManager.Instance.TeamInfo.Members[i].Id);
		        members[i].gameObject.SetActive(true);
	        }
	        else
	        {
		        members[i].gameObject.SetActive(false);
	        }
        }
	}

	public void OnClickLeaveBtn()
	{
		MessageBox.Show("确定要退出队伍吗？","提示", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
		{
			TeamService.Instance.SendTeamLeaveRequest(TeamManager.Instance.TeamInfo.Id);
		};
	}

}
