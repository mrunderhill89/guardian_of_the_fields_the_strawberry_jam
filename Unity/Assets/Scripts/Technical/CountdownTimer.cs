using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Vexe.Runtime.Types;
public class CountdownTimer : BetterBehaviour {
	float _time_left = 0.0f;
	[Show]
	public float time_left{
		get{ return _time_left;}
		set{ _time_left = value;}
	}
	public CountdownTimer set_time(float t){
		_time_left = t;
		return this;
	}

	[Show]
	public string formatted_time{
		get{ return ToString();}
	}
	public UnityEvent on_countdown = new UnityEvent();

	void Update () {
		if (_time_left > 0.0f) {
			_time_left -= Time.deltaTime;
			if (_time_left <= 0.0f){
				_time_left = 0.0f;
				on_countdown.Invoke();
			}
		}
	}
	public int milliseconds(){
		return Mathf.FloorToInt(time_left * 10000.0f) % 10000;
	}
	public int seconds(){
		return Mathf.FloorToInt(time_left) % 60;
	}
	public int minutes(){
		return Mathf.FloorToInt(time_left/60.0f)% 60;
	}
	public int hours(){
		return Mathf.FloorToInt(time_left/3600.0f);
	}
	public override string ToString(){
		return hours () + "'" + minutes() + "\"" + seconds () + "." + milliseconds();
	}
}
