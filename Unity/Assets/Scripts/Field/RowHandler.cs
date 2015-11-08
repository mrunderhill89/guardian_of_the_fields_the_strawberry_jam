using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UniRx;
public class RowHandler : BetterBehaviour{
	public int cells_forward = 20;
	public int cells_backward = 20;
	public float cell_distance = 1.0f;
	public int break_at = GameStartData.break_distance;
	public int break_length = 5;
	
	public string prefab = "GroundCell";
	LinkedList<Cell> cells = new LinkedList<Cell>();
	
	public List<Action<GameObject>> create_events = new List<Action<GameObject>>();
	public RowHandler on_create(Action<GameObject> act){
		create_events.Add (act);
		//Run the event on any cells we've already created, just in case.
		foreach(Cell cell in cells){
			act(cell.obj);
		}
		return this;
	}

	public List<Action<GameObject>> destroy_events = new List<Action<GameObject>>();
	public RowHandler on_destroy(Action<GameObject> act){
		destroy_events.Add (act);
		return this;
	}

	public float cell_to_z(int cell_num){
		return (cell_distance * cell_num) + transform.position.z;
	}
	public int z_to_cell(float z){
		return Mathf.FloorToInt((z - transform.position.z)/cell_distance);
	}
	public bool cell_valid(int cell_num){
		if (cell_num % break_at < break_length) return false;
		return true;
	}
	public bool have_cell(int cell_num){
		if (Count == 0) return false;
		if (cell_num >= rear_cell_index && cell_num <= front_cell_index) return true;
		return false;
	}
	
	[Show]
	public int front_cell_index{
		get{ 
			if (Count == 0) return 0;
			return cells.Last.Value.cell_number;
		}
	}

	[Show]
	public int rear_cell_index{
		get{ 
			if (Count == 0) return 0;
			return cells.First.Value.cell_number;
		}
	}

	[Show]
	public int camera_index{
		get{return z_to_cell(Camera.main.transform.position.z);}
	}
	public int front_camera_index{get{return camera_index+cells_forward;}}
	public int rear_camera_index{get{return camera_index-cells_backward;}}

	[Show]
	public int Count{
		get{return cells.Count;}
	}

	public GameObject cell(int list_position){
		return cells.ToList()[list_position].obj;
	}
	
	void Update(){
		//Check the front
		if (front_camera_index > front_cell_index){
			//We need a new front cell
			if (cell_valid(front_camera_index) && !have_cell(front_camera_index)){
				cells.AddLast(create_cell(front_camera_index));
			}
		} else if (front_camera_index < front_cell_index){
			pop_front();
		}
		//Check the back
		if (rear_camera_index < rear_cell_index){
			//We need a new rear cell
			if (cell_valid(rear_camera_index) && !have_cell(rear_camera_index)){
				cells.AddFirst(create_cell(rear_camera_index));
			}
		} else if (rear_camera_index > rear_cell_index){
			pop_rear();
		}
	}

	void Start(){
		fill(rear_camera_index,front_camera_index);
	}
	
	[Show]
	public void fill(int from, int to){
		for(int i = from; i <= to; i++){
			if (cell_valid(i) && !have_cell(i)){
				cells.AddLast(create_cell(i));
			}
		}
	}

	Cell create_cell(int cell_number){
		float z = cell_to_z(cell_number);
		Cell cell = new Cell();
		cell.cell_number = cell_number;
		cell.obj = GameObject.Instantiate(Resources.Load(prefab)) as GameObject;
		cell.obj.transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y, z);
		cell.obj.transform.SetParent(transform,true);
		foreach(Action<GameObject> act in create_events){
			act(cell.obj);
		}
		return cell;
	}

	void pop_front(){
		if (cells.Count > 0){
			Cell cell = cells.Last.Value;
			foreach(Action<GameObject> act in destroy_events){
				act(cell.obj);
			}
			cells.RemoveLast();
			Destroy(cell.obj);
		}
	}
	void pop_rear(){
		if (cells.Count > 0){
			Cell cell = cells.First.Value;
			foreach(Action<GameObject> act in destroy_events){
				act(cell.obj);
			}
			cells.RemoveFirst();
			Destroy(cell.obj);
		}
	}
	internal class Cell{
		internal GameObject obj;
		internal int cell_number;
	}
}

/*
	public float create_distance = 20.0f;
	public float remove_distance = -10.0f;
	public float cell_distance = 1.0f;
	public int break_distance = 10;
	public int break_length = 2;

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
	*/
