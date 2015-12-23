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

	void Awake () {
		rx_score.Subscribe((s)=>{
			if (s != null){
				date_time.text = s.time.date_recorded_local().ToString();
			}
		});
	}
}
