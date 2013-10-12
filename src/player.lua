Player = {}
Player.__index = Player


function Player.new(name, x, y, size)
    local p = {}            -- new object
    setmetatable(p, Player)

    -- init
    p.name = name
    p.x = x
    p.y = y
    p.size = size

    return p
end

function Player:draw()
    love.graphics.setColor(255, 0, 0, 128)
    love.graphics.rectangle('fill', self.x, self.y, self.size, self.size)
end

return Player