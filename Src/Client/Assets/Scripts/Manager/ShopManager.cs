using Common.Data;
using Service;

namespace Manager
{
    public class ShopManager : Singleton<ShopManager>
    {
        public void Init()
        {
            NPCManager.Instance.RegisterNPCEvent(NPCFunction.InvokeShop , OpenShop);
        }

        private bool OpenShop(NPCDefine npc)
        {
            this.ShowShop(npc.Param);
            return true;
        }

        private void ShowShop(int npcShopID)
        {
            ShopDefine shop;
            if (DataManager.Instance.Shops.TryGetValue(npcShopID, out shop))
            {
                UIShop uiShop = UIManager.Instance.Show<UIShop>();
                if(uiShop != null)
                {
                    uiShop.SetShop(shop);
                }
            }
        }

        public bool BuyItem(int shopID, int shopItemID)
        {
            ItemService.Instance.SendBuyItem(shopID, shopItemID);
            return true;
        }
    }
}