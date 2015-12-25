using UnityEngine;
using Vexe.Runtime.Types;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class StrawberryGenerator : BetterBehaviour {
	public State state;
	public Vector2 min = new Vector2(0.1f,0.0f);
	public Vector2 max = new Vector2(1.0f,1.0f);
	void Awake () {
		if (state == null) {
			state = gameObject.AddComponent<State>()
			.on_entry(new StateEvent((Automata a)=>{
				a.transform.SetParent(transform);
				a.transform.localPosition = CylinderToCube(
					RandomUtils.random_float(0.0f,360.0f * Mathf.Deg2Rad),
					RandomUtils.random_float(min.x,max.x),
					RandomUtils.random_float(min.y,max.y)
				);
				a.GetComponent<ObjectVisibility>().visible = true;
			}))
			.on_exit(new StateEvent((Automata a)=>{
				a.transform.SetParent(null,true);
			}));
			state.instance_name = "berries";
		}
	}
	
	public void eject_berries(){
		state.eject_all(2);
	}
	
	public static Vector3 CylinderToCube(float r, float d, float h){
		return new Vector3(
			Mathf.Cos(r) * d,
			h,
			Mathf.Sin(r) * d
		);
	}
}
