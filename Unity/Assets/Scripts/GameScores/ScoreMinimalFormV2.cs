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
	public Text player_name;
	public Text score_text;
	
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
	
	public void SelectMyScore(){
		ScoreDetailedForm.select_score(score);
	}
	
	void Awake () {
		rx_score.Subscribe((s)=>{
			if (s != null){
				date_time.text = s.time.date_recorded_local().ToString();
				score_text.text = s.final_score().ToString("0.00");
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
	}
	
	void OnClick() {
		Debug.Log("Clicked!");
	}
}
