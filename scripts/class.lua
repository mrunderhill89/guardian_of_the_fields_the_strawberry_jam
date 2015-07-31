local _ = require("moses_min")

function deep_clone(name, value)
	if (type(value) == "table") then
		return _.map(value, function(key,sub)
			return deep_clone(key, sub)
		end)
	end
	return value
end
local Class = {
	define = function(proto) 
		--Set up our object prototype.
		local proto = _.extend(_.defaults(proto or {}, 
		{
			set = {}, --setter functions
			get = {}, --getter functions
			defaults = {}, --default values
			initialize = function(params) --returns basic properties
				return _.defaults(params or {}, _.map(proto.defaults, deep_clone))
			end,
			constructor = function(inst, ...) --constructs the object from given table
				inst.props = _.map(
						proto.initialize(...),
						function(key, value)
							if (proto.set[key]) then
								return proto.set[key](inst, value)
							else
								return value
							end
						end
				)
				inst.set = function(index, value) --helper function that allows chaining
					inst[index] = value
					return inst
				end
				setmetatable(inst,proto)
				return inst
			end,
			new = function(...) --simply constructs with empty object
				local inst = proto.constructor({}, ...)
				return inst
			end,
		}),{
			__index = function(this, index)
				if (proto.get[index] ~= nil) then --we have a getter function
					return proto.get[index](this, this.props[index])
				end
				if (this.props[index] ~= nil) then --we only have the property
					return this.props[index]
				end
				return proto[index] --this is a static property
			end,
			__newindex = function(this, index, value)
				if (proto.set[index] ~= nil) then --we have a setter function
					rawset( this, index, proto.set[index](this, value, this[index]) )
				else --no setter function, just set it directly
					rawset( this, index, value )
				end
			end
		})
		setmetatable(proto,Class)
		return proto
	end,
	assert_type = function(t,data)
		assert(type(data) == t, "Type Error: Expected "..t..", got "..type(data).." instead.")
	end,
	force_type = function(this, t)
		return function(that,value)
			this.assert_type(t,value)
			return value
		end
	end,
	force_private = function(value, index)
		error("Unauthorized access to property "..index)
	end
}

return Class
