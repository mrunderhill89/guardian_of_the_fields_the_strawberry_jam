function deep_clone(value)
	if (type(value) == "table") then
		local copy = {}
		for key,value in pairs(value) do
			copy[key] = deep_clone(value[key])
		end
		return copy
	end
	return value
end
local Class = {
	define = function(proto) 
		local proto = proto or {}
		proto.set = proto.set or {}
		proto.get = proto.get or {}
		proto.defaults = proto.defaults or {}
		if (proto.init == nil) then
			proto.init = function(params)
				local params = params or {}
				local inst = {props = {}}
				for key,value in pairs(proto.defaults) do
					if (not params[key]) then
						params[key] = deep_clone(value)
					end
				end
				for key,value in pairs(params) do
					if (proto.set[key]) then
						inst.props[key] = proto.set[key](value)
					else
						inst.props[key] = value
					end
				end
				return inst
			end
		end
		setmetatable(proto,Class)
		proto.new = function(...)
			local inst = proto.init(...)
			setmetatable(inst,proto)
			return inst
		end
		proto.__index = function(this, index)
			if (proto.get[index] ~= nil) then
				return proto.get[index](this, this.props[index])
			end
			if (this.props[index] ~= nil) then
				return this.props[index]
			end
			return proto[index]
		end
		proto.__newindex = function(this, index, value)
			if (proto.set[index] ~= nil) then
				rawset( this, index, proto.set[index](this, value) )
			else
				rawset( this, index, value )
			end
		end
		return proto
	end,
	assert_type = function(data, t)
		assert(type(data) == t, "Expected type "..t..", got "..type(data).." instead.")
	end
}

return Class
