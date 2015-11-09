using Vexe.Runtime.Types;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UniRx;

public class BasketComponent : BetterBehaviour {
	public State slot;
	public Transition drop;
	public Transition remove;
	public Transform spawn_point;
	public static List<BasketComponent> baskets = new List<BasketComponent>();
	public OverflowDetector overflow;
	public BasketWeightIndicator weight_text;

	protected bool _locked = false;
	[Show]
	public bool locked{
		get{ return _locked; }
		set{ _locked = value; update_text(); }
	}
	protected bool _second_chance = false;
	[Show]
	public bool second_chance{
		get{ return _second_chance; }
		set{ _second_chance = value; update_text(); }
	}

	protected Dictionary<GameObject, Vector3> valid_positions;
	void Awake () {
		baskets.Add(this);
		slot = NamedBehavior.GetOrCreateComponentByName<State>(gameObject, "slot");
		drop = NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "deposit");
		remove = NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "remove");
		valid_positions = new Dictionary<GameObject, Vector3>();
	}

	void Start(){
		StrawberryStateMachine state_machine = SingletonBehavior.get_instance<StrawberryStateMachine>();
		GameStateManager player_state = SingletonBehavior.get_instance<GameStateManager>();
		if (overflow == null)
			overflow = GetComponentInChildren<OverflowDetector> ();
		slot.chain_parent (state_machine.fsm.state("basket"))
			.on_entry (new StateEvent(ParentToBasket))
			.on_exit (new StateEvent(UnparentToBasket))
			.on_update(new StateEvent(this.UpdatePhysics));
		drop.chain_from(state_machine.fsm.state("fall"))
			.chain_auto_run(false)
			.chain_priority(2)
			.chain_to(slot)
			.add_test(new TransitionTest((Automata a)=>{
				if (a.gameObject.GetComponent<StrawberryComponent>() == null){
					return false;
				}
				return player_state.can_drag() && !this.locked;
			}));
		remove.chain_from(slot)
			.chain_to (state_machine.fsm.state("drag"))
			.chain_auto_run(false)
			.add_test(new TransitionTest(()=>{
				return player_state.can_drag() && !this.locked;
			}));
	}

	void update_text(){
		if (weight_text != null){
			weight_text.update(total_weight);
		}
	}

	void OnDestroy(){
		baskets.Remove(this);
	}

	void ParentToBasket(Automata a){
		a.gameObject.transform.SetParent(transform, true);
		update_text();
	}

	void UnparentToBasket(Automata a){
		a.gameObject.transform.SetParent(null, true);
		update_text();
	}

	[Show]
	public float total_weight{
		get{
			if (slot == null)
				return 0.0f;
			return slot.visitors.Select((Automata a) => {
				StrawberryComponent sb = a.GetComponent<StrawberryComponent> ();
				if (sb == null) return 0.0f;
				return sb.weight;
			}).Aggregate<float,float>(0.0f, (total, next) => {
				return total + next;
			});
		}
	}

	void UpdatePhysics(Automata a){
		Rigidbody body = a.gameObject.GetComponent<Rigidbody> ();
		if (body != null) {
			if (locked || !SingletonBehavior.get_instance<GameStateManager>().basket_physics_enabled()){
				body.isKinematic = true;
			} else {
				body.isKinematic = false;
			}
		}
	}

	void OnTriggerStay(Collider that) {
		GameObject obj = that.gameObject;
		if (obj == null) {
			Debug.LogWarning("Found a collider with no gameobject. Deleting...");
			Destroy(that);
		} else {
			Automata a = obj.GetComponent<Automata> ();
			if (a != null) {
				drop.trigger_single (a);
				valid_positions[obj] = obj.transform.position;
			}
		}
	}

	void OnTriggerExit(Collider that){
		GameObject obj = that.gameObject;
		Automata a = obj.GetComponent<Automata>();
		if (a != null && a.current == slot){
			//GameMessages.Log("Uh-oh, a strawberry fell out of your basket!");
			StrawberryStateMachine state_machine = SingletonBehavior.get_instance<StrawberryStateMachine>();
			state_machine.fsm.transition("basket_fall").trigger_single(a);
			obj.transform.position = valid_positions[obj];
			Rigidbody body = obj.GetComponent<Rigidbody>();
			body.velocity = Vector3.zero;
		} else {
			valid_positions.Remove(obj);
		}
	}

	public bool is_overflow(){
		return overflow.is_overflow();
	}

	public IEnumerable<StrawberryComponent> get_gathered_strawberries(){
		StrawberryComponent next_component;
		foreach(Automata a in slot.visitors){
			next_component = a.GetComponent<StrawberryComponent>();
			if (next_component != null) yield return next_component;
		}
	}
	
	public static IEnumerable<StrawberryComponent> get_all_strawberries(){
		foreach(BasketComponent basket in baskets){
			foreach(StrawberryComponent berry in basket.get_gathered_strawberries()){
				yield return berry;
			}
		}
	}
	
	public static BasketComponent get_lightest_basket(){
		BasketComponent lightest = null;
		foreach(BasketComponent basket in baskets){
			if (lightest == null || basket.total_weight < lightest.total_weight){
				lightest = basket;
			}
		}
		return lightest;
	}
}
