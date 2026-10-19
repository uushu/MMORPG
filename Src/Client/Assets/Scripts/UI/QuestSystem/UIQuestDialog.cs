using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;

public class UIQuestDialog : UIWindow {

	public UIQuestInfo questInfo;
	public Quest quest;

	public GameObject openBtn;
	public GameObject submitBtn;

	void Start()
	{
		
	}


	public void SetQuest(Quest quest)
	{
		this.quest = quest;
		this.UpdataQuest();
		if (this.quest.Info == null)
		{
			openBtn.SetActive(true);
			submitBtn.SetActive(false);
		}
		else
		{
			openBtn.SetActive(false);
			submitBtn.SetActive(true);
		}
	}

	private void UpdataQuest()
	{
		if (quest != null)
		{
			if (this.questInfo != null)
			{
				this.questInfo.SetQuestInfo(quest);
			}
		}
	}
}
