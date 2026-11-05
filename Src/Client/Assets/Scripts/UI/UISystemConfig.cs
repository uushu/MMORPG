using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystemConfig : UIWindow
{

	public Image musicOn;
	public Image musicOff;
	
	public Image soundOn;
	public Image soundOff;
	
	public Toggle musicToggle;
	public Toggle soundToggle;
	
	public Slider musicSlider;
	public Slider soundSlider;


	private float lastPlay = 0;
	void Start()
	{
		this.musicOn.gameObject.SetActive(Config.MusicOn);
		this.musicOff.gameObject.SetActive(!Config.MusicOn);
		this.soundOn.gameObject.SetActive(Config.SoundOn);
		this.soundOff.gameObject.SetActive(!Config.SoundOn);
		
		this.musicToggle.isOn = Config.MusicOn;
		this.soundToggle.isOn = Config.SoundOn;
		this.musicSlider.value = Config.MusicVolume;
		this.soundSlider.value = Config.SoundVolume;
	}

	public void MusicToggle(bool isOn)
	{
		if(isOn)
        {
            musicOn.gameObject.SetActive(true);
            musicOff.gameObject.SetActive(false);
        }
        else
        {
            musicOn.gameObject.SetActive(false);
            musicOff.gameObject.SetActive(true);
        }
		Config.MusicOn = isOn;
		SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
	}
	
	public void SoundToggle(bool isOn)
	{
		if(isOn)
        {
            soundOn.gameObject.SetActive(true);
            soundOff.gameObject.SetActive(false);
        }
        else
        {
            soundOn.gameObject.SetActive(false);
            soundOff.gameObject.SetActive(true);
        }
		Config.SoundOn = isOn;
		SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
	}

	public override void OnYesClick()
	{
		base.OnYesClick();
		PlayerPrefs.Save();
		
	}

	public void MusicVolum(float value)
	{
		Config.MusicVolume = (int)value;
		PlaySound();
	}

	public void SoundVolum(float value)
	{
		Config.SoundVolume = (int)value;
		PlaySound();
	}

	private void PlaySound()
	{
		// 拉滑块的时候，每隔0.1秒播放声音看合不合适
		if (Time.realtimeSinceStartup - lastPlay > 0.1)
		{
			lastPlay = Time.realtimeSinceStartup;
			SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
		}
	}
}
