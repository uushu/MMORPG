using System;
using System.Collections;
using System.Collections.Generic;
using Candlelight.UI;
using Manager;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIChat : MonoBehaviour {

	public HyperText chatArea;
	public TabView chatTab;
	public Dropdown chatChannel;
	public InputField chatInput;
	public Text chatTarget;


	private void Start()
	{
		this.chatTab.OnTabSelect += OnDisplayChannelSelect;
		ChatManager.Instance.OnChat += RefreshUI;
	}

	private void OnDestroy()
	{
		this.chatTab.OnTabSelect -= OnDisplayChannelSelect;
		ChatManager.Instance.OnChat -= RefreshUI;
	}

	private void Update()
	{
		InputManager.Instance.IsInputMode = chatInput.isFocused;
	}

	
	private void OnDisplayChannelSelect(int channelId)
	{
		ChatManager.Instance.displayChannel = (ChatManager.Channel)channelId;
		RefreshUI();
	}
	
	
	private void RefreshUI()
	{
		this.chatArea.text = ChatManager.Instance.GetCurrentMessage();
		this.chatChannel.value = (int)ChatManager.Instance.sendChannel - 1;
		if(ChatManager.Instance.SendChannel == ChatChannel.Private)
		{
			this.chatTarget.gameObject.SetActive(true);
			if(ChatManager.Instance.PrivateID != 0 )
				this.chatTarget.text = ChatManager.Instance.PrivateName + ":";
			else
				this.chatTarget.text = "<无>";
		}
		else
		{
			this.chatTarget.gameObject.SetActive(false);
		}
		
	}

	public void OnClickChatLink(HyperText text, HyperText.LinkInfo link)
	{
		if (string.IsNullOrEmpty(link.Name))
			return;
		// 样式
		// <a name="c:ID:Name" class="player">Name</a>
		
		if(link.Name.StartsWith("c:"))
		{
			string[] strs = link.Name.Split(":".ToCharArray());
			UIPopCharMenu menu = UIManager.Instance.Show<UIPopCharMenu>();
			menu.targetId = int.Parse(strs[1]);
			menu.targetName = strs[2];
		}
	}
	
	public void OnClickSend()
	{
		OnEndInput(this.chatInput.text);
	}

	public void OnEndInput(string text)
	{
		if (!string.IsNullOrEmpty(text.Trim()))
			this.SendChat(text);
		this.chatInput.text = "";
	}

	private void SendChat(string text)
	{
		ChatManager.Instance.SendChat(text,ChatManager.Instance.PrivateID,ChatManager.Instance.PrivateName);
	}
	
	public void OnSendChannelChanged(int index)
	{
		if (ChatManager.Instance.sendChannel == (ChatManager.Channel)(index + 1))
			return;
		if(!ChatManager.Instance.SetSendChannel((ChatManager.Channel)(index + 1)))
		{
			this.chatChannel.value = (int)ChatManager.Instance.sendChannel - 1;
		}
		else
			RefreshUI();
	}
}
