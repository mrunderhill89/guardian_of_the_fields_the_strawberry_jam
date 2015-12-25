using UnityEngine;
using System.Collections;

public class GroundGenerator : MonoBehaviour {
	public RowGenerator generator;
	void Start () {
		generator.on_create((cell)=>{
			cell.GetComponent<DisplaceMesh>().construct();
		});
	}
	
}
