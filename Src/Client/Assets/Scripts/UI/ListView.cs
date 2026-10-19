using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


// 可序列化特性，允许该类在Unity编辑器中被序列化
[Serializable] 

public class ItemSelectevent : UnityEvent<ListView.ListViewItem> 
{
}

public class ListView : MonoBehaviour
{

	// 当列表项被选中时触发的事件
	public UnityAction<ListViewItem> onItemSelected ;
	
	// 当列表项被点击时触发的事件
	public event Action<ListViewItem> ItemClicked;
	
	public class ListViewItem : MonoBehaviour, IPointerClickHandler
	{
		private bool selected; 
		public bool Selected
		{
			get { return selected; }
			set
			{
				if(selected != value)
				{
					selected = value;
					OnSelected(selected);
				}
			}
		}
		
		// 对外部的ListView的引用
		public ListView owner;

		// 虚拟方法：处理选中状态变化
		public virtual void OnSelected(bool selected)
		{

		}
		
		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;
			if (owner == null)
				return;

			owner.SelectedItem = this;
			owner.ItemClicked?.Invoke(this);
		}
	}
	
	
	List<ListViewItem> items = new List<ListViewItem>();
	private ListViewItem selectedItem;
	public ListViewItem SelectedItem
	{
		get { return selectedItem; }

		set
		{
			if (selectedItem == value)
				return;
			
			if (selectedItem != null)
				selectedItem.Selected = false;
			
			selectedItem = value;
			
			if(selectedItem != null)
			{
				selectedItem.Selected = true;
				onItemSelected?.Invoke(selectedItem);
			}

		}
	}

	public void AddItem(ListViewItem item)
	{
		item.owner = this;
		this.items.Add(item);

	}
	
	public void RemoveAll()
	{
		ClearSelection();
		foreach (var it in items)
		{
			Destroy(it.gameObject);
		}
		items.Clear();
	}

	public void ClearSelection()
	{
		this.SelectedItem = null;
	}
}

