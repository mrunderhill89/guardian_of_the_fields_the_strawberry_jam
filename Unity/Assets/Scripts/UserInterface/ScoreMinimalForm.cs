using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using UniRx;

public class ScoreMinimalForm : BetterBehaviour {
	[DontSerialize][Show]
	public ReactiveProperty<ScoreHandler.TotalScore> score 
		= new ReactiveProperty<ScoreHandler.TotalScore>();
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

	void Start () {
		score.Subscribe((total_score)=>{
			if (total_score != null){
				date_time.text = total_score.date_recorded.ToString();
				//Baskets
				accepted_baskets.text = total_score.baskets.accepted.ToString();
				overweight_baskets.text = total_score.baskets.overweight.ToString();
				underweight_baskets.text = total_score.baskets.underweight.ToString();
				overflow_baskets.text = total_score.baskets.overflow.ToString();
				//Gathered
				gathered_ripe.text = total_score.strawberries["basket"].ripe.ToString();
				gathered_overripe.text = total_score.strawberries["basket"].overripe.ToString();
				gathered_underripe.text = total_score.strawberries["basket"].underripe.ToString();
				gathered_undersize.text = total_score.strawberries["basket"].undersize.ToString();
				//Dropped
				dropped_ripe.text = total_score.strawberries["fall"].ripe.ToString();
				dropped_overripe.text = total_score.strawberries["fall"].overripe.ToString();
				dropped_underripe.text = total_score.strawberries["fall"].underripe.ToString();
				dropped_undersize.text = total_score.strawberries["fall"].undersize.ToString();
			}
		});
	}
}
