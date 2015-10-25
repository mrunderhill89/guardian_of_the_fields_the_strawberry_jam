using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
[ExecuteInEditMode]
public class ObjectVisibility : BetterBehaviour {
	public List<MonoBehaviour> behaviors = new List<MonoBehaviour>();
	public List<MeshRenderer> renderers = new List<MeshRenderer>();
	protected bool _visible = true;
	[Show]
	public bool visible{
		get{ return _visible;}
		set{ c_set_visible(value); }
	}
	public ObjectVisibility c_set_visible(bool value){
		_visible = value;
		update ();
		return this;
	}
	void Start(){
		update ();
	}
	//Small-u update should only be run from other functions.
	void update () {
		foreach (MonoBehaviour behavior in behaviors) {
			behavior.enabled = _visible;
		}
		foreach (MeshRenderer renderer in renderers) {
			renderer.enabled = _visible;
		}
	}
}
