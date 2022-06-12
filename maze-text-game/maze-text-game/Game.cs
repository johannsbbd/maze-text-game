using System;
using System.Collections.Generic;
using System.Drawing;

namespace maze_text_game
{
    public class Game
    {
        private string sessionId;
        private Map map;
        private int playerLimit;
        private Dictionary<string, Player> players;
        private Player victoriousPlayer = null;
        private Dictionary<string, Point> playerPositions;

        public Game(int playerLimit, Size mapSize) { 
            this.playerLimit = playerLimit;
            this.sessionId = Guid.NewGuid().ToString();
            this.map = new Map(mapSize.Height, mapSize.Width);
            this.players = new Dictionary<string, Player>();
            this.playerPositions = new Dictionary<string, Point>();
        }

        public bool MovePlayer(Direction direction, string playerSessionId) {
            return false;
        }
    }

    public enum Direction { 
        North, 
        East, 
        South, 
        West,
    }

    public enum State { 
        Waiting,
        InProgress,
        Ended,
    }
}
