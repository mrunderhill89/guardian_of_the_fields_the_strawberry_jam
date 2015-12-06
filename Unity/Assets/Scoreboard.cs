using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;

public class Scoreboard : BetterBehaviour {
	public Transform Place;
	public ScoreHandler Scores;
	public Vector2 score_space = new Vector2(0.0f,-100.0f);
	[Show]
	public static string ScorePrefab = "ScoreMinimal";
	void Start () {
		if (Place == null)
			Place = transform;
		int score_count = 0;
		foreach(ScoreHandler.TotalScore score in Scores.saved_scores){
			GameObject score_view = GameObject.Instantiate(Resources.Load(ScorePrefab)) as GameObject;
			score_view.transform.SetParent(Place,false);
			score_view.GetComponent<ScoreMinimalForm>().score.Value = score;
			score_view.transform.Translate(score_space * score_count);
			score_count++;
		}
	}
}
