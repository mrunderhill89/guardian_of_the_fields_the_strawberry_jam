using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PaceManager : BetterBehaviour {
	public Dictionary<string, PaceData> paces = new Dictionary<string, PaceData>();
	protected PaceData current_pace;
	// Use this for initialization
	void Start () {
		GameStateManager game_state = SingletonBehavior.get_instance<GameStateManager> ();
		paces ["fast"] = ScriptableObject.CreateInstance<PaceData>()
			.manager(this)
			.speed(0.8f)
			.attach_to_state(game_state.state("look_forward"))
			.attach_to_state(game_state.state("look_left"))
			.attach_to_state(game_state.state("look_right"))
			.attach_to_state(game_state.state("look_backward"))
			.attach_to_state(game_state.state("pack"));
		paces ["medium"] = ScriptableObject.CreateInstance<PaceData>()
			.manager(this)
			.speed(0.5f)
			.attach_to_state(game_state.state("look_forward"))
			.attach_to_state(game_state.state("look_left"))
			.attach_to_state(game_state.state("look_right"));
		paces ["slow"] = ScriptableObject.CreateInstance<PaceData>()
			.manager(this)
			.speed(0.2f)
			.attach_to_state(game_state.state("look_forward"))
			.attach_to_state(game_state.state("look_left"))
			.attach_to_state(game_state.state("look_right"));
		paces ["reverse"] = ScriptableObject.CreateInstance<PaceData>()
			.manager(this)
			.speed(-0.2f)
			.attach_to_state(game_state.state("look_behind"))
			.attach_to_state(game_state.state("look_left"))
			.attach_to_state(game_state.state("look_right"));
		set_pace("slow");
	}
	public bool is_in_pace(PaceData pace){
		return current_pace == pace;
	}
	[Show]
	public void set_pace(string name){
		if (paces.ContainsKey (name))
			current_pace = paces [name];
	}
}