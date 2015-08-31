using Vexe.Runtime.Types;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
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