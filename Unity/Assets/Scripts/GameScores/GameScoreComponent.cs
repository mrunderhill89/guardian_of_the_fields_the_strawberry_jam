using UnityEngine;
using System.Collections;
using GameScores;
using Vexe.Runtime.Types;
using UniRx;
public class GameScoreComponent : BetterBehaviour {
	public Score working_score;
	void Start () {
		working_score = new Score();
		GameSettingsComponent.rx_working_rules.Subscribe ((settings) => {
			working_score.settings = settings;
		});
	}
	
	void Update () {
		working_score.berries.get_category ("gathered").from_state_machine ("basket");
		working_score.berries.get_category ("dropped").from_state_machine ("fall");
	}

}
