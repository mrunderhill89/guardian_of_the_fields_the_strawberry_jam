using System;
using UnityEditor;
using Vexe.Editor;
using Vexe.Editor.Drawers;
using Vexe.Editor.Types;
using Vexe.Runtime.Types;
using System.Collections.Generic;
using UniRx;

[InitializeOnLoad]
public static class CustomMapper
{
	static CustomMapper()
	{
		MemberDrawersHandler.Mapper
			.Insert<IObservable<bool>, ObservablePropertyDrawer<bool>>()
			.Insert<IObservable<int>, ObservablePropertyDrawer<int>>()
			.Insert<ReactiveProperty<Reactive_HFSM>, StateReferenceDrawer>()
			.Insert<IObservable<Reactive_HFSM[][]>, TransitionPathDrawer>();
	}
}

public class ObservablePropertyDrawer<T> : ObjectDrawer<IObservable<T>>{
	T displayValue = default(T);
	public override void OnGUI()
	{
		if (memberValue != null) {
			memberValue.Subscribe ((value)=>{
				displayValue = value;
			});
			gui.Label(displayText+":"+displayValue.ToString());
		} else {
			gui.Label(displayText+": Observable not set.");
		}
	}
}

public class ObjectReactivePropertyDrawer<T> : ObjectDrawer<ReactiveProperty<T>> where T:UnityEngine.Object{
	public override void OnGUI()
	{
		if (memberValue == null)
			memberValue = new ReactiveProperty<T>();

		memberValue.Value = (T)gui.DraggableObject<T>(displayText, "Reactive Value", memberValue.Value);
	}
}

public class TransitionPathDrawer : ObjectDrawer<IObservable<Reactive_HFSM[][]>>{
	public override void OnGUI()
	{
	}
}

public class StateReferenceDrawer : ObjectDrawer<ReactiveProperty<Reactive_HFSM>>
{
	public string state_name;
	public override void OnGUI()
	{
		if (memberValue == null)
			memberValue = new ReactiveProperty<Reactive_HFSM>();

		ReactiveProperty<Reactive_HFSM> prop = memberValue;
		if (prop.Value == null) {
			state_name = "null";
		} else {
			state_name = prop.Value.name;
		}
		prop.Value = (Reactive_HFSM)gui.DraggableObject<Reactive_HFSM>(displayText+" (name:"+state_name+")", "Select a reactive HFSM.", prop.Value);
	}
}