using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using GameScores;
using UniRx;

public class ScoreDetailedForm : BetterBehaviour, IScoreSource {
	protected static Subject<Score> selected_scores = new Subject<Score>();
	protected ReactiveProperty<Score> _rx_score;
	public ReactiveProperty<Score> rx_score{
		get { 
			if (_rx_score == null)
				_rx_score = selected_scores.ToReactiveProperty<Score>();
			return _rx_score; 
		}
		private set{ _rx_score = value;}
	}
	[Show]
	public Score score{
		get { return rx_score.Value; }
		set { rx_score.Value = value; }
	}
	//Date and Time
	public Text date_time;
	public Text play_time;
	public Text game_length;
	public Text player_name;
	
	[DontSerialize]
	ReadOnlyReactiveProperty<string> rx_player_name;
	
	IDisposable select_subscription;
	
	public static bool IsNullOrWhiteSpace(string s){
		if (s == null || s.Length <= 0) 
			return true;
		foreach(char c in s){
			if(c != ' ') return false;
		}
		return true;
	}
	
	public static void select_score(Score s){
		selected_scores.OnNext(s);
	}
	
	public static Color score_bonus_color;
	public static Color score_penalty_color;
	public static Text format_score_text(Text t, float value, string format = "0.00"){
		t.text = (value >= 0.0f?"+":"")+value.ToString(format);
		if (value < 0){
			t.color = score_penalty_color;
		} else {
			t.color = score_bonus_color;
		}
		return t;
	}
	
	void Awake () {
		select_subscription = selected_scores.Subscribe((s)=>{
			if (s != null){
				if (date_time != null)
					date_time.text = s.time.date_recorded_local().ToString();
				//Yeah, it's ugly abuse of static variables, but I'll fix it later.
				score = s;
				MenuStateMachine.fsm.trigger_transition("scores->details");
			}
		});
		
		rx_player_name = rx_score.SelectMany(s=>{
			if (s == null)
				return Observable.Never<String>();
			return s.rx_player_name.AsObservable();
		}).CombineLatest(LanguageController.controller.rx_load_text("score_default_name"), (name, default_name)=>{
			if (IsNullOrWhiteSpace(name))
				return default_name;
			return name;
		}).ToReadOnlyReactiveProperty<string>();
		
		rx_player_name.Subscribe(t=>{
			if (player_name != null)
				player_name.text = t;
		});
	}
	
	void OnDestroy(){
		select_subscription.Dispose();
	}
}
