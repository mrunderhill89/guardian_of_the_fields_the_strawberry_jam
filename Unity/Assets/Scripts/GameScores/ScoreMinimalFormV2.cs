using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using GameScores;
using UniRx;

public class ScoreMinimalFormV2 : BetterBehaviour, IScoreSource {
	protected ReactiveProperty<GameScores.Score> _rx_score 
		= new ReactiveProperty<GameScores.Score>();
	public ReactiveProperty<Score> rx_score{
		get { return _rx_score;}
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
	public Text finished;
	public Text player_name;
	
	public Text total_weight;
	public Text average_weight;
	public Image average_ripeness;

	[DontSerialize]
	ReadOnlyReactiveProperty<string> rx_finished;
	[DontSerialize]
	ReadOnlyReactiveProperty<string> rx_player_name;
	
	public static bool IsNullOrWhiteSpace(string s){
		if (s == null || s.Length <= 0) 
			return true;
		foreach(char c in s){
			if(c != ' ') return false;
		}
		return true;
	}
	
	void Awake () {
		rx_score.Subscribe((s)=>{
			if (s != null){
				date_time.text = s.time.date_recorded_local().ToString();
				play_time.text = GameTimer.to_stopwatch(s.time.played_for);
				game_length.text = GameTimer.to_stopwatch(s.settings.time.game_length);
				total_weight.text = s.total_weight("gathered").ToString("0.00");
				average_weight.text = s.average_weight("gathered").ToString("0.00");
				average_ripeness.color = StrawberryColor.get_color(s.average_ripeness("gathered"));
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
			if (player_name.text != null)
				player_name.text = t;
		});
		
		rx_finished = rx_score.SelectMany(s=>{
			if (s == null) return Observable.Never<String>();
			if (s.finished()){
				return LanguageController.controller.rx_load_text("time_finished");
			}
			return LanguageController.controller.rx_load_text("time_not_finished");
		}).ToReadOnlyReactiveProperty<string>();
		
		rx_finished.Subscribe(t=>{
			if (finished != null){
				finished.text = t;
			}
		});
	}
}
