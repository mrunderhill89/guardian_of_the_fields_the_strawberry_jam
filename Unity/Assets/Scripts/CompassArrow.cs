using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;

public class CompassArrow : BetterBehaviour {

	public string direction;
	InputController input;
	void Start(){
		input = SingletonBehavior.get_instance<InputController> ();
	}

	public bool is_visible(){
		if (input.direction_transitions [direction] != null) {
			IList<Transition> transitions = input.direction_transitions[direction];
			if (transitions.Count > 0){
				foreach(Transition trans in transitions){
					if (trans.is_visited()){
						return true;
					}
				}
			}
		}
		return false;
	}
	// Update is called once per frame
	void Update () {
		gameObject.GetComponent<Renderer> ().enabled = is_visible ();
	}
}
