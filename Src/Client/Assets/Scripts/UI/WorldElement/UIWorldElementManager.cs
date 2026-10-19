using System.Collections;
using System.Collections.Generic;
using Entities;
using Manager;
using UnityEngine;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager> {

	public GameObject elementPrefab;
	public GameObject npcStatusPrefab;
	
	public Dictionary<Transform, GameObject> elementNames = new Dictionary<Transform, GameObject>();

	public Dictionary<Transform, GameObject> elementStatus = new Dictionary<Transform, GameObject>();


	public void AddCharNameBar(Transform owner,Character character)
	{
		GameObject goNameBar = Instantiate(elementPrefab,this.transform);
		goNameBar.name = "NameBar" + character.entityId;
		goNameBar.GetComponent<UIWorldElement>().owner = owner;
		goNameBar.GetComponent<UINameBar>().character = character;
		goNameBar.SetActive(true);
		this.elementNames[owner] = goNameBar;
	}
	public void RemoveCharNameBar(Transform owner)
	{
		if (this.elementNames.ContainsKey(owner))
		{
			Destroy(this.elementNames[owner]);
			this.elementNames.Remove(owner);
		}
	}
	
	public void AddNpcQuestStatus(Transform owner,NpcQuestStatus status)
	{
		if (this.elementStatus.ContainsKey(owner))
		{
			elementStatus[owner].GetComponent<UIQuestStatus>().SetQuestStatus(status);
		}
		else
		{
			GameObject goStatus = Instantiate(npcStatusPrefab,this.transform);
			goStatus.name = "NpcQustStatus" + owner.name;
			goStatus.GetComponent<UIWorldElement>().owner = owner;
			goStatus.GetComponent<UIQuestStatus>().SetQuestStatus(status);
			goStatus.SetActive(true);
			this.elementStatus[owner] = goStatus;
		}
	}
	public void RemoveNpcStatus(Transform owner)
	{
		if (this.elementStatus.ContainsKey(owner))
		{
			Destroy(this.elementStatus[owner]);
			this.elementStatus.Remove(owner);
		}
	}
	
}
