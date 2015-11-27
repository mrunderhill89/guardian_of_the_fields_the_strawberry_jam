using UnityEngine;
using System.Collections;
using System;
using Vexe.Runtime.Types;
public class ScrollGroundTexture : BetterBehaviour {
	public Transform target;
	new public Renderer renderer;
	public float parallax = 1.0f;
	public System.Func<Vector3,Vector2> project = along_z;

	public static Vector2 along_z(Vector3 three_d){
		return new Vector2 (0.0f, three_d.z);
	}

	[Show]
	public Vector2 offset{
		get{ 
			if (target != null && renderer != null) {
				return project(target.position * parallax);
			}
			return Vector2.zero; 
		}
	}
	void LateUpdate(){
		renderer.material.SetTextureOffset ("_MainTex", offset);
	}
}
