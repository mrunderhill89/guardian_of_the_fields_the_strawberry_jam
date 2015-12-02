using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using UniRx;

public class RowHandler2 : BetterBehaviour {
	public int view_ahead = 100;
	public int view_behind = 100;
	public float cell_distance = 1.0f;
	public Transform target;
	public Vector3 direction = new Vector3(0.0f,0.0f,1.0f);
	public bool log = false;


	public float pos_to_dist(Vector3 pos){
		return Vector3.Dot(pos-transform.position, direction.normalized);
	}
	public int dist_to_cell(float dist){
		return Mathf.RoundToInt(dist / cell_distance);
	}
	public int pos_to_cell(Vector3 pos){
		return dist_to_cell (pos_to_dist (pos));
	}
	public float cell_to_dist(int cell){
		return cell * cell_distance;
	}
	public Vector3 dist_to_pos(float dist){
		return (direction.normalized * dist) + transform.position;
	}
	public Vector3 cell_to_pos(int cell){
		return dist_to_pos (cell_to_dist (cell));
	}
	[DontSerialize]
	public Subject<int> creation = new Subject<int>();
	[DontSerialize]
	public Subject<int> destruction = new Subject<int>();

	[DontSerialize][Show]
	public ReactiveProperty<int> index = new ReactiveProperty<int>();



	void Start(){
		if (target == null)
			target = Camera.main.transform;
		index.Subscribe ((int i) => {
			fill (i-view_behind, i+view_ahead);
		});
		fill (0 - view_behind, view_ahead);
	}
	protected int _front_index = -1;
	protected int _rear_index = 1;
	[Show]
	public int front_index{
		get{ return _front_index;}
		private set{ 
			_front_index = value;
		}
	}
	[Show]
	public int rear_index{
		get{ return _rear_index;}
		private set{ 
			_rear_index = value;
		}
	}
	public static bool int_between(int i, int min, int max){
		return i >= min && i <= max;
	}
	[Show]
	void fill(int rear, int front){
		if (front != front_index || rear != rear_index) {
			int most_forward = Math.Max (front, front_index);
			int most_backward = Math.Min (rear, rear_index);
			for (int i = most_backward; i <= most_forward; i++) {
				bool in_new_range = int_between (i, rear, front);
				bool in_old_range = int_between (i, rear_index, front_index);
				//Creation - in new range, but not in old one
				if (in_new_range && !in_old_range) {
					creation.OnNext (i);
				}
				//Deletion - in the old range, but not in new one
				if (in_old_range && !in_new_range) {
					destruction.OnNext (i);
				}
			}
		}
		rear_index = rear;
		front_index = front;
	}
	void Update(){
		if (target != null) {
			index.Value = pos_to_cell (target.position);
		}
	}
}
