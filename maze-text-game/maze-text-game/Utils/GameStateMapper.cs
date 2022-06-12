using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace maze_text_game.Utils
{
    public static class GameStateMapper
    {
        public static string GameStateEnumMap(State gameState)
        {
            switch (gameState)
            {
                case State.Waiting: return "Waiting for more players";
                case State.InProgress: return "Game in progress";
                case State.Ended: return "Game has ended";
                default: return "Unknown Game state";
            }
        }
    }
}
