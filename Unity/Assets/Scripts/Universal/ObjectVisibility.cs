using UnityEngine;
using System.Linq;
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
				obj.SetActive(vis);
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
		assert_lists_correct ();
	}

	private void log_error(string msg){
		Debug.LogError (gameObject.name + ".ObjectVisibility:" + msg);
	}

	public void assert_lists_correct(){
		if (objects == null) {
			log_error("'objects' should be a list, instead it's null.");
		}
		if (behaviors == null) {
			log_error("'behaviors' should be a list, instead it's null.");
		}
		if (renderers == null) {
			log_error("'renderers' should be a list, instead it's null.");
		}
		if (UI_behaviors == null) {
			log_error("'UI_behaviors' should be a list, instead it's null.");
		}
		objects = objects.Where ((obj, index) => {
			if (obj == null){
				log_error("Found invalid object at index "+index);
				return false;
			}
			return true;
		}).ToList();
		behaviors = behaviors.Where ((obj, index) => {
			if (obj == null){
				log_error("Found invalid behavior at index "+index);
				return false;
			}
			return true;
		}).ToList();
		renderers = renderers.Where ((obj, index) => {
			if (obj == null){
				log_error("Found invalid renderer at index "+index);
				return false;
			}
			return true;
		}).ToList();
		UI_behaviors = UI_behaviors.Where ((obj, index) => {
			if (obj == null){
				log_error("Found invalid UI behavior at index "+index);
				return false;
			}
			return true;
		}).ToList();
	}

	public static ObjectVisibility GetVisibility(GameObject obj, ObjectVisibility existing){
		if (existing != null)
			return existing;
		if (obj.GetComponent<ObjectVisibility>() == null)
			return obj.AddComponent<ObjectVisibility>();
		return obj.GetComponent<ObjectVisibility>();
	}
}
