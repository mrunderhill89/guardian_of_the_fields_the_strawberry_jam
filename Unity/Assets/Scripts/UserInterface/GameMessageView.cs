using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Vexe.Runtime.Types;
using UniRx;

public class GameMessageView : BetterBehaviour {
	public Text text_box;
	Queue<GameMessages.GameMessage> stored_messages = new Queue<GameMessages.GameMessage>();
	public int max_messages = 5;
	public float message_life = 3.0f;
	float clear_next_message = 0.0f;
	// Use this for initialization
	void Start () {
		GameMessages.message_stream.Subscribe ((msg) => {
			stored_messages.Enqueue(msg);
			if (stored_messages.Count > max_messages){
				stored_messages.Dequeue();
			}
			clear_next_message = message_life;
		});
	}
	
	// Update is called once per frame
	void Update () {
		int message_count = 0;
		text_box.text = "";
		foreach (GameMessages.GameMessage msg in stored_messages.ToList()) {
			if (message_count < max_messages){
				text_box.text += msg.text+"\n";
				message_count++;
			}
		}
		if (clear_next_message <= 0.0f) {
			if (stored_messages.Count > 0)
				stored_messages.Dequeue();
			clear_next_message = message_life;
		} else {
			clear_next_message -= Time.deltaTime;
		}
	}
	[Show]
	public void log(string msg){
		GameMessages.Log (msg);
	}
}
