using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using GameScores;
using UniRx;

public class ScoreTimelineView : BetterBehaviour {
	public IScoreSource source;
	
	public Transform start_point;
	public Transform end_point;
	
	public float padding = 5.0f;
	
	public Transform time_icon;
	public Transform distance_icon;
	
	protected float _length = 500.0f;
	[Show]
	public float length {
		get { 
			return 460.0f;
		}
	}
	public float slider_x (float player, float goal){
		float t = Mathf.Clamp(player / goal, 0.0f, 1.0f);
		return t * length;
	}

	public Text time_recorded_text;
	public Text distance_recorded_text;

	public Text time_goal_text;
	public Text distance_goal_text;
	
	public Text average_speed_text;
	public Text flat_score_text;
	public Text range_score_text;
	public Text total_score_text;

	IObservable<float> rx_player_time;
	IObservable<float> rx_player_distance;
	IObservable<float> rx_goal_time;
	IObservable<float> rx_goal_distance;

	void Start () {
		//Time
		rx_player_time = source.rx_score.Select(score=>{
			return score.time.played_for;
		});
		rx_goal_time = source.rx_score.SelectMany(score=>{
			return score.settings.time.rx_game_length;
		});
		
		//Distance
		rx_player_distance = source.rx_score.Select(score=>{
			return score.time.distance_covered;
		});
		rx_goal_distance = source.rx_score.SelectMany(score=>{
			return score.settings.win_condition.distance_covered.rx_target;
		});
		
		//Icons
		rx_player_time.CombineLatest(rx_goal_time, (player, goal)=>{
			return new {player = player, goal = goal};
		}).Subscribe(data => {
			update_section(data.player, data.goal, time_icon, GameTimer.to_stopwatch, time_recorded_text, time_goal_text);
		});
		rx_player_distance.CombineLatest(rx_goal_distance, (player, goal)=>{
			return new {player = player, goal = goal};
		}).Subscribe(data => {
			update_section(data.player, data.goal, distance_icon, (distance)=>{
				return distance.ToString("0.00")+" m";
			}, distance_recorded_text, distance_goal_text);
		});
	}
	
	void update_section(float player, float goal, Transform icon, Func<float,string> format, Text value_text, Text goal_text){
		if (icon != null){
			float x = slider_x(player, goal);
			float y = icon.localPosition.y;
			float z = icon.localPosition.z;
			icon.localPosition = new Vector3(x,y,z);
		}
		if (value_text != null){
			float percent = 100.0f * player / goal;
			value_text.text = format(player) + " (" + percent.ToString("0.0") +"%)";
		}
		if (goal_text != null){
			goal_text.text = format(goal);
		}
	}
}
