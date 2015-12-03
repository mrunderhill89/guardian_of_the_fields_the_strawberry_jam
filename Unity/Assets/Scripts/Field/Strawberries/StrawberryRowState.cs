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
	public RowHandler2 row;
	public RowGenerator generator;
	public string prefab = "PlantCell";
	public string start_prefab = "RowStart";
	public string end_prefab = "RowEnd";
	public int num_receiving = 1;
	public int start_break = 4;
	void Awake(){
		rows.Add (this);
		if (row == null)
			row = GetComponent<RowHandler2> ();
		if (generator == null)
			generator = GetComponent<RowGenerator> ();
		generator.pattern.Clear ();
		row.target = Camera.main.transform;
		generator.pattern.Add(start_prefab);
		for (int i = 0; i < GameStartData.instance.break_distance; i++) {
			generator.pattern.Add (prefab);
		}
		generator.pattern.Add(end_prefab);
		for (int i = 0; i < GameStartData.instance.break_length; i++) {
			generator.pattern.Add ("");
		}
		state = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "row");
		generator.on_create((GameObject cell) => {
			StrawberryGenerator sb_generator = cell.GetComponentInChildren<StrawberryGenerator> ();
			if (sb_generator != null){
				sb_generator.state.chain_parent(state);
			}
		}).on_destroy((GameObject cell) => {
			StrawberryGenerator sb_generator = cell.GetComponentInChildren<StrawberryGenerator>();
			if (sb_generator != null){
				sb_generator.PreDestroy();
			}
		});
	}
	void Start(){
		state.chain_parent(StrawberryStateMachine.main.fsm.state("field")).initial_function(distribute);
	}

	State distribute(){
		//If the game is just starting, we can place into any cell we want.
		int rear_index = GameStateManager.main.is_loading()?
			start_break:Math.Max(row.front_index - num_receiving,0);
		if (generator.Count > 0){
			GameObject cell = generator.random_entry(rear_index,row.front_index, (GameObject obj)=>{
				return obj.GetComponentInChildren<StrawberryGenerator>() != null;
			});
			if (cell != null)
				return cell.GetComponentInChildren<StrawberryGenerator>().state;
		}
		return null;
	}
}
