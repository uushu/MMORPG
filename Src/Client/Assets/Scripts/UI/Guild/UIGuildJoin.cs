using System;
using System.Collections;
using System.Collections.Generic;
using Service;
using SkillBridge.Message;
using UI;
using UnityEngine;

public class UIGuildJoin : UIWindow
{

	public UIGuildInfo guildInfo;
	public GameObject guildItem;
	public Transform root;
	public ListView listView;

	private UIGuildItem selectItem;

	void Start()
	{
		this.listView.onItemSelected += this.OnGuildItemSelected;
		this.guildInfo.Info = null;
		GuildService.Instance.OnGuildListResult += this.UpdateGuildList; // 自动刷新公会列表
		GuildService.Instance.SendGuildListRequest(); // 发送公会列表请求
	}

	private void OnDestroy()
	{
		this.listView.onItemSelected -= this.OnGuildItemSelected;
		GuildService.Instance.OnGuildListResult -= this.UpdateGuildList;
		
	}
	private void InitItems(List<NGuildInfo> guilds)
	{
		foreach (var item in guilds)
		{
			GameObject go = Instantiate(guildItem, root);
			UIGuildItem ui = go.GetComponent<UIGuildItem>();
			ui.SetGuildInfo(item);
			this.listView.AddItem(ui);
		}
	}

	/// <summary>
	/// 选中更新详细页
	/// </summary>
	/// <param name="item"></param>
	private void OnGuildItemSelected(ListView.ListViewItem item)
	{
		this.selectItem = item as UIGuildItem;
		this.guildInfo.Info = this.selectItem.Info;
	}
	
	private void UpdateGuildList(List<NGuildInfo> guilds)
	{
		ClearList();
		InitItems(guilds);
	}



	private void ClearList()
	{
		this.listView.RemoveAll();
	}


	public void OnClickJoin()
	{
		if (selectItem == null)
		{
			MessageBox.Show("请选择要加入的公会");
			return;
		}

			
		MessageBox.Show(string.Format("确定要加入公会{0}吗？", selectItem.Info.Name), "申请加入公会", MessageBoxType.Confirm,
				"确定", "取消").OnYes =
			() =>
			{
				GuildService.Instance.SendGuildJoinRequest(selectItem.Info.Id);
			};
	}
	
}
