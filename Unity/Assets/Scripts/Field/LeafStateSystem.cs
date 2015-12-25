using UnityEngine;
using Vexe.Runtime.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;

public class LeafStateSystem : BetterBehaviour {
	public static LeafStateSystem instance = null;
	public StateMachine fsm;
	public string prefab = "stem";
	[DontSerialize]
	public UnityEngine.Object loaded_prefab;

	public int max_leaves = 500;
	public float load_percent = 0.75f;
	public int leaves_per_frame = 5;
	public int num_leaves{
		get{
			if (fsm != null && fsm.has_state("root"))
				return fsm.state("root").count();
			return 0;
		}
	}
	public int unassigned_leaves{
		get{
			if (fsm != null && fsm.has_state("root"))
				return fsm.state("root").count_own();
			return 0;
		}
	}
	
	public float loading_progress{
		get{ 
			if (load_percent == 0.0f) return 1.0f;
			return num_leaves / (max_leaves * load_percent); 
		}
	}
	public bool finished_loading{
		get{ return num_leaves >= max_leaves * load_percent;}
	}
	
	void Awake () {
		instance = this;
		fsm = StateMachine.GetMachine(gameObject, fsm);
		fsm.state("root").on_entry(new StateEvent((Automata a)=>{
			InitializeLeaf(a.gameObject);
		}));
		loaded_prefab = Resources.Load(prefab);
	}
		
	void InitializeLeaf(GameObject obj){
		ObjectVisibility vis = obj.GetComponent<ObjectVisibility>();
		vis.visible = false;
		obj.transform.SetParent(null);
	}
	
	GameObject create_leaf(){
		GameObject leaf = GameObject.Instantiate(loaded_prefab) as GameObject;
		fsm.automata(leaf.name, leaf.GetComponent<Automata>()).move_direct(fsm.state("root"));
		return leaf;
	}
	
	void Update () {
		for (int l = 0; l < leaves_per_frame; l++){
			if (num_leaves < max_leaves){
				create_leaf();
			}
			if (LeafGenerator.closest != null && !LeafGenerator.closest.is_full){
				Automata next_leaf = null;
				if (unassigned_leaves > 0){
					next_leaf = fsm.state("root").own_visitors().First();
				} else if (LeafGenerator.furthest != null 
					&& LeafGenerator.furthest != LeafGenerator.closest
					&& !LeafGenerator.furthest.is_empty){
					next_leaf = LeafGenerator.furthest.state.own_visitors().First();
					next_leaf.eject();
					InitializeLeaf(next_leaf.gameObject);
				}
				if (next_leaf != null){
					next_leaf.move_direct(LeafGenerator.closest.state);
				}
			}
		}
	}
}
