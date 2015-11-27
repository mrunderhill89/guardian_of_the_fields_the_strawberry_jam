using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DisplaceFade : Displacer {
	public List<Displacer> source_displacers = new List<Displacer>();
	public Vector3 fade_direction = new Vector3(1.0f,0.0f,0.0f);
	public AnimationCurve fade_curve;
	public override Vector3 displace(Vector3 local, Vector3 world, Vector3 displaced){
		float distance = Mathf.Abs(Vector3.Dot(local, fade_direction));
		return source_displacers.Aggregate(Vector3.zero, (vec3, displacer)=>{
			return vec3 + displacer.displace(local,world,displaced);
		}) * (1.0f-Mathf.Clamp(fade_curve.Evaluate(distance),0.0f,1.0f));
	}
}
