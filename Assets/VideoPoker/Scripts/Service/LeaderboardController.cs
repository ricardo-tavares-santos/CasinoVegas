using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SocialPlatforms;
#if UNITY_ANDROID
using GooglePlayGames;
#endif

public class LeaderboardController : MonoBehaviour 
{
	

	#if UNITY_IOS
	//insert your leaderboard id you creat in iTuneConnect here
	public string LEADERBOARD_BEST_SCORE = "CasinoClassicGame_TotalMoney";
	private static LeaderboardController instance;
	public static LeaderboardController Instance{
		get{ 
			return instance;
		}
	}
	void Awake()
	{
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject);
		} else if (instance != this) {
			Destroy (gameObject);
		}
		StartCoroutine(checkInternetConnection((isConnected)=>{
			if(isConnected)
			{
				Connect ();	
	            //Submit score to leaderboard
	             submitbestscore(DataManager.Instance.Coins);
			}
		}));

	}
	IEnumerator checkInternetConnection(Action<bool> action){
		WWW www = new WWW("http://google.com");
		yield return www;
		if (www.error != null) {
			action (false);
		} else {
			action (true);
		}
	} 

	private void Connect() {	
		Social.localUser.Authenticate ((bool success) => 
		{	
		});
	}
		
	public void ShowLeaderBoard_iOS() 
	{
	    SoundController.Sound.ClickBtn ();
		Social.ShowLeaderboardUI ();
	    submitbestscore(DataManager.Instance.Coins);
	    Debug.Log ("ShowLB_iOS");
	}				

	public void submitbestscore(int score) {		
		Social.ReportScore (score, LEADERBOARD_BEST_SCORE, success => {});
	}	


	#elif UNITY_ANDROID
	public static int checkSignIn;
	private static LeaderboardController instance;
	public static LeaderboardController Instance{
		get{ 
			return instance;
		}
	}
	#region PUBLIC_VAR
	public string leaderboard1;
	#endregion

	#region DEFAULT_UNITY_CALLBACKS

	void Awake()
	{
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject);
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	void Start ()
	{
		leaderboard1 = GPGSIds.leaderboard_total_coins;
		PlayGamesPlatform.Activate ();
		this.LogIn ();
	}
	#endregion

	#region BUTTON_CALLBACKS
	/// <summary>
	/// Login In Into Your Google+ Account
	/// </summary>
	public void LogIn ()
	{
		Social.localUser.Authenticate ((bool success) => 
			{
			if (success)
			{
				OnAddScoreToLeaderBorad ();
				checkSignIn=1;

			} else 
			{
				checkSignIn=0;
			}
		});
	}


	public void ShowLeaderBoard_Android ()
	{
		SoundController.Sound.ClickBtn ();
		OnAddScoreToLeaderBorad ();
		Debug.Log ("ShowLB_Android");
		((PlayGamesPlatform)Social.Active).ShowLeaderboardUI (leaderboard1); // Show current (Active) leaderboard
	}
		
	public void OnAddScoreToLeaderBorad ()
	{
		if (Social.localUser.authenticated) {
			Social.ReportScore (DataManager.Instance.Coins, leaderboard1, (bool success) => {
				if (success) 
				{

				} else 
				{
					
				}
			});
		}
	}

	public void OnLogOut ()
	{
		((PlayGamesPlatform)Social.Active).SignOut ();
	}
	#endregion
	#endif
	public void ShowLeaderBoard()
	{
		#if UNITY_IOS
		ShowLeaderBoard_iOS();
		#elif UNITY_ANDROID
		ShowLeaderBoard_Android();
		#endif
		
	}



}
