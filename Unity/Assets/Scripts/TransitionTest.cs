using Vexe.Runtime.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UniRx;

using FullTest = System.Func<Automata,Transition,bool>;
using AutomataTest = System.Func<Automata,bool>;
using Test = System.Func<bool>;

public class TransitionTest
{

	#region Attributes

	private FullTest full;
	private AutomataTest automata;
	private Test neutral;

	#endregion


	#region Public methods
	public TransitionTest(FullTest test){
		full = test;
	}
	public TransitionTest(AutomataTest test){
		automata = test;
	}
	public TransitionTest(Test test){
		neutral = test;
	}

	public bool run(Automata a, Transition trans)
	{
		if (full != null){
			if (!full(a,trans)) return false;
		}
		if (automata != null){
			if (!automata(a)) return false;
		}
		if (neutral != null){
			if (!neutral()) return false;
		}
		return true;
	}

	#endregion


}

