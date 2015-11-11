using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Vexe.Runtime.Types;
public class WrapToTerrain : BetterBehaviour {
	[Serialize][Hide]
	protected HashSet<MeshFilter> meshes = new HashSet<MeshFilter>();
	[Show]
	public List<MeshFilter> _meshes{
		get{ return meshes.ToList();}
	}
	protected Dictionary<MeshFilter,Vector3[]> original_verts = new Dictionary<MeshFilter,Vector3[]>();
	public float displace_factor = 1.0f;
	public float original_factor = 0.0f;
	public Func<Vector3,Vector3> displace = (v)=>{return v;};
	public bool merge = true;

	void record_verts(MeshFilter filter){
		original_verts[filter] = filter.sharedMesh.vertices;
	}
	void update_verts(MeshFilter filter){
		Vector3[] new_verts = filter.sharedMesh.vertices;
		var position_map = new Dictionary<Vector3,Vector3>();
		for (int v = 0; v < new_verts.Length; v++){
			Vector3 original = original_verts[filter][v];
			Vector3 world = filter.transform.TransformPoint(original);
			if (!merge || !position_map.ContainsKey(original)){
				position_map[original] =	displace_factor * displace(world)
				+ original_factor * original;
			}
			new_verts[v] = position_map[original];
		}
		filter.mesh.vertices = new_verts;
	}
	void Start () {
		foreach (MeshFilter filter in meshes){
			record_verts(filter);
		}
	}
	
	void Update () {
		foreach (MeshFilter filter in meshes){
			update_verts(filter);
		}
	}
	[Show]
	public void add_mesh(MeshFilter filter){
		meshes.Add(filter);
		record_verts(filter);
	}
}
