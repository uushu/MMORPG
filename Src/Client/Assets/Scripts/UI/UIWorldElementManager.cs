using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager> {

	public GameObject elementPrefab;
	
	public Dictionary<Transform, GameObject> element = new Dictionary<Transform, GameObject>();


	public void AddCharNameBar(Transform owner,Character character)
	{
		GameObject goNameBar = Instantiate(elementPrefab,this.transform);
		goNameBar.name = "NameBar" + character.entityId;
		goNameBar.GetComponent<UIWorldElement>().charOwner = owner;
		goNameBar.GetComponent<UINameBar>().character = character;
		goNameBar.SetActive(true);
		this.element[owner] = goNameBar;
	}
	public void RemoveCharNameBar(Transform owner)
	{
		if (this.element.ContainsKey(owner))
		{
			Destroy(this.element[owner]);
			this.element.Remove(owner);
		}
	}
	
}
