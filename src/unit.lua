Class = require 'src.hump.class'

Unit = Class {
    init = function(self, name, x, y, size)
        self.name = name
        self.x = x
        self.y = y
        self.size = size
		self.speed = 0
    end
}

function Unit:draw()
    love.graphics.setColor(0, 255, 0, 128)
    love.graphics.rectangle('fill', self.x, self.y, self.size, self.size)
end

function Unit:moveUnit(dt)
    self.y = self.y + (self.speed * dt)
end

function Unit:speedUp()
	if self.speed/100 == 0 then
		self.speed = self.speed + 3
	end
	if self.speed/100 == 1 then
		self.speed = self.speed + 2
	end
	if self.speed/100 == 2 then
		self.speed = self.speed + 1
	end
end

function Unit:speedDown()
  	if self.speed/100 == 0 then
		self.speed = self.speed -2
	end

	if self.speed/100 == 1 then
		self.speed = self.speed -4
	end
	
	if self.speed/100 == 2 then
		self.speed = self.speed -6
	end
end