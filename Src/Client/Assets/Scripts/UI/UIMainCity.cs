using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;
using UnityEngine.UI;

public class UIMainCity : MonoBehaviour
{
	public Text name;
	public Text level;

	void Start()
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
	
}
