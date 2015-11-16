using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Vexe.Runtime.Types;

public class BasketUIComponent : BetterBehaviour {
	public Text accepted_text;
	public Text over_text;
	public Text under_text;
	public Text overflow_text;
	
	// Update is called once per frame
	void Update () {
		BasketComponent.BasketScoreData data = BasketComponent.current_score;
		accepted_text.text = data.accepted.ToString();
		over_text.text = data.overweight.ToString();
		under_text.text = data.underweight.ToString();
		overflow_text.text = data.overflow.ToString();
	}
}
