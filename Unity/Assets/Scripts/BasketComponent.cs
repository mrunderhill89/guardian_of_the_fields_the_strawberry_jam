using UnityEngine;
using System.Collections;

public class BasketComponent : MonoBehaviour {
	public State slot;
	public Transition drop;
	void Awake () {
		slot = NamedBehavior.GetOrCreateComponentByName<State>(gameObject, "slot");
		drop = NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "drop");
	}

	void Start(){
		StrawberryStateMachine state_machine = SingletonBehavior.get_instance<StrawberryStateMachine>();
		slot.parent(state_machine.states["basket"]);
		drop.from(state_machine.states["fall"])
			.priority(2)
			.to(slot)
			.add_test(new TransitionTest((Automata a)=>{
				if (a.gameObject.GetComponent<StrawberryComponent>() == null){
					Debug.LogWarning("Colliding with non-strawberry object:"+a.name);
					return false;
				}
				Debug.Log("Colliding with strawberry:"+a.name);
				return true;
			}))
			.on_entry(new TransitionEvent(()=>{
				Debug.Log("Depositing Strawberry in:"+name);
			}))
			.generate_path();
	}

	void OnTriggerStay(Collider that) {
		Automata a = that.gameObject.GetComponent<Automata>();
		if (a != null){
			drop.trigger_single(a);
		} else {
			Debug.LogWarning("Colliding with non-automata-equipped collider:"+that.name);
		}
	}

	void Update () {
	
	}
}
