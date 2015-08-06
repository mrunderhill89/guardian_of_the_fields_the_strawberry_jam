using UnityEngine;
using System;

public class StrawberryModel
{
	protected double _quality = 0.75;
	public double quality(){
		return _quality;
	}
	public StrawberryModel quality(double value){
		_quality = value;
		return this;
	}

	protected Vector3 _size;
	public Vector3 size(){
		return _size;
	}
	public StrawberryModel size(Vector3 value){
		_size = value;
		return this;
	}
	protected Vector3 _position;
	public Vector3 position(){
		return _position;
	}
	public StrawberryModel position(Vector3 value){
		_position = value;
		return this;
	}
	public StrawberryModel ()
	{
		_size = new Vector3 (1.0f, 1.0f, 1.0f);
		_position = new Vector3 (0.0f, 0.0f, 0.0f);
	}
	public override string ToString(){
		return "Strawberry[quality:"+quality()+", size:"+size().ToString()+", position:"+position().ToString()+"]";
	}
}