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
	public static int to_milliseconds(float t){
		return Mathf.FloorToInt(t * F_MS_IN_SECOND) % I_MS_IN_SECOND;
	}
	public static int to_seconds(float t){
		return Mathf.FloorToInt(t) % I_SECONDS_IN_MINUTE;
	}
	public static int to_minutes(float t){
		return Mathf.FloorToInt(t/F_SECONDS_IN_MINUTE)% I_MINUTES_IN_HOUR;
	}
	public static int to_hours(float t){
		return Mathf.FloorToInt(t/F_SECONDS_IN_HOUR);
	}
	public static float to_f_hours(float t){
		return t/F_SECONDS_IN_HOUR;
	}
	[Serializable]
	public class Time{
		public float total = 0.0f;
		public Time(float _t = 0.0f){
			total = _t;
		}
		[Show]
		public int milliseconds{
			get{return to_milliseconds(total);}
		}
		[Show]
		public int seconds{
			get{return to_seconds(total);}
		}
		[Show]
		public int minutes{
			get{return to_minutes(total);}
		}
		[Show]
		public int hours{
			get{return to_hours(total);}
		}
		public float f_hours{
			get{return to_f_hours(total);}
		}
		[Show]
		public float game_time{
			get{return real_to_game(total);}
		}
		[Show]
		public int game_hours{
			get{return to_hours(game_time);}
		}
		[Show]
		public int game_minutes{
			get{return to_minutes(game_time);}
		}
		[Show]
		public float game_f_hours{
			get{return to_f_hours(game_time);}
		}
		[Show]
		public string as_stopwatch
		{
			get{return hours.ToString()+"'"+minutes.ToString("00")+"\""+seconds.ToString("00")+"."+milliseconds.ToString("00");}
		}
		[Show]
		public string as_clock{
			get{return (game_hours%12==0?12:game_hours%12).ToString()+":"+game_minutes.ToString("00")+((game_hours<12)?" AM":" PM");}
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

	public static float game_length{
		get{ return GameSettingsComponent.working_rules.time.game_length;}
	}
	public static float start_hour{
		get{ return GameSettingsComponent.working_rules.time.start_hour;}
	}
	public static float end_hour{
		get{ return GameSettingsComponent.working_rules.time.end_hour;}
	}

	public static float real_to_game(float real){
		return (
			start_hour +
			(real / game_length) * 
			(end_hour - start_hour)
		) * F_SECONDS_IN_HOUR;
	}
	public static float game_to_real(float game){
		return (
			game_length * ((game/F_SECONDS_IN_HOUR) - start_hour)
			)/(end_hour - start_hour);
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
