using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.UI;

public class UIBagItem : MonoBehaviour
{

	public Image mainImage;
	private Image secondImage;

	public Text mainText;

	public void SetMainIcon(string icon, string name)
	{
		this.mainImage.overrideSprite = Resloader.Load<Sprite>(icon);
		this.mainText.text = name;
	}
}
