using System.Collections;
using System.Collections.Generic;
using Service;
using SkillBridge.Message;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{

	public InputField userName;
	public InputField password;
	
	public Button registerButton;
	public Button loginButton;

	void Start()
	{
		UserService.Instance.OnLogin = OnLogin;
	}

	public void OnClickLogin()
	{
		if (string.IsNullOrEmpty(this.userName.text))
		{
			MessageBox.Show("请输入账号");
			return;
		}
		if (string.IsNullOrEmpty(this.password.text))
		{
			MessageBox.Show("请输入密码");
			return;
		}
		//SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
		// Enter Game
		UserService.Instance.SendLogin(this.userName.text,this.password.text);

		
		
	}

	void OnLogin(Result result , string message)
	{
		if (result == Result.Success)
		{
			SceneManager.Instance.LoadScene("CharSelect");
			//SoundManager.Instance.PlayMusic(SoundDefine.Music_Select);
		}
		else 
			MessageBox.Show(message,"错误",MessageBoxType.Error);
	}
}
