using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;

public class GameStateLabel : BetterBehaviour {
	public GameStateManager game;
	public Text text;
	public Dictionary<string,string> state_names = new Dictionary<string,string>();
	void Update(){
		if (text != null && game != null){
			text.text = game.fsm.match<string>(state_names, "Game State");
		}
	}
}
