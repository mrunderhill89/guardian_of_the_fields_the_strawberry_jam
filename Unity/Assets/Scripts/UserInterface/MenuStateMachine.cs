﻿using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;

public class MenuStateMachine : BetterBehaviour {
	public StateMachine fsm;
	// Use this for initialization
	void Start () {
		if (fsm == null)
			fsm = GetComponent<StateMachine> ();
		fsm.state ("root")
		.add_child (
			fsm.state ("main")
			, true
		).add_child (
			fsm.state ("scores")
		).add_child (
			fsm.state ("settings")
		);
		fsm.new_transition("main->scores", t => {
			t.chain_from(fsm.state("main"))
				.chain_to(fsm.state("scores"))
					.chain_auto_run(false);
		}).new_transition("scores->main", t => {
			t.chain_from(fsm.state("scores"))
				.chain_to(fsm.state("main"))
					.chain_auto_run(false);
		}).new_transition("main->settings", t => {
			t.chain_from(fsm.state("main"))
				.chain_to(fsm.state("settings"))
					.chain_auto_run(false);
		}).new_transition("settings->main", t => {
			t.chain_from(fsm.state("settings"))
				.chain_to(fsm.state("main"))
					.chain_auto_run(false);
		});
		fsm.new_automata ("player", (a) => {
			a.move_direct (fsm.state ("root"));
		});
	}
	
}