using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
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
	public Transform put_baskets_here;
	//Strawberries Gathered
	public Text gathered_ripe;
	public Text gathered_overripe;
	public Text gathered_underripe;
	public Text gathered_undersize;
	public Text gathered_total;
	//Strawberries Dropped
	public Text dropped_ripe;
	public Text dropped_overripe;
	public Text dropped_underripe;
	public Text dropped_undersize;
	public Text dropped_total;

	void Awake () {
		rx_score.Subscribe((score)=>{
			if (score != null){
				date_time.text = score.time.date_recorded_local().ToString();
				gathered_ripe.text = score.ripe_berries("gathered").Count().ToString();
				gathered_overripe.text = score.overripe_berries("gathered").Count().ToString();
				gathered_underripe.text = score.underripe_berries("gathered").Count().ToString();
				gathered_undersize.text = score.underweight_berries("gathered").Count().ToString();
				gathered_total.text = score.total_berries("gathered").Count().ToString();

				dropped_ripe.text = score.ripe_berries("dropped").Count().ToString();
				dropped_overripe.text = score.overripe_berries("dropped").Count().ToString();
				dropped_underripe.text = score.underripe_berries("dropped").Count().ToString();
				dropped_undersize.text = score.underweight_berries("dropped").Count().ToString();
				dropped_total.text = score.total_berries("gathered").Count().ToString();
			}
		});
	}
}
