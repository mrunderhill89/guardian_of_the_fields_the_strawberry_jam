using UnityEngine;
using System.Collections;

public class StrawberryScale : MonoBehaviour {
	public StrawberryComponent data;
	//Normalized vector containing the relative X,Y, and Z scale coordinates.
	public Matrix4x4 shape_matrix = Matrix4x4.identity;
	public Vector3 shape; //Make this protected once we know it works!
	public AnimationCurve quality_to_min = new AnimationCurve ();
	public AnimationCurve quality_to_max = new AnimationCurve ();
	public Vector2 scale_range = new Vector2(0.04f,0.10f);
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
		float quality_percent = data.quality / StrawberryComponent.quality_range.y;
		final_scale = scale_range.x + RandomUtils.random_float (
			quality_to_min.Evaluate (quality_percent) * scale_range.y,
			quality_to_max.Evaluate (quality_percent) * scale_range.y
		);
		transform.localScale = shape * final_scale;
	}
	// Update is called once per frame
	void Update () {

	}
}
