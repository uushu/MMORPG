using System.Collections;
using System.Collections.Generic;
using Service;
using UnityEngine;

public class UISetting : UIWindow {

	public void OnClickCharSelect()
	{
		var pop = UIManager.Instance.Show<UIPopCharMenu>();
		// SceneManager.Instance.LoadScene("CharacterSelect");
		// UserService.Instance.SendGameLeave();
	}

	public void OnClickQuitGame()
	{
		UserService.Instance.SendGameLeave(true);
	}

}
