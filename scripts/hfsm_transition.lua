local class = require("class")
return class.define(
		{
			init = function(_to, _condition, _action)
				assert(_to ~= nil, "Transition has a null target state.")
				assert(type(_condition) == "function", "Transition needs a function for its condition. Got "..type(_condition).." instead.")
				assert((_action == nil) or (type(_action) == "function"), "Transition needs either nil a function for its action. Got "..type(_action).." instead.")
				local inst = {
					props = {
						to = _to,
						condition = _condition,
						action = _action
					}
				}
				return inst
			end,
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
