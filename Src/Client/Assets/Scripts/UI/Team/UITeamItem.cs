using System.Collections;
using System.Collections.Generic;
using SkillBridge.Message;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UITeamItem : ListView.ListViewItem
{

	public Image classIcon;
	public Text name;
	public Slider hpSlider;
	public Image leader;

	private int idx;
	private NCharacterInfo characterInfo;

	public void SetMemberInfo(int idx, NCharacterInfo item, bool isLeader)
	{
		this.idx = idx;
		this.characterInfo = item;
		if(this.name!=null) this.name.text = "Lv."+this.characterInfo.Level.ToString().PadRight(4)+this.characterInfo.Name;
		if(this.classIcon != null) this.classIcon.overrideSprite = SpriteManager.Instance.Icons[(int)this.characterInfo.Class-1];
        if(this.leader!=null) this.leader.gameObject.SetActive(isLeader);
	}

}
