using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using Service;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildApply : UIWindow
{
	public GameObject GuildApplyItem;
	public Transform root;
	public ListView list;

	void Start()
	{
		GuildService.Instance.OnGuildUpdate += UpdateList;
		GuildService.Instance.SendGuildListRequest();
		this.UpdateList();
	}

	private void OnDestroy()
	{
		GuildService.Instance.OnGuildUpdate -= UpdateList;
	}

	private void UpdateList()
	{
		if (!GuildManager.Instance.HasGuild)
		{
			this.Close();
			return;
		}
		ClearList();
		InitItem();
		
	}

	

	void InitItem()
	{
		foreach (var apply in GuildManager.Instance.guildInfo.Applies)
		{
			GameObject go = Instantiate(GuildApplyItem, root);
			UIGuildApplyItem ui = go.GetComponent<UIGuildApplyItem>();
			ui.SetGuildApply(apply);
			this.list.AddItem(ui);
		}
	}
	private void ClearList()
	{
		list.RemoveAll();
	}

}
