using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;

public class GameStateLabel : BetterBehaviour {
	public GameStateManager game;
	public Text text;
	public Dictionary<string,string> state_names = new Dictionary<string,string>();

	[Show]
	public string key{
		get{
			if (game.fsm != null){
				foreach (string name in state_names.Keys){
					if (game.fsm.is_state_visited(name)){
						return state_names[name];
					}
				}
			}
			return "";
		}
	}
	public void Update(){
		text.text = LanguageTable.get(key);
	}
}
