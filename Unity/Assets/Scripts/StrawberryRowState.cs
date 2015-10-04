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
	protected List<StrawberryBush> cells;
	[DontSerialize]
	public GameStateManager player;
	[DontSerialize]
	new public Camera camera;
	void Awake(){
		rows.Add(this);
		state = NamedBehavior.GetOrCreateComponentByName<State>(gameObject, "state")
			.initial(distribute);
		cells = new List<StrawberryBush>();
	}
	void Start(){
		if (player == null) player = SingletonBehavior.get_instance<GameStateManager>();
		if (camera == null) camera = Camera.main;
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

	StrawberryBush generate_cell(){
		float z = get_furthest_cell_distance()+cell_distance;
		GameObject cell = GameObject.Instantiate(Resources.Load ("GroundCell")) as GameObject;
		StrawberryBush component = cell.GetComponent<StrawberryBush>();
		component.state.parent(state);
		cell.transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y, z);
		cell.transform.SetParent(transform,true);
		cells.Add(component);
		Debug.Log(cell.transform.position);
		return component;
	}
	void pop_cell(){
		if (cells.Count > 0){
			StrawberryBush back_cell = cells[0];
			back_cell.Remove();
			cells.RemoveAt(0);
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
		if (player.is_loading()){
			//If the game is just starting, we can place into any cell we want.
			return cells[RandomUtils.random_int(0,cells.Count)].state;
		} else if (cells.Count > 0){
			//Otherwise, we can only place new strawberries into the most recently-added cell.
			return cells.Last().state;
		} else {
			return null;
		}
	}
}
