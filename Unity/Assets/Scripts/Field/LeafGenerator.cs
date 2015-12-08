using UnityEngine;
using Vexe.Runtime.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;

public class LeafGenerator : BetterBehaviour {
	public int min_leaves = 5, max_leaves = 20, num_leaves = 0;
	public Vector2 radius = new Vector2(0.1f,0.25f);
	public float crown_height = 0.25f;
	
	public Vector3 twist_min = new Vector3(0.0f,0.0f,0.0f);
	public Vector3 twist_max = new Vector3(0.0f,0.0f,0.0f);
	
	public State state;

	public ObjectVisibility billboard = null;
	
	[DontSerialize]
	public static LeafGenerator closest = null;
	[DontSerialize]
	public static LeafGenerator furthest = null;

	public bool is_full{
		get{ return state.count() >= visible_leaves;}
	}
	public bool is_empty{
		get{ return state.count() <= 0;}
	}
	
	public float billboard_percent = 0.5f;
	public bool show_billboard{
		get{ return state.count() <= Mathf.FloorToInt(visible_leaves * billboard_percent);}
	}
	
	public float distance{
		get{ return Vector3.Distance(transform.position, Camera.main.transform.position); }
	}
	public static float max_distance = 100.0f;
	public static AnimationCurve vis_curve;

	[Show]
	public int visible_leaves{
		get{
			float rel_dist = Mathf.Clamp(distance/max_distance, 0.0f, 1.0f);
			if (vis_curve != null){
				rel_dist = vis_curve.Evaluate(rel_dist);
			}
			return Mathf.CeilToInt(num_leaves * (1.0f - rel_dist));
		}
	}
	
	void Start () {
		num_leaves = RandomUtils.random_int(min_leaves,max_leaves);
		state = State.GetState(gameObject, state)
			.chain_parent(LeafStateSystem.instance.fsm.state("root"))
			.on_entry(new StateEvent((Automata a)=>{
				GameObject leaf = a.gameObject;
				leaf.transform.SetParent(transform);
				float r = RandomUtils.random_float(0.0f,360.0f);
				leaf.transform.localPosition = CylinderToCube(
					r * Mathf.Deg2Rad,
					RandomUtils.random_float(radius),
					crown_height
				);
				leaf.transform.localRotation = Quaternion.Euler(
					RandomUtils.random_vec3(twist_min,twist_max)+
					new Vector3(0.0f, r, 0.0f)
				);
				ObjectVisibility vis = a.GetComponent<ObjectVisibility>();
				vis.visible = true;
				DisplaceTransform dis = a.GetComponent<DisplaceTransform>();
				dis.read().write();
			}));
	}

	void Update (){
		if (!is_full){
			if (closest == null ||
				closest.is_full ||
				closest.distance > distance
			){
				closest = this;
			}
		}
		if (billboard != null){
			billboard.visible = show_billboard;
		}
		if (!is_empty){
			if (furthest == null ||
				furthest.is_empty ||
				furthest.distance < distance
			){
				furthest = this;
			}
		}
	}

	public static Vector3 CylinderToCube(float r, float d, float h){
		return new Vector3(
			Mathf.Cos(r) * d,
			h,
			Mathf.Sin(r) * d
		);
	}
}
