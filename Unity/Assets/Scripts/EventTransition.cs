using System;

public class EventTransition: I_HFSM_Transition
{	
	//Action
	protected Action _action = null;
	public Action action(){
		return _action;
	}
	public EventTransition action(Action a){
		_action = a;
		return this;
	}
	//To and From states
	protected HFSM_State _to_state = null;
	public HFSM_State to_state{
		get{ return _to_state;}
		private set{ _to_state = value; _level = -1;}
	}
	protected HFSM_State _from_state = null;
	public HFSM_State from_state{
		get{ return _from_state;}
		private set{ _from_state = value; _level = -1;}
	}
	//Level
	protected int _level = -1;
	public int level {
		get{
			return from_state.level - to_state.level;
		}
	}
	protected bool activated = false;
	public void fire(){
		activated = true;
	}
	public bool test(){
		if (activated) {
			activated = false;
			return true;
		}
		return false;
	}
	public EventTransition ()
	{
	}
}