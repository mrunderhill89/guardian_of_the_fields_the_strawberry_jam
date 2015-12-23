using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using Vexe.Runtime.Types;
using GameScores;

public class StrawberryUIComponent : BetterBehaviour {
	public StrawberryStateMachine berry_state;
	public IScoreSource scores;
	public Text ripe_text;
	public Text over_text;
	public Text under_text;
	public Text small_text;
	public string category = "gathered";
	
	// Update is called once per frame
	void Update () {
		ripe_text.text = scores.score.ripe_berries(category).Count ().ToString();
		over_text.text = scores.score.overripe_berries(category).Count ().ToString();
		under_text.text = scores.score.underripe_berries(category).Count ().ToString();
		small_text.text = scores.score.underweight_berries(category).Count ().ToString();
	}
}
