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
		take.auto_run(false);
	}
	void Start(){
		StrawberryStateMachine state_machine = SingletonBehavior.get_instance<StrawberryStateMachine>();
		slot.parent = state_machine.states["hold"];
		take.from(state_machine.states["drag"])
			.priority(2)
			.to(slot)
			.generate_path();
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
