using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using GameScores;
using Vexe.Runtime.Types;
using UniRx;
public class GameScoreComponent : BetterBehaviour {
	[DontSerialize][Show]
	public Score working_score;
	public GameTimer timer;
	void Start () {
		working_score = new Score();
		if (timer == null)
			timer = GetComponent<GameTimer> ();
		GameSettingsComponent.rx_working_rules.Subscribe ((settings) => {
			working_score.settings = settings;
		});
	}
	
	void Update () {
		working_score.berries.get_category ("gathered").from_state_machine ("basket");
		working_score.berries.get_category ("dropped").from_state_machine ("fall");
		working_score.baskets.baskets = BasketComponent.baskets.Select<BasketComponent,BasketSingleScore>((BasketComponent component) => {
			return new BasketSingleScore().chain_weight(component.total_weight).chain_overflow(component.is_overflow());
		}).ToList();
		working_score.time.played_for = timer.time.total;
	}

}
