using System.Collections;
using System.Collections.Generic;
using Manager;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildItem : ListView.ListViewItem
{

	public Text id;
	public Text name;
	public Text memberCount;
	public Text leader;
	public Image bg;
	public Sprite normalBg;
	public Sprite selectedBg;

	public override void OnSelected(bool selected)
	{
		this.bg.overrideSprite = selected ? selectedBg : normalBg;
	}
	
	public NGuildInfo Info;

	public void SetGuildInfo(NGuildInfo item)
	{
		this.Info = item;
		if (this.id != null)
			this.id.text = item.Id.ToString();
		if (this.name != null)
			this.name.text = item.Name;
		if (this.memberCount != null)
			this.memberCount.text = string.Format("{0}/{1}", item.memberCount, GuildManager.Instance.MaxMemberCount);
		if (this.leader != null)
			this.leader.text = item.leaderName;
	}
}
