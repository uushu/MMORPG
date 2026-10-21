using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIInputBox : MonoBehaviour {

	public Text title;
	public Text message;
	public Text tips;
	public InputField input;
	public Button btnYes;
	public Button btnNo;
	

	public Text btnYesTitle;
	public Text btnNoTitle;

	public delegate bool SubmitHandler(string inputText, out string tips);

	public event SubmitHandler OnSubmit;
	
	public UnityAction OnCancel;
	private string emptyTips;

	void Start()
	{
            
	}

	void Update()
	{
            
	}

	public void Init(string title, string message,  string btnOK = "", string btnCancel = "" , string emptyTips = "")
	{
		if (!string.IsNullOrEmpty(title)) this.title.text = title;
		this.message.text = message;
		this.tips.text = null;
		this.OnSubmit = null;
		this.emptyTips = emptyTips;
		
		if (!string.IsNullOrEmpty(btnOK)) this.btnYesTitle.text = btnOK;
		if (!string.IsNullOrEmpty(btnCancel)) this.btnNoTitle.text = btnCancel;

		this.btnYes.onClick.AddListener(OnClickYes);
		this.btnNo.onClick.AddListener(OnClickNo);

		
		// if (type == MessageBoxType.Error)
		//     SoundManager.Instance.PlaySound(SoundDefine.SFX_Message_Error);
		// else
		//     SoundManager.Instance.PlaySound(SoundDefine.SFX_Message_Info);
	}


	void OnClickYes()
	{
		//SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Confirm);
		this.tips.text = "";
		if (string.IsNullOrEmpty(this.input.text))
		{
			this.tips.text = this.emptyTips;
            return;
		}

		if (this.OnSubmit != null)
		{
			string tips;
			if(!OnSubmit(this.input.text, out tips))
			{
				this.tips.text = tips;
				return;
			}
		}
		Destroy(this.gameObject);
	}
	void OnClickNo()
	{
		//SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Close);
		Destroy(this.gameObject);
		if (this.OnCancel != null)
			this.OnCancel();
	}
	
	
}
