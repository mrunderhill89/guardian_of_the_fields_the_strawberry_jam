using UnityEngine;
using System.Collections;

public class BeeSting : MonoBehaviour {
	public void OnMouseDown(){
		sting();
	}
	public void sting(){
		GameMessages.Log ("Ouch! You got stung by a bee!");
		//Will do more than a nasty message later.
	}
}
