using UnityEngine;
using System.Collections;

public class QuitRateShareAds : MonoBehaviour
{
	public string URLRate;
	public void QuitGame()
	{
		Application.Quit ();
//		Debug.Log ("Quit Game");
	}
	public void RateGame()
	{
		//Insert your game on store
		SoundController.Sound.ClickBtn ();
		#if UNITY_ANDROID
		Application.OpenURL(URLRate);
		#elif UNITY_IOS
		Application.OpenURL (URLRate);
		#endif
		//
	}
}
