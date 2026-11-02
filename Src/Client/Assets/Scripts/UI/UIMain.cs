using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using Models;
using Service;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoSingleton<UIMain>
{
	public Text name;
	public Text level;

	public UITeam TeamWindow;

	protected override void OnStart()
	{
		UserService.Instance.OnCharacterEnter += UpdateInfo;
		UpdateInfo();
	}

	private void OnDestroy()
	{
		UserService.Instance.OnCharacterEnter -= UpdateInfo;
	}

	void Update()
	{
		
	}

	void UpdateInfo()
	{
		this.name.text = string.Format("{0} [{1}]",User.Instance.CurrentCharacter.Name,User.Instance.CurrentCharacter.Id);
		this.level.text = User.Instance.CurrentCharacter.Level.ToString();

	}

	public void OnClickBackToCharSelect()
	{
		SceneManager.Instance.LoadScene("CharacterSelect");
		UserService.Instance.SendGameLeave();
	}
	
	public void OnClickUIBag()
	{
		 UIManager.Instance.Show<UIBag>();
	}
	public void OnClickUIEquip()
	{
		 UIManager.Instance.Show<UIEquip>();
	}

	public void OnClickUIQuestSystem()
	{
		UIManager.Instance.Show<UIQuestSystem>();
	}
	
	
	public void OnClickUIFriend()
	{
		UIManager.Instance.Show<UIFriend>();
	}
	
	public void OnClickUIGuild()
	{
		GuildManager.Instance.ShowGuild();
	}
	
	public void OnClickUIRide()
	{
		
	}
	
	public void OnClickUISetting()
	{
		
	}
	
	
	public void OnClickUISkill()
	{
		
	}

	
	public void ShowTeamUI(bool show)
	{
		TeamWindow.ShowTeam(show);
	}
}
