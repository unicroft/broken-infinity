require 'src.class'

Player = class(function(p, name, x, y, size)
    -- init
    p.name = name
    p.x = x
    p.y = y
    p.size = size
    end
)

function Player:draw()
    love.graphics.setColor(255, 0, 0, 128)
    love.graphics.rectangle('fill', self.x, self.y, self.size, self.size)
end

return Player