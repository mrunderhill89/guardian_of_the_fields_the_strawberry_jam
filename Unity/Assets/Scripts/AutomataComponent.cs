using Vexe.Runtime.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using StateMove = Vexe.Runtime.Types.Tuple<StateComponent,StateComponent>;

public class AutomataComponent : BetterBehaviour {
	public ReactiveProperty<StateComponent> current;
	[DontSerialize]
	public IObservable<StateMove> on_move;
	[DontSerialize]
	public IObservable<StateComponent> on_enter;
	[DontSerialize]
	public IObservable<StateComponent> on_exit;
	[DontSerialize]
	public ReadOnlyReactiveProperty<List<StateComponent>> parents;
	// Use this for initialization
	void Start () {
		on_move = current.Scan (new StateMove(null,null), (StateMove last_move, StateComponent next) => {
			return new StateMove(last_move.Item2,next);
		}).Where((tuple)=>{ return tuple.Item1 != tuple.Item2;});
		on_exit = on_move.Select ((tuple) => {
			return tuple.Item1;
		}).Where((state)=>{
			return state != null;
		});
		on_enter = on_move.Select ((tuple) => {
			return tuple.Item2;
		}).Where((state)=>{
			return state != null;
		});
		/*
		parents = current.SelectMany ((state) => {

		});*/
	}

	// Update is called once per frame
	void Update () {
	}
}