using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;

public class GameNameLabel : BetterBehaviour {
	public GameStateManager game;
	public Text text;
	public Dictionary<string,string> state_names = new Dictionary<string,string>();
}
