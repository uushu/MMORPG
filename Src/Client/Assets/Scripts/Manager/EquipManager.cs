using Models;
using Service;
using SkillBridge.Message;

namespace Manager
{
    public class EquipManager : Singleton<EquipManager>
    {
        public delegate void OnEquipChangeHandler();
        public event OnEquipChangeHandler OnEquipChanged;
        public Item[] Equips = new Item[(int)EquipSlot.SlotMax];
        private byte[] data; // 装备数据

        public void Init(byte[] data)
        {
            this.data = data;
            this.ParseEquipData(data);
        }

        public bool Contain(int equipId)
        {
            for (int i = 0; i < Equips.Length; i++)
            {
                if(Equips[i]!=null&&Equips[i].Id==equipId)
                {
                    return true;
                }
            }

            return false;
        }

        public Item GetEquip(EquipSlot slot)
        {
            return Equips[(int)slot];
        }
        /// <summary>
        /// 字节数组 -> 结构体数组
        /// </summary>
        /// <param name="data"></param>
        unsafe void ParseEquipData(byte[] data)
        {
            fixed (byte* pt = this.data)
            {
                for (int i = 0; i < this.Equips.Length; i++)
                {
                    int itemId = *(int*)(pt + i * sizeof(int));
                    if(itemId>0)
                        Equips[i] = ItemManager.Instance.Items[itemId];
                    else
                        Equips[i] = null;
                }
            }
        }

        unsafe public byte[] GetEquipData()
        {
            fixed (byte* pt = this.data)
            {
                for (int i = 0; i < this.Equips.Length; i++)
                {
                    int* itemId = (int*)(pt + i * sizeof(int));
                    if (Equips[i] == null)
                        *itemId = 0;
                    else
                        *itemId = Equips[i].Id;
                }
            }

            return data;
        }
        public void EquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, true);
        }

        public void UnEquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, false);
        }
        
        public void OnEquipItem(Item equip)
        {
            if (this.Equips[(int)equip.EquipInfo.Slot] != null && Equips[(int)equip.EquipInfo.Slot].Id == equip.Id)
                return;
            this.Equips[(int)equip.EquipInfo.Slot] = ItemManager.Instance.Items[equip.Id];

            if (OnEquipChanged != null)
                OnEquipChanged();

        }

        public void OnUnEquipItem(EquipSlot slot)
        {
            if(this.Equips[(int)slot] != null)
            {
                this.Equips[(int)slot] = null;
                if (OnEquipChanged != null)
                    OnEquipChanged();
            }
        }
    }
}