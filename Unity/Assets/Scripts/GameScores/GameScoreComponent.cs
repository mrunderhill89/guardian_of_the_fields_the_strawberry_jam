using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using GameScores;
using Vexe.Runtime.Types;
using UniRx;
public class GameScoreComponent : BetterBehaviour, IScoreSource {
	protected ReactiveProperty<GameScores.Score> _rx_score 
		= new ReactiveProperty<GameScores.Score>();
	[Show]
	public ReactiveProperty<Score> rx_score{
		get { return _rx_score;}
		private set{ _rx_score = value;}
	}
	public Score score{
		get { return rx_score.Value;}
		set { rx_score.Value = value;}
	}
	public GameTimer timer;
	public Transform cart_position;

	void Start () {
		score = new Score();
		if (timer == null)
			timer = GetComponent<GameTimer> ();
		GameSettingsComponent.rx_working_rules.Take(1).Subscribe ((settings) => {
			score.settings = settings;
		});
		score.baskets.rx_baskets = BasketComponent.rx_baskets.RxSelect(comp=>{
			return comp.score_data;
		});
	}

	public string player_name{
		get{ return score.player_name; }
		set{ score.player_name = value; }
	}

	public bool lock_strawberries = false;
	public bool lock_baskets = false;
	public bool lock_timer = false;
	void Update () {
		if (!lock_strawberries) {
			score.berries.get_category ("gathered").from_state_machine ("basket");
			score.berries.get_category ("dropped").from_state_machine ("fall");
		}
		if (!lock_timer) {
			score.time.played_for = timer.time.total;
			if (cart_position != null){
				score.time.distance_covered = cart_position.position.z;
			}
		}
	}
	
	[Show]
	public GameScoreComponent record_score(){
		SavedScoreComponent.save_static(score.copy_of());
		return this;
	}
}
