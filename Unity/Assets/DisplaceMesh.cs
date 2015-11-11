﻿using UnityEngine;
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

	[Show]
	void construct(){
		filter.mesh.vertices = filter.sharedMesh.vertices.Select((Vector3 local)=>{
			Vector3 world = transform.TransformPoint(local);
			return displacers.Select((Displacer displacer)=>{
				return displacer.displace(local,world);
			}).Aggregate(Vector3.zero, (Vector3 sum, Vector3 part)=>{
				return sum+part;
			});
		}).ToArray();
	}
	
	void Update(){
		construct();
	}
}
