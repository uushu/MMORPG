using System.Collections;
using System.Collections.Generic;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UICharInfo : ListView.ListViewItem
{
	public NCharacterInfo info;
	
	public Text charName;
	public Text charClass;
	public Image hightLight;
	public int characterIdx;

	public override void OnSelected(bool selected)
	{
		this.hightLight.gameObject.SetActive(selected);
	}


	private void Start()
	{
		if (info != null)
		{
			this.charClass.text = this.info.Class.ToString();
			this.charName.text = this.info.Name.ToString();
		}
	}


}
