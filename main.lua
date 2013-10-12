-- creation du player
require "src.player"

function love.load()
    player1 = Player.new("Player 1", 25, 250, 100)
end

function love.update(dt)

end

function love.draw()
    love.graphics.print("Hello",300,300)
    player1:draw()
end
