using System.Collections;
using System.Collections.Generic;
using Manager;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildInfo : MonoBehaviour {

	public Text guildName;
	public Text guildID;
	public Text guildLeader;
	public Text guildCount;
	public Text guildIntroduce;

	private NGuildInfo guildInfo;

	public NGuildInfo Info
	{
		get { return this.guildInfo; }
		set
		{
			this.guildInfo = value;
			this.UpdateUI();
		}
	}

	private void UpdateUI()
	{
		if (this.guildInfo == null)
		{
			this.gameObject.SetActive(false);
		}
		else
		{
			
            this.guildName.text = this.Info.Name;
            this.guildID.text = "ID: " + this.Info.Id;
            this.guildLeader.text = "会长: " + this.Info.leaderName;
            this.guildCount.text = string.Format("成员数量: {0}/{1}", this.Info.memberCount, GuildManager.Instance.MaxMemberCount);
            this.guildIntroduce.text = this.guildInfo.Introduce;
            this.gameObject.SetActive(true);
		}
	}
}
