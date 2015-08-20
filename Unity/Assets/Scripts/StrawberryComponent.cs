using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UniRx;
public class StrawberryComponent : MonoBehaviour {
	public static Subject<StrawberryComponent> selected;
	public static IObservable<StrawberryComponent> currently_held;
	public static bool allow_selection = false;
	static StrawberryComponent(){
		selected = new Subject<StrawberryComponent> ();
		currently_held = selected.Scan<StrawberryComponent, StrawberryComponent>(null, (orig, next) => {
			if (next == null){
				//if (orig != null){orig.drag.allow_dragging=false;}
				return null;
			}
			if (orig != null || !allow_selection) {
				return orig;
			}
			//next.drag.allow_dragging=true;
			return next;}
		);
		currently_held.Subscribe ((component) => {
		});
	}

	public double quality = 1.00;
	protected Subject<bool> set_picked;	
	public IObservable<bool> is_picked;

	public Subject<GameObject> container;
	public Draggable drag = null;

	// Use this for initialization
	void Start () {
		//Disable dragging until we determine whether we can pick up berries yet.
		quality = RandomUtils.random_double(0.5, 1.25);
		set_picked = new Subject<bool>();
		set_picked.OnNext(false);
		is_picked = set_picked.AsObservable ();
	}

	// Update is called once per frame
	void Update () {
	}

	void OnMouseDown(){
		selected.OnNext (this);
		set_picked.OnNext (true);
	}
	void OnMouseUp(){
		selected.OnNext (null);
	}
}
