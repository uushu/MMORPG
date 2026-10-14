using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using Models;
using UnityEngine;
using UnityEngine.UI;

public class UIBag : UIWindow
{
    public GameObject tabPage1;
    public GameObject tabPage2;


    public Text pageText;
    public Text money;

    public Transform[] pages;
    public GameObject bagItem;
    private List<Image> slots;

    private void OnEnable()
    {
        RefrashUI();
    }

    void Start()
    {
        if (slots == null)
        {
            slots = new List<Image>();
            for (int page = 0; page < pages.Length; page++)
            {
                slots.AddRange(pages[page].GetComponentsInChildren<Image>(true));

            }
        }

        StartCoroutine(InitBags());
    }

    IEnumerator InitBags()
    {
        for (int i = 0; i < BagManager.Instance.Items.Length; i++)
        {
            var item = BagManager.Instance.Items[i];
            if (item.ItemId > 0)
            {
                GameObject go = Instantiate(bagItem, slots[i].transform);
                var ui = go.GetComponent<UIBagItem>();
                var def = ItemManager.Instance.Items[item.ItemId].Define;
                ui.SetMainIcon(def.Icon, item.Count.ToString());
            }
        }

        for (int i = BagManager.Instance.Items.Length; i < slots.Count; i++)
        {
            slots[i].color = Color.gray;
        }

        yield return null;
    }

    /// <summary>
    /// 背包重置逻辑
    /// </summary>
    void Clear()
    {
        foreach (var slot in slots)
        {
            if (slot.transform.childCount > 0)
            {
                Destroy(slot.transform.GetChild(0).gameObject);
            }
        }
    }

    /// <summary>
    /// 刷新背包数据
    /// </summary>
    public void RefrashUI()
    {
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
        if( slots == null)
            return;
        this.Clear();
        StartCoroutine(this.InitBags());
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


    public void OnClickSortBtn()
    {
        BagManager.Instance.Reset();
    }
}