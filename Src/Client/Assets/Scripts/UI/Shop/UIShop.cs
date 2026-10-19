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

    public GameObject tabPage1;
    public GameObject tabPage2;
	
    
    public Text pageText;
    public Text money;
    public Text title;

    public Transform[] itemRoot;
    public GameObject shopItem;
    
    private ShopDefine shop;
    private UIShopItem selectedItem;
    
    private int clickCount = 0;
    private float clickLastTime;
    private float clickWindow = .2f;
    private UIShopItem lastSelectedItem;

    private void OnEnable()
    {
        User.Instance.GoldChanged += SetMoney;
    }

    private void OnDisable()
    {
        User.Instance.GoldChanged -= SetMoney;
    }

    void Start()
    {
        StartCoroutine(InitItems());

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
                ui.SetShopItem(kv.Key, kv.Value, this);
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
    public void SelectShopItem(UIShopItem item)
    {
        if (selectedItem != null)
        {
            selectedItem.Selected = false;
        }

        selectedItem = item;
        selectedItem.Selected = true;
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
        SelectShopItem(item);
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
        if (tabPage2.activeSelf)
        {
            SelectTab(0);
        }
    }
    public void OnClickTabBtnR()
    {
        if (tabPage1.activeSelf)
        {
            SelectTab(1);
        }
    }

    private void SelectTab(int index)
    {
        if(tabPage1 == null || tabPage2 == null || pageText == null)
            return;
        
        bool firstSelected = index == 0;
        tabPage1.SetActive(firstSelected);
        tabPage2.SetActive(!firstSelected);

        pageText.text = firstSelected ? "1/2" : "2/2";

    }


   
}
