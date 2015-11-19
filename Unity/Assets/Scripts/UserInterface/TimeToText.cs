using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class TimeToText : MonoBehaviour {
	public Text text_component;
	public GameTimer timer;
	public enum TimeFormat{
		Real,
		Game
	}
	protected static Dictionary<TimeFormat, Func<GameTimer, string>> formats = 
	new Dictionary<TimeFormat, Func<GameTimer, string>>{
		{TimeFormat.Real, (timer)=>{ return timer.real_time.as_stopwatch; }},
		{TimeFormat.Game, (timer)=>{ return timer.game_time.as_clock; }}
	};
	public TimeFormat format = TimeFormat.Real;
	void Start(){
		if (text_component == null)
			text_component = GetComponent<Text> ();
		if (timer == null)
			timer = GetComponent<GameTimer> ();
	}
	void Update () {
		text_component.text = formats[format](timer);
	}
}
