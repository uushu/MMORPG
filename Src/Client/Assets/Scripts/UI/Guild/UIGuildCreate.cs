using System.Collections;
using System.Collections.Generic;
using Service;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildCreate : UIWindow
{

	public InputField inputName;
	public InputField inputIntroduce;

	void Start()
	{
		GuildService.Instance.OnGuildCreateResult += OnGuildCreate;
	}

	void OnDestroy()
	{
		GuildService.Instance.OnGuildCreateResult -= OnGuildCreate;
	}

	

	public override void OnYesClick()
	{
		if (string.IsNullOrEmpty(inputName.text))
		{
			MessageBox.Show("请输入公会名称", "错误", MessageBoxType.Error);
			return;
		}

		if (inputName.text.Length < 3 || inputName.text.Length > 10)
		{
			MessageBox.Show("公会名称必须为3-10个字符", "错误", MessageBoxType.Error);
			return;
		}

		if (string.IsNullOrEmpty(inputIntroduce.text))
		{
			MessageBox.Show("请输入公会宣言", "错误", MessageBoxType.Error);
			return;
		}

		if (inputIntroduce.text.Length < 3 || inputIntroduce.text.Length > 50)
		{
			MessageBox.Show("公会名称必须为3-50个字符", "错误", MessageBoxType.Error);
			return;
		}
		GuildService.Instance.SendGuildCreate(inputName.text, inputIntroduce.text);
		
	}
	
	private void OnGuildCreate(bool result)
	{
		if(result)
			this.Close(WindowResult.Yes);
	}
}

