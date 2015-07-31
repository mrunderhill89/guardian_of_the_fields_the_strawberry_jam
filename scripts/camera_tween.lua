local class = require("class")
local _ = require("moses")

local tween = class.define({
	defaults = {
		points = {}
	},
	calculate = function(this)
		local sum = _.reduce(
				this.points, 
				function(sum, point)
					return _.extend(sum, 
						_.map(point, 
							function(key, value)
								if (sum[key]) then
									return sum[key] + value
								end
								return value
							end
						)
					)
				end
			{}
		)
		if (type(sum.influence) ~= "number") then
			sum.influence = _.size(this.points)
		elseif (sum.influence == 0.0)
			sum.influence = math.max(sum.influence,1.0)
		end
		return _.map(
			sum, 
			function(key,value,influence)
				return value/influence
			end, 
			sum.influence
		)
	end
})
return tween
