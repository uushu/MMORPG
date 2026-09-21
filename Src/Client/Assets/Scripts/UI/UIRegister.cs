using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRegister : MonoBehaviour {

	public InputField userName;
	public InputField password;
	public InputField passwordConfirm;

	public Button enterGame;

	public void OnClickRegister()
	{
		if (string.IsNullOrEmpty(this.userName.text))
		{
			//MessageBox.Show("请输入账号");
			return;
		}

		if (string.IsNullOrEmpty(this.password.text))
		{
			//MessageBox.Show("请输入密码");
			return;
		}
		if (string.IsNullOrEmpty(this.passwordConfirm.text))
		{
			//MessageBox.Show("请输入确认密码");
			return;
		}

		if (password.text != passwordConfirm.text)
		{
			//MessageBox.Show("两次输入密码不一致");
			return;
		}
	}
	
}
