using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;

public class CompassArrow : BetterBehaviour {
	public string direction;
	InputController input;
	UI_Visibility visibility;
	void Start(){
		input = SingletonBehavior.get_instance<InputController> ();
		if (visibility == null){
			visibility = GetComponent<UI_Visibility>();
			if (visibility == null){
				visibility = gameObject.AddComponent<UI_Visibility>();
			}
		}
		visibility.add_element(GetComponent<Image>())
		.add_element(GetComponent<Button>());
		GetComponent<Button>().onClick.AddListener(()=>{
			if (visibility.visible){
				input.invoke_dir(direction);
			}
		});
	}

	public bool should_be_visible(){
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
		visibility.set_visibility(should_be_visible());
	}
}
