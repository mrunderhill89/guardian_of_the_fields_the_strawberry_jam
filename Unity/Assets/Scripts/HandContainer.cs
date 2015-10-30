using UnityEngine;
using System.Collections;

public class HandContainer : MonoBehaviour {
	public State slot;
	public Transition take;
	public Behaviour glow = null;
	public Color deselected;
	public Color selected;
	void Awake () {
		slot = NamedBehavior.GetOrCreateComponentByName<State>(gameObject, "slot");
		take = NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "take");
		glow = (gameObject.GetComponent("Halo") as Behaviour);
		glow.enabled = false;
	}
	void Start(){
		StrawberryStateMachine berry_state = SingletonBehavior.get_instance<StrawberryStateMachine>();
		slot.chain_parent(berry_state.fsm.state("hold"));
		take.chain_from(berry_state.fsm.state("drag"))
			.chain_to(slot)
			.chain_priority(2)
			.chain_auto_run(false);
	}
	void Update(){
	}
	void OnMouseEnter(){
		glow.enabled = true;
	}
	void OnMouseExit(){
		glow.enabled = false;
	}
	void OnMouseUp() {
		Debug.Log("MouseUp on hand container slot.");
		take.trigger();
	}
}
