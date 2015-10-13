using UnityEngine;
using System.Collections;

public class StrawberryScale : MonoBehaviour {
	public StrawberryComponent data;
	//Normalized vector containing the relative X,Y, and Z scale coordinates.
	public Matrix4x4 shape_matrix = Matrix4x4.identity;
	public Vector3 shape;
	public float min_scale = 1.0f;
	public float random_scale = 1.0f;
	public float quality_scale = 1.0f;
	public Gradient quality_effect = new Gradient();
	protected float final_scale;
	// Use this for initialization
	public void Start () {
		if (data == null)
			data = GetComponent<StrawberryComponent> ();
		Initialize();
	}
	float get_quality_scale(){
		Color rgb = quality_effect.Evaluate (data.quality/StrawberryComponent.quality_range.y);
		return rgb.r;
	}
	public void Initialize(){
		shape = shape_matrix.MultiplyVector(RandomUtils.random_vec3 (1.0f,2.0f));
		shape.Normalize();
		final_scale = min_scale 
			+ (random_scale * RandomUtils.random_float (0.0f, 1.0f))
			+ (quality_scale * get_quality_scale());
		transform.localScale = shape * final_scale;
	}
	// Update is called once per frame
	void Update () {

	}
}
