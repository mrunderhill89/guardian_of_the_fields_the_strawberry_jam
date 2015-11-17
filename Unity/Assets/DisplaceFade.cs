using UnityEngine;
using System.Collections;

public class DisplaceFade : Displacer {
	public Displacer source_displacer;
	public Vector3 fade_direction = new Vector3(1.0f,0.0f,0.0f);
	public AnimationCurve fade_curve;
	public override Vector3 displace(Vector3 local, Vector3 world, Vector3 displaced){
		if (source_displacer != null){
			float distance = Mathf.Abs(Vector3.Dot(local, fade_direction));
			return source_displacer.displace(local,world,displaced) * (1.0f-Mathf.Clamp(fade_curve.Evaluate(distance),0.0f,1.0f));
		}
		return Vector3.zero;
	}
}
