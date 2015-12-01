﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Vexe.Runtime.Types;

public class DisplaceTransform : BetterBehaviour {
	public List<Displacer> displacers = new List<Displacer>();
	[Serialize][Hide]
	protected Transform _input = null;
	[Serialize][Hide]
	protected Transform _output = null;
	[Show]
	public Transform input{
		get{
			if (_input == null){
				input = gameObject.transform;
			}
			return _input;
		}
		set{
			_input = value;
		}
	}
	[Show]
	public Transform output{
		get{
			if (_output == null){
				output = input;
			}
			return _output;
		}
		set{
			_output = value;
		}
	}

	Vector3 local;
	Vector3 world;
	
	public bool auto_read = false;
	public bool auto_write = false;
	
	void Start () {
		read();
		write();
	}
	
	void Update(){
		if (auto_read) read();
		if (auto_write) write();
	}
	
	void read(){
		local = input.localPosition;
		world = input.position;
	}
	
	void write () {
		output.position = displacers.Aggregate(Vector3.zero, 
			(Vector3 sum, Displacer displacer)=>{return sum + displacer.displace(local, world, sum);}
		);
	}
}
