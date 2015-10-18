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

	//float health_drain = ???;
	
	public PaceData attach_to_state(State target, float speed){
		target.on_update(new StateEvent(()=>{
			if (_manager.is_in_pace(this)){
				_manager.target_speed(speed);
			}
		})).on_exit(new StateEvent(()=>{
			if (_manager.is_in_pace(this)){
				_manager.target_speed(0.0f);
			}
		}));
		return this;
	}
}