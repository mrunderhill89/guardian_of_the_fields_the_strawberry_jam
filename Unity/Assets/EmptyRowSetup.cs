using UnityEngine;
using System.Collections;

public class EmptyRowSetup : MonoBehaviour {
	public RowHandler2 row;
	public RowGenerator generator;
	public string prefab = "EmptyGroundCell";
	void Start () {
		if (row == null)
			row = GetComponent<RowHandler2> ();
		if (generator == null)
			generator = GetComponent<RowGenerator> ();
		generator.pattern.Clear ();
		row.target = Camera.main.transform;
		for (int i = 0; i < GameStartData.break_distance; i++){
			generator.pattern.Add(prefab);
		}
		for (int i = 0; i < GameStartData.break_length; i++){
			generator.pattern.Add("");
		}
	}
}
