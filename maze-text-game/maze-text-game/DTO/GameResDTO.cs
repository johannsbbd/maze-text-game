using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace maze_text_game.DTO
{
    public class GameResDTO
    {
        public string RenderedMap { get; init; }

        public string GameState { get; init; }

        public string WinningPlayer { get; init; }
    }
}
