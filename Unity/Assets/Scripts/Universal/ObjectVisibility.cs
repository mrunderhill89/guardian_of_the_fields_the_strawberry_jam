using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Vexe.Runtime.Types;
using UniRx;
[ExecuteInEditMode]
public class ObjectVisibility : BetterBehaviour {
	public enum VisibilityStatus{
		AlwaysVisible,
		FollowParent,
		NeverVisible
	}
	[Serialize]
	public List<GameObject> objects = new List<GameObject>();
	[Serialize]
	public List<MonoBehaviour> behaviors = new List<MonoBehaviour>();
	[Serialize]
	public List<MeshRenderer> renderers = new List<MeshRenderer>();
	[Serialize]
	public List<UIBehaviour> UI_behaviors = new List<UIBehaviour>();
	
	public ReactiveProperty<ObjectVisibility> obs_parent = new ReactiveProperty<ObjectVisibility>();
	public ReactiveProperty<VisibilityStatus> obs_status = new ReactiveProperty<VisibilityStatus>(VisibilityStatus.FollowParent);
	public VisibilityStatus status{
		get{
			init_status();
			return obs_status.Value; 
		}
		set{
			if (obs_status == null) init_status(value);
			else obs_status.Value = value; 
		}
	}
	[DontSerialize]
	public ReadOnlyReactiveProperty<bool> obs_visible;
	[Show]
	public bool visible{
		get{
			init_visible();
			return obs_visible.Value; 
		}
		set{
			update(value);
			if (value){
				status = VisibilityStatus.FollowParent;
			} else {
				status = VisibilityStatus.NeverVisible;
			}
		}
	}
	
	void init_parent(ObjectVisibility initial = null){
		if (obs_parent == null)
			obs_parent = new ReactiveProperty<ObjectVisibility>(initial);
	}
	
	void init_status(VisibilityStatus initial = VisibilityStatus.FollowParent){
		if (obs_status == null)
			obs_status = new ReactiveProperty<VisibilityStatus>(initial);
	}
	
	void init_visible(){
		if (obs_visible == null){
			init_status();
			init_parent();
			obs_visible = obs_parent.SelectMany((parent)=>{
				if (parent == null)
					return Observable.Return<bool>(true);
				parent.init_visible();
				return parent.obs_visible;
			}).CombineLatest(obs_status, (bool p_visible, VisibilityStatus stat)=>{
				if (stat == VisibilityStatus.AlwaysVisible) return true;
				if (stat == VisibilityStatus.NeverVisible) return false;
				return p_visible;
			}).ToReadOnlyReactiveProperty();
			obs_visible.DistinctUntilChanged().Subscribe(update);
		};
	}
	
	void update(bool vis){
		foreach(GameObject obj in objects){
			if (obj != null){
				obj.SetActive(vis);
			} else {
				Debug.LogWarning("Got null object in visibility list:"+gameObject.ToString());
			}
		}
		foreach(MonoBehaviour behavior in behaviors){
			behavior.enabled = vis;
		}
		foreach(MeshRenderer renderer in renderers){
			renderer.enabled = vis;
		}
		foreach(UIBehaviour ui in UI_behaviors){
			ui.enabled = vis;
		}
	}
	
	void Awake(){
		init_visible();


	}
}
