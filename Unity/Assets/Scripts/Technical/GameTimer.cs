using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
public class GameTimer : BetterBehaviour {
	public const float F_MS_IN_SECOND = 10000.0f;
	public const int   I_MS_IN_SECOND = 10000;
	public const float F_SECONDS_IN_MINUTE = 60.0f;
	public const int   I_SECONDS_IN_MINUTE = 60;
	public const int   I_MINUTES_IN_HOUR = 60;
	public const float F_SECONDS_IN_HOUR = 3600.0f;
	public const int   I_SECONDS_IN_HOUR = 3600;
	[Serializable]
	public class Time{
		public float total = 0.0f;
		public Time(float _t = 0.0f){
			total = _t;
		}
		[Show]
		public int milliseconds{
			get{return Mathf.FloorToInt(total * F_MS_IN_SECOND) % I_MS_IN_SECOND;}
		}
		[Show]
		public int seconds{
			get{return Mathf.FloorToInt(total) % I_SECONDS_IN_MINUTE;}
		}
		[Show]
		public int minutes{
			get{return Mathf.FloorToInt(total/F_SECONDS_IN_MINUTE)% I_MINUTES_IN_HOUR;}
		}
		[Show]
		public int hours{
			get{return Mathf.FloorToInt(total/F_SECONDS_IN_HOUR);}
		}
		public float f_hours{
			get{return total/F_SECONDS_IN_HOUR;}
		}
		[Show]
		public string as_stopwatch
		{
			get{return hours.ToString()+"'"+minutes.ToString("00")+"\""+seconds.ToString("00")+"."+milliseconds.ToString("00");}
		}
		[Show]
		public string as_clock{
			get{return (hours%12==0?12:hours%12).ToString()+":"+minutes.ToString("00")+((hours<12)?" AM":" PM");}
		}
	}
	public class Countdown{
		[Show]
		float time = 0.0f;
		Action<Time> act;
		public Countdown(float t, Action<Time> a){
			time = t;
			act = a;
		}
		public void invoke(Time t){
			act(t);
		}
		public Time remaining(Time t){
			return new Time(time - t.total);
		}
	}
	
	[DontSerialize]
	public Time time = new Time(0.0f);
	public List<Countdown> countdowns = new List<Countdown>();
	public bool started = false;

	[Show]
	public Time real_time{
		get{ return time;}
	}
	[Show]
	public Time game_time{
		get{
			return new Time(real_to_game(time.total));
		}
	}
	
	public float real_to_game(float real){
		return (
			GameStartData.instance.start_hour +
			(time.total / GameStartData.instance.game_length) * 
			(GameStartData.instance.end_hour - GameStartData.instance.start_hour)
		) * F_SECONDS_IN_HOUR;
	}
	public static float game_to_real(float game){
		return (
			GameStartData.instance.game_length * ((game/F_SECONDS_IN_HOUR) - GameStartData.instance.start_hour)
			)/(GameStartData.instance.end_hour - GameStartData.instance.start_hour);
	}
	
	public GameTimer add_countdown(float t, Action<Time> act, bool from_now = true){
		float cd_time = from_now? time.total+t:t;
		countdowns.Add(new Countdown(cd_time, act));
		return this;
	}
	
	void Update () {
		if (started){
			time.total += UnityEngine.Time.deltaTime;
			countdowns = countdowns.Where((Countdown cd)=>{
				if (cd.remaining(time).total < 0.0f){
					cd.invoke(time);
					return false;
				}
				return true;
			}).ToList();
		}
	}
}
