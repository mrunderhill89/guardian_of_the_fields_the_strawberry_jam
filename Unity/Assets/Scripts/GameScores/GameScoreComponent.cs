using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using GameScores;
using Vexe.Runtime.Types;
using UniRx;
public class GameScoreComponent : BetterBehaviour, IScoreSource {
	[DontSerialize][Show]
	protected ReactiveProperty<GameScores.Score> _rx_score 
		= new ReactiveProperty<GameScores.Score>();
	public ReactiveProperty<Score> rx_score{
		get { return _rx_score;}
		private set{ _rx_score = value;}
	}
	public Score score{
		get { return rx_score.Value;}
		set { rx_score.Value = value;}
	}
	public GameTimer timer;
	void Start () {
		score = new Score();
		if (timer == null)
			timer = GetComponent<GameTimer> ();
		GameSettingsComponent.rx_working_rules.Subscribe ((settings) => {
			score.settings = settings;
		});
	}

	public bool lock_strawberries = false;
	public bool lock_baskets = false;
	public bool lock_timer = false;
	void Update () {
		if (!lock_strawberries) {
			score.berries.get_category ("gathered").from_state_machine ("basket");
			score.berries.get_category ("dropped").from_state_machine ("fall");
		}
		if (!lock_baskets) {
			score.baskets.baskets = BasketComponent.baskets.Select<BasketComponent,BasketSingleScore> ((BasketComponent component) => {
				return new BasketSingleScore ()
					.chain_weight (component.total_weight)
					.chain_overflow (component.is_overflow ())
					.chain_count(component.slot.count());
			}).ToList();
		}
		if (!lock_timer) {
			score.time.played_for = timer.time.total;
		}
	}
	[Show]
	public GameScoreComponent record_score(){
		SavedScoreComponent.record_score (score.copy_of());
		SavedScoreComponent.export_static ();
		return this;
	}
}
