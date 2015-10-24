using UnityEngine;
using UnityEngine.UI;
using  UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
public class UI_Visibility : BetterBehaviour {
	protected HashSet<UIBehaviour> elements = new HashSet<UIBehaviour>();
	protected bool _visible = true;
	[Show]
	public bool visible{
		get{return _visible;}
		set{ set_visibility(value); }
	}
	[Show]
	public int Count{
		get{return elements.Count;}
	}
	public UI_Visibility set_visibility(bool value){
		_visible = value;
		foreach(UIBehaviour element in elements){
			element.enabled = value;
		}
		return this;
	}
	public UI_Visibility add_element(UIBehaviour element){
		elements.Add(element);
		element.enabled = _visible;
		return this;
	}
	
	public UI_Visibility add_element(UIBehaviour[] _elements){
		foreach(UIBehaviour element in _elements){
			add_element(element);
		}
		return this;
	}
}
