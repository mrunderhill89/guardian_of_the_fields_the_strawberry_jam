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

	[DontSerialize]
	ReadOnlyReactiveProperty<string> rx_finished;

	void Awake () {
		rx_score.Subscribe((s)=>{
			if (s != null){
				date_time.text = s.time.date_recorded_local().ToString();
				play_time.text = GameTimer.to_stopwatch(s.time.played_for);
				game_length.text = GameTimer.to_stopwatch(s.settings.time.game_length);
			}
		});
		rx_finished = rx_score.SelectMany(s=>{
			if (s == null) return Observable.Never<String>();
			if (s.time.played_for < s.settings.time.game_length){
				return LanguageTable.get_property("time_not_finished");
			}
			return LanguageTable.get_property("time_finished");
		}).ToReadOnlyReactiveProperty<string>();
		rx_finished.Subscribe(t=>{
			finished.text = t;
		});
	}
}
