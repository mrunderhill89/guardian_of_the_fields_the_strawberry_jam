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
				return result
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
				return this.current:handle_transition(result) 
			end
		end,
		lazy_set_current = function(this, child)
			return function()
				if (this.current ~= child) then
					if (this.current ~= nil) then
						this.current.on_exit()
						end
					this.current = child
					child.on_entry()
				end
			end
		end,
		_handle_transition = function(f,t,fa,ta)
			assert(f ~= nil and t ~= nil,"No common ancestor.")
			if (f == t) then return fa, ta end
			--Act based on its level
			if (f.level >= t.level) then
				fp = f.parent
				_.push(fa, fp:lazy_set_current(nil))
				return fp:_handle_transition(d, fa, ta)
			else
				tp = t.parent
				_.addTop(ta, tp:lazy_set_current(t))
				return f:_handle_transition(tp, fa, ta)
			end
		end,
		handle_transition = function(this,result)
			if (result.trans ~= nil) then
				local fa,ta = this:_handle_transition(target,{},{})
				result.add_actions(fa).add_actions(ta)
				result.trans = nil
			end
			return result
		end,
		run = function(this)
			this:update():run()
			return this
		end
	}
)
