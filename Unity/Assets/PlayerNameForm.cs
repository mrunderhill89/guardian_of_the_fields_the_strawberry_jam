using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using GameScores;
using UniRx;
using Vexe.Runtime.Types;
public class PlayerNameForm : BetterBehaviour {
	public InputField input;
	public IScoreSource source;

	void Start () {
		input.onValueChanged.AddListener((player)=>{
			source.score.player_name = player;
		});
	}
}
