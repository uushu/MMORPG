using Assets;
using Common.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;


public class UIShopItem : MonoBehaviour,ISelectHandler, IPointerClickHandler
{

	public Image icon;
	public Text name;
	public Text count;
	public Text price;
	public Text limitClass;
	public Image backGround;
	public Sprite normalBg;
	public Sprite selectedBg;

	private bool selected;
	private UIShop shop;
	private ItemDefine item;
	private ShopItemDefine ShopItem { get; set; }

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
	
	public int ShopItemID { get; set; }

	
	
	public void SetShopItem(int shopItemID, ShopItemDefine shopItemDefine, UIShop owner)
	{
		this.shop = owner;
		this.ShopItemID = shopItemID;
		this.ShopItem = shopItemDefine;
		this.item = DataManager.Instance.Items[this.ShopItem.ItemID];
		
		this.name.text = this.item.Name;
		this.count.text = this.ShopItem.Count.ToString();
		this.price.text = this.ShopItem.Price.ToString();
		this.limitClass.text = this.item.LimitClass.ToString();
		this.icon.overrideSprite = Resloader.Load<Sprite>(item.Icon);

	}
	
	public void OnPointerClick(PointerEventData eventData)
	{
        if(eventData.button == PointerEventData.InputButton.Left)
	        shop.OnClickShopItem(this);
	        
	}


	public void OnSelect(BaseEventData eventData)
	{
		this.Selected = true;
		this.shop.SelectShopItem(this);
	}
}
