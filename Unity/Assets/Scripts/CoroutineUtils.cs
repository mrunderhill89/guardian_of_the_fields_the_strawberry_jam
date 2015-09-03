using Vexe.Runtime.Types;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UniRx;

public class ActionWrapper{
	protected Action act;
	public ActionWrapper(Action _act){
		act = _act;
	}
	public ActionWrapper(UnityEvent evn){
		act = evn.Invoke;
	}
	public IEnumerator run(){
		act();
		yield return null;
	}
}

public class MultiAction{
	public static IEnumerator run_list(MonoBehaviour runner, IEnumerable<ActionWrapper> actions){
		foreach (ActionWrapper aw in actions) {
			yield return runner.StartCoroutine(aw.run());
		}
		yield return null;
	}
}