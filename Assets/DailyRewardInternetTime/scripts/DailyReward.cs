using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BooStudio;

public class DailyReward : MonoBehaviour {
	
	//UI
	public Text timeLabel; //only use if your timer uses a label
	public Button timerButton; //used to disable button when needed
	public Image _progress;
	//TIME ELEMENTS
	public int hours; //to set the hours
	public int minutes; //to set the minutes
	public int seconds; //to set the seconds
	private bool _timerComplete = false;
    private bool _timerIsReady;
	private TimeSpan _startTime;
	private TimeSpan _endTime;
	private TimeSpan _remainingTime;
	//progress filler
	private float _value = 0f;
	//reward to claim
//	public int RewardToEarn;
	public GameObject rewardUI;
	public GameObject rewardUIBG;
	public GameObject dailyRewardBtn;
	Animator dailyRewardAnimator;
	public int minRewardCoinValue = 20;
	public int maxRewardCoinValue = 50;
//	private int LastNotificationId = 0;


	void Awake()
	{
		//??? LocalNotification.ClearNotifications();
	}

	void Start()
	{
		dailyRewardAnimator = dailyRewardBtn.GetComponent<Animator>();
		if (PlayerPrefs.GetString ("_timer") == "")
		{ 
//			Debug.Log ("==> Enableing button");
			enableButton ();
//			Invoke ("isenableButtonNow",1.5f);

		} else 
		{
			disableButton ();
			StartCoroutine ("CheckTime");
		}
	}
//	public void isenableButtonNow()
//	{
//		enableButton ();
//	}


	//update the time information with what we got some the internet
	private void updateTime()
	{
		if (PlayerPrefs.GetString ("_timer") == "Standby") {
			PlayerPrefs.SetString ("_timer", TimeManager.sharedInstance.getCurrentTimeNow ());
            PlayerPrefs.SetInt ("_date", TimeManager.sharedInstance.getCurrentDateNow());
        }else if (PlayerPrefs.GetString ("_timer") != "" && PlayerPrefs.GetString ("_timer") != "Standby")
        {
            int _old = PlayerPrefs.GetInt("_date");
            int _now = TimeManager.sharedInstance.getCurrentDateNow();
            
            
            //check if a day as passed
            if(_now > _old)
            {//day as passed
//                Debug.Log("Day has passed");
                enableButton ();
                return;
            }else if (_now == _old)
            {//same day
//                Debug.Log("Same Day - configuring now");
                _configTimerSettings();
                return;
            }else
            {
//                Debug.Log("error with date");
                return;
            }
        }
//         Debug.Log("Day had passed - configuring now");
         _configTimerSettings();
	}

//setting up and configureing the values
//update the time information with what we got some the internet
private void _configTimerSettings()
{
    _startTime = TimeSpan.Parse (PlayerPrefs.GetString ("_timer"));
    _endTime = TimeSpan.Parse (hours + ":" + minutes + ":" + seconds);
    TimeSpan temp = TimeSpan.Parse (TimeManager.sharedInstance.getCurrentTimeNow ());
    TimeSpan diff = temp.Subtract (_startTime);
    _remainingTime = _endTime.Subtract (diff);
    //start timmer where we left off
    setProgressWhereWeLeftOff ();
    
    if(diff >= _endTime)
    {
        _timerComplete = true;
        enableButton ();
    }else
    {
        _timerComplete = false;
        disableButton();
        _timerIsReady = true;
    }
}

//initializing the value of the timer
	private void setProgressWhereWeLeftOff()
	{
		float ah = 1f / (float)_endTime.TotalSeconds;
		float bh = 1f / (float)_remainingTime.TotalSeconds;
		_value = ah / bh;
//		_value =  bh/ah;
		_progress.fillAmount = _value;
//		Debug.Log (_value);
	}



	//enable button function
	private void enableButton()
	{
//		if(LevelManager.THIS.gameState != GameState.MainScene)
//		{
			timerButton.interactable = true;
			timeLabel.text = "CLAIM";
			dailyRewardAnimator.SetBool("activate",true);
			dailyRewardAnimator.SetBool("deactivate",false);  	
		//--Local Nortification
//		LocalNotification.SendNotification(1, 10800000, "Your gift is ready to be opened", "Tap to claim awesome gift", new Color32(0xff, 0x44, 0x44, 255), true, true, true, "app_icon");
		//test for 60 seconds notification
	//???	LocalNotification.SendNotification(1, 600000, "Your gift is ready to be opened", "Tap to claim awesome gift", new Color32(0xff, 0x44, 0x44, 255), true, true, true, "app_icon");
//		}



	}



	//disable button function
	private void disableButton()
	{
		timerButton.interactable = false;
		timeLabel.text = "WAIT";
		dailyRewardAnimator.SetBool("deactivate",true);
		dailyRewardAnimator.SetBool("activate",false);
	}


	//use to check the current time before completely any task. use this to validate
	private IEnumerator CheckTime()
	{
		disableButton ();
		timeLabel.text = "---";
//		Debug.Log ("==> Checking for new time");
		yield return StartCoroutine (
			TimeManager.sharedInstance.getTime()
		);
		updateTime ();
//		Debug.Log ("==> Time check complete!");

	}


	//trggered on button click
	public void rewardClicked()
	{
		if (timeLabel.text == "CLAIM") {
//			Debug.Log ("==> Claim Button Clicked");
			PlayerPrefs.SetString ("_timer", "Standby");
			StartCoroutine ("CheckTime");
			dailyRewardAnimator.SetBool ("deactivate", true);
			dailyRewardAnimator.SetBool ("activate", false);
			SoundController.Sound.ClickBtn ();
			float value = UnityEngine.Random.value;
			int reward = GetRandomRewardCoins ();
			int roundedReward = (reward / 5) * 5;
			ShowRewardUI (roundedReward);
		}
	}



	//update method to make the progress tick
	void Update()
	{
        if(_timerIsReady)
        {
            if (!_timerComplete && PlayerPrefs.GetString ("_timer") != "")
                {
                    _value -= Time.deltaTime * 1f / (float)_endTime.TotalSeconds;
                    _progress.fillAmount = _value;
                
                    //this is called once only
                    if (_value <= 0 && !_timerComplete) {
                        //when the timer hits 0, let do a quick validation to make sure no speed hacks.
                    validateTime ();
                    _timerComplete = true;
                }
            }
        }
	}



	//validator
	private void validateTime()
	{
		Debug.Log ("==> Validating time to make sure no speed hack!");
		StartCoroutine ("CheckTime");
	}


//	private void claimReward(int x)
//	{
//		dailyRewardAnimator.SetBool("deactivate",true);
//		dailyRewardAnimator.SetBool("activate",false);
//		SoundController.Sound.ClickBtn ();
//		LevelManager.THIS.gameState = GameState.Rewarding;
//		float value = UnityEngine.Random.value;
//		int reward = GetRandomRewardCoins ();
//
//		// Round the number and make it mutiplies of 5 only.
//		int roundedReward = (reward / 5) * 5;
//		// Show the reward UI
//		ShowRewardUI (roundedReward);
//
//	}
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
		dailyRewardBtn.SetActive(true);
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
