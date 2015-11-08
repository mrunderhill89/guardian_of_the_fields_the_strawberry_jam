using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;
public class GameTimer : BetterBehaviour {
	float _time = 0.0f;
	[Show]
	public float time{
		get{ return _time;}
	}
	[Show]
	public string formatted_time{
		get{ return ToString();}
	}
	void Update () {
		_time += Time.deltaTime;
	}
	public int milliseconds(){
		return Mathf.FloorToInt(time * 10000.0f) % 10000;
	}
	public int seconds(){
		return Mathf.FloorToInt(time) % 60;
	}
	public int minutes(){
		return Mathf.FloorToInt(time/60.0f)% 60;
	}
	public int hours(){
		return Mathf.FloorToInt(time/3600.0f);
	}
	public override string ToString(){
		return hours () + "'" + minutes() + "\"" + seconds () + "." + milliseconds();
	}
}
