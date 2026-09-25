using System.Collections;
using System.Collections.Generic;
using Service;
using SkillBridge.Message;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIRegister : MonoBehaviour {

	public InputField userName;
	public InputField password;
	public InputField passwordConfirm;
	public Button registerBtn;
	
	public GameObject loginPanel;
	void Start()
	{
		UserService.Instance.OnRegister = OnRegister;
	}
	
	void OnEnable()
	{
		if(userName!=null)
			userName.text = string.Empty;
		if(password!=null)
			password.text = string.Empty;
		if(passwordConfirm!=null)
			passwordConfirm.text = string.Empty;
	}
	
	// 输入校验
	public void OnClickRegister()
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
		if (string.IsNullOrEmpty(this.passwordConfirm.text))
		{
			MessageBox.Show("请输入确认密码");
			return;
		}

		if (password.text != passwordConfirm.text)
		{
			MessageBox.Show("两次输入密码不一致");
			return;
		}
		//SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
		UserService.Instance.SendRegister(this.userName.text,this.password.text);
		
		
	}

	void OnRegister(Result result, string message)
	{
		if (result == Result.Success)
		{
            MessageBox.Show("注册成功,请登录", "提示", MessageBoxType.Information).OnYes = this.CloseRegister;
			
		}
		else
		{
			MessageBox.Show(message, "错误", MessageBoxType.Error);
		}
	}

	void CloseRegister()
	{
		this.gameObject.SetActive(false);
		loginPanel.SetActive(true);
	}
}
