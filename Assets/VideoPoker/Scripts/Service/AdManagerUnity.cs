using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//#if UNITY_ADS
using UnityEngine.Advertisements;
//#endif
public class AdManagerUnity : MonoBehaviour 
{
//	#if UNITY_ADS
	public static AdManagerUnity ads;
	//Insert you Unity Ads ID here
	#if UNITY_IOS
	[SerializeField] string gameID = "3034034";
	#elif UNITY_ANDROID
	[SerializeField] string gameID = "3034035";
	#endif
	void Awake()
	{
		Advertisement.Initialize (gameID, false);
		if (ads != null)
		{
			Destroy(gameObject);
		}
		else
		{
			ads = this;
			DontDestroyOnLoad(gameObject);
		}
		if (Advertisement.IsReady ("rewardedVideo")) 
		{
		}

	}

	public void ShowAd(string zone = "")
	{
		#if UNITY_EDITOR
		StartCoroutine(WaitForAd ());
		#endif

		if (string.Equals (zone, ""))
			zone = null;

		ShowOptions options = new ShowOptions ();
		options.resultCallback = AdCallbackhandler;

		if (Advertisement.IsReady (zone))
			Advertisement.Show (zone, options);
	}

	void AdCallbackhandler (ShowResult result)
	{
		switch(result)
		{
		case ShowResult.Finished:
			Advertisement.Initialize (gameID, false);
			if (DailyEvent.isFreeAd == 1)
			{
				GameObject.FindObjectOfType<DailyEvent> ().FreeAdsCallBack ();
			} 
			else if (DailyEvent.isFreeAd == 0&&SlotSettingUI.IsFreeAds==0)
			{	
				GameObject.FindObjectOfType<Reward> ().GetMoreCoin ();
			}
			else if (DailyEvent.isFreeAd == 2)
			{	
				DailyEvent.isFreeAd = 0;
				DataManager.Instance.AddCoins (Random.Range(50,100));
				GameObject.FindObjectOfType<UIAnimation> ().ReshowStartBtn ();
			}
			else if (SlotSettingUI.IsFreeAds == 1)
			{	
				Elona.Slot.Elos.checktut = false;
				SlotSettingUI.IsFreeAds = 0;
				DataManager.Instance.AddCoins (Random.Range(50,100));
				SoundController.Sound.CallBackSuccess ();
			}
			else if (DailyEvent.isFreeAd == 9)
			{	
				DailyEvent.isFreeAd = 0;
				DataManager.Instance.AddCoins (Random.Range(50,100));
				GameObject.FindObjectOfType<UIAnimation> ().ReshowStartBtn ();
			}			

			break;
		case ShowResult.Skipped:
			Advertisement.Initialize (gameID, false);
			Debug.Log ("Ad skipped. Son, I am dissapointed in you");
			break;
		case ShowResult.Failed:
			Advertisement.Initialize (gameID, false);
			Debug.Log("I swear this has never happened to me before");
			break;
		}
	}

	IEnumerator WaitForAd()
	{
		float currentTimeScale = Time.timeScale;
		Time.timeScale = 0f;
		yield return null;

		while (Advertisement.isShowing)
			yield return null;

		Time.timeScale = currentTimeScale;
	}
//#endif
}