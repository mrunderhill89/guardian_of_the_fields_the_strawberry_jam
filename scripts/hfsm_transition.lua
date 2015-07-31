local class = require("class")
return class.define(
	{
		defaults = {
			remember_state = false
		},
		set = {
			level = class:force_type("number")
		},
		test = function(this, args)
			if (this.condition ~= nil) then
				return this:condition(args)
			end
			return false
		end,
		run = function(this, args)
			if (this.action ~= nil) then
				return this:action(args)
			end
		end
	}
)
