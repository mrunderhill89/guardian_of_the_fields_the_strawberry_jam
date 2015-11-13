using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Vexe.Runtime.Types;

public class DisplaceNoise : Displacer{
	public Vector4 x_component = new Vector4(1.0f,0.0f,0.0f,0.0f);
	public Vector4 y_component = new Vector4(0.0f,0.0f,1.0f,0.0f);
	public Vector2 x_effect = new Vector2(-1.0f,1.0f);
	public Vector2 y_effect = new Vector2(-1.0f,1.0f);
	public Vector2 z_effect = new Vector2(-1.0f,1.0f);
	public AnimationCurve x_curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
	public AnimationCurve y_curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
	public AnimationCurve z_curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
	public bool use_local = false;
	public override Vector3 displace(Vector3 local, Vector3 world, Vector3 displaced){
		Vector3 input = use_local? local:world;
		Vector2 noise_position = new Vector2(
			x_component.x * input.x 
				+ x_component.y * input.y + 
				x_component.z * input.z + 
				x_component.w,
			y_component.x * input.x 
				+ y_component.y * input.y + 
				y_component.z * input.z + 
				y_component.w
		);
		float noise_value = Mathf.PerlinNoise(noise_position.x, noise_position.y);
		return new Vector3(
			Mathf.Lerp(x_effect.x, x_effect.y, x_curve.Evaluate(noise_value)),
			Mathf.Lerp(y_effect.x, y_effect.y, y_curve.Evaluate(noise_value)),
			Mathf.Lerp(z_effect.x, z_effect.y, z_curve.Evaluate(noise_value))
		);
	}
}