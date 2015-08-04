local class = require("class")
local _ = require("moses_min")
local Result = require("hfsm_result")
function assert_child(this, state)
	if (state == nil) then return nil end
	if (state.parent == this) then return state end
	error("State must be a child of this HFSM or nil.")
end
return class.define(
	{
		defaults = {
			name = "state",
			children = {},
			transitions = {},
			level = 1
		},
		set = {
			current = assert_child,
			initial = assert_child,
			level = class:force_type("number"),
			parent = function(this,new,old)
				if (old ~= new) then
					if (old and old.children[this] == this) then
						old:remove_child(this)
					end
					if (new and not new.children[this]) then
						new:add_child(this)
					end
					this.level = new.level+1
				end
				return new
			end
		},
		size = function(this)
			return _.size(this.children)
		end,
		add_child = function(this,state)
			this.children[state] = state
			state.parent = this
			if (this.initial == nil) then
				this.initial = state
			end
			return this
		end,
		remove_child = function(this,state)
			this.children[state] = nil
			state.parent = nil
			if (this.initial == state) then
				this.initial = nil
			end
			if (this.current == state) then
				this.current = this.initial
			end
			return this
		end,
		add_transition = function(this,trans)
			this.transitions[trans] = trans
			trans.level = this.level - trans.to.level
			return this
		end,
		update = function(this, result)
			local result = result or Result.new()
			--If we're starting from scratch, enter the initial state.
			if (this.current == nil) then
				if (this:size() == 0) then
					--This is a leaf state and has no children, so just return its own update action.
					result:add_action(this.on_update)
				elseif (this.initial ~= nil) then
					this.current = this.initial
					result:add_action(this.current.on_entry)
				end
			else
				--Try to find a transition in the current state.
				local triggered = nil
				for ti, trans in pairs(this.current.transitions) do
					if (trans:test()) then
						triggered = trans
						break
					end
				end
				--If we've found one, load it into the result struct.
				if (triggered ~= nil) then
					result.trans = triggered
					result.level = triggered.level
				else
				--Otherwise, recurse downwards for a result
					result = this.current:update(result)
				end
				--Check if the result contains a transition.
				if (result.trans ~= nil) then
					--Act based on its level
					if (result.level > 0) then
						--Transition destined for higher level.
						result:add_action(this.current.on_exit)
						--Reset the state if the transition calls for it.
						if (not result.trans.remember_state) then
							this.current = nil
						end
						result.level = result.level -1
					else
						--Both same- and lower-level transitions share some code.
						local target = result.trans.to
						if (result.level == 0) then
						--Transition is on the same level.
							result:add_action(this.current.on_exit)
							result:add_action(result.trans.action)
							result:add_action(target.on_entry)
							this.current = target
							result:add_action(target.on_update)
						else
						--Transition is on lower level.
							result:add_action(result.trans.action)
							result:add_actions(target.parent:update_down(target, result.level, {}))
						end
						result.trans = nil;
					end
				end
			end
			return result
		end,
		update_down = function(this, target, level, actions)
			return actions;
		end,
		run = function(this)
			this:update():run()
			return this
		end
	}
)
