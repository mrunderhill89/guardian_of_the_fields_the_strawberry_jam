using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Vexe.Runtime.Types;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class DisplaceMesh : BetterBehaviour {
	public List<Displacer> displacers = new List<Displacer>();
	public MeshFilter filter;
	public bool auto_update = false;
	protected Vector3[] original_verts;
	[Show]
	void construct(){
		filter.mesh.vertices = original_verts.Select((Vector3 local)=>{
			Vector3 world = transform.TransformPoint(local);
			return displacers.Select((Displacer displacer)=>{
				return displacer.displace(local,world);
			}).Aggregate(Vector3.zero, (Vector3 sum, Vector3 part)=>{
				return sum+part;
			});
		}).ToArray();
		RecalculateNormalsSmooth(filter.mesh);
	}
	
	public static void RecalculateNormalsSmooth(Mesh mesh){
		mesh.normals = mesh.normals.Select((Vector3 original)=>{
			return new Vector3(0.0f,0.0f,0.0f);
		}).ToArray();
		Vector3 triangle_normal, side_ab, side_bc, vert_a, vert_b, vert_c;
		int[] triangles = mesh.triangles;
		for (int v = 0; v < triangles.Length; v+=3){
			vert_a = mesh.vertices[triangles[v]];
			vert_b = mesh.vertices[triangles[v+1]];
			vert_c = mesh.vertices[triangles[v+2]];
			side_ab = vert_a - vert_b;
			side_bc = vert_b - vert_c;
			triangle_normal = Vector3.Cross(side_ab,side_bc);
			mesh.normals[triangles[v]] += triangle_normal;
			mesh.normals[triangles[v+1]] += triangle_normal;
			mesh.normals[triangles[v+2]] += triangle_normal;
		}
	}
	
	void Start(){
		original_verts = filter.sharedMesh.vertices;
		construct();
	}
	void Update(){
		if (auto_update)
			construct();
	}
}
