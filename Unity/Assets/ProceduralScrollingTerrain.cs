using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
public class ProceduralScrollingTerrain : BetterBehaviour {
	public class TerrainFunction {
		public Func<Vector3,float> get_raw_height;
		public AnimationCurve curve = new AnimationCurve();
		public float scale = 1.0f;
		public float get_height(Vector3 position){
			return scale * curve.Evaluate (get_raw_height (position));
		}
	}
	public List<TerrainFunction> functions;
	public Terrain terrain;
	public TerrainData data{
		get{return terrain.terrainData;}
	}
	public Transform target;
	protected Vector3 origin;

	public Vector2 get_cell(Vector3 world){
		Vector3 relative = world - origin;
		if (terrain == null)
			return Vector2.zero;
		return new Vector2 (
			Mathf.Floor(relative.x / data.heightmapScale.x),
			Mathf.Floor(relative.z / data.heightmapScale.z)
		);
	}
	[Show]
	public Vector2 target_cell{
		get{ 
			if (target == null)
				return Vector2.zero;
			return get_cell(target.position); 
		}
	}
	Vector2 current_cell;
	void Start(){
		origin = transform.position;
		current_cell = target_cell;
	}
	void Update () {
		if (current_cell != target_cell) {
			Reconstruct();
			current_cell = target_cell;
		}
	}

	void Reconstruct(){
		transform.localPosition = new Vector3(
			(target_cell.x * data.heightmapScale.x) - data.size.x,
			transform.localPosition.y,
			(target_cell.y * data.heightmapScale.z - data.size.z)
		);
	}
}
