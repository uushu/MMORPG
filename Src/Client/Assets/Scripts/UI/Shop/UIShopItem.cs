using Assets;
using Common.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;


public class UIShopItem : ListView.ListViewItem,ISelectHandler
{

	public Image icon;
	public Text name;
	public Text count;
	public Text price;
	public Text limitClass;
	public Image backGround;
	public Sprite normalBg;
	public Sprite selectedBg;
	
	private ItemDefine item;
	private ShopItemDefine ShopItem { get; set; }
	public int ShopItemID { get; set; }


	public override void OnSelected(bool selected)
	{
		this.backGround.overrideSprite = selected ? selectedBg : normalBg;
	}

	public void SetShopItem(int shopItemID, ShopItemDefine shopItemDefine)
	{
	
		this.ShopItemID = shopItemID;
		this.ShopItem = shopItemDefine;
		this.item = DataManager.Instance.Items[this.ShopItem.ItemID];
		
		this.name.text = this.item.Name;
		this.count.text = this.ShopItem.Count.ToString();
		this.price.text = this.ShopItem.Price.ToString();
		this.limitClass.text = this.item.LimitClass.ToString();
		this.icon.overrideSprite = Resloader.Load<Sprite>(item.Icon);

	}
	


	public void OnSelect(BaseEventData eventData)
	{
		if(this.owner != null)
			this.owner.SelectedItem = this;
	}
}
