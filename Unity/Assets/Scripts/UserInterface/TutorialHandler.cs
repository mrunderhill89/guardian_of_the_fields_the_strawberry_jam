using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UniRx;
public class TutorialHandler : BetterBehaviour {
	[Serializable]
	public class TutorialMessage{
		public void display(){
			GameMessages.Log("Tutorial Message!");
		}
	}
	public static Dictionary<string,TutorialMessage> messages = new Dictionary<string,TutorialMessage>();
	
	public static void Log(string key){
		if (messages.ContainsKey(key)){
			messages[key].display();
		}
	}
}
