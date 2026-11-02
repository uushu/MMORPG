using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using Models;
using Service;
using SkillBridge.Message;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildMain : UIWindow {

	public UIGuildInfo guildInfo;
	public GameObject guildMemberItem;
	public Transform root;
	public ListView listView;

	public GameObject AdminPanel;
	public GameObject LeaderPanel;
	public GameObject SearchNoResultPanel;
	
	public InputField SearchInput;
	public InputField IntroduceInput;

	private UIGuildMemberItem selectItem;

	void Start()
	{
		GuildService.Instance.OnGuildUpdate += UpdateUI;
		this.listView.onItemSelected += this.OnGuildMemerSelected;
		this.UpdateUI();
	}

	private void OnDestroy()
	{
		GuildService.Instance.OnGuildUpdate -= this.UpdateUI;
		listView.onItemSelected -= this.OnGuildMemerSelected;
	}

	private void OnGuildMemerSelected(ListView.ListViewItem item)
	{
		this.selectItem = item as UIGuildMemberItem;
	}


	/// <summary>
	/// 初始化所有公会列表信息
	/// </summary>
	private void InitItems()
	{
		string keyWord = SearchInput.text.Trim();
		var members = GuildManager.Instance.guildInfo.Members.FindAll(m=>m.Info.Name.Contains(keyWord));
		foreach (var member in members)
		{
			GameObject go = Instantiate(guildMemberItem, root);
			UIGuildMemberItem ui = go.GetComponent<UIGuildMemberItem>();
			ui.SetGuildMember(member);
			this.listView.AddItem(ui);
		}
		SearchNoResultPanel.SetActive(members.Count == 0);
	}
	private void UpdateUI()
	{
		if (!GuildManager.Instance.HasGuild)
		{
			this.Close();
			return;
		}

		if (GuildManager.Instance.myMemberInfo.Title != GuildTitle.President)
			IntroduceInput.readOnly = true;
		this.guildInfo.Info = GuildManager.Instance.guildInfo;
		
		if(IntroduceInput.readOnly) 
			IntroduceInput.text = GuildManager.Instance.guildInfo.Introduce;
		else
			IntroduceInput.ForceLabelUpdate();
		AdminPanel.SetActive(GuildManager.Instance.myMemberInfo.Title > GuildTitle.None);
		LeaderPanel.SetActive(GuildManager.Instance.myMemberInfo.Title == GuildTitle.President);
		ClearList();
		InitItems();
	}
	
	

	private void ClearList()
	{
		this.selectItem = null;
		this.listView.RemoveAll();
	}

	public void OnClickApplyList()
	{
		UIManager.Instance.Show<UIGuildApply>();
	}
	
	public void OnClickSearch()
	{
		
		this.UpdateUI();
		
	}


	
	public void OnClickLeave()
	{
		bool isLeader = GuildManager.Instance.myMemberInfo.Title == GuildTitle.President;
		if (isLeader)
			MessageBox.Show("确定要解散公会吗？", "解散公会", MessageBoxType.Confirm).OnYes =
				() => GuildService.Instance.SendGuildLeaveRequest();
		else
			MessageBox.Show("确定要离开公会吗？", "离开公会", MessageBoxType.Confirm).OnYes =
				() => GuildService.Instance.SendGuildLeaveRequest();
	}
	
	public void OnClickChat()
	{
		// TODO: 私聊逻辑
	}
	
	public void OnClickKickout()
	{
		if (selectItem == null)
		{
			MessageBox.Show("请选择要踢出的成员");
			return;
		}
		
		if(selectItem.Info.characterId == User.Instance.CurrentCharacter.Id)
		{
			MessageBox.Show("不能踢出自己","错误",MessageBoxType.Error);
			return;
		}
		
		if (selectItem.Info.Title == GuildTitle.President)
		{
			MessageBox.Show("你无权踢出会长");
			return;
		}

		MessageBox.Show(string.Format("确定要踢出成员[{0}]吗？", selectItem.Info.Info.Name), "踢出公会", MessageBoxType.Confirm,
				"确定", "取消").OnYes =
			() =>
			{
				GuildService.Instance.SendAdminCommand(GuildAdmainCommand.Kickout,this.selectItem.Info.Info.Id);
			};
	}
	
	public void OnClickDepose()
	{
		if (selectItem == null)
		{
			MessageBox.Show("请选择要罢免的成员");
			return;
		}

		if (selectItem.Info.Title == GuildTitle.None)
		{
			MessageBox.Show("对方貌似无职可免");
			return;
		}
		
		if(selectItem.Info.characterId == User.Instance.CurrentCharacter.Id)
		{
			MessageBox.Show("不能罢免自己","错误",MessageBoxType.Error);
			return;
		}

		if (selectItem.Info.Title == GuildTitle.President)
		{
			MessageBox.Show("你无权罢免会长");
			return;
		}
	
		MessageBox.Show(string.Format("确定要罢免成员[{0}]的公会职位吗？", selectItem.Info.Info.Name), "罢免职位", MessageBoxType.Confirm,
				"确定", "取消").OnYes =
			() =>
			{
				GuildService.Instance.SendAdminCommand(GuildAdmainCommand.Depose,this.selectItem.Info.Info.Id);
			};
	}


	public void OnClickPromote()
	{
		if (selectItem == null)
		{
			MessageBox.Show("请选择要升职的成员");
			return;
		}
		
		if(selectItem.Info.characterId == User.Instance.CurrentCharacter.Id)
		{
			MessageBox.Show("不能升职自己","错误",MessageBoxType.Error);
			return;
		}

		if (selectItem.Info.Title != GuildTitle.None)
		{
			MessageBox.Show("对方已有职位");
			return;
		}
		
			
		MessageBox.Show(string.Format("确定要晋升成员[{0}]的公会职位吗？", selectItem.Info.Info.Name), "晋升职位", MessageBoxType.Confirm,
				"确定", "取消").OnYes =
			() =>
			{
				GuildService.Instance.SendAdminCommand(GuildAdmainCommand.Promote,this.selectItem.Info.Info.Id);
			};
	}
	
	public void OnClickTransfer()
	{
		if (selectItem == null)
		{
			MessageBox.Show("请选择要转让的成员");
			return;
		}
		
		if(selectItem.Info.characterId == User.Instance.CurrentCharacter.Id)
		{
			MessageBox.Show("不能转让会长给自己","错误",MessageBoxType.Error);
			return;
		}

		MessageBox.Show(string.Format("确定要把会长转让给成员[{0}]吗？", selectItem.Info.Info.Name), "转让会长", MessageBoxType.Confirm,
				"确定", "取消").OnYes =
			() =>
			{
				GuildService.Instance.SendAdminCommand(GuildAdmainCommand.Treansfer,this.selectItem.Info.Info.Id);
			};
	}

	public void OnClickSetIntroduce()
	{
		if(GuildManager.Instance.myMemberInfo.Title != GuildTitle.President)
		{
			MessageBox.Show("只有会长有权限修改公会简介","错误",MessageBoxType.Error);
			return;
			
		}

		if (IntroduceInput.readOnly)
		{
			IntroduceInput.readOnly = false;
            IntroduceInput.ActivateInputField();
            return;
		}

		string introduce = IntroduceInput.text.Trim();
		if(introduce.Length < 3 || introduce.Length > 50)
		{
			MessageBox.Show("公会简介必须为3-50个字符","错误",MessageBoxType.Error);
			return;
		}
		IntroduceInput.readOnly = true;
		GuildService.Instance.SendAdminCommand(GuildAdmainCommand.SetIntroduce,0,introduce);
	}

	
}
