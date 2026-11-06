using System;
using System.Collections;
using System.Collections.Generic;
using Common.Data;
using Manager;
using Models;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : UIWindow
{

    public TabView tabs;
	
    
    public Text pageText;
    public Text money;
    public Text title;

    public Transform[] itemRoot;
    public GameObject shopItem;
    public ListView listView;
    
    private ShopDefine shop;
    private UIShopItem selectedItem;
    
    private int clickCount = 0;
    private float clickLastTime;
    private float clickWindow = .2f;
    private UIShopItem lastSelectedItem;
    

    private void OnEnable()
    {
        StatusService.Instance.GoldChanged += SetMoney;
    }

    private void OnDisable()
    {
        StatusService.Instance.GoldChanged -= SetMoney;
    }

    void Start()
    {
        this.tabs.OnTabSelect += OnPageChanged;
        
        this.listView.onItemSelected += OnShopItemSelected;
        this.listView.ItemClicked += OnShopItemClicked;
        
        StartCoroutine(InitItems());

    }

    private void OnDestroy()
    {
        this.tabs.OnTabSelect -= OnPageChanged;
        this.listView.onItemSelected -= OnShopItemSelected;
        this.listView.ItemClicked -= OnShopItemClicked;

    }

    // 选中商品
    private void OnShopItemSelected(ListView.ListViewItem item)
    {
        this.selectedItem = (UIShopItem) item;
    }
    //双击判断
    private void OnShopItemClicked(ListView.ListViewItem item)
    {
        OnClickShopItem((UIShopItem) item);
    }

    private void OnPageChanged(int index)
    {
        this.pageText.text = (index + 1) + "/" + this.tabs.tabPages.Length;
    }

    private IEnumerator InitItems()
    {
        int count = 0;
        // int page = 0;
        foreach (var kv in DataManager.Instance.ShopItems[shop.ID])
        {
            if (kv.Value.Status > 0)
            {
                int pageIndex = count / 20;
                GameObject go = Instantiate(shopItem,itemRoot[pageIndex]);
                UIShopItem ui = go.GetComponent<UIShopItem>();
                ui.SetShopItem(kv.Key, kv.Value);
                this.listView.AddItem(ui);
                count++;
                // if (count > 40)
                // {
                //     count = 0;
                //     page++;
                //     itemRoot[page].gameObject.SetActive(true);
                // }
            }
        }
        yield return null;
    }
   

    public void SetShop(ShopDefine shop)
    {
        this.shop = shop;
        this.title.text = shop.Name;
        SetMoney(User.Instance.CurrentCharacter.Gold);
    }

    private void SetMoney(long gold)
    {
        this.money.text = gold.ToString();
    }
    public void OnClickBuy()
    {
        if (this.selectedItem == null)
        {
            MessageBox.Show("请选择要购买的道具", "购买提示");
            return;
        }

        if (!ShopManager.Instance.BuyItem(this.shop.ID, this.selectedItem.ShopItemID))
        {
            
        }
    }
    public void OnClickShopItem(UIShopItem item)
    {
        if (clickCount > 1)
            clickCount = 1;
        if (clickCount == 1 && lastSelectedItem == item && Time.unscaledTime - clickLastTime < clickWindow)
        {
            OnClickBuy();
            clickCount = 0;
            lastSelectedItem = null;
            return;
            
        }
        
        clickCount++;
        clickLastTime = Time.unscaledTime;
        lastSelectedItem = item;


    }


    public void OnClickTabBtnL()
    {
        if(this.tabs.index >0)
            this.tabs.SelectTab(this.tabs.index - 1);
    }
    public void OnClickTabBtnR()
    {
        if(this.tabs.index < this.tabs.tabPages.Length - 1)
            this.tabs.SelectTab(this.tabs.index + 1);
    }
   
}
