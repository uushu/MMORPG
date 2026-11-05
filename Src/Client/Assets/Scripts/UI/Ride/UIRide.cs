using System.Collections;
using System.Collections.Generic;
using Manager;
using Models;
using SkillBridge.Message;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIRide : UIWindow
{
	public Text description;
	public GameObject itemPrefab;
	public ListView listView;

	public UIRideItem selectedItem;
	void Start()
	{
		this.listView.onItemSelected += OnItemSelected;
		RefrashUI();
	}

	private void OnDestroy()
	{
		this.listView.onItemSelected -= OnItemSelected;
	}

	public void OnItemSelected(ListView.ListViewItem item)
	{
		this.selectedItem = item as UIRideItem;
		this.description.text = this.selectedItem.item.Define.Description;
	}

	private void RefrashUI()
	{
		ClearItem();
		InitItem();
	}
	private void InitItem()
	{
		foreach (var kv in ItemManager.Instance.Items)
		{
			if (kv.Value.Define.Type == ItemType.Ride && 
			    (kv.Value.Define.LimitClass == User.Instance.CurrentCharacter.Class|| kv.Value.Define.LimitClass == CharacterClass.None))
			{
				GameObject go = GameObject.Instantiate(itemPrefab, listView.transform);
				UIRideItem ui = go.GetComponent<UIRideItem>();
				ui.SetRideItem(kv.Value,this, false);
				this.listView.AddItem(ui);
			}
		}
	}

	
	private void ClearItem()
	{
		this.listView.RemoveAll();
	}

	public void DoRide()
	{
		if (this.selectedItem == null)
		{
			MessageBox.Show("请选择一个坐骑","提示");
			return;
		}
		User.Instance.Ride(this.selectedItem.item.Id);
	}
	

	
}
