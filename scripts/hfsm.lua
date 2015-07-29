local class = require("class")
local hfsm = {
	state = class.define(
		{
			init = function(params)
				if (params == nil) then
					params = {}
				end
				local inst = {
					name = params.name or "hfsm",
					on_entry = params.entry,
					on_update = params.on_update,
					on_exit = params.on_exit,
					children = params.children || {},
					transitions = params.transitions || {},
					current = params.current,
					initial = params.initial,
					parent = params.parent,
					level = 0
				}
				return inst
			end
		}
	),
	transition = class.define(
		{
		}
	)
}

return hfsm
