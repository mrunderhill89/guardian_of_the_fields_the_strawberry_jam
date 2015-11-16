using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Vexe.Runtime.Types;

public class StrawberryUIComponent : BetterBehaviour {
	public StrawberryStateMachine berry_state;
	public Text ripe_text;
	public Text over_text;
	public Text under_text;
	public Text small_text;
	public string state_name = "basket";
	
	// Update is called once per frame
	void Update () {
		StrawberryStateMachine.StrawberryScoreData data = berry_state.get_score_data(state_name);
		ripe_text.text = data.ripe.ToString();
		over_text.text = data.overripe.ToString();
		under_text.text = data.underripe.ToString();
		small_text.text = data.undersize.ToString();
	}
}
