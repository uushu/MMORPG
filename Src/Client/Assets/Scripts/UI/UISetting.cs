using System.Collections;
using System.Collections.Generic;
using Service;
using UnityEngine;

public class UISetting : UIWindow {

	public void OnClickCharSelect()
	{
		
		SceneManager.Instance.LoadScene("CharacterSelect");
		SoundManager.Instance.PlayMusic(SoundDefine.Music_Select);
		UserService.Instance.SendGameLeave();
	}

	public void OnClickQuitGame()
	{
		UserService.Instance.SendGameLeave(true);
	}
	public void OnClickMusicSetting()
	{
		UIManager.Instance.Show<UISystemConfig>();
		this.Close();
	}

}
