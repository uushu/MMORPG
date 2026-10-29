using System.Collections;
using System.Collections.Generic;
using Common.Utils;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildMemberItem : ListView.ListViewItem
{
	public Text name;
	public Text level;
	public Text Class;
	public Text title;

	public Text joinTime;
	public Text status;

	public Image bg;
	public Sprite normalBg;
	public Sprite selectedBg;

	public NGuildMemberInfo Info;

	public override void OnSelected(bool selected)
	{
		this.bg.overrideSprite = selected ? selectedBg : normalBg;
	}

	public void SetGuildMember(NGuildMemberInfo item)
	{
		this.Info = item;
		if (this.name != null)
			this.name.text = item.Info.Name;
		if (this.level != null)
			this.level.text = item.Info.Level.ToString();
		if (this.Class != null)
			this.Class.text = item.Info.Class.ToString();
		if (this.title != null)
			this.title.text = item.Title.ToString();
		if (this.joinTime != null)
			this.joinTime.text = TimeUtil.GetTime(item.joinTime).ToShortDateString();
		if (this.status != null)
			this.status.text = item.Status == 1 ? "在线" :TimeUtil.GetTime(item.lastTime).ToShortDateString();

	}

}
