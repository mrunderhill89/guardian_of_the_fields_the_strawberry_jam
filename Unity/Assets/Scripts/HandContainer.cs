using UnityEngine;
using System.Collections;

public class HandContainer : MonoBehaviour {
	public StateComponent slot;
	public TransitionComponent take;
	public Behaviour glow = null;
	public Color deselected;
	public Color selected;
	void Awake () {
		slot = NamedBehavior.GetOrCreateComponentByName<StateComponent>(gameObject, "slot");
		take = NamedBehavior.GetOrCreateComponentByName<TransitionComponent>(gameObject, "take");
		glow = (gameObject.GetComponent("Halo") as Behaviour);
		glow.enabled = false;
		take.auto_run = false;
	}
	void Start(){
		StrawberryStateMachine state_machine = SingletonBehavior.get_instance<StrawberryStateMachine>();
		slot.parent = state_machine.states["hold"];
		take.from_state = state_machine.states["drag"];
		take.to_state = slot;
		take.generate_path();
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
		take.trigger();
	}
}
