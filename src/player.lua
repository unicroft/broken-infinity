Class = require 'src.hump.class'

Player = Class {
    init = function(self, name, x, y, size)
        self.name = name
        self.x = x
        self.y = y
        self.size = size
    end
}

function Player:draw()
    love.graphics.setColor(255, 0, 0, 128)
    love.graphics.rectangle('fill', self.x, self.y, self.size, self.size)
end
