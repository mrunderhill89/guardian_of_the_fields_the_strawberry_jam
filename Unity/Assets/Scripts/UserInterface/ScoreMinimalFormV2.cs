using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using UniRx;

public class ScoreMinimalFormV2 : BetterBehaviour {
	[DontSerialize][Show]
	public ReactiveProperty<GameScores.Score> rx_score 
		= new ReactiveProperty<GameScores.Score>();
	public GameScores.Score Score{
		get { return rx_score.Value;}
		set { rx_score.Value = value;}
	}
	//Date and Time
	public Text date_time;
	//Baskets
	public Text accepted_baskets;
	public Text overweight_baskets;
	public Text underweight_baskets;
	public Text overflow_baskets;
	//Strawberries Gathered
	public Text gathered_ripe;
	public Text gathered_overripe;
	public Text gathered_underripe;
	public Text gathered_undersize;
	//Strawberries Dropped
	public Text dropped_ripe;
	public Text dropped_overripe;
	public Text dropped_underripe;
	public Text dropped_undersize;

	void Awake () {
		rx_score.Subscribe((score)=>{
			if (score != null){
				date_time.text = score.time.date_recorded_local().ToString();
			}
		});
	}
}
