using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BooStudio;
using UnityEngine.Advertisements;

public class DailyEvent : MonoBehaviour {
	public Button timerButton;
	public Text timeLabel; //this is the text lable to show the players the status of the timer
    public string StartTime; //use 00:00:00 where 14:00:00 represents 2:00pm
    public string EndTime; //use 00:00:00 where 07:30:00 represents 7:30am
	private double tcounter;
	private TimeSpan eventStartTime;
	private TimeSpan eventEndTime;
	private TimeSpan currentTime;
	private TimeSpan _remainingTime;
	private string Timeformat;
	private bool timerSet;
	private bool countIsReady;
	private bool countIsReady2;
	Animator dailyRewardAnimator;
	public GameObject rewardUI;
	public GameObject rewardUIBG;
	public GameObject dailyRewardBtn;
	public static int isFreeAd=0;
	public int minRewardCoinValue = 5;
	public int maxRewardCoinValue = 20;
	public Text CountNumAds;
//start up
	void Start () {
		dailyRewardAnimator = dailyRewardBtn.GetComponent<Animator>();
		eventStartTime = TimeSpan.Parse (StartTime);
		eventEndTime = TimeSpan.Parse (EndTime);
        disableButton("---");
		StartCoroutine ("CheckTime");
	}

//use IEnumerator to make sure all the download happen before we go any further.
	private IEnumerator CheckTime()
	{
//		Debug.Log ("==> Checking the time - from daily event");
		yield return StartCoroutine (
			TimeManager.sharedInstance.getTime()
		);
		updateTime ();
//		Debug.Log ("==> Time check complete!");

	}

//after we download the php file we want to update the current time.
	private void updateTime()
	{
		currentTime = TimeSpan.Parse (TimeManager.sharedInstance.getCurrentTimeNow ());
		timerSet = true;
	}

	void Update()
	{
		if(timeLabel.text!="ACTIVE")
		{
			StartCoroutine ("CheckTime");
		}
		CountNumAds.text = ""+DataManager.Instance.FreeAdNumber;
		if (timerSet) 
		{
			if (currentTime >= eventStartTime && currentTime <= eventEndTime && DataManager.Instance.FreeAdNumber <= 10 && DataManager.Instance.FreeAdNumber > 0&&Advertisement.IsReady ("rewardedVideo")) {//this means the event as already started and players can click and join
				_remainingTime = eventEndTime.Subtract (currentTime);
				tcounter = _remainingTime.TotalMilliseconds;
				countIsReady2 = true;
				dailyRewardAnimator.SetBool ("activate", true);
				dailyRewardAnimator.SetBool ("deactivate", false);
				enableButton ("ACTIVE");

			}
			else if (currentTime >= eventStartTime && currentTime <= eventEndTime && DataManager.Instance.FreeAdNumber <= 10 && DataManager.Instance.FreeAdNumber > 0&&!Advertisement.IsReady ("rewardedVideo")) {//this means the event as already started and players can click and join
				_remainingTime = eventEndTime.Subtract (currentTime);
				tcounter = _remainingTime.TotalMilliseconds;
				countIsReady = true;
				dailyRewardAnimator.SetBool("activate",false);
				dailyRewardAnimator.SetBool("deactivate",true);
				disableButton("WAIT");

			}
			else if (currentTime >= eventStartTime && currentTime <= eventEndTime && DataManager.Instance.FreeAdNumber <= 0) 
			{
				_remainingTime = eventStartTime.Subtract(currentTime);
				tcounter = _remainingTime.TotalMilliseconds;
				countIsReady = true;
				dailyRewardAnimator.SetBool("activate",false);
				dailyRewardAnimator.SetBool("deactivate",true);
				disableButton("WAIT");
			}
			else if (currentTime < eventStartTime&&DataManager.Instance.FreeAdNumber<=0) 
			{
				_remainingTime = eventStartTime.Subtract(currentTime);
				tcounter = _remainingTime.TotalMilliseconds;
				countIsReady = true;
				dailyRewardAnimator.SetBool("activate",false);
				dailyRewardAnimator.SetBool("deactivate",true);
				disableButton("WAIT");
			} 
			else if (currentTime < eventStartTime&&DataManager.Instance.FreeAdNumber <= 10 && DataManager.Instance.FreeAdNumber > 0) 
			{
				_remainingTime = eventStartTime.Subtract(currentTime);
				tcounter = _remainingTime.TotalMilliseconds;
				countIsReady = true;
				dailyRewardAnimator.SetBool("activate",false);
				dailyRewardAnimator.SetBool("deactivate",true);
				disableButton("WAIT");
//				disableButton("" + GetRemainingTime(tcounter));
			} 
			else if (currentTime >= eventEndTime && DataManager.Instance.FreeAdNumber <= 0) 
			{
				_remainingTime = eventStartTime.Subtract(currentTime);
				tcounter = _remainingTime.TotalMilliseconds;
				countIsReady2 = true;
				dailyRewardAnimator.SetBool("activate",true);
				dailyRewardAnimator.SetBool("deactivate",false);
				enableButton("ACTIVE");
				DataManager.Instance.AddFreeAdNumber (10);	
			}
			else if (currentTime >= eventEndTime && DataManager.Instance.FreeAdNumber >0) 
			{
				_remainingTime = eventStartTime.Subtract(currentTime);
				tcounter = _remainingTime.TotalMilliseconds;
				countIsReady2 = true;
				dailyRewardAnimator.SetBool("activate",true);
				dailyRewardAnimator.SetBool("deactivate",false);
				enableButton("ACTIVE");
			}
		}

		if (countIsReady) { startCountdown ();}
		if (countIsReady2) { startCountdown2 ();}
	}

	//setup the time format string
	public string GetRemainingTime(double x)
	{
		TimeSpan tempB = TimeSpan.FromMilliseconds(x);
		Timeformat = string.Format("{0:D2}:{1:D2}:{2:D2}", tempB.Hours, tempB.Minutes, tempB.Seconds);
		return Timeformat;
	}

	//Button Can't Tap
	private void startCountdown()
	{
		timerSet = false;
		tcounter-= Time.deltaTime * 1000;
		disableButton("WAIT");
		if (tcounter <= 0) {
			countIsReady = false;
			validateTime ();

		}
	}

	//Button Can Tap
	private void startCountdown2()
	{
		
		timerSet = false;
		tcounter-= Time.deltaTime * 1000;
		enableButton("ACTIVE");

		if (tcounter <= 0) {
			countIsReady2 = false;
			validateTime ();
		}

		if(DataManager.Instance.FreeAdNumber<=0)
		{
			countIsReady2 = false;
			disableButton("WAIT");
			dailyRewardAnimator.SetBool("activate",false);
			dailyRewardAnimator.SetBool("deactivate",true);
		}
	}

	//enable button function
	private void enableButton(string x)
	{
		timerButton.interactable = true;
		timeLabel.text = x; 
	}


	//disable button function
	private void disableButton(string x)
	{
		timerButton.interactable = false;
		timeLabel.text = x;
	}

	//validator
	private void validateTime()
	{
		StartCoroutine ("CheckTime");
	}
	public void FreeCoinWatchAds()
	{

		if (timerButton.interactable&&(Advertisement.IsReady ("rewardedVideo"))&&timeLabel.text=="ACTIVE") 
		{

			if (DataManager.Instance.FreeAdNumber <= 10 && DataManager.Instance.FreeAdNumber > 0) 
			{
				DataManager.Instance.RemoveFreeAdNumber (1);
				SoundController.Sound.ClickBtn ();
				GameObject.FindObjectOfType<AdManagerUnity> ().ShowAd ("rewardedVideo");
				isFreeAd = 1;
			}
			else
				SoundController.Sound.DisactiveButtonSound ();
			if(DataManager.Instance.FreeAdNumber<=0)
			{
				dailyRewardAnimator.SetBool("activate",false);
				dailyRewardAnimator.SetBool("deactivate",true);
				startCountdown ();
			}

		} else 
			SoundController.Sound.DisactiveButtonSound ();
	}
	public void FreeCoinWatchAds2()
	{

				//DataManager.Instance.RemoveFreeAdNumber (1);
				SoundController.Sound.ClickBtn ();
				GameObject.FindObjectOfType<AdManagerUnity> ().ShowAd ("rewardedVideo");
				isFreeAd = 1;
	}	
	
	
	public void FreeAdsCallBack()
	{
		isFreeAd = 0;
		float value = UnityEngine.Random.value;
		int reward = GetRandomRewardCoins ();
		int roundedReward = (reward / 5) * 5;
		ShowRewardUI (roundedReward);

	}

	public void ShowRewardUI(int reward)
	{
		rewardUI.SetActive(true);
		rewardUIBG.SetActive (true);
		rewardUI.GetComponent<RewardUIController>().Reward(reward);

		RewardUIController.RewardUIClosed += OnRewardUIClosed;
	}

	void OnRewardUIClosed()
	{
		RewardUIController.RewardUIClosed -= OnRewardUIClosed;
	}

	public void HideRewardUI()
	{
		rewardUIBG.SetActive (false);
		rewardUI.GetComponent<RewardUIController>().Close();
	}
	private int GetRandomRewardCoins()
	{
		return UnityEngine.Random.Range(minRewardCoinValue, maxRewardCoinValue + 1);

	}

}
