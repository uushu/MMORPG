using Manager;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestStatus : MonoBehaviour
{
    public Image[] statusIm;
    private NpcQuestStatus status;
    public void SetQuestStatus(NpcQuestStatus status)
    {
        this.status = status;
        for (int i = 0; i < 4; i++)
        {
            if(this.statusIm[i] !=null)
                this.statusIm[i].gameObject.SetActive(( i== (int)status));
        }
    }
}