using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using Models;
using Service;
using UI;
using UnityEngine;

public class UIFriend : UIWindow {

	public GameObject friendPrefab;
	public ListView list;
	public Transform root;
	public UIFriendItem SelectItem { get; set; }

	void Start()
	{
		FriendService.Instance.OnFriendUpdate = RefreshUI;
		this.list.onItemSelected += this.OnFriendSelected;
		RefreshUI();
	}

	private void OnDestroy()
	{
		list.onItemSelected -= this.OnFriendSelected;
		FriendService.Instance.OnFriendUpdate -= RefreshUI;
	}

	private void OnFriendSelected(ListView.ListViewItem item)
	{
		this.SelectItem = item as UIFriendItem;
	}


	
	public void OnClickFriendAdd()
	{
		InputBox.Show("请输入要添加的用户名称或ID", "添加好友").OnSubmit += OnFriendAddSubmit;
	}

	private bool OnFriendAddSubmit(string inputText, out string tips)
	{
		tips = "";
		int friendId = 0;
		string friendName = "";
		if(!int.TryParse(inputText, out friendId))
			friendName = inputText;

		if (friendId == User.Instance.CurrentCharacter.Id || friendName == User.Instance.CurrentCharacter.Name)
		{
			tips = "不能添加自己为好友";
			return false;
		}
		FriendService.Instance.SendFriendAddRequest(friendId, friendName);
        return true;
	}
	
	
	public void OnClickFriendChat()
	{
		MessageBox.Show("暂未开放");
	}
	
	public void OnClickFriendRemove()
	{
		if (SelectItem == null)
		{
			MessageBox.Show("请选择要删除的好友");
			return;
		}

		MessageBox.Show(string.Format("确定删除好友{0}吗？", SelectItem.Info.friendInfo.Name), "删除好友", MessageBoxType.Confirm,
				"删除", "取消").OnYes =
			() =>
			{
				FriendService.Instance.SendFriendRemoveRequest(SelectItem.Info.Id, SelectItem.Info.friendInfo.Id);
			};
	}

	void RefreshUI()
	{
		ClearFriendList();
		InitFriendItems();
	}
	

	private void ClearFriendList()
	{
		this.list.RemoveAll();
	}
	private void InitFriendItems()
	{
		foreach (var friend in FriendManager.Instance.allFriends)
		{
			GameObject go = Instantiate(friendPrefab,list.transform);
			UIFriendItem item = go.GetComponent<UIFriendItem>();
			item.SetFriendInfo(friend);
			this.list.AddItem(item);
		}
	}
	
}
