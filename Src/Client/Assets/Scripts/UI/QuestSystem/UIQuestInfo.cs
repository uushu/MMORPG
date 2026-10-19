using System.Collections;
using System.Collections.Generic;
using Assets;
using Common.Data;
using Models;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestInfo : MonoBehaviour {

	public Text title;
	public Text describe;
	public Text[] targets;
	public Text rewardMoney;
	public Text rewardExp;

	public Image[] rewardItems;
	

	public void SetQuestInfo(Quest quest)
	{
		var define = quest.Define;
		// 初始化任务描述信息
		this.title.text = string.Format("[{0} {1}]",define.Type,define.Name);
		if (quest.Info != null && (quest.Info.Status == QuestStatus.Completed || quest.Info.Status == QuestStatus.Finished))
		{
			this.describe.text = define.DialogFinish;
		}
		else
		{
			this.describe.text = define.Dialog;
		}
		
		// 初始化目标信息
		var targetTypes = new[] {define.Target1, define.Target2, define.Target3};
		var targetIDs = new[] {define.Target1ID, define.Target2ID, define.Target3ID};
		var targetNums = new[] {define.Target1Num, define.Target2Num, define.Target3Num};
		for (int i = 0; i < targets.Length; i++)
		{
			if ( targetTypes[i] != QuestTarget.None)
			{
				targets[i].gameObject.SetActive(true);
				targets[i].text = string.Format("{0} {1} {2}", targetTypes[i], targetIDs[i], targetNums[i]);
			}
			else
			{
				targets[i].gameObject.SetActive(false);
			}
		}
		
		this.rewardMoney.text = string.Format("金币：{0}", define.RewardGold);
		this.rewardExp.text = string.Format("经验：{0}", define.RewardExp);

		int[] rewardItemIDs =
		{
			define.RewardItem1, define.RewardItem2, define.RewardItem3
		};
		int[] rewardItemCount =
		{
			define.RewardItem1Count, define.RewardItem2Count, define.RewardItem3Count
		};
		for (int i = 0; i < this.rewardItems.Length; i++)
		{
			if (rewardItemCount[i] > 0)
			{
				
				Image icon = rewardItems[i].transform.Find("Icon").GetComponent<Image>();
				Text num = rewardItems[i].GetComponentInChildren<Text>(true);
				ItemDefine item;
				if (DataManager.Instance.Items.TryGetValue(rewardItemIDs[i], out item))
					icon.overrideSprite = Resloader.Load<Sprite>(item.Icon);
				num.text = "x" + rewardItemCount[i];
				rewardItems[i].gameObject.SetActive(true);
			}
			else
			{
				rewardItems[i].gameObject.SetActive(false);
			}
		}
		foreach (var fitter in this.GetComponentsInChildren<ContentSizeFitter>())
		{
			fitter.SetLayoutVertical();
		}
	}
}
