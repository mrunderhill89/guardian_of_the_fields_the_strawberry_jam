using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class StrawberryRowState : BetterBehaviour{
	[DontSerialize]
	public static List<StrawberryRowState> rows;
	static StrawberryRowState(){
		rows = new List<StrawberryRowState>();
	}
	public static State random_row(){
		if (rows.Count > 0){
			State state = rows[RandomUtils.random_int(0,rows.Count)].state;
			return state;
		}
		return null;
	}
	public State state;
	public RowHandler row;
	public int num_receiving = 1;
	[DontSerialize]
	public GameStateManager player;
	[DontSerialize]
	public StrawberryStateMachine berry_state;
	void Awake(){
		rows.Add(this);
		state = NamedBehavior.GetOrCreateComponentByName<State>(gameObject, "row")
			.initial(distribute);
	}
	void Start(){
		if (row == null) row = GetComponent<RowHandler> ();
		if (berry_state == null) berry_state = SingletonBehavior.get_instance<StrawberryStateMachine>();
		if (player == null) player = SingletonBehavior.get_instance<GameStateManager>();
		state.parent(berry_state.fsm.state("field"));
		row.on_create((GameObject cell) => {
			StrawberryGenerator generator = cell.GetComponent<StrawberryGenerator>();
			generator.state.parent(state);
		}).on_destroy((GameObject cell) => {
			StrawberryGenerator generator = cell.GetComponent<StrawberryGenerator>();
			generator.PreDestroy();
		});
	}

	State distribute(Automata a){
		//If the game is just starting, we can place into any cell we want.
		int front_index = player.is_loading()?
			0:Math.Max(row.Count - num_receiving,0);
		if (row.Count > 0){
			GameObject cell = row.cell(RandomUtils.random_int(front_index,row.Count));
			return cell.GetComponent<StrawberryGenerator>().state;
		}
		return null;
	}
}
