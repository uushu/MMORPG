using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using Models;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIEquip : UIWindow
{

	public Text title;
	public Text money;
	public GameObject itemPrefab;
	public GameObject itemEquipedPrefab;
	public Transform ItemListRoot;

	public List<Transform> slots;

	private UIEquipItem selectedEquipItem;
	private UIEquipItem lastSelectedItem;
	private float lastClickTime;
	private float clickWindow = .2f;
	
	void Start()
	{
		RefrashUI();
		EquipManager.Instance.OnEquipChanged += RefrashUI;
	}

	private void OnDestroy()
	{
		EquipManager.Instance.OnEquipChanged -= RefrashUI;
	}
	
	

	private void RefrashUI()
	{
		ClearAllEquipList();
		InitAllEquipList();
		ClearAllEquiped();
		InitAllEquiped();
		selectedEquipItem = null;
		this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
	}

	public void SelectEquipItem(UIEquipItem item)
	{
		if (selectedEquipItem != null && selectedEquipItem !=item)
			selectedEquipItem.Selected = false;
		selectedEquipItem = item;
		selectedEquipItem.Selected = true;
	}
	public void OnClickEquipItem(UIEquipItem item)
	{
		if (item == null) return;
		SelectEquipItem(item);
		bool isClicked = lastSelectedItem == item && (Time.unscaledTime- lastClickTime) <= clickWindow;

		lastClickTime = Time.unscaledTime;
		lastSelectedItem = item;

		if (isClicked)
		{
			item.Equip();
			lastSelectedItem = null;
		}
	}


	/// <summary>
	/// 初始化所有装备列表
	/// </summary>
	private void InitAllEquipList()
	{
		foreach (var kv in ItemManager.Instance.Items)
		{
			if (kv.Value.Define.Type == ItemType.Equip && kv.Value.Define.LimitClass == User.Instance.CurrentCharacter.Class)
			{
				if(EquipManager.Instance.Contain(kv.Value.Id))
					continue;
                GameObject go = GameObject.Instantiate(itemPrefab, ItemListRoot);
                UIEquipItem ui = go.GetComponent<UIEquipItem>();
                ui.SetEquiItem(kv.Key, kv.Value, this, false);
			}
		}
	}

	private void ClearAllEquipList()
	{
		foreach (var item in ItemListRoot.GetComponentsInChildren<UIEquipItem>())
		{
			Destroy(item.gameObject);
		}
	}
	private void ClearAllEquiped()
	{
		foreach (var item in slots)
		{
			if(item.childCount > 1)
				Destroy(item.GetChild(1).gameObject);
		}
	}
	
	/// <summary>
	/// 初始化已装备列表
	/// </summary>
	private void InitAllEquiped()
	{
		for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
		{
			var item = EquipManager.Instance.Equips[i];
			if (item != null)
			{
				GameObject go = GameObject.Instantiate(itemEquipedPrefab, slots[i]);
				UIEquipItem ui = go.GetComponent<UIEquipItem>();
				ui.SetEquiItem(item.Id, item, this, true);
			}
		}
	}

	public void DoEquip(Item item)
	{
		EquipManager.Instance.EquipItem(item);
	}
	
	public void UnEquip(Item item)
	{
		EquipManager.Instance.UnEquipItem(item);
	}
	
}
