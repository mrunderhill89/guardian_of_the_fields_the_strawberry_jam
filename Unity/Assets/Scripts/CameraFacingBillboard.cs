using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;
[ExecuteInEditMode]
public class CameraFacingBillboard : BetterBehaviour {
	public enum UpVectorOption{
		camera,
		parent,
		world
	}
	public enum ForwardVectorOption{
		position,
		forward
	}
	public GameObject target;
	[Serialize][Hide]
	protected bool _offset;
	[Show]
	public bool offset{
		get{return _offset;}
		set{ c_set_offset(value); }
	}
	public CameraFacingBillboard c_set_offset(bool value){
		_offset = value;
		return this;
	}
	[Serialize][Hide]
	protected UpVectorOption _up_vector = UpVectorOption.camera;
	[Show]
	public UpVectorOption up_vector{
		get{ return _up_vector;}
		set{ _up_vector = value;}
	}
	public Vector3 up{
		get{
			switch (up_vector){
			case UpVectorOption.world:
				return Vector3.up;
			case UpVectorOption.parent:
				return transform.rotation * Vector3.up;
			default:
				return Camera.main.transform.rotation * Vector3.up;
			}
		}
	}
	[Serialize][Hide]
	protected ForwardVectorOption _forward_vector = ForwardVectorOption.forward;
	[Show]
	public ForwardVectorOption forward_vector{
		get{ return _forward_vector;}
		set{ _forward_vector = value;}
	}
	public Vector3 forward{
		get{
			switch (forward_vector){
			case ForwardVectorOption.forward:
				return transform.position + Camera.main.transform.rotation * Vector3.forward;
			default:
				return Camera.main.transform.position;
			}
		}
	}

	void Update () {
		if (target == null)
			target = gameObject;
		target.transform.LookAt(forward, up);
		if (offset && target != gameObject) {
			target.transform.Rotate(transform.rotation.eulerAngles);
		}
	}
}
