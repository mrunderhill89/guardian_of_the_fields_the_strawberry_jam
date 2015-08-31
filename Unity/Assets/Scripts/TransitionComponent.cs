using Vexe.Runtime.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;

public class TransitionComponent : BetterBehaviour {
	public StateComponent from;
	public StateComponent to;
	public List<Func<bool>> tests;
	void Start(){
	}
	
	public void trigger(){
	}
	
	public UnityEvent on_run;
	public UnityEvent on_trigger;
}