local hfsm = require("hfsm")
function lazy_print(message)
	return function()
		print(message)
	end
end

local stateA = hfsm.state.new({
	name = "State A",
	on_entry = lazy_print("Hello State A!"),
	on_update = lazy_print("Running State A..."),
	on_exit = lazy_print("Goodbye State A!")
})

local stateB = hfsm.state.new({
	name = "State B",
	on_entry = lazy_print("Hello State B!"),
	on_update = lazy_print("Running State B..."),
	on_exit = lazy_print("Goodbye State B!")
})

stateA:add_transition(hfsm.transition.new({
	to = stateB,
	condition = function() return true end,
	action = lazy_print("I'm on my way!")
}))

local root = hfsm.state.new({
	name = "Root",
}):add_child(stateA):add_child(stateB)

root:run():run():run():run()
