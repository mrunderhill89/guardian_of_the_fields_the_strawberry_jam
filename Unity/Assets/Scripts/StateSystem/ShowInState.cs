using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;

public class ShowInState : BetterBehaviour {
	public ObjectVisibility vis;
	public StateMachine fsm;
	public Dictionary<string,ObjectVisibility.VisibilityStatus> state_map = 
		new Dictionary<string, ObjectVisibility.VisibilityStatus>(); 
	public ObjectVisibility.VisibilityStatus default_status = ObjectVisibility.VisibilityStatus.FollowParent;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (vis != null && fsm != null) {
			vis.status = fsm.match<ObjectVisibility.VisibilityStatus>(state_map,default_status);
		}
	}
}
