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
		).add_child (
			fsm.state ("scores")
		).add_child (
			fsm.state ("settings")
		).add_child (
			fsm.state ("credits")
		).add_child (
			fsm.state ("language")
			, true
		).add_child (
			fsm.state ("controls")
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
		}).new_transition("main->credits", t => {
			t.chain_from(fsm.state("main"))
				.chain_to(fsm.state("credits"))
					.chain_auto_run(false);
		}).new_transition("credits->main", t => {
			t.chain_from(fsm.state("credits"))
				.chain_to(fsm.state("main"))
					.chain_auto_run(false);
		}).new_transition("main->language", t => {
			t.chain_from(fsm.state("main"))
				.chain_to(fsm.state("language"))
					.chain_auto_run(false);
		}).new_transition("language->main", t => {
			t.chain_from(fsm.state("language"))
				.chain_to(fsm.state("main"))
					.chain_auto_run(false);
		}).new_transition("main->controls", t => {
			t.chain_from(fsm.state("main"))
				.chain_to(fsm.state("controls"))
					.chain_auto_run(false);
		}).new_transition("controls->main", t => {
			t.chain_from(fsm.state("controls"))
				.chain_to(fsm.state("main"))
					.chain_auto_run(false);
		});
		fsm.new_automata ("player", (a) => {
			a.move_direct (fsm.state ("root"));
		});
	}
	
}
