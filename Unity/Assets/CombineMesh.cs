using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Vexe.Runtime.Types;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CombineMesh : BetterBehaviour {
	public List<MeshFilter> filters = new List<MeshFilter>();
	public bool merge = true;
	public MeshFilter filter;

	[Show]
	void construct(){
		CombineInstance[] combine = new CombineInstance[filters.Count];
        for (int i  = 0; i < filters.Count; i++){
		    combine[i].mesh = filters[i].sharedMesh;
            combine[i].transform = filters[i].transform.localToWorldMatrix;
			filters[i].gameObject.SetActive(filters[i].gameObject == gameObject);
		}
        filter.mesh = new Mesh();
        filter.mesh.CombineMeshes(combine, merge);
	}
	
	void Start(){
		construct();
	}
}
