using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;

public class StrawberryColor : BetterBehaviour {
	public static Gradient color_gradient;
	public static float max_quality = 2.00f;
	public Color color;
	protected Material material;
	static StrawberryColor(){
		color_gradient = new Gradient ();
	}
	new public Renderer renderer;
	public StrawberryComponent data;
	// Use this for initialization
	void Start () {
		if (data == null)
			data = GetComponent<StrawberryComponent> ();
		renderer = transform.Find("Strawberry_Mesh/Cube").GetComponent<Renderer>();
		if (material == null)
			material = new Material(Shader.Find ("Standard"));
		renderer.material = material;
	}

	// Update is called once per frame
	void Update () {
		float quality = data.quality;
		color = color_gradient.Evaluate (quality / max_quality);
		material.SetColor ("_Color", color);
	}
}
