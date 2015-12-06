using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;

public class Scoreboard : BetterBehaviour {
	public Transform Place;
	public ScoreHandler Scores;
	[Show]
	public static string ScorePrefab = "ScoreMinimal";
	void Start () {
		if (Place == null)
			Place = transform;
		foreach(ScoreHandler.TotalScore score in Scores.saved_scores){
			GameObject score_view = GameObject.Instantiate(Resources.Load(ScorePrefab)) as GameObject;
			score_view.transform.SetParent(Place,false);
			score_view.GetComponent<ScoreMinimalForm>().score.Value = score;
		}
	}
}
