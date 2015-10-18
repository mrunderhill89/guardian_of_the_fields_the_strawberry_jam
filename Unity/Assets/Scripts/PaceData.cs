using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PaceData : BetterScriptableObject{
	PaceManager _manager;
	public PaceManager manager(){
		return _manager;
	}
	public PaceData manager(PaceManager m){
		_manager = m;
		return this;
	}
	
	float _speed;
	public float speed(){
		return _speed;
	}
	public PaceData speed(float value){
		_speed = value;
		return this;
	}
	//float health_drain = ???;
	
	public PaceData attach_to_state(State target){
		target.on_update(new StateEvent(()=>{
			if (_manager.is_in_pace(this)){
				move(_manager.transform);
			}
		}));
		return this;
	}
	public void move(Transform transform){
		transform.Translate(new Vector3(0.0f,0.0f,_speed * Time.deltaTime));
	}
}