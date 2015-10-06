using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UniRx;
public class RowHandler : BetterBehaviour{
	public float create_distance = 20.0f;
	public float remove_distance = -10.0f;
	public float cell_distance = 1.0f;

	public string prefab = "GroundCell";
	protected List<GameObject> cells;

	public List<Action<GameObject>> create_events;
	public RowHandler on_create(Action<GameObject> act){
		create_events.Add (act);
		return this;
	}

	public List<Action<GameObject>> destroy_events;
	public RowHandler on_destroy(Action<GameObject> act){
		destroy_events.Add (act);
		return this;
	}

	[DontSerialize]
	new public Camera camera;

	void Awake(){
		cells = new List<GameObject>();
		create_events = new List<Action<GameObject>>();
		destroy_events = new List<Action<GameObject>>();
	}
	void Start(){
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

	public int Count{
		get{return cells.Count;}
	}

	public GameObject cell(int index){
		return cells [index];
	}
	GameObject generate_cell(){
		float z = get_furthest_cell_distance()+cell_distance;
		GameObject cell = GameObject.Instantiate(Resources.Load(prefab)) as GameObject;
		cell.transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y, z);
		cell.transform.SetParent(transform,true);
		cells.Add(cell);
		foreach(Action<GameObject> act in create_events){
			act(cell);
		}
		return cell;
	}

	void pop_cell(){
		if (cells.Count > 0){
			GameObject cell = cells[0];
			foreach(Action<GameObject> act in destroy_events){
				act(cell);
			}
			cells.RemoveAt(0);
			Destroy(cell);
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
}
