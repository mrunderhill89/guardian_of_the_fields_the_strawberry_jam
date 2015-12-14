using UnityEngine;
using System.Collections;

public class StrawberryScale : MonoBehaviour {
	public StrawberryComponent data;
	//Normalized vector containing the relative X,Y, and Z scale coordinates.
	public Matrix4x4 shape_matrix = Matrix4x4.identity;
	public Vector3 shape; //Make this protected once we know it works!
	public AnimationCurve quality_to_min = new AnimationCurve ();
	public AnimationCurve quality_to_max = new AnimationCurve ();
	protected float final_scale;
	// Use this for initialization
	public void Start () {
		if (data == null)
			data = GetComponent<StrawberryComponent> ();
		Initialize();
	}
	public void Initialize(){
		shape = shape_matrix.MultiplyVector(RandomUtils.random_vec3 (1.0f,2.0f));
		shape.Normalize();
		float quality_percent = data.quality / GameSettingsComponent.working_rules.strawberry.max_ripeness;
		final_scale = GameSettingsComponent.working_rules.strawberry.min_size + RandomUtils.random_float (
			quality_to_min.Evaluate (quality_percent) * GameSettingsComponent.working_rules.strawberry.max_size,
			quality_to_max.Evaluate (quality_percent) * GameSettingsComponent.working_rules.strawberry.max_size
		);
		transform.localScale = shape * final_scale;
	}
}
