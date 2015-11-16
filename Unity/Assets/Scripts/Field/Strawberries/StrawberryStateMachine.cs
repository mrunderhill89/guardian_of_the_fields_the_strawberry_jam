using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class StrawberryStateMachine : SingletonBehavior {
	public static Vector3 nowhere = new Vector3(-100.0f,-100.0f,-100.0f);
	[DontSerialize]
	public StateMachine fsm;
	public int field_strawberries = 100;
	public GameStateManager player_state;
	new void Awake () {
		base.Awake();
		fsm = gameObject.AddComponent<StateMachine>();
		fsm.state ("root")
			.add_child (
				fsm.state ("field")
				.initial_function(()=>{
					return StrawberryRowState.random_row();
				})
				.on_entry(new StateEvent((Automata a)=>{
					StrawberryComponent sb = a.gameObject.GetComponent<StrawberryComponent>();
					sb.Initialize();
				}))
				, true
			).add_child (
				fsm.state ("drag")
			).add_child (
				fsm.state ("fall")
				.on_entry(new StateEvent((Automata a)=>{
					Rigidbody body = a.GetComponent<Rigidbody>();
					if (body != null){
						body.useGravity = true;
						body.isKinematic = false;
					}
				}))
			).add_child (
				fsm.state ("hand")
				.on_entry(new StateEvent((Automata a)=>{
					Rigidbody body = a.GetComponent<Rigidbody>();
					if (body != null){
						body.useGravity = false;
						body.isKinematic = true;
					}
				}))
			).add_child (
				fsm.state ("basket")
			);
		fsm.new_transition("field_drag", (t)=>{
			t.chain_from(fsm.state("field"))
			.chain_to (fsm.state("drag"))
			.add_test(new TransitionTest(()=>{
				return player_state.can_pick();
			}))
			.on_exit(new TransitionEvent(()=>{
				GenerateStrawberries(1);
			}))
			.chain_auto_run(false)
			;
		}).new_transition("fall_drag", (t)=>{
			t.chain_from(fsm.state("fall"))
			.chain_to (fsm.state("drag"))
			.chain_auto_run(false)
			.add_test(new TransitionTest(()=>{
				return player_state.can_drag();
			}))
					;
		}).new_transition("hand_drag", (t)=>{
			t.chain_from(fsm.state("hand"))
			.chain_to (fsm.state("drag"))
			.chain_auto_run(false)
			.add_test(new TransitionTest(()=>{
				return player_state.can_drag();
			}))
			;
		}).new_transition("basket_fall", (t)=>{
			t.chain_from(fsm.state("basket"))
			.chain_to (fsm.state("fall"))
			.chain_auto_run(false)
			;
		}).new_transition("drag_fall", (t)=>{
			t.chain_from(fsm.state("drag"))
			.chain_to (fsm.state("fall"))
			.chain_auto_run(false)
			;
		}).new_transition("spawn_direct", (t)=>{
			t.chain_from(fsm.state("root"))
			.chain_to (fsm.state("fall"))
			.chain_auto_run(false)
			.on_exit(new TransitionEvent((Automata a)=>{
				a.GetComponent<StrawberryComponent>().Initialize();
				a.GetComponent<ObjectVisibility>().visible = true;
			}))
			;
		});

	}
	void Start(){
		player_state = SingletonBehavior.get_instance<GameStateManager> ();
		GenerateStrawberries(field_strawberries);
	}
	public bool finished_loading(){
		State field = fsm.state("field");
		return field.is_visited() && !field.has_travellers();
	}

	public void GenerateStrawberries(int num){
		GameObject berry;
		for (int u = 0; u < num; u++){
			berry = GameObject.Instantiate(Resources.Load ("Strawberry")) as GameObject;
			berry.transform.position = nowhere;
			string berry_name = berry.name+":"+(fsm.count_automata()+1).ToString();
			fsm.automata(berry_name, berry.GetComponent<Automata>())
				.move_direct(fsm.state("root"));
		}
	}
	[Show]
	public void GenerateStrawberryAtPlayer(){
		GameObject berry = GameObject.Instantiate(Resources.Load ("Strawberry")) as GameObject;
			berry.transform.position = BasketComponent.get_lightest_basket().spawn_point.position;
			string berry_name = berry.name+":"+(fsm.count_automata()+1).ToString();
			fsm.transition("spawn_direct").trigger_single(fsm.automata(berry_name, berry.GetComponent<Automata>()));
	}
	public IEnumerable<StrawberryComponent> get_strawberries(string state_name){
		StrawberryComponent next_component;
		foreach(Automata a in fsm.state(state_name).visitors){
			next_component = a.GetComponent<StrawberryComponent>();
			if (next_component != null) yield return next_component;
		}
	}
	public class StrawberryScoreData{
		public int ripe = 0;
		public int overripe = 0;
		public int underripe = 0;
		public int undersize = 0;
		public void reset(){
			ripe = 0;
			overripe = 0;
			underripe = 0;
			undersize = 0;
		}
	}
	protected Dictionary<string, StrawberryScoreData> saved_scores = new Dictionary<string,StrawberryScoreData>();
	public StrawberryScoreData get_score_data(string state_name){
		if (!saved_scores.ContainsKey(state_name))
			saved_scores[state_name] = new StrawberryScoreData();
		if (!lock_scores){
			saved_scores[state_name].reset();
			if (fsm != null){
				foreach (StrawberryComponent berry in get_strawberries(state_name)){
					if (berry.is_under_size()){
						saved_scores[state_name].undersize++;
					}
					if (berry.is_over_ripe()){
						saved_scores[state_name].overripe++;
					} else if (berry.is_under_ripe()){
						saved_scores[state_name].underripe++;
					} else if (!berry.is_under_size()){
						saved_scores[state_name].ripe++;
					}
				}
			}
		}
		return saved_scores[state_name];
	}
	public bool lock_scores = false;
	[Show]
	public StrawberryScoreData gathered{
		get{
			return get_score_data("basket");
		}
	}
	[Show]
	public StrawberryScoreData dropped{
		get{
			return get_score_data("fall");
		}
	}
}
