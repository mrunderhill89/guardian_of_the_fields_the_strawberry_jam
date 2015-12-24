using Vexe.Runtime.Types;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UniRx;

public class BasketComponent : BetterBehaviour {
	[DontSerialize]
	public static ReactiveCollection<BasketComponent> rx_baskets = new ReactiveCollection<BasketComponent>();
	public static List<BasketComponent> baskets{
		get{ return rx_baskets.ToList();}
		private set{ rx_baskets.SetRange(value); }
	}

	public State slot;
	public Transition drop;
	public Transition remove;
	public Transform spawn_point;
	public OverflowDetector overflow;
	public BasketWeightIndicator weight_text;
	[DontSerialize]
	public GameScores.BasketSingleScore score_data;

	public float total_weight {
		get{ return score_data.weight; }
	}

	protected Dictionary<GameObject, Vector3> valid_positions;
	void Awake () {
		slot = NamedBehavior.GetOrCreateComponentByName<State>(gameObject, "slot");
		drop = NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "deposit");
		remove = NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "remove");
		valid_positions = new Dictionary<GameObject, Vector3>();
		score_data = new GameScores.BasketSingleScore();
		score_data.id = rx_baskets.Count+1;
		rx_baskets.Add(this);
	}

	void Start(){
		StrawberryStateMachine state_machine = StrawberryStateMachine.main;
		GameStateManager player_state = GameStateManager.main;
		if (overflow == null)
			overflow = GetComponentInChildren<OverflowDetector> ();
		overflow.on_panic(()=>{
			score_data.is_overflow = true;
		}).on_relax(()=>{
			score_data.is_overflow = false;
		});
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
				return player_state.can_drag();
			}));
		remove.chain_from(slot)
			.chain_to (state_machine.fsm.state("drag"))
			.chain_auto_run(false)
			.add_test(new TransitionTest(()=>{
				return player_state.can_drag();
			}));
	}
	void OnDestroy(){
		rx_baskets.Remove(this);
	}

	void ParentToBasket(Automata a){
		a.gameObject.transform.SetParent(transform, true);
		update_stats();
	}

	void UnparentToBasket(Automata a){
		a.gameObject.transform.SetParent(null, true);
		update_stats();
	}

	void update_stats(){
		score_data.weight = slot.visitors.Select((Automata a) => {
			StrawberryComponent sb = a.GetComponent<StrawberryComponent> ();
			if (sb == null) return 0.0f;
			return sb.weight;
		}).Aggregate<float,float>(0.0f, (total, next) => {
			return total + next;
		});
		score_data.count = slot.count();
	}

	public bool is_overweight(){
		return score_data.is_overweight(GameSettingsComponent.working_rules.win_condition.max_basket_weight);
	}

	public bool is_underweight(){
		return score_data.is_underweight(GameSettingsComponent.working_rules.win_condition.min_basket_weight);
	}

	public bool is_overflow(){
		return overflow.is_overflow();
	}

	void UpdatePhysics(Automata a){
		Rigidbody body = a.gameObject.GetComponent<Rigidbody> ();
		if (body != null) {
			if (!GameStateManager.main.basket_physics_enabled()){
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
			StrawberryStateMachine state_machine = StrawberryStateMachine.main;
			state_machine.fsm.transition("basket_fall").trigger_single(a);
			obj.transform.position = valid_positions[obj];
			Rigidbody body = obj.GetComponent<Rigidbody>();
			body.velocity = Vector3.zero;
		} else {
			valid_positions.Remove(obj);
		}
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
			if (lightest == null || basket.score_data.weight < lightest.score_data.weight){
				lightest = basket;
			}
		}
		return lightest;
	}
}
