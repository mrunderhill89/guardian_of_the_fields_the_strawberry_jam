using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Vexe.Runtime.Types;
using UniRx;

[ExecuteInEditMode]
public class ObjectOpacity : BetterBehaviour {
	[Hide]
	public FloatReactiveProperty rx_opacity = new FloatReactiveProperty(1.0f);
	[Show]
	public float opacity{
		get{ return rx_opacity.Value; }
		set{ rx_opacity.Value = value; }
	}
	
	public float drift_speed = 0.0f;
	[Hide]
	public FloatReactiveProperty rx_target_opacity = new FloatReactiveProperty(1.0f);
	[Show]
	public float target_opacity{
		get{ return rx_target_opacity.Value; }
		set{ rx_target_opacity.Value = value; }
	}
	[Show]
	public bool instant_drift{
		get{ return drift_speed < 0.0f; }
	}
	
	
	[Serialize]
	public List<MeshRenderer> meshes = new List<MeshRenderer>();
	protected List<Material> mesh_materials;
	[Serialize]
	public List<TextMesh> texts = new List<TextMesh>();
	[Serialize]
	public List<SpriteRenderer> sprites = new List<SpriteRenderer>();
	[Serialize]
	public List<Image> images = new List<Image>();
	
	protected IDisposable subscription = null;
	
	// Use this for initialization
	void Awake () {
		mesh_materials = meshes.Select((MeshRenderer mesh)=>{
			Material copy = new Material(mesh.material);
			mesh.material = copy;
			return copy;
		}).ToList();
		Subscribe();
	}
	
	[Show]
	void Subscribe(){
		if (subscription == null){
			subscription = rx_opacity.DistinctUntilChanged().Subscribe((a)=>{
				foreach(Material material in mesh_materials){
					Color color = material.GetColor ("_Color");
					material.SetColor("_Color", new Color(
						color.r,
						color.g,
						color.b,
						a
					));
				}
				foreach(TextMesh text in texts){
					text.color = new Color(
						text.color.r,
						text.color.g,
						text.color.b,
						a
					);
				}
				foreach(SpriteRenderer sprite in sprites){
					sprite.color = new Color(
						sprite.color.r,
						sprite.color.g,
						sprite.color.b,
						a
					);
				}
				foreach(Image image in images){
					image.color = new Color(
						image.color.r,
						image.color.g,
						image.color.b,
						a
					);
				}
			});
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (instant_drift){
			opacity = target_opacity;
		} else {
			if (opacity < target_opacity){
				opacity += Mathf.Min(drift_speed * Time.deltaTime, target_opacity - opacity);
			}
			if (opacity > target_opacity){
				opacity -= Mathf.Min(drift_speed * Time.deltaTime, opacity - target_opacity);
			}
		}
	}
}
