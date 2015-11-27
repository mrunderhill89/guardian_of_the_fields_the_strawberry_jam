using Vexe.Runtime.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UniRx;

using FullAction = System.Action<Automata,State>;
using AutomataAction = System.Action<Automata>;
using StateAction = System.Action<State>;

public class StateEvent
{

	#region Attributes

	private FullAction full;
	private AutomataAction automata;
	private StateAction state;
	private Action neutral;
	private int times_ran = 0;
	private int limit = -1;
	#endregion


	#region Public methods

	public StateEvent(FullAction act){
		full = act;
	}
	public StateEvent(AutomataAction act){
		automata = act;
	}
	public StateEvent(StateAction act){
		state = act;
	}
	public StateEvent(Action act){
		neutral = act;
	}

	public StateEvent Limit(int times){
		limit = times;
		return this;
	}

	public void run(Automata a, State s)
	{
		if (limit < 0 || times_ran < limit){
			if (full != null){
				full(a,s);
			}
			if (automata != null){
				automata(a);
			}
			if (state != null){
				state(s);
			}
			if (neutral != null){
				neutral();
			}
			times_ran++;
		}
	}

	#endregion


}

