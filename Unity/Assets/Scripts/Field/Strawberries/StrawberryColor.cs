using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;

public class StrawberryColor : BetterBehaviour {
	public static Gradient color_gradient;
	public static float max_quality = 2.00f;
	public Color color;
	protected Material material;
	[Show]
	protected Texture bump_map;
	static StrawberryColor(){
		if (color_gradient == null){
			color_gradient = new Gradient ();
		}
	}
	new public Renderer renderer;
	public StrawberryComponent data;
	// Use this for initialization
	void Start () {
		if (data == null)
			data = GetComponent<StrawberryComponent> ();
		if (renderer == null){
			Transform berry_mesh = transform.Find("StrawberryMesh/Berry");
			if (berry_mesh != null){
				renderer = berry_mesh.GetComponent<Renderer>();
			}
		}
		if (renderer != null){
			if (material == null){
				material = new Material(renderer.material);
			}
			bump_map = renderer.material.GetTexture("_BumpMap");
			renderer.material = material;
		}
	}

	// Update is called once per frame
	void Update () {
		if (material != null){
			color = get_color(data.quality);
			material.SetColor ("_Color", color);
			material.SetTexture("_BumpMap", bump_map);
		}
	}
	
	public static Color get_color(float quality){
		return color_gradient.Evaluate (quality / max_quality);
	}
}
