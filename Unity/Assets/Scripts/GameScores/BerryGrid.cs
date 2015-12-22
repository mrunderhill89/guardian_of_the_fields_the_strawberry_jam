using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using GameScores;
using UniRx;

public class BerryGrid : BetterBehaviour {
	public IScoreSource source;
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

	void Update(){
		if (source.score != null) {
			Score s = source.score;
			gathered_ripe.text = s.ripe_berries("gathered").Count().ToString();
			gathered_overripe.text = s.overripe_berries("gathered").Count().ToString();
			gathered_underripe.text = s.underripe_berries("gathered").Count().ToString();
			gathered_undersize.text = s.underweight_berries("gathered").Count().ToString();
			gathered_total.text = s.total_berries("gathered").Count().ToString();
			
			dropped_ripe.text = s.ripe_berries("dropped").Count().ToString();
			dropped_overripe.text = s.overripe_berries("dropped").Count().ToString();
			dropped_underripe.text = s.underripe_berries("dropped").Count().ToString();
			dropped_undersize.text = s.underweight_berries("dropped").Count().ToString();
			dropped_total.text = s.total_berries("dropped").Count().ToString();
		}
	}
}
