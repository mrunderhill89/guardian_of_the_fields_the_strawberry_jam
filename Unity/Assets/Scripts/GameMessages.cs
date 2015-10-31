using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Vexe.Runtime.Types;
using UniRx;

public static class GameMessages {
	public struct GameMessage{
		public string text;
		float _time;
		public float time{
			get{ return _time;}
			internal set{ _time = value;}
		}
	}
	static Subject<GameMessage> _message_stream = new Subject<GameMessage>();
	public static Subject<GameMessage> message_stream{
		get{ return _message_stream;}
	}
	public static void Log(string txt){
		GameMessage msg = new GameMessage ();
		msg.text = txt;
		msg.time = Time.time;
		message_stream.OnNext(msg);
	}
}