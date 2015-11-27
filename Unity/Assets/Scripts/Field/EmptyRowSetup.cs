using UnityEngine;
using System.Collections;

public class EmptyRowSetup : MonoBehaviour {
	public RowHandler2 row;
	public RowGenerator generator;
	public string prefab = "EmptyGroundCell";
	public string start_prefab = "RowStart";
	public string end_prefab = "RowEnd";

	void Start () {
		if (row == null)
			row = GetComponent<RowHandler2> ();
		if (generator == null)
			generator = GetComponent<RowGenerator> ();
		generator.pattern.Clear ();
		row.target = Camera.main.transform;
		generator.pattern.Add (start_prefab);
		for (int i = 0; i < GameStartData.instance.break_distance; i++){
			generator.pattern.Add(prefab);
		}
		generator.pattern.Add (end_prefab);
		for (int i = 0; i < GameStartData.instance.break_length; i++){
			generator.pattern.Add("");
		}
	}
}
