using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using Vexe.Runtime.Types;
using GameScores;
public class BasketUIComponent : BetterBehaviour {
	public IScoreSource scores;
	public Text accepted_text;
	public Text over_text;
	public Text under_text;
	public Text overflow_text;
	
	// Update is called once per frame
	void Update () {
		accepted_text.text = scores.score.accepted_baskets().Count().ToString();
		over_text.text = scores.score.overweight_baskets().Count().ToString();
		under_text.text = scores.score.underweight_baskets().Count().ToString();
		overflow_text.text = scores.score.overflow_baskets().Count().ToString();
	}
}
