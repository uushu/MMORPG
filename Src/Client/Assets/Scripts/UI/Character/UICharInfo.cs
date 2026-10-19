using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharInfo : MonoBehaviour
{
	public SkillBridge.Message.NCharacterInfo info;
	
	public Text charName;
	public Text charClass;
	public Image hightLight;

	public bool Selected
	{
		get
		{
			return hightLight.IsActive();
		}
		set
		{
			hightLight.gameObject.SetActive(value);
		}
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
