using Vexe.Runtime.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
public class TransitionSorter : BetterBehaviour {
	protected List<TransitionComponent> transitions;
	// Use this for initialization
	void Awake () {
		transitions = new List<TransitionComponent>();
	}
	public TransitionSorter Add(TransitionComponent trans){
		return this;
	}
	// Update is called once per frame
	void Update () {
	}
}
