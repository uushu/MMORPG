using System.Collections;
using System.Collections.Generic;
using Common.Data;
using Manager;
using Models;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestSystem : UIWindow
{

	public Text title;
	public GameObject itemPrefab;
	public ListView listMain;
	public ListView listBranch;
	public UIQuestInfo questInfo;
	public TabView Tabs;

	private bool showAvailableList = true;

	void Start()
	{
		this.listMain.onItemSelected += OnQuestSelected;
		this.listBranch.onItemSelected += OnQuestSelected;
		this.Tabs.OnTabSelect += OnSelectTab;
		RefreshUI();

	}


	void OnDestroy()
	{
		this.listMain.onItemSelected -= OnQuestSelected;
		this.listBranch.onItemSelected -= OnQuestSelected;
		this.Tabs.OnTabSelect -= OnSelectTab;
	}
	void OnQuestSelected(ListView.ListViewItem item)
	{
		
		UIQuestItem questItem = item as UIQuestItem;
		if(questItem == null) return;
		if(item.owner == this.listMain) this.listBranch.ClearSelection();
		else this.listMain.ClearSelection();
		this.questInfo.gameObject.SetActive(true);
		this.questInfo.SetQuestInfo(questItem.quest);
	}
	private void OnSelectTab(int idx)
	{
		this.showAvailableList = idx == 0;
		RefreshUI();
	}
	
	private void RefreshUI()
	{
		ClearAllQuestList();
		InitAllQuestList();
	}
	

	private void ClearAllQuestList()
	{
		this.questInfo.gameObject.SetActive(false);
		this.listMain.RemoveAll();
		this.listBranch.RemoveAll();
	}
	private void InitAllQuestList()
	{
		foreach (var kv in QuestManager.Instance.allQuests)
		{
			if (showAvailableList)
			{
				if (kv.Value.Info != null)
				{
					continue;
				}
			}
			else
			{
				if (kv.Value.Info == null)
				{
					continue;
				}
			}
			GameObject gp = Instantiate(itemPrefab,kv.Value.Define.Type == QuestType.Main ? this.listMain.transform : this.listBranch.transform);
			UIQuestItem item = gp.GetComponent<UIQuestItem>();
			item.SetQuestInfo(kv.Value);
			if(kv.Value.Define.Type == QuestType.Main)
				this.listMain.AddItem(item);
            else
	            this.listBranch.AddItem(item);
			
				
		}
	}
}
