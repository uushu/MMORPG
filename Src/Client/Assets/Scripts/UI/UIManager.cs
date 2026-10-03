using System;

using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{

	class UIElement
	{
		public string Resources; // UI资源路径
		public bool Cache; // 是否缓存
		public GameObject Instance; // UI实例
	}

	private Dictionary<Type, UIElement> uiElements = new Dictionary<Type, UIElement>();

	public UIManager()
	{
		this.uiElements.Add(typeof(UITest),new UIElement(){ Resources = "UI/UITest", Cache = true});
	}

	~UIManager()
	{
		// foreach (var uiElement in uiElements.Values)
		// {
		// 	if (uiElement.Cache && uiElement.Instance != null)
		// 	{
		// 		GameObject.Destroy(uiElement.Instance);
		// 	}
		// }
	}
	
	
	public T Show<T>()
	{
		// UI弹出音效
		Type type = typeof(T);
		if (this.uiElements.ContainsKey(type))
		{
			UIElement uiElement = this.uiElements[type];
			if (uiElement.Instance != null)
			{
				uiElement.Instance.SetActive(true);
			}
			else
			{
				UnityEngine.Object prefab =Resources.Load(uiElement.Resources);
				if (prefab == null)
				{
					return default(T);
				}
				uiElement.Instance = GameObject.Instantiate(prefab) as GameObject;
				uiElement.Instance.transform.SetParent(UIMain.Instance.transform, false);
				uiElement.Instance.transform.SetAsLastSibling();
			}
			return uiElement.Instance.GetComponent<T>();
		}
		return default(T);
	}

	public void Close (Type type)
	{
		if (this.uiElements.ContainsKey(type))
		{
			UIElement uiElement = this.uiElements[type];
			if (uiElement.Cache)
			{
				uiElement.Instance.SetActive(false);
			}
			else
			{
				GameObject.Destroy(uiElement.Instance);
				uiElement.Instance = null;
			}
		}
	}
	
	
}
