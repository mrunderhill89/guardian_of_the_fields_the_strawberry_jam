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
	new public MeshCollider collider;
	public bool auto_update = false;
	protected Vector3[] original_verts;
	[Show]
	public void construct(){
		Vector3[] original_normals = filter.sharedMesh.normals;
		filter.mesh.vertices = original_verts.Select((Vector3 local)=>{
			Vector3 world = transform.TransformPoint(local);
			return displacers.Aggregate(Vector3.zero, (Vector3 sum, Displacer displacer)=>{
				return sum + displacer.displace(local, world, sum);
			});
		}).ToArray();
		if (normal_recalculation == NormalRecalculation.KeepOriginal) {
			filter.mesh.normals = original_normals;
		} else if (normal_recalculation == NormalRecalculation.RecalculateFlat) {
			filter.mesh.RecalculateNormals();
		} else if (normal_recalculation == NormalRecalculation.RecalculateSmooth) {
			RecalculateNormalsSmooth(filter.mesh);
		}
		if (collider != null){
			collider.sharedMesh = filter.mesh;
		}
	}
	public enum NormalRecalculation
	{
		KeepOriginal,
		RecalculateFlat,
		RecalculateSmooth,
		NoNormals
	}
	public NormalRecalculation normal_recalculation = NormalRecalculation.RecalculateSmooth;

	public static void RecalculateNormalsSmooth(Mesh mesh, bool unbiased = false){
		Vector3 triangle_normal, side_ab, side_bc, vert_a, vert_b, vert_c;
		int[] triangles = mesh.triangles;
		Vector3[] normals = new Vector3[mesh.normals.Length];
		for (int n = 0; n < normals.Length; n++) {
			normals[n] = new Vector3(0.0f,0.0f,0.0f);
		}
		for (int v = 0; v < triangles.Length; v+=3){
			vert_a = mesh.vertices[triangles[v]];
			vert_b = mesh.vertices[triangles[v+1]];
			vert_c = mesh.vertices[triangles[v+2]];
			side_ab = vert_a - vert_b;
			side_bc = vert_b - vert_c;
			triangle_normal = Vector3.Cross(side_ab,side_bc);
			if (unbiased){
				triangle_normal.Normalize();
			}
			normals[triangles[v]] += triangle_normal;
			normals[triangles[v+1]] += triangle_normal;
			normals[triangles[v+2]] += triangle_normal;
		}
		mesh.normals = normals;
	}
	void Awake(){
		if (collider == null)
			collider = gameObject.GetComponent<MeshCollider>();
		original_verts = filter.sharedMesh.vertices;
	}
	void Update(){
		if (auto_update)
			construct();
	}
}
