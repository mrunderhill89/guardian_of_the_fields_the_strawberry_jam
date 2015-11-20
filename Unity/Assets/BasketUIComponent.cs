using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Vexe.Runtime.Types;

public class BasketUIComponent : BetterBehaviour {
	public ScoreHandler scores;
	public Text accepted_text;
	public Text over_text;
	public Text under_text;
	public Text overflow_text;
	
	// Update is called once per frame
	void Update () {
		accepted_text.text = scores.current_score.baskets.accepted.ToString();
		over_text.text = scores.current_score.baskets.overweight.ToString();
		under_text.text = scores.current_score.baskets.underweight.ToString();
		overflow_text.text = scores.current_score.baskets.overflow.ToString();
	}
}
