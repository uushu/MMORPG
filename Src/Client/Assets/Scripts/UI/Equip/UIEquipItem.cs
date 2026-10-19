using System.Collections;
using System.Collections.Generic;
using Assets;
using Common.Data;
using Manager;
using Models;
using SkillBridge.Message;
using UI;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEquipItem : MonoBehaviour , IPointerClickHandler{

	public Image icon;
	public Text title;
	public Text level;
	public Text limitClass;
	public Text limitCategory;
	public Image backGround;
	public Sprite normalBg;
	public Sprite selectedBg;

	private bool selected;
	public bool Selected
	{
		get
		{
			return selected;
		}
		set
		{
			selected = value;
			this.backGround.overrideSprite = selected ? selectedBg : normalBg;
		}
	}
	
	public int EquipIndex { get; set; }
	private UIEquip owner;
	private Item item;
	private bool isEquiped = false;
	
	public void SetEquiItem(int idx, Item item, UIEquip uiEquip, bool isEquiped)
	{
		this.owner = uiEquip;
		this.EquipIndex = idx;
		this.item = item;
		this.isEquiped = isEquiped;
		
		if(this.title!=null) this.title.text = this.item.Define.Name;
		if(this.level!=null) this.level.text = this.item.Define.Level.ToString();
		if(this.limitClass!=null) this.limitClass.text=this.item.Define.LimitClass.ToString();
		if(this.limitCategory!=null) this.limitCategory.text=this.item.Define.Category.ToString();
		if (this.icon != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.item.Define.Icon);
		
	}
	
	

	
	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			if (this.isEquiped)
				UnEquip();
			else
				owner.OnClickEquipItem(this); 
		}
		
	}

	public void Equip()
	{
		DoEquip();
	}
	
	private void DoEquip()
	{
		var message = MessageBox.Show(string.Format("要穿上装备{0}吗？", this.item.Define.Name), "确认", MessageBoxType.Confirm);
		message.OnYes = () =>
		{
			var oldEquip = EquipManager.Instance.GetEquip(item.EquipInfo.Slot);
			if (oldEquip != null)
			{
				var me = MessageBox.Show(string.Format("要穿上装备{0}吗？", this.item.Define.Name), "确认", MessageBoxType.Confirm);
				me.OnYes = () =>
				{
					this.owner.DoEquip(this.item);
				};
			}
			else 
				this.owner.DoEquip(this.item);
		};
	}

	private void UnEquip()
	{
		var message = MessageBox.Show(string.Format("要脱下装备{0}吗？", this.item.Define.Name), "确认", MessageBoxType.Confirm);
		message.OnYes = () =>
		{

			owner.UnEquip(this.item);
		};
	}
}
