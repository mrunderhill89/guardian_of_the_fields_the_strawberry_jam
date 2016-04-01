using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Vexe.Runtime.Types;

public class DistanceToText : BetterBehaviour {
	public Text text_component;
	public Transform target;
	
	public float distance{
		get{
			if (target == null) return 0;
			return target.position.z;
		}
	}

	public string format(float d){
		if (Mathf.Abs(d) > 1000.0f){
			return (d/1000.0f).ToString("0.000")+" km";
		}
		return d.ToString("0.0")+" m";
	}

	// Update is called once per frame
	void Update () {
		text_component.text = format(distance);
	}
}
