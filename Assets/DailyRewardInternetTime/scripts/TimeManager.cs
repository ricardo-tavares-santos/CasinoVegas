using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {

	public static TimeManager sharedInstance = null;
	private string _url = "https://unitydeveloperhosting2018info.000webhostapp.com/timespan.php"; //change this to your own
	private string _timeData;
	private string _currentTime;
	private string _currentDate;


	//make sure there is only one instance of this always.
	void Awake() {
		if (sharedInstance == null) {
			sharedInstance = this;
		} else if (sharedInstance != this) {
			Destroy (gameObject);  
		}
		DontDestroyOnLoad(gameObject);
	}
		
	public IEnumerator getTime()
	{
		WWW www = new WWW (_url);
		yield return www;
		_timeData = www.text;
		string[] words = _timeData.Split('/');	
		_currentDate = words[0];
		_currentTime = words[1];
	}

	void Start()
	{
		StartCoroutine ("getTime");
	}

	public int getCurrentDateNow()
	{
		string[] words = _currentDate.Split('-');
        int x = int.Parse(words[0]+ words[1] + words[2]);
        return x;
	}
	public string getCurrentTimeNow()
	{
		return _currentTime;
	}


}

