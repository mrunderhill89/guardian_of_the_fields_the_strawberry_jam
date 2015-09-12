using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
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
			.Insert<State, MultiComponentDrawer<State>>()
			.Insert<StrawberryRowState, MultiComponentDrawer<StrawberryRowState>>()
			.Insert<Transition, MultiComponentDrawer<Transition>>()
			.Insert<
				ReactiveProperty<State>, 
				ReactivePropertyDrawer<State, MultiComponentDrawer<State>>
			>().Insert<
				ReactiveProperty<List<State>>, 
				ListPropertyDrawer<State>
			>().Insert<
				ReactiveProperty<List<ActionWrapper>>, 
				ListPropertyDrawer<ActionWrapper>
			>().Insert<
				ReactiveProperty<bool>, 
				ReactivePropertyDrawer<bool, BoolDrawer>
			>().Insert<
				ReactiveProperty<int>, 
				ReactivePropertyDrawer<int, IntDrawer>
			>().Insert<
				HashSet<Automata>,
				HashSetDrawer<Automata,ListDrawer<Automata>>
			>();
	}
}

public class ListPropertyDrawer<T>: ReactivePropertyDrawer<List<T>, ListDrawer<T>>{
}

public class HashSetDrawer<T, D>: ObjectDrawer<HashSet<T>> where D:ListDrawer<T>, new(){
	public D sub_draw;
	public EditorMember sub_member;
	public override void OnGUI(){
		if (sub_member == null) {
			sub_member = EditorMember.WrapGetSet (
				() => {
				if (memberValue == null)
					return new List<T> ();
				return memberValue.ToList ();
			}, 
				(value) => {
				}, 
				member.RawTarget, member.UnityTarget, typeof(List<T>), member.Name, member.Id, member.Attributes
			);
		}
		if (sub_draw == null){
			sub_draw = new D();
			sub_draw.Initialize(sub_member, attributes, gui);
		} else {
			sub_draw.OnGUI();
		}
	}
}

public class ReactivePropertyDrawer<T, D>: ObjectDrawer<ReactiveProperty<T>> where D:ObjectDrawer<T>, new(){
	public D sub_draw;
	public EditorMember sub_member;
	public override void OnGUI(){
		if (memberValue == null){
			memberValue = new ReactiveProperty<T>();
		}
		if (sub_member == null){
			sub_member =  EditorMember.WrapGetSet(
				()=>{return memberValue.Value;}, 
				(value)=>{memberValue.Value = (T)value;}, 
				member.RawTarget, member.UnityTarget, typeof(T), member.Name, member.Id, member.Attributes);
		}
		if (sub_draw == null){
			sub_draw = new D();
			sub_draw.Initialize(sub_member, attributes, gui);
		} else {
			sub_draw.OnGUI();
		}
	}
}

public class MultiComponentDrawer<T> : ObjectDrawer<T> where T:NamedBehavior{
	protected UnityEngine.Object game_object;
	protected Dictionary<string,T> components;
	protected List<string> options;
	protected int selection;
	protected string instance_name;
	protected bool fold = false;
	public override void OnGUI(){
		if (memberValue != null){
			game_object = memberValue.gameObject;
			instance_name = memberValue.instance_name;
		}
		string foldout_label = displayText+" "+(memberValue==null?"(null)":"("+memberValue.instance_name+")");
		fold = gui.Foldout(foldout_label, fold);
		if (fold){
			game_object = gui.Object("Game Object", "Select host game object.", game_object);
			components = new Dictionary<string,T>();
			if (game_object != null){
				if (game_object.GetType() == typeof(T) ||game_object.GetType().IsSubclassOf(typeof(T))){
					memberValue = (T)game_object;
					instance_name = memberValue.instance_name;
					game_object = memberValue.gameObject;
				}
				components = NamedBehavior.GetComponentsByName<T>((GameObject)game_object);
			} else {
				memberValue = null;
			}
			options = new List<String>();
			options.Add("None");
			options.AddRange(components.Keys.ToArray());
			selection = Mathf.Clamp(selection, 0, options.Count-1);
			selection = gui.Popup("Available:", selection, options.ToArray());
			instance_name = gui.Text("Write In:", instance_name);
			//Prioritize write-in names, then selections.
			if (instance_name != null && components.ContainsKey(instance_name)){
				memberValue = components[instance_name];
			} else {
				string selected_name = options[selection];
				if (selection > 0 && components.ContainsKey(selected_name)){
					memberValue = components[selected_name];
				} else {
					memberValue = null;
				}
			}
		}
	}
}