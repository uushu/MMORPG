using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class LoadingManager : MonoBehaviour {
	public GameObject UITips;
	public GameObject UILoading;
	public GameObject UILogin;

	public Slider progressBar;
	public Text progressText;
	public Text progressNumber;

	private IEnumerator Start()
	{
		log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xmml"));
		UnityLogger.Init();
		Common.Log.Init("Unity");
		Common.Log.Info("LoadingManager start");

		
		UITips.SetActive(true);
		UILoading.SetActive(false);
		UILogin.SetActive(false);
		
		yield return new WaitForSeconds(2f);
		UILoading.SetActive(true);
		yield return new WaitForSeconds(1f);
		UITips.SetActive(false);

		yield return DataManager.Instance.LoadData();

		for (float i = 50; i < 100;)
		{
			i += Random.Range(0.1f, 1.5f);
			i = Mathf.Min(i, 100f);
			progressBar.value = i;
			
			progressNumber.text = Mathf.RoundToInt(i) + "%";
			yield return new WaitForEndOfFrame();
			
		}
		UILoading.SetActive(false);
		UILogin.SetActive(true);

		yield return null;

	}
}
