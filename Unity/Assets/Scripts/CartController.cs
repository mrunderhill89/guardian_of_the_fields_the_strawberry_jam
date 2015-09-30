using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;

public class CartController : BetterBehaviour {
	public Vector3 cart_speed = new Vector3(0.0f,0.0f,1.0f);
	// Use this for initialization
	void Start () {
	
	}

	public void move(){
		this.transform.Translate(cart_speed * Time.deltaTime);
	}
}
