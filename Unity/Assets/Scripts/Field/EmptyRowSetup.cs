using UnityEngine;
using System.Collections;

public class EmptyRowSetup : MonoBehaviour {
	public RowHandler2 row;
	public RowGenerator generator;
	public string prefab = "EmptyGroundCell";
	public string start_prefab = "RowStart";
	public string end_prefab = "RowEnd";

	public int break_distance{
		get{
			return GameSettingsComponent.working_rules.breaks.distance;
		}
	}
	public int break_length{
		get{
			return GameSettingsComponent.working_rules.breaks.length;
		}
	}

	void Start () {
		if (row == null)
			row = GetComponent<RowHandler2> ();
		if (generator == null)
			generator = GetComponent<RowGenerator> ();
		generator.pattern.Clear ();
		row.target = Camera.main.transform;
		generator.pattern.Add (start_prefab);
		for (int i = 0; i < break_distance; i++){
			generator.pattern.Add(prefab);
		}
		generator.pattern.Add (end_prefab);
		for (int i = 0; i < break_length; i++){
			generator.pattern.Add("");
		}
		generator.on_create((cell)=>{
			LeafGenerator leaves = cell.GetComponentInChildren<LeafGenerator>();
			if (leaves != null)
				leaves.Start();
			DisplaceMesh displace = cell.GetComponentInChildren<DisplaceMesh>();
			if (displace != null)
				displace.construct();
		});
	}
}
