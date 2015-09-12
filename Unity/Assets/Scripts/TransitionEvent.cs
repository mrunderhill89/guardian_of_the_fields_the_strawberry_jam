using Vexe.Runtime.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UniRx;

using FullAction = System.Action<Automata,Transition>;
using AutomataAction = System.Action<Automata>;
using TransitionAction = System.Action<State>;

public class TransitionEvent
{

	#region Attributes

	private FullAction full;
	private AutomataAction automata;
	private Action neutral;

	#endregion


	#region Public methods
	
	public TransitionEvent(FullAction act){
		full = act;
	}
	public TransitionEvent(AutomataAction act){
		automata = act;
	}
	public TransitionEvent(Action act){
		neutral = act;
	}

	public void run(Automata a, Transition trans)
	{
		if (full != null){
			full(a,trans);
		}
		if (automata != null){
			automata(a);
		}
		if (neutral != null){
			neutral();
		}
	}

	#endregion


}

