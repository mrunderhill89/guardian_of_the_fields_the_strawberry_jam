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
		float _life;
		public float time{
			get{ return _time;}
			internal set{ _time = value;}
		}
		public float life{
			get{ return _life;}
			internal set{ _life = value;}
		}
		public bool is_alive(float t){
			return t < (life+time);
		}
	}
	static Subject<GameMessage> _message_stream = new Subject<GameMessage>();
	public static Subject<GameMessage> message_stream{
		get{ return _message_stream;}
	}
	public static void Log(string txt, float life = 3.0f){
		GameMessage msg = new GameMessage ();
		msg.text = txt;
		msg.time = Time.time;
		msg.life = life;
		message_stream.OnNext(msg);
	}
}