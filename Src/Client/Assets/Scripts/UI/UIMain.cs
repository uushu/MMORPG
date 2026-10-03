using System.Collections;
using System.Collections.Generic;
using Models;
using Service;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoSingleton<UIMain>
{
	public Text name;
	public Text level;

	protected override void OnStart()
	{
		UpdateInfo();
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
	public void OnClickUITest()
	{
		
		UITest ui=UIManager.Instance.Show<UITest>();
		ui.title.text = "测试弹窗";
		
	}
	
}
