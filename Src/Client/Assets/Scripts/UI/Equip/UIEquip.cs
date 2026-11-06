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
	public ListView listView;


	private UIEquipItem selectedEquipItem;
	private UIEquipItem lastSelectedItem;
	private float lastClickTime;
	private float clickWindow = .2f;
	
	void Start()
	{
		this.listView.onItemSelected += OnEquipItemSelect;
		this.listView.ItemClicked += OnEquipItemClicked;
		RefrashUI();
		EquipManager.Instance.OnEquipChanged += RefrashUI;
	}

	private void OnDestroy()
	{
		this.listView.onItemSelected -= OnEquipItemSelect;
		this.listView.ItemClicked -= OnEquipItemClicked;

		EquipManager.Instance.OnEquipChanged -= RefrashUI;
	}

	private void OnEquipItemSelect(ListView.ListViewItem item)
	{
		this.selectedEquipItem = (UIEquipItem)item;
	}

	private void OnEquipItemClicked(ListView.ListViewItem item)
	{
		OnClickEquipItem((UIEquipItem)item);
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

	
	public void OnClickEquipItem(UIEquipItem item)
	{
		if (item == null) return;
		
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
                this.listView.AddItem(ui);
			}
		}
	}

	private void ClearAllEquipList()
	{
		this.listView.RemoveAll();
		
		this.selectedEquipItem = null;
		this.lastSelectedItem = null;
		this.lastClickTime= 0;
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
	
	private void ClearAllEquiped()
	{
		foreach (var item in slots)
		{
			if(item.childCount > 1)
				Destroy(item.GetChild(1).gameObject);
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
