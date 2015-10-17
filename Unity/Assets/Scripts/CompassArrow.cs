using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;

public class CompassArrow : BetterBehaviour {
	public string direction;
	protected bool _hidden = false;
	InputController input;
	void Start(){
		input = SingletonBehavior.get_instance<InputController> ();
		GetComponent<Button>().onClick.AddListener(()=>{
			if (! this._hidden){
				input.invoke_dir(direction);
			}
		});
	}

	public bool should_be_hidden(){
		if (input.direction_transitions [direction] != null) {
			IList<Transition> transitions = input.direction_transitions[direction];
			if (transitions.Count > 0){
				foreach(Transition trans in transitions){
					if (trans.is_visited()){
						return false;
					}
				}
			}
		}
		return true;
	}
	public bool hidden(){return _hidden;}
	[Show]
	public CompassArrow hidden(bool value){
		_hidden = value;
		GetComponent<Image>().enabled = !value;
		GetComponent<Button>().enabled = !value;
		return this;
	}
	// Update is called once per frame
	void Update () {
		hidden(should_be_hidden());
	}
}
