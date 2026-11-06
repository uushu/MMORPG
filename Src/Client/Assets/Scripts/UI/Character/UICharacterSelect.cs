using System.Collections;
using System.Collections.Generic;
using Models;
using Service;
using SkillBridge.Message;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterSelect : MonoBehaviour
{

	public GameObject createPanel;
	public GameObject selectPanel;
	
	public Button selectBtn;
	public InputField charName;
	private CharacterClass charClass;

	public Transform uiCharList;
	public GameObject uiCharInfo;

	public ListView listView;
	public Text describ;

	private int selectCharacterIdx = -1;
	
	public UICharacterView characterView;


	void Start()
	{
		this.listView.onItemSelected += OnCharacterSelected;
		
		InitCharacterSelect(true);
		UserService.Instance.OnCharacterCreate = OnCharacterCreate;
	}

	void OnDestroy()
	{
		this.listView.onItemSelected -= OnCharacterSelected;

	}

	private void OnCharacterSelected(ListView.ListViewItem item)
	{
		UICharInfo character = (UICharInfo)item;
		OnSelectCharacter(character.characterIdx);
	}

	public void InitCharacterCreate()
	{
		createPanel.SetActive(true);
		selectPanel.SetActive(false);
		charName.text = string.Empty;
		selectBtn.onClick.Invoke();
		//OnSelectClass(1);
	}

	void Update()
	{
		
	}

	public void OnClickCreate()
	{
		if (string.IsNullOrEmpty(this.charName.text))
		{
			MessageBox.Show("请输入角色昵称！");
			return;
		}
		

		SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
		UserService.Instance.SendCharacterCreate(this.charName.text,this.charClass);
	}
    public void InitCharacterSelect(bool init)
    {
        createPanel.SetActive(false);
        selectPanel.SetActive(true);

        if (!init)
	        return;
        this.listView.RemoveAll();
        this.selectCharacterIdx = -1;

        UICharInfo firstItem = null;
        
        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {

	        GameObject go = Instantiate(uiCharInfo, this.uiCharList);
	        UICharInfo item = go.GetComponent<UICharInfo>();
	        item.info = User.Instance.Info.Player.Characters[i];
	        item.characterIdx = i;
	        item.OnSelected(false);
	        this.listView.AddItem(item);
	        go.SetActive(true);
	        
	        if( i==0 )
		        firstItem = item;
        }


		this.listView.SelectedItem = firstItem;
    }
    
    /// <summary>
    /// 选择职业
    /// </summary>
    /// <param name="charClass"></param>
    public void OnSelectClass(int charClass)
    {
	    SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
	    this.charClass = (CharacterClass)charClass;

	    characterView.CurrentCharacter = charClass - 1;

	    for (int i = 0; i < 3; i++)
	    {
		    // titles[i].gameObject.SetActive(i == charClass - 1);
		    // names[i].text = DataManager.Instance.Characters[i + 1].Name;
	    }

	    describ.text = DataManager.Instance.Characters[charClass].Description;

    }


    void OnCharacterCreate(Result result, string message)
    {
        if (result == Result.Success)
        {
            InitCharacterSelect(true);

        }
        else
            MessageBox.Show(message, "错误", MessageBoxType.Error);
    }




    public void OnSelectCharacter(int idx)
    {
        this.selectCharacterIdx = idx;
        var cha = User.Instance.Info.Player.Characters[idx];
        Debug.LogFormat("Select Char:[{0}]{1}[{2}]", cha.Id, cha.Name, cha.Class);
        characterView.CurrentCharacter = ((int)cha.Class - 1);
        
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
    }
    public void OnClickPlay()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        if (selectCharacterIdx >= 0)
        {
            UserService.Instance.SendGameEnter(selectCharacterIdx);
        }
    }
	

}
