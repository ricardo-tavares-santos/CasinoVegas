using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;
#if UNITY_ANDROID
//using GooglePlayGames;
#endif

public class LevelManager : MonoBehaviour
{
	public static LevelManager THIS;
	Animator animconvert;
	public GameObject bgOverlay;
	void Start () 
	{
		//----Unlock Achieve------
		//--Insert your achievement id in your google play console here---
		#if UNITY_ANDROID
		if(DataManager.Instance.Coins>=1000&DataManager.Instance.Coins<10000&LeaderboardController.checkSignIn==1)
		{
			Social.ReportProgress("CgkIpozY7LUeEAIQAQ", 100.0f, (bool success) => {
				// handle success or failure
			});
		}
		else if(DataManager.Instance.Coins>=10000&DataManager.Instance.Coins<15000&LeaderboardController.checkSignIn==1)
		{
			Social.ReportProgress("CgkIpozY7LUeEAIQAg", 100.0f, (bool success) => {
				// handle success or failure
			});
		}
		else if(DataManager.Instance.Coins>=15000&DataManager.Instance.Coins<50000&LeaderboardController.checkSignIn==1)
		{
			Social.ReportProgress("CgkIpozY7LUeEAIQAw", 100.0f, (bool success) => {
				// handle success or failure
			});
		}
		else if(DataManager.Instance.Coins>=50000&DataManager.Instance.Coins<500000&LeaderboardController.checkSignIn==1)
		{
			Social.ReportProgress("CgkIpozY7LUeEAIQBA", 100.0f, (bool success) => {
				// handle success or failure
			});
		}
		else if(DataManager.Instance.Coins>=500000)
		{
			Social.ReportProgress("CgkIpozY7LUeEAIQBQ", 100.0f, (bool success) => {
				// handle success or failure
			});
		}
		#endif
		//---------------

		THIS = this;
		animconvert = GetComponentInChildren<Animator> ();
		StartCoroutine (InLoadScene ());

	}
	void Update()
	{

	}

	//-------------AUTO LOAD SCENE WHEN FIRST SCENCE------------------
	public IEnumerator InLoadScene()
	{
		animconvert.SetTrigger ("First");
		bgOverlay.SetActive(true);
		SoundController.Sound.Out ();
		yield return new WaitForSeconds (1f);
		animconvert.SetTrigger ("Out");
		SoundController.Sound.In ();
		MusicController.Music.BG_Home ();
		bgOverlay.SetActive(false);
	}

	public void VideoPokerScene()
	{
		MusicController.Music.BG_VideoPoker ();
		SoundController.Sound.ClickBtn ();
		SceneManager.LoadScene ("JackOrBetter");
		Debug.Log ("VideoPoker Scene");
	}
	public void BlackJackScene()
	{
		MusicController.Music.BG_BlackJack ();
		SoundController.Sound.ClickBtn ();
		SceneManager.LoadScene ("Blackjack");
	}
	public void SlotMachineScene()
	{
		MusicController.Music.BG_SlotMachine ();
		SoundController.Sound.ClickBtn ();
		SceneManager.LoadScene ("SlotMachine");
		Debug.Log ("SlotMachine Scene");
	}

	public void HomeScene()
	{
		MusicController.Music.BG_Home ();
		SoundController.Sound.ClickBtn ();
		SceneManager.LoadScene ("HomeScene");
	}



}
