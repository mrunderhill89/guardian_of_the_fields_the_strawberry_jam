local class = require("class")
return class.define({
	defaults = {
		actions = {},
		trans = nil,
		level = 0
	},
	add_action = function(this, action)
		table.insert(this.actions, action)
		return this
	end,
	add_actions = function(this, actions)
		for i,action in ipairs(actions) do
			this:add_action(action)
		end
		return this
	end,
	run = function(this, ...)
		for i,action in ipairs(this.actions) do
			action(...)
		end
		return this
	end
})
