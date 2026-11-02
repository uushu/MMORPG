using System.Collections;
using System.Collections.Generic;
using Service;
using SkillBridge.Message;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildApplyItem : ListView.ListViewItem {

	public Text name;
	public Text level;
	public Text Class;
	public NGuildApplyInfo applyInfo;

	public void SetGuildApply(NGuildApplyInfo apply)
	{
		this.applyInfo = apply;
		if (name != null) name.text = applyInfo.Name;
		if (level != null) level.text = applyInfo.Level.ToString();
		if (Class != null) Class.text = ((CharacterClass)applyInfo.Class).ToString();
		

	}

	public void OnAccept()
	{
		MessageBox.Show(string.Format("确定同意[{0}]加入公会吗?", this.applyInfo.Name), "申请审批", MessageBoxType.Confirm, "确定", "取消")
			.OnYes = () =>
		{
			GuildService.Instance.SendGuildJoinApply(true,this.applyInfo);
		};
		
	}
	public void OnReject()
	{
		MessageBox.Show(string.Format("确定拒绝[{0}]的入会申请吗?", this.applyInfo.Name), "申请审批", MessageBoxType.Confirm, "确定", "取消")
			.OnYes = () =>
		{
			GuildService.Instance.SendGuildJoinApply(false,this.applyInfo);
		};
		
	}
	
}
