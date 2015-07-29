local Class = {
	define = function(proto) 
		if (proto == nil) then
			proto = {}
		end
		if (proto.init == nil) then
			proto.init = function(params)
				return params 
			end
		end
		proto.new = function(params)
			if (params == nil) then
				params = {}
			end
			local inst = proto.init(params)
			setmetatable(inst,proto)
			return inst
		end
		proto.__index = proto
		return proto
	end
}

return Class
