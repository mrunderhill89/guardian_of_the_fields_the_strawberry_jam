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
	public float create_distance = 20.0f;
	public float remove_distance = -10.0f;
	public float cell_distance = 1.0f;
	public int num_receiving = 1;
	protected List<GameObject> cells;
	[DontSerialize]
	public GameStateManager player;
	[DontSerialize]
	new public Camera camera;
	[DontSerialize]
	public StrawberryStateMachine berry_state;
	void Awake(){
		rows.Add(this);
		state = NamedBehavior.GetOrCreateComponentByName<State>(gameObject, "row")
			.initial(distribute);
		cells = new List<GameObject>();
	}
	void Start(){
		if (berry_state == null) berry_state = SingletonBehavior.get_instance<StrawberryStateMachine>();
		if (player == null) player = SingletonBehavior.get_instance<GameStateManager>();
		if (camera == null) camera = Camera.main;
		state.parent(berry_state.fsm.state("field"));
	}
	void Update(){
		generate_cells_to_distance(get_create_distance());
		trim_cells_to_distance(get_remove_distance());
	}


	float get_furthest_cell_distance(){
		if (cells.Count > 0){
			return cells.Last().transform.position.z;
		}
		return get_remove_distance();
	}
	float get_furthest_back_distance(){
		if (cells.Count > 0){
			return cells.First().transform.position.z;
		}
		return get_remove_distance();
	}
	float get_create_distance(){
		return camera.transform.position.z + create_distance;
	}
	float get_remove_distance(){
		return camera.transform.position.z + remove_distance;
	}

	GameObject generate_cell(){
		float z = get_furthest_cell_distance()+cell_distance;
		GameObject cell = GameObject.Instantiate(Resources.Load ("GroundCell")) as GameObject;
		cell.transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y, z);
		cell.transform.SetParent(transform,true);
		cell.GetComponent<StrawberryGenerator>().state.parent(state);
		cells.Add(cell);
		return cell;
	}
	void pop_cell(){
		if (cells.Count > 0){
			GameObject back_cell = cells[0];
			cells.RemoveAt(0);
			back_cell.GetComponent<StrawberryGenerator>().PreDestroy();
			Destroy(back_cell);
		}
	}
	void generate_cells_to_distance(float d){
		while (get_furthest_cell_distance() < d){
			generate_cell();
		}
	}
	void trim_cells_to_distance(float d){
		while (get_furthest_back_distance() < d){
			pop_cell();
		}
	}
	State distribute(Automata a){
		//If the game is just starting, we can place into any cell we want.
		int front_index = player.is_loading()?
			0:Math.Max(cells.Count - num_receiving,0);
		if (cells.Count > 0){
			GameObject cell = cells[RandomUtils.random_int(front_index,cells.Count)];
			return cell.GetComponent<StrawberryGenerator>().state;
		}
		return null;
	}
}
