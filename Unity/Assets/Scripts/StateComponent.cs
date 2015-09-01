using Vexe.Runtime.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using StateChain = System.Collections.Generic.List<StateComponent>;
public class StateComponent : NamedBehavior {
	public ReactiveProperty<StateComponent> parent;
	public ReactiveProperty<StateComponent> initial;
	public ReactiveProperty<StateComponent> current;

	public List<AutomataComponent> visitors;
	[DontSerialize]
	public Subject<AutomataComponent> coming;
	[DontSerialize]
	public Subject<AutomataComponent> going;

}