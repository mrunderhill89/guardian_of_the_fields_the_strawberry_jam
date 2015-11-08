using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;

public class CompassArrow : BetterBehaviour {
	public string direction;
	InputController input;
	ObjectVisibility visibility;
	void Start(){
		input = SingletonBehavior.get_instance<InputController> ();
		if (visibility == null){
			visibility = GetComponent<ObjectVisibility>();
		}
		GetComponent<Button>().onClick.AddListener(()=>{
			if (visibility.visible){
				input.invoke_dir(direction);
			}
		});
	}

	public bool should_be_visible(){
		if (input.direction_transitions[direction] != null) {
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
		visibility.visible = should_be_visible();
	}
}
