using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;
using UnityEngine.UI;

public class UINameBar : MonoBehaviour {
	public Text nameText;
	public Image image;
	
	public Character character;

	void Start()
	{
		
	}

	void Update()
	{
		UpdateInfo();
	}

    void UpdateInfo()
    {
        if (this.character != null)
        {
            string name = this.character.Name + " Lv." + this.character.Info.Level;
            if(name != this.nameText.text)
            {
                nameText.text = name;
            }
        }
    }
}
