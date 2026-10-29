using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using Service;
using UnityEngine;

public class UIGuildMain : UIWindow {

	public UIGuildInfo guildInfo;
	public GameObject guildMemberItem;
	public Transform root;
	public ListView listView;

	private UIGuildMemberItem selectItem;

	void Start()
	{
		GuildService.Instance.OnGuildUpdate = UpdateUI;
		this.listView.onItemSelected += this.OnGuildMemerSelected;
		this.UpdateUI();
	}

	private void OnDestroy()
	{
		GuildService.Instance.OnGuildUpdate = null;
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
		foreach (var member in GuildManager.Instance.guildInfo.Members)
		{
			GameObject go = Instantiate(guildMemberItem, root);
			UIGuildMemberItem ui = go.GetComponent<UIGuildMemberItem>();
			ui.SetGuildMember(member);
			this.listView.AddItem(ui);
		}
	}
	private void UpdateUI()
	{
		this.guildInfo.Info = GuildManager.Instance.guildInfo;

		ClearList();
		InitItems();
	}
	
	

	private void ClearList()
	{
		this.listView.RemoveAll();
	}
}
