using System;
using Models;
using SkillBridge.Message;
using UnityEngine;

namespace Manager
{
    public class BagManager : Singleton<BagManager>
    {
        public int UnLocked;
        public BagItem[] Items;

        private NBagInfo Info;

        unsafe public void Init(NBagInfo info)
        {
            this.Info = info;
            this.UnLocked = info.unLocked;
            this.Items = new BagItem[this.UnLocked];

            if (info.Items != null && info.Items.Length >= sizeof(BagItem)*this.UnLocked)
            {
                AnalyzeNBagInfo(info.Items);
            }
            else
            {
                Info.Items =new byte[sizeof(BagItem)*this.UnLocked];
                Reset();
            }
        }


        /// <summary>
        /// 重置物品数组，并根据物品的堆叠限制进行合理的堆叠分配
        /// </summary>
        unsafe public  void Reset()
        {
            Array.Clear(this.Items, 0, this.Items.Length);
            int i = 0;
            foreach (var kv in ItemManager.Instance.Items)
            {
                int count = kv.Value.Count;
                if ( count > kv.Value.Define.StackLimit)
                {
                    while (count > kv.Value.Define.StackLimit)
                    {
                        this.Items[i].ItemId = (ushort)kv.Key;
                        this.Items[i].Count = (ushort)kv.Value.Define.StackLimit;
                        i++;
                        count -= kv.Value.Define.StackLimit;
                    }
                }
                this.Items[i].ItemId = (ushort)kv.Key;
                this.Items[i].Count = (ushort)count;
                i++;
            }
        }

        /// <summary>
        /// 字节数组 -> 结构体数组
        /// </summary>
        /// <param name="data"></param>
        unsafe void AnalyzeNBagInfo(byte[] data)
        {
            fixed (byte* pt = data)
            {
                for (int i = 0; i < this.UnLocked; i++)
                {
                    BagItem* bi= (BagItem*)(pt+i*sizeof(BagItem));
                    this.Items[i] = *bi;
                }
            }
        }

        unsafe public NBagInfo GetNBagInfo()
        {
            fixed (byte* pt = Info.Items)
            {
                for (int i = 0; i < this.UnLocked; i++)
                {
                    BagItem* bi = (BagItem*)(pt+i*sizeof(BagItem));
                    *bi =this.Items[i];
                }
            }
            return this.Info;
        }

        public void AddItem(int itemId, int count)
        {
            ushort addCount = (ushort)count;
            
            for (int i = 0; i < Items.Length; i++)
            {
                if (this.Items[i].ItemId == itemId)
                {
                    ushort canAdd = (ushort)(DataManager.Instance.Items[itemId].StackLimit - this.Items[i].Count);
                    if (canAdd >= addCount)
                    {
                        this.Items[i].Count += addCount;
                        addCount = 0;
                        break;
                    }
                    else
                    {
                        this.Items[i].Count += canAdd;
                        addCount -= canAdd;
                    }
                }
            }

            if (addCount > 0)
            {
                 int stackLimit = DataManager.Instance.Items[itemId].StackLimit;
                for (int i = 0; i < Items.Length; i++)
                {
                    if (this.Items[i].ItemId == 0)
                    {
                        ushort canAdd = (ushort)Mathf.Min(stackLimit, addCount);
                        this.Items[i].ItemId = (ushort) itemId;
                        this.Items[i].Count = canAdd;
                        addCount -= canAdd;
                        if (addCount <= 0)
                            break;
                    }
                }
            }
        }

        public void RemoveItem(int itemId, int count)
        {
            
        }
    }
}