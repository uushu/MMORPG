using System.Collections;
using System.Collections.Generic;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIFriendItem : ListView.ListViewItem
{

	public Text name;
	public Text level;
	public Text Class;
	public Text status;

	public Image bg;
	public Sprite normalBg;
	public Sprite selectedBg;


	public override void OnSelected(bool selected)
	{
		this.bg.overrideSprite = selected ? selectedBg : normalBg;
	}

	public NFriendInfo Info;
	
	public void SetFriendInfo(NFriendInfo info)
	{
		this.Info = info;
		this.name.text = info.friendInfo.Name;
		this.level.text = info.friendInfo.Level.ToString();
		this.Class.text = info.friendInfo.Class.ToString();
		this.status.text = info.Status == 1 ? "在线" : "离线";
	}
}
