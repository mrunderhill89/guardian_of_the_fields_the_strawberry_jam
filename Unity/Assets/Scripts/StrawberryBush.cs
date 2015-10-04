using UnityEngine;
using Vexe.Runtime.Types;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class StrawberryBush : MonoBehaviour {
	/*
	Purpose of this component:
		* Create a procedural strawberry bush with appropriate leaves
		and flowers (once implemented).
		* 
		* Determine when a strawberry bush has exited the player's view.
		** When this happens, delete any internal data and recycle any strawberries left
		on the plant back into the field state for re-use.
	*/
	//Keep a public list of available bushes to put strawberries on.
	List<GameObject> leaves;
	public State state;
	public Vector2 min = new Vector2(0.1f,0.0f);
	public Vector2 max = new Vector2(1.0f,1.0f);
	// Use this for initialization
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
				a.GetComponent<StrawberryComponent>().hidden(false);
			}))
			.on_exit(new StateEvent((Automata a)=>{
				a.transform.SetParent(null,true);
			}));
			state.instance_name = "berries";
		}
	}
	
	public Vector3 CylinderToCube(float r, float d, float h){
		return new Vector3(
			Mathf.Cos(r) * d,
			h,
			Mathf.Sin(r) * d
		);
	}
	public void Remove(){
		Debug.Log(name+": Removing...");
		foreach(Automata a in state.visitors.ToArray()){
			//Bush->Row->Field
			a.eject(2);
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
