using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Vexe.Runtime.Types;
using UniRx;
[ExecuteInEditMode]
public class ObjectVisibility : BetterBehaviour {
	[Serialize]
	public List<MonoBehaviour> behaviors = new List<MonoBehaviour>();
	[Serialize]
	public List<MeshRenderer> renderers = new List<MeshRenderer>();
	[Serialize]
	public List<UIBehaviour> UI_behaviors = new List<UIBehaviour>();
	[Serialize][Hide]
	protected ObjectVisibility _parent;
	[Show]
	public ObjectVisibility parent{
		get{
			if (_parent == this) _parent = null;
			return _parent;
		}
		set{ if (_parent != this) _parent = value;}
	}
	public enum VisibilityStatus{
		AlwaysVisible,
		FollowParent,
		NeverVisible
	}
	protected VisibilityStatus _status = VisibilityStatus.FollowParent;
	[Show]
	public VisibilityStatus status{
		get{return _status;}
		set{ c_set_status(value); }
	}
	[Show]
	public bool visible{
		get{ 
			if (_status == VisibilityStatus.AlwaysVisible
				|| _status == VisibilityStatus.FollowParent && (
					parent == null || parent.visible
				)
			){return true;}
			return false;
		}
		set{ c_set_visible(value); }
	}
	public ObjectVisibility c_set_visible(bool value){
		if (value){
			return c_set_status(VisibilityStatus.FollowParent);
		} else {
			return c_set_status(VisibilityStatus.NeverVisible);
		}
	}
	public ObjectVisibility c_set_status(VisibilityStatus value){
		_status = value;
		return this;
	}
	//Small-u update should only be run from other functions.
	void Update () {
		foreach (MonoBehaviour behavior in behaviors) {
			if (behavior != null) behavior.enabled = visible;
		}
		foreach (MeshRenderer renderer in renderers) {
			if (renderer != null) renderer.enabled = visible;
		}
		foreach(UIBehaviour ui in UI_behaviors){
			if (ui != null) ui.enabled = visible;
		}
	}
}
