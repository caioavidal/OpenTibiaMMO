﻿local talkAction = TalkAction("/goto")

function talkAction.onSay(player, words, param)
    -- if not player:getGroup():getAccess() then
	-- 	return true
	-- end

    local target = Creature(param)
    if target then
        player:teleportTo(target:getPosition())
    else
        player:sendCancelMessage("Creature not found.")
    end
    return true
end

talkAction:register()
