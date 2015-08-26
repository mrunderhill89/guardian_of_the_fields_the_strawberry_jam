using System;
using UnityEditor;
using Vexe.Editor;
using Vexe.Editor.Drawers;
using Vexe.Editor.Types;
using Vexe.Runtime.Types;
using System.Collections.Generic;
using System.ComponentModel;
using UniRx;

[InitializeOnLoad]
public static class CustomMapper
{
	static CustomMapper()
	{
		MemberDrawersHandler.Mapper
			.Insert<ReactiveProperty<Reactive_HFSM>, StateReferenceDrawer> ()
			.Insert<IObservable<bool>, ObservableDrawer<Boolean>> ()
			.Insert<ReactiveProperty<bool>, ReactivePropertyDrawer<Boolean>> ()
			.Insert<IObservable<int>, ObservableDrawer<int>> ()
			.Insert<ReactiveProperty<int>, ReactivePropertyDrawer<int>> ()
			.Insert<IObservable<string>, ObservableDrawer<String>> ()
			.Insert<ReactiveProperty<string>, ReactivePropertyDrawer<String>> ()
			.Insert<IObservable<List<Action>>, ObservableListDrawer<List<System.Action>,Action>> ()
			.Insert<IObservable<List<Reactive_HFSM>>, ObservableListDrawer<List<Reactive_HFSM>,Reactive_HFSM>> ()
			;
	}
}

public class ObservableDrawer<T> : ObjectDrawer<IObservable<T>>{
	T displayValue = default(T);
	public override void OnGUI()
	{
		if (memberValue != null) {
			memberValue.Subscribe ((value) => {
				displayValue = value;
			});
			if (!displayValue.Equals(default(T))) {
				gui.Label (displayText + ":" + displayValue.ToString ());
			} else {
				gui.Label (displayText + ":" + default(T).ToString ());
			}
		} else {
			gui.Label(displayText+": observable not set.");
		}
	}
}

public class ObservableListDrawer<T,L> : ObjectDrawer<IObservable<T>> where T:IList<L>{
	T displayList = default(T);
	public override void OnGUI()
	{
		if (memberValue != null) {
			memberValue.Subscribe ((value) => {
				displayList = value;
			});
			if (displayList != null) {
				gui.Label (displayText + ": count = " + displayList.Count);
			} else {
				gui.Label (displayText + ": null");
			}
		} else {
			gui.Label(displayText+": observable not set.");
		}
	}
}

public class ReactivePropertyDrawer<T> : ObjectDrawer<ReactiveProperty<T>> {
	public override void OnGUI()
	{
		if (memberValue == null)
			memberValue = new ReactiveProperty<T>();

		string text = gui.Text(displayText, memberValue.Value.ToString());
		var converter = TypeDescriptor.GetConverter(typeof(T));
		if(converter != null)
		{
			//Cast ConvertFromString(string text) : object to (T)
			memberValue.Value = (T)converter.ConvertFromString(text);
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