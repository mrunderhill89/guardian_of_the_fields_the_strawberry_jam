using Vexe.Runtime.Types;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using UniRx;
public class StrawberryComponent : BetterBehaviour {
	[DontSerialize]
	public static bool allow_picking_unpicked = false;

	public double quality = 1.00;

	public Draggable drag = null;
	public Behaviour glow = null;
	public BoolReactiveProperty picked;
	// Use this for initialization
	void Start () {
		picked = new BoolReactiveProperty (false);
		//Disable dragging until we determine whether we can pick up berries yet.
		quality = RandomUtils.random_double(0.5, 1.25);
		drag = gameObject.GetComponent<Draggable>();
		glow = (gameObject.GetComponent("Halo") as Behaviour);
		glow.enabled = false;
		if (drag != null) {
			drag.can_grab = this.can_pick;
			drag.on_pickup.AddListener(() => {
				picked.Value = true;
			});
		}
	}
	public bool can_pick(Vector3 screen_pos){
		return picked.Value || allow_picking_unpicked;
	}

	void OnMouseEnter(){
		glow.enabled = true;
	}

	void OnMouseExit(){
		glow.enabled = false;
	}

	// Update is called once per frame
	void Update () {
	}
}
