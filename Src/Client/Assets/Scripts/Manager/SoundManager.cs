using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoSingleton<SoundManager>
{

	public AudioSource MusicAudioSource;
	public AudioSource SoundAudioSource;

	public AudioMixer audioMixer;
	
	const string MusicPath = "Music/";
	const string SoundPath = "Sound/";

	private bool musicOn;

	public bool MusicOn
	{
		get
		{
			return musicOn;
		}
		set
		{
			musicOn = value;
			this.MusicMute(!musicOn);
		}
	}
	
	private bool soundOn;

	public bool SoundOn
	{
		get
		{
			return soundOn;
		}
		set
		{
			soundOn = value;
			// 静音设置
			this.SoundMute(!soundOn);
		}
	}

	private int musicVolume;

	public int MusicVolume
	{
		get
		{
			return musicVolume;
		}
		set
		{
			if (musicVolume != value)
			{
				musicVolume = value;
				if(musicOn) this.SetVolume("MusicVolume", musicVolume);
			}
		}
	}
	private int soundVolume;

	public int SoundVolume
	{
		get { return soundVolume; }
		set
		{
			if (soundVolume != value)
			{
				soundVolume = value;
				if(soundOn) this.SetVolume("SoundVolume", soundVolume);
			}
		}
	}

	void Start()
	{
		this.MusicVolume = Config.MusicVolume;
		this.SoundVolume = Config.SoundVolume;
		this.MusicOn = Config.MusicOn;
		this.SoundOn = Config.SoundOn;
	}

	private void MusicMute(bool mute)
	{
		this.SetVolume("MusicVolume", mute ?0:musicVolume);
	}
	private void SoundMute(bool mute)
	{
		this.SetVolume("SoundVolume", mute ?0:soundVolume);
	}

	private void SetVolume(string volumeName, int value)
	{
		float volume = value * 0.5f - 50f;
		this.audioMixer.SetFloat(volumeName, volume);

	}

	public void PlayMusic(string musicName)
	{
		AudioClip clip = Resloader.Load<AudioClip>(MusicPath + musicName);
		if (clip == null)
		{
			Debug.LogWarningFormat("PlayMusic: {0} not found", musicName);
			return;
		}
		if(MusicAudioSource.isPlaying)
			MusicAudioSource.Stop();
		MusicAudioSource.clip = clip;
		MusicAudioSource.loop = true;
		MusicAudioSource.Play();
	}

	public void PlaySound(string soundName)
	{
		AudioClip clip = Resloader.Load<AudioClip>(SoundPath + soundName);
		if (clip == null)
		{
			Debug.LogWarningFormat("PlaySound: {0} not found", soundName);
			return;
		}
		SoundAudioSource.PlayOneShot(clip);
	}

	protected void PlayerClipOnAudioSource(AudioSource source, AudioClip clip, bool isLoop)
	{
		
	}
}
